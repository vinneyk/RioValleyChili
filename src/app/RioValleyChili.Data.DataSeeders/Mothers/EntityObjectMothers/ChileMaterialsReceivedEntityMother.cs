using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class ChileMaterialsReceivedEntityMother : EntityMotherLogBase<ChileMaterialsReceived, ChileMaterialsReceivedEntityMother.CallbackParameters>
    {
        private readonly RioValleyChiliDataContext _newContext;
        private readonly NewContextHelper _newContextHelper;

        public ChileMaterialsReceivedEntityMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            _newContext = newContext;
            _newContextHelper = new NewContextHelper(_newContext);
        }

        private enum EntityTypes
        {
            ChileMaterialsReceived,
            ChileMaterialsReceivedItem
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<ChileMaterialsReceived> BirthRecords()
        {
            _loadCount.Reset();

            var dataSource = OldContext.CreateObjectSet<tblIncoming>()
                .Include(i => i.tblLot, i => i.tblPackaging, i => i.tblVariety)
                .Where(i => i.TTypeID == (int?)TransType.DeHy || i.TTypeID == (int?)TransType.Other)
                .GroupBy(i => i.Lot).ToList();

            foreach(var tblIncomings in dataSource)
            {
                _loadCount.AddRead(EntityTypes.ChileMaterialsReceived);

                var oldLot = tblIncomings.First().tblLot;
                var transType = tblIncomings.Select(i => i.TTypeID).Distinct().Single().ToTransType();

                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(tblIncomings.Key, out lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.InvalidLotNumber)
                        {
                            Lot = oldLot
                        });
                    continue;
                }

                var employeeId = oldLot.EmployeeID ?? _newContextHelper.DefaultEmployee.EmployeeId;
                if(oldLot.EmployeeID == null)
                {
                    Log(new CallbackParameters(CallbackReason.DefaultEmployee)
                        {
                            Lot = oldLot,
                            EmployeeId = employeeId
                        });
                }
                
                if(oldLot.EntryDate == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullEntryDate)
                        {
                            Lot = oldLot
                        });
                    continue;
                }
                var entryDate = oldLot.EntryDate.Value.ConvertLocalToUTC();

                var supplierNames = tblIncomings.Select(i => i.Company_IA).Distinct().ToList();
                if(supplierNames.Count != 1)
                {
                    Log(new CallbackParameters(CallbackReason.NoSingleSupplierName)
                        {
                            Lot = oldLot
                        });
                    continue;
                }
                var supplierName = (supplierNames.Single() ?? "").Trim();
                if(string.IsNullOrEmpty(supplierName))
                {
                    var locales = tblIncomings.Select(l => l.DehyLocale).Where(l => !string.IsNullOrEmpty(l)).Distinct().ToList();
                    if(locales.Count == 1)
                    {
                        supplierName = locales.Single();
                        Log(new CallbackParameters(CallbackReason.UsedDehyLocaleAsDehydrator)
                            {
                                Lot = oldLot,
                                DehyLocale = supplierName
                            });
                    }
                }

                var supplier = _newContextHelper.GetCompany(supplierName, CompanyType.Dehydrator, CompanyType.Supplier);
                if(supplier == null)
                {
                    Log(new CallbackParameters(CallbackReason.SupplierNotLoaded)
                        {
                            Lot = oldLot,
                            DehydratorName = supplierName
                        });
                    continue;
                }

                var chileLot = _newContextHelper.GetChileLotWithProduct(lotKey);
                if(chileLot == null)
                {
                    Log(new CallbackParameters(CallbackReason.ChileLotNotLoaded)
                        {
                            Lot = oldLot
                        });
                    continue;
                }

                var treatmentIds = tblIncomings.Select(i => i.TrtmtID).Distinct().ToList();
                if(treatmentIds.Count != 1)
                {
                    Log(new CallbackParameters(CallbackReason.NoSingleTrmtID)
                        {
                            Lot = oldLot,
                        });
                    continue;
                }
                var treatmentId = treatmentIds.Single();

                var treatment = _newContextHelper.GetInventoryTreatment(treatmentId);
                if(treatment == null)
                {
                    Log(new CallbackParameters(CallbackReason.TreatmentNotLoaded)
                        {
                            Lot = oldLot,
                            TrmtID = treatmentId
                        });
                }

                var newItems = new List<ChileMaterialsReceivedItem>();
                var itemSequence = 1;

                var oldItemGroups = tblIncomings.GroupBy(i => new ChileMaterialsItemReceivedKeys(i), new ChileMaterialsItemReceivedKeysEqualityComparer());
                foreach(var oldItems in oldItemGroups)
                {
                    var newItem = CreateItemReceived(oldItems, lotKey, itemSequence);
                    if(newItem != null)
                    {
                        newItems.Add(newItem);
                        ++itemSequence;
                    }
                }

                _loadCount.AddLoaded(EntityTypes.ChileMaterialsReceived);
                _loadCount.AddLoaded(EntityTypes.ChileMaterialsReceivedItem, (uint) newItems.Count);
                
                yield return new ChileMaterialsReceived
                    {
                        ChileMaterialsReceivedType = transType == TransType.DeHy ? ChileMaterialsReceivedType.Dehydrated : ChileMaterialsReceivedType.Other,
                        EmployeeId = employeeId,
                        TimeStamp = entryDate,

                        LotDateCreated = chileLot.LotDateCreated,
                        LotDateSequence = chileLot.LotDateSequence,
                        LotTypeId = chileLot.LotTypeId,

                        LoadNumber = oldLot.LoadNum,
                        DateReceived = chileLot.LotDateCreated,

                        ChileProductId = chileLot.ChileProductId,
                        SupplierId = supplier.Id,
                        TreatmentId = treatment.Id,

                        Items = newItems
                    };
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private ChileMaterialsReceivedItem CreateItemReceived(IGrouping<ChileMaterialsItemReceivedKeys, tblIncoming> oldItems, ILotKey lotKey, int newSequence)
        {
            _loadCount.AddRead(EntityTypes.ChileMaterialsReceivedItem);

            if(oldItems.Any(i => i.Quantity == null))
            {
                return null;
            }
            var quantity = (int)oldItems.Sum(i => i.Quantity);

            var warehouseLocation = _newContextHelper.GetLocation(oldItems.Key.LocID);
            if(warehouseLocation == null)
            {
                Log(new CallbackParameters(CallbackReason.WarehouseLocationNotLoaded)
                    {
                        Items = oldItems
                    });
                return null;
            }

            var packagingProduct = _newContextHelper.GetPackagingProduct(oldItems.Key.PkgID);
            if(packagingProduct == null)
            {
                Log(new CallbackParameters(CallbackReason.PackagingNotLoaded)
                    {
                        Items = oldItems
                    });
                return null;
            }

            return new ChileMaterialsReceivedItem
                {
                    LotDateCreated = lotKey.LotKey_DateCreated,
                    LotDateSequence = lotKey.LotKey_DateSequence,
                    LotTypeId = lotKey.LotKey_LotTypeId,
                    ItemSequence = newSequence,

                    ToteKey = oldItems.Key.Tote,
                    Quantity = quantity,

                    LocationId = warehouseLocation.Id,
                    PackagingProductId = packagingProduct.Id,
                    GrowerCode = oldItems.Key.DehyLocale,
                    ChileVariety = oldItems.Key.Variety
                };
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            NullEntryDate,
            NoSingleSupplierName,
            SupplierNotLoaded,
            InvalidLotNumber,
            WarehouseLocationNotLoaded,
            PackagingNotLoaded,
            DefaultEmployee,
            ChileLotNotLoaded,
            UsedDehyLocaleAsDehydrator,
            StringTruncated,
            NoSingleTrmtID,
            TreatmentNotLoaded
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            protected override CallbackReason ExceptionReason { get { return ChileMaterialsReceivedEntityMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return ChileMaterialsReceivedEntityMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return ChileMaterialsReceivedEntityMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            public tblLot Lot { get; set; }

            public IGrouping<ChileMaterialsItemReceivedKeys, tblIncoming> Items { get; set; }

            public string DehydratorName { get; set; }
            public int EmployeeId { get; set; }
            public string DehyLocale { get; set; }
            public int TrmtID { get; set; }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case ChileMaterialsReceivedEntityMother.CallbackReason.NullEntryDate:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.NoSingleSupplierName:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.SupplierNotLoaded:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.InvalidLotNumber:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.WarehouseLocationNotLoaded:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.PackagingNotLoaded:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.ChileLotNotLoaded:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.NoSingleTrmtID:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.TreatmentNotLoaded:
                        return ReasonCategory.RecordSkipped;

                    case ChileMaterialsReceivedEntityMother.CallbackReason.DefaultEmployee:
                    case ChileMaterialsReceivedEntityMother.CallbackReason.UsedDehyLocaleAsDehydrator:
                        return ReasonCategory.Informational;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}