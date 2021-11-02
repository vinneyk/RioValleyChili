using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Helpers
{
    public class SyncLotHelper
    {
        private readonly RioAccessSQLEntities _oldContext;
        private readonly ILotUnitOfWork _unitOfWork;
        private readonly OldContextHelper _oldContextHelper;

        public SyncLotHelper(RioAccessSQLEntities oldContext, ILotUnitOfWork unitOfWork, OldContextHelper oldContextHelper)
        {
            _oldContext = oldContext;
            _unitOfWork = unitOfWork;
            _oldContextHelper = oldContextHelper;
        }

        [Issue("Lot history will only be recorded if lot stat or attribute has changed.",
            References = new[] { "RVCADMIN-1232" })]
        public tblLot SynchronizeOldLot(LotKey lotKey, bool overrideOldContextLotAsCompleted, bool updateSerializationOnly = false)
        {
            var newLot = _unitOfWork.LotRepository.GetLotForSynch(lotKey);
            if(newLot == null)
            {
                throw new Exception(string.Format("Lot[{0}] not found in new context.", lotKey));
            }
            
            var customer = GetProductionCustomer(newLot);
            var oldLot = GetOrCreateLot(newLot);
            var lotHistory = CreateNewLotHistoryRecord(oldLot);

            UpdateOldLot(oldLot, newLot, customer, overrideOldContextLotAsCompleted, updateSerializationOnly);

            if(!updateSerializationOnly && CommitLotHistory(oldLot, lotHistory))
            {
                _oldContext.tblLotAttributeHistories.AddObject(lotHistory);
            }

            return oldLot;
        }

        private Customer GetProductionCustomer(Lot lot)
        {
            if(lot.ChileLot != null)
            {
                var company = _unitOfWork.ProductionBatchRepository.FilterByKey(lot.ToLotKey())
                    .Select(b => new[] { b.PackSchedule.Customer }
                        .Where(c => c != null)
                        .Select(c => new
                            {
                                Customer = c,
                                c.Company,
                            })
                        .FirstOrDefault())
                    .FirstOrDefault();
                if(company != null)
                {
                    return company.Customer;
                }
            }

            return null;
        }

        private tblLot GetOrCreateLot(Lot lot)
        {
            var oldLot = _oldContext.GetLot(lot, false);
            if(oldLot == null)
            {
                var lotNumber = LotNumberBuilder.BuildLotNumber(lot);
                tblProduct product = null;
                switch(lot.LotTypeEnum.ToProductType())
                {
                    case ProductTypeEnum.Additive:
                        var additiveProduct = _unitOfWork.AdditiveProductRepository.FindByKey(lot.AdditiveLot.ToAdditiveProductKey(), p => p.Product);
                        product = _oldContextHelper.GetProduct(additiveProduct.Product.ProductCode);
                        break;

                    case ProductTypeEnum.Chile:
                        var chileProduct = _unitOfWork.ChileProductRepository.FindByKey(lot.ChileLot.ToChileProductKey(), p => p.Product);
                        product = _oldContextHelper.GetProduct(chileProduct.Product.ProductCode);
                        break;

                    case ProductTypeEnum.Packaging:
                        var packagingProduct = _unitOfWork.PackagingProductRepository.FindByKey(lot.PackagingLot.ToPackagingProductKey(), p => p.Product);
                        product = GetOrCreatePackagingProduct(packagingProduct, lot, lot.TimeStamp);
                        break;
                }

                oldLot = new tblLot
                    {
                        Lot = lotNumber,
                        EmployeeID = lot.EmployeeId,
                        EntryDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL(),

                        PTypeID = lot.LotTypeId,
                        ProdID = product.ProdID,
                        Julian = lotNumber.Julian,
                        TargetWgt = 0,
                        Notes = "Old Context Synchronization"
                    };

                _oldContext.tblLots.AddObject(oldLot);
            }

            return oldLot;
        }

        private static void UpdateOldLot(tblLot oldLot, Lot newLot, Customer customer, bool completedOverride, bool updateSerializationOnly)
        {
            if(!updateSerializationOnly)
            {
                if(customer != null)
                {
                    oldLot.Company_IA = customer.Company.Name;
                }

                oldLot.Notes = newLot.Notes;
                oldLot.PurchOrder = newLot.PurchaseOrderNumber;
                oldLot.ShipperNum = newLot.ShipperNumber;
                
                if(string.IsNullOrWhiteSpace(oldLot.Company_IA) || newLot.Vendor != null)
                {
                    oldLot.Company_IA = newLot.Vendor == null ? null : newLot.Vendor.Name;
                }

                LotSyncHelper.SetLotAttributes(oldLot, newLot.Attributes.ToList());
                LotSyncHelper.SetTestData(oldLot, newLot);
                LotSyncHelper.SetLotStat(oldLot, newLot, completedOverride, newLot.ChileLot == null ? null : newLot.ChileLot.ChileProduct.ProductAttributeRanges.ToList());
            }

            oldLot.Serialized = SerializableLot.Serialize(newLot);
        }

        private tblProduct GetOrCreatePackagingProduct(PackagingProduct packagingProduct, IEmployeeKey employeeKey, DateTime timestamp)
        {
            tblPackaging tblPackaging;
            var product = _oldContextHelper.GetProductFromPackagingId(packagingProduct.Product.ProductCode, out tblPackaging);
            if(product == null)
            {
                product = new tblProduct
                    {
                        ProdID = _oldContextHelper.GetNextProductId(5),
                        Product = packagingProduct.Product.Name,
                        ProdGrpID = 98,
                        PTypeID = 5,
                        PkgID = tblPackaging == null ? (int?)null : tblPackaging.PkgID,
                        TrtmtID = 0,
                        EmployeeID = employeeKey.EmployeeKey_Id,
                        EntryDate = timestamp.ConvertUTCToLocal(),
                        InActive = false
                    };
                _oldContext.CreateObjectSet<tblProduct>().AddObject(product);
            }

            return product;
        }

        private static tblLotAttributeHistory CreateNewLotHistoryRecord(tblLot lot)
        {
            var history = new tblLotAttributeHistory
                {
                    Lot = lot.Lot,
                    ArchiveDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL(),
                    EmployeeID = lot.EmployeeID,
                    EntryDate = lot.EntryDate,
                    
                    Notes = lot.Notes,

                    TestDate = lot.TestDate,
                    TesterID = lot.TesterID,
                    TestNotes = lot.TestNotes,

                    LotStat = lot.LotStat,
                    LoBac = lot.LoBac
                };

            foreach(var attribute in StaticAttributeNames.AttributeNames)
            {
                var get = lot.AttributeGet(attribute);
                var set = history.AttributeSet(attribute);

                if(get != null && set != null)
                {
                    set(get());
                }
            }

            return history;
        }

        [Issue("Updated to create history record if TestDate has changed (that actually translates to the MAX non-Computed AttributeDate in the web system." +
               "Doesn't seem to be a serious issue, just noticed while debugging apparent failure in creating history records and decided to update this way with V's OK - RI 2016-10-31" +
               "Will also create history if TesterID ends up having changed - not an explicit requirement but am following the logic that TestDate has established. -RI 2016-11-9",
               References = new[] { "RVCADMIN-1360", "RVCADMIN-1369" })]
        private static bool CommitLotHistory(tblLot lot, tblLotAttributeHistory history)
        {
            if(lot.LotStat != history.LotStat)
            {
                return true;
            }

            if(lot.TestDate != history.TestDate)
            {
                return true;
            }

            if(lot.TesterID != history.TesterID)
            {
                return true;
            }

            foreach(var attribute in StaticAttributeNames.AttributeNames)
            {
                var lotGet = lot.AttributeGet(attribute);
                var historyGet = history.AttributeGet(attribute);

                if(historyGet != null)
                {
                    var lotValue = lotGet == null ? null : lotGet();
                    var historyValue = historyGet();
                    
                    if(lotValue != null || historyValue != null)
                    {
                        if(lotValue == null || historyValue == null)
                        {
                            return true;
                        }

                        if(Math.Abs(lotValue.Value - historyValue.Value) > 0.01m)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}