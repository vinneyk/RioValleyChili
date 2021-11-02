using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Helpers
{
    public class SyncProductionResultsHelper
    {
        private RioAccessSQLEntities OldContext { get { return _oldContextHelper.OldContext; } } 
        private readonly OldContextHelper _oldContextHelper;

        public SyncProductionResultsHelper(OldContextHelper oldContextHelper)
        {
            if(oldContextHelper == null) { throw new ArgumentNullException("oldContextHelper"); }
            _oldContextHelper = oldContextHelper;
        }

        public void SetResults(ProductionBatch productionBatch, tblLot lot)
        {
            var productionStart = productionBatch.Production.Results.ProductionBegin.ConvertUTCToLocal().RoundMillisecondsForSQL();
            
            lot.Shift = productionBatch.Production.Results.ShiftKey;
            lot.Produced = productionStart.Date; // this is how they are expecting to find lots for entry of production results. -VK 2/19/14
            lot.ProductionDate = productionBatch.LotDateCreated.Date;
            lot.BatchBegTime = productionStart;
            lot.BatchEndTime = productionBatch.Production.Results.ProductionEnd.ConvertUTCToLocal().RoundMillisecondsForSQL();
            lot.ProductionLine = ProductionLineParser.GetProductionLineNumber(productionBatch.Production.Results.ProductionLineLocation);
            lot.BatchStatID = (int?)BatchStatID.Produced;

            SetIncomingItems(productionBatch, lot);
            SetOutgoingItems(productionBatch, lot);
        }

        public void SetIncomingItems(ProductionBatch productionBatch, tblLot lot)
        {
            var currentIncoming = lot.tblIncomings.Where(i => i.TTypeID == (int?)TransType.Production).ToList();
            var newIncoming = productionBatch.Production.Results.ResultItems.Select(CreateIncoming).ToList();

            //EF has issue with tracking entities with duplicate or no primary keys and considering them equal. Answer seems to be to disable entity tracking on the object set.
            //http://stackoverflow.com/questions/4994203/unbelievable-duplicate-in-an-entity-framework-query
            // -RI 2016/2/1
            var mergeOption = OldContext.ViewAvailables.MergeOption;
            OldContext.ViewAvailables.MergeOption = MergeOption.NoTracking;
            var inventory = OldContext.ViewAvailables.Where(i => i.Lot == lot.Lot).ToList();
            OldContext.ViewAvailables.MergeOption = mergeOption;

            foreach(var modification in GetModifications(currentIncoming, newIncoming))
            {
                var item = inventory.SingleOrDefault(i => i.LocID == modification.LocID && i.PkgID == modification.PkgID && i.TrtmtID == modification.TrtmtID);
                if((item != null ? item.Quantity : 0) + modification.ModifyQuantity < 0)
                {
                    throw new Exception(string.Format("Inventory Lot[{0}] PkgID[{1}] LocID[{2}] TrtmtID[{3}] would have resulted in negative quantity.", modification.Lot, modification.PkgID, modification.LocID, modification.TrtmtID));
                }
            }

            currentIncoming.ForEach(i => OldContext.tblIncomings.DeleteObject(i));
            newIncoming.ForEach(i => OldContext.tblIncomings.AddObject(i));
        }

        public void SetOutgoingItems(ProductionBatch productionBatch, tblLot lot)
        {
            var currentOutgoing = lot.tblOutgoingInputs.Where(i => i.TTypeID == (int?) TransType.Production).ToList();
            var newOutgoing = lot.inputBatchItems.Select(b => CreateOutgoingFromBatchItem(b, productionBatch)).ToList();

            currentOutgoing.ForEach(o => OldContext.tblOutgoings.DeleteObject(o));
            newOutgoing.ForEach(o => OldContext.tblOutgoings.AddObject(o));
        }

        private static IEnumerable<ModifyInventory> GetModifications(IEnumerable<tblIncoming> currentIncoming, IEnumerable<tblIncoming> newIncoming)
        {
            var modifications = currentIncoming.Select(i => new ModifyInventory(i) { ModifyQuantity = (int)-i.Quantity }).ToList();

            foreach(var incoming in newIncoming)
            {
                var modification = modifications.FirstOrDefault(m =>
                    m.Lot == incoming.Lot &&
                    m.PkgID == incoming.PkgID &&
                    m.LocID == incoming.LocID &&
                    m.TrtmtID == incoming.TrtmtID);
                if(modification == null)
                {
                    modification = new ModifyInventory(incoming);
                    modifications.Add(modification);
                }

                modification.ModifyQuantity += (int)incoming.Quantity;
            }

            return modifications;
        }

        private static tblOutgoing CreateOutgoingFromBatchItem(tblBatchItem batchItem, IEmployeeKey employeeKey)
        {
            return new tblOutgoing
                {
                    EntryDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL(),
                    NewLot = batchItem.NewLot,
                    Lot = batchItem.Lot,
                    Quantity = batchItem.Quantity,
                    TtlWgt = batchItem.TtlWgt,
                    EmployeeID = employeeKey.EmployeeKey_Id,
                    PkgID = batchItem.PkgID,
                    Tote = batchItem.Tote,
                    NetWgt = batchItem.NetWgt,
                    TTypeID = (int?)TransType.Production,
                    BOMID = batchItem.BOMID,
                    BatchLot = batchItem.NewLot,
                    LocID = batchItem.LocID,
                    AstaCalc = batchItem.AstaCalc,
                    PackSchID = batchItem.PackSchID,
                    TrtmtID = batchItem.TrtmtID,
                };
        }

        private tblIncoming CreateIncoming(LotProductionResultItem resultItem)
        {
            var entryDate = resultItem.ProductionResults.DateTimeEntered.ConvertUTCToLocal().RoundMillisecondsForSQL();
            var packaging = _oldContextHelper.GetPackaging(resultItem.PackagingProduct);
            var location = _oldContextHelper.GetLocation(resultItem.Location);

            return new tblIncoming
                {
                    EntryDate = entryDate,
                    Lot = LotNumberBuilder.BuildLotNumber(resultItem),
                    TTypeID = (int?)TransType.Production,
                    PkgID = packaging.PkgID,
                    VarietyID = 0,
                    Quantity = resultItem.Quantity,
                    NetWgt = packaging.NetWgt,
                    TtlWgt = packaging.NetWgt * resultItem.Quantity,
                    LocID = location.LocID,
                    TrtmtID = resultItem.TreatmentId,
                    EmployeeID = resultItem.ProductionResults.EmployeeId
                };
        }

        private class ModifyInventory
        {
            public int Lot;
            public int PkgID;
            public int LocID;
            public int TrtmtID;
            public int ModifyQuantity;

            public ModifyInventory(tblIncoming incoming)
            {
                Lot = incoming.Lot;
                PkgID = incoming.PkgID;
                LocID = incoming.LocID;
                TrtmtID = incoming.TrtmtID;
            }
        }
    }
}