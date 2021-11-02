using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Helpers
{
    public class SyncProductionBatchPickedInventoryHelper
    {
        public static void SetLotBatchAttributes(tblLot lot, List<LotAttribute> lotAttributes)
        {
            var asta = lotAttributes.FirstOrDefault(a => a.AttributeShortName == Constants.ChileAttributeKeys.Asta);
            if(asta != null)
            {
                lot.bAsta = (decimal?)asta.AttributeValue;
            }

            var h2o = lotAttributes.FirstOrDefault(a => a.AttributeShortName == Constants.ChileAttributeKeys.H2O);
            if(h2o != null)
            {
                lot.bH2O = (decimal?)h2o.AttributeValue;
            }

            var scan = lotAttributes.FirstOrDefault(a => a.AttributeShortName == Constants.ChileAttributeKeys.Scan);
            if(scan != null)
            {
                lot.bScan = (decimal?)scan.AttributeValue;
            }

            var ab = lotAttributes.FirstOrDefault(a => a.AttributeShortName == Constants.ChileAttributeKeys.AB);
            if(ab != null)
            {
                lot.bAB = (decimal?)ab.AttributeValue;
            }

            var gran = lotAttributes.FirstOrDefault(a => a.AttributeShortName == Constants.ChileAttributeKeys.Gran);
            if(gran != null)
            {
                lot.bGran = (decimal?)gran.AttributeValue;
            }
        }

        private readonly IProductionUnitOfWork _unitOfWork;
        private readonly RioAccessSQLEntities _oldContext;

        public SyncProductionBatchPickedInventoryHelper(IProductionUnitOfWork unitOfWork, RioAccessSQLEntities oldContext)
        {
            if(unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }
            if(oldContext == null) { throw new ArgumentNullException("oldContext"); }
            _unitOfWork = unitOfWork;
            _oldContext = oldContext;
        }

        public void SetBOMBatchItems(ProductionBatch productionBatch, tblLot lot)
        {
            foreach(var item in lot.inputBatchItems.ToList())
            {
                _oldContext.tblBatchItems.DeleteObject(item);
            }
            foreach(var bom in lot.tblBOMs.ToList())
            {
                _oldContext.tblBOMs.DeleteObject(bom);
            }

            var newBOMs = CreateDaBOMs(productionBatch, lot.Lot);
            foreach(var newBOM in newBOMs)
            {
                _oldContext.tblBOMs.AddObject(newBOM.Value);
            }
            foreach(var batchItem in CreateBatchItems(productionBatch, newBOMs))
            {
                _oldContext.tblBatchItems.AddObject(batchItem);
            }
            
            SetLotAttributes(productionBatch, lot);
        }

        private static void SetLotAttributes(ProductionBatch productionBatch, tblLot lot)
        {
            var lotAttributes = productionBatch.Production.ResultingChileLot.Lot.Attributes.ToList();
            LotSyncHelper.SetLotAttributes(lot, lotAttributes);
            SetLotBatchAttributes(lot, lotAttributes);
        }

        private IEnumerable<tblBatchItem> CreateBatchItems(ProductionBatch batch, IDictionary<int, tblBOM> daBOMs)
        {
            var lotNumber = LotNumberParser.BuildLotNumber(batch);
            foreach(var pickedItem in batch.Production.PickedInventory.Items)
            {
                var availableQuantity = pickedItem.Quantity;
                var inventory = _unitOfWork.InventoryRepository.FindByKey(new InventoryKey(pickedItem));
                if(inventory != null)
                {
                    availableQuantity += inventory.Quantity;
                }
                var weight = pickedItem.PackagingProduct.Weight;
                var avaiableWeight = availableQuantity * weight;

                int? astaCalc = null;
                var asta = pickedItem.Lot.Attributes.FirstOrDefault(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName);
                if(asta != null)
                {
                    var productionDate = asta.AttributeDate;
                    if(batch.Production.Results != null)
                    {
                        productionDate = batch.Production.Results.ProductionEnd;
                    }
                    astaCalc = AstaCalculator.CalculateAsta(asta.AttributeValue, asta.AttributeDate, productionDate, DateTime.UtcNow);
                }

                yield return new tblBatchItem
                    {
                        EntryDate = pickedItem.PickedInventory.TimeStamp.ConvertUTCToLocal(),
                        Lot = LotNumberParser.BuildLotNumber(pickedItem),
                        TTypeID = (int?)TransType.Batching,
                        PkgID = int.Parse(pickedItem.Lot.LotTypeEnum == LotTypeEnum.Packaging ? pickedItem.Lot.PackagingLot.PackagingProduct.Product.ProductCode : pickedItem.PackagingProduct.Product.ProductCode),
                        Tote = pickedItem.ToteKey,
                        Quantity = pickedItem.Quantity,
                        NetWgt = (decimal?)weight,
                        TtlWgt = (decimal?)(pickedItem.Quantity * weight),
                        LocID = pickedItem.FromLocation.LocID.Value,
                        TrtmtID = pickedItem.TreatmentId,
                        EmployeeID = pickedItem.PickedInventory.EmployeeId,
                        NewLot = lotNumber,
                        BatchLot = lotNumber,
                        AstaCalc = astaCalc,
                        AvgAsta = asta == null ? null : (decimal?)asta.AttributeValue,
                        //ODetail = 
                        PackSchID = batch.PackSchedule.PackSchID,
                        AQ = availableQuantity,
                        AW = (decimal?)avaiableWeight,
                        LoBac = batch.Production.ResultingChileLot.AllAttributesAreLoBac,
                    
                        tblBOM = GetDaBOM(daBOMs, pickedItem)
                    };
            }
        }

        private IDictionary<int, tblBOM> CreateDaBOMs(ProductionBatch batch, int lotNumber)
        {
            var ingredients = batch.Production.ResultingChileLot.ChileProduct.Ingredients.ToDictionary(i => i.AdditiveType.Id, i => i);

            var fgPercentage = 1.0;
            var daBoms = new Dictionary<int, tblBOM>();
            if(batch.PackSchedule.WorkType.Description.ToUpper().Contains("NEW"))
            {
                foreach(var ingredient in ingredients)
                {
                    fgPercentage -= ingredient.Value.Percentage;
                    daBoms.Add(ingredient.Key, CreateDaBOM(lotNumber, ingredient.Key, (decimal)ingredient.Value.Percentage, batch.Production.PickedInventory));
                }
            }
            else
            {
                daBoms.Add((int)Ingredient.Dextrose, CreateDaBOM(lotNumber, (int)Ingredient.Dextrose, 0, batch.Production.PickedInventory));
            }
            daBoms.Add((int)Ingredient.Fbase, CreateDaBOM(lotNumber, (int)Ingredient.Fbase, (decimal)fgPercentage, batch.Production.PickedInventory));

            foreach(var item in batch.Production.PickedInventory.Items)
            {
                var lotTypeIngredient = ToLotTypeIngredient(item.LotTypeId.ToLotType().Value);
                if(lotTypeIngredient != null)
                {
                    if(!daBoms.ContainsKey((int)lotTypeIngredient))
                    {
                        daBoms.Add((int)lotTypeIngredient, CreateDaBOM(lotNumber, (int)lotTypeIngredient, 0, batch.Production.PickedInventory));
                    }
                }
                else
                {
                    var additiveLot = _unitOfWork.AdditiveLotRepository.FilterByKey(new LotKey(item), a => a.AdditiveProduct).FirstOrDefault();
                    if(additiveLot != null)
                    {
                        if(!daBoms.ContainsKey(additiveLot.AdditiveProduct.AdditiveTypeId))
                        {
                            daBoms.Add(additiveLot.AdditiveProduct.AdditiveTypeId, CreateDaBOM(lotNumber, additiveLot.AdditiveProduct.AdditiveTypeId, 0, batch.Production.PickedInventory));
                        }
                    }
                }
            }

            return daBoms;
        }

        private static tblBOM CreateDaBOM(int lotNumber, int id, decimal percentage, EmployeeIdentifiableBase employeeIdentifiable)
        {
            return new tblBOM
            {
                Lot = lotNumber,
                IngrID = id,
                Percentage = percentage,
                EmployeeID = employeeIdentifiable.EmployeeId,
                EntryDate = employeeIdentifiable.TimeStamp.ConvertUTCToLocal()
            };
        }

        private tblBOM GetDaBOM(IDictionary<int, tblBOM> daBOMs, PickedInventoryItem pickedItem)
        {
            tblBOM daBOM = null;
            var ingredient = ToLotTypeIngredient((LotTypeEnum)pickedItem.LotTypeId);
            if(ingredient != null)
            {
                daBOMs.TryGetValue((int)ingredient, out daBOM);
            }
            else
            {
                var additiveLot = _unitOfWork.AdditiveLotRepository.FilterByKey(new LotKey(pickedItem), a => a.AdditiveProduct.AdditiveType).FirstOrDefault();
                if(additiveLot != null)
                {
                    daBOMs.TryGetValue(additiveLot.AdditiveProduct.AdditiveTypeId, out daBOM);
                }
            }
            return daBOM;
        }

        public enum Ingredient
        {
            Fbase = 0,
            MWD = 31,
            GRP = 40,
            Wbase = 90,
            Dextrose = 10
            //Pkg = 99
        }

        private static Ingredient? ToLotTypeIngredient(LotTypeEnum lotType)
        {
            switch(lotType)
            {
                case LotTypeEnum.DeHydrated: return Ingredient.MWD;
                case LotTypeEnum.WIP: return Ingredient.Wbase;
                case LotTypeEnum.FinishedGood: return Ingredient.Fbase;
                case LotTypeEnum.GRP: return Ingredient.GRP;
            }
            return null;
        }
    }
}