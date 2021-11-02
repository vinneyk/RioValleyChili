using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class MillAndWetdownMother : EntityMotherLogBase<ChileLotProduction, MillAndWetdownMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;

        public MillAndWetdownMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> analyzerCallback)
            : base(oldContext, analyzerCallback)
        {
            _newContextHelper = new NewContextHelper(newContext);
        }

        private enum EntityTypes
        {
            ChileLotProduction,
            PickedInventory,
            PickedInventoryItem,
            LotProductionResults,
            LotProductionResultItem
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<ChileLotProduction> BirthRecords()
        {
            _loadCount.Reset();
            
            foreach(var lot in SelectMillAndWetdownToLoad(OldContext))
            {
                _loadCount.AddRead(EntityTypes.ChileLotProduction);

                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(lot.Lot, out lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.InvalidLotNumber)
                        {
                            Lot = lot
                        });
                    continue;
                }

                var chileLot = _newContextHelper.GetChileLotWithProduct(lotKey);
                if(chileLot == null)
                {
                    Log(new CallbackParameters(CallbackReason.ChileLotNotLoaded)
                        {
                            Lot = lot,
                            LotKey = lotKey
                        });
                    continue;
                }

                bool usedDefaultProductionLine;
                DateTime entryDate;
                var productionResults = CreateLotProductionResults(chileLot, lot, out usedDefaultProductionLine, out entryDate);
                if(productionResults == null)
                {
                    continue;
                }

                var pickedInventory = CreatePickedInventory(entryDate, lot);
                if(pickedInventory == null)
                {
                    continue;
                }

                var production = new ChileLotProduction
                    {
                        EmployeeId = productionResults.EmployeeId,
                        TimeStamp = entryDate,

                        LotDateCreated = chileLot.LotDateCreated,
                        LotDateSequence = chileLot.LotDateSequence,
                        LotTypeId = chileLot.LotTypeId,

                        ProductionType = ProductionType.MillAndWetdown,
                        PickedInventoryDateCreated = pickedInventory.DateCreated,
                        PickedInventorySequence = pickedInventory.Sequence,

                        Results = productionResults,
                        PickedInventory = pickedInventory,
                    };

                if(usedDefaultProductionLine)
                {
                    Log(new CallbackParameters(CallbackReason.DefaultProductionLine)
                        {
                            Lot = lot,
                            ProductionLine = production.Results.ProductionLineLocation
                        });
                }

                _loadCount.AddLoaded(EntityTypes.ChileLotProduction);
                _loadCount.AddLoaded(EntityTypes.PickedInventory);
                _loadCount.AddLoaded(EntityTypes.PickedInventoryItem, (uint)production.PickedInventory.Items.Count);
                _loadCount.AddLoaded(EntityTypes.LotProductionResults);
                _loadCount.AddLoaded(EntityTypes.LotProductionResultItem, (uint) production.Results.ResultItems.Count);

                yield return production;
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private PickedInventory CreatePickedInventory(DateTime entryDate, LotDTO lot)
        {
            _loadCount.AddRead(EntityTypes.PickedInventory);

            var identifiable = lot.tblOutgoingInputs.Cast<IEmployeeIdentifiableDTO>().ToList();
            identifiable.Add(lot);

            int? employeeID;
            DateTime? timeStamp;
            identifiable.GetLatest(out employeeID, out timeStamp);

            if(timeStamp == null)
            {
                Log(new CallbackParameters(CallbackReason.PickedInventoryUndeterminedTimestamp)
                {
                    Lot = lot
                });
                return null;
            }

            employeeID = employeeID ?? _newContextHelper.DefaultEmployee.EmployeeId;
            if(employeeID == _newContextHelper.DefaultEmployee.EmployeeId)
            {
                Log(new CallbackParameters(CallbackReason.PickedInventoryUsedDefaultEmployee)
                {
                    Lot = lot,
                    EmployeeId = employeeID.Value
                });
            }

            var dateCreated = entryDate.Date;
            var sequence = PickedInventoryKeyHelper.Singleton.GetNextSequence(dateCreated);

            var pickedInventory = new PickedInventory
                {
                    EmployeeId = employeeID.Value,
                    TimeStamp = timeStamp.Value,

                    DateCreated = dateCreated,
                    Sequence = sequence,
                    PickedReason = PickedReason.Production,
                    Archived = true,
                };

            pickedInventory.Items = CreatePickedInventoryItems(pickedInventory, lot.tblOutgoingInputs).ToList();

            return pickedInventory;
        }

        private IEnumerable<PickedInventoryItem> CreatePickedInventoryItems(IPickedInventoryKey pickedInventoryKey, IEnumerable<OutgoingDTO> outgoings)
        {
            var pickedItemSequence = 0;
            foreach(var outgoing in outgoings)
            {
                _loadCount.AddRead(EntityTypes.PickedInventoryItem);

                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(outgoing.Lot, out lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.OutgoingInvalidLotNumber)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                if(!_newContextHelper.LotLoaded(lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.OutgoingLotNotLoaded)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                var packagingProduct = outgoing.PTypeID == (int?)LotTypeEnum.Packaging ? _newContextHelper.NoPackagingProduct : _newContextHelper.GetPackagingProduct(outgoing.PkgID);
                if(packagingProduct == null)
                {
                    Log(new CallbackParameters(CallbackReason.OutgoingPackagingNotLoaded)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                var warehouseLocation = _newContextHelper.GetLocation(outgoing.LocID);
                if(warehouseLocation == null)
                {
                    Log(new CallbackParameters(CallbackReason.OutgoingWarehouseLocationNotLoaded)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                var treatment = _newContextHelper.GetInventoryTreatment(outgoing.TrtmtID);
                if(treatment == null)
                {
                    Log(new CallbackParameters(CallbackReason.OutgoingTreatmentNotLoaded)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                yield return new PickedInventoryItem
                    {
                        DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                        Sequence = pickedInventoryKey.PickedInventoryKey_Sequence,
                        ItemSequence = ++pickedItemSequence,

                        LotDateCreated = lotKey.LotKey_DateCreated,
                        LotDateSequence = lotKey.LotKey_DateSequence,
                        LotTypeId = lotKey.LotKey_LotTypeId,

                        PackagingProductId = packagingProduct.Id,
                        FromLocationId = warehouseLocation.Id,
                        TreatmentId = treatment.Id,
                        CurrentLocationId = warehouseLocation.Id,
                        ToteKey = outgoing.Tote ?? "",
                        Quantity = (int) (outgoing.Quantity ?? 0),
                        CustomerProductCode = outgoing.CustProductCode
                    };
            }
        }

        private LotProductionResults CreateLotProductionResults(ILotKey lotKey, LotDTO lot, out bool usedDefaultProductionLine, out DateTime entryDate)
        {
            usedDefaultProductionLine = false;
            entryDate = new DateTime();

            _loadCount.AddRead(EntityTypes.LotProductionResults);

            var employeeId = lot.EmployeeID ?? _newContextHelper.DefaultEmployee.EmployeeId;
            if(lot.EmployeeID == null)
            {
                Log(new CallbackParameters(CallbackReason.DefaultEmployee)
                    {
                        Lot = lot,
                        EmployeeId = employeeId
                    });
            }

            if(lot.EntryDate == null)
            {
                Log(new CallbackParameters(CallbackReason.NullEntryDate)
                    {
                        Lot = lot
                    });
                return null;
            }
            entryDate = lot.EntryDate.Value.ConvertLocalToUTC();

            var productionLineNumber = 1;
            if(lot.ProductionLine == null)
            {
                usedDefaultProductionLine = true;
            }
            else
            {
                productionLineNumber = lot.ProductionLine.Value;
            }

            var productionLine = _newContextHelper.GetProductionLine(productionLineNumber);
            if(productionLine == null)
            {
                Log(new CallbackParameters(CallbackReason.ProductionLineNotLoaded)
                    {
                        Lot = lot,
                        ProductionLineNumber = productionLineNumber
                    });
                return null;
            }

            DateTime productionBegin;
            if(lot.BatchBegTime == null)
            {
                if(lot.ProductionDate == null)
                {
                    Log(new CallbackParameters(CallbackReason.ProductionBeginCouldNotBeDetermined)
                        {
                            Lot = lot
                        });
                    return null;
                }

                Log(new CallbackParameters(CallbackReason.ProductionBeginFromProductionDate)
                    {
                        Lot = lot
                    });
                productionBegin = lot.ProductionDate.Value.ConvertLocalToUTC();
            }
            else
            {
                productionBegin = lot.BatchBegTime.Value.ConvertLocalToUTC();
            }

            DateTime productionEnd;
            if(lot.BatchEndTime == null)
            {
                if(lot.ProductionDate == null)
                {
                    Log(new CallbackParameters(CallbackReason.ProductionEndTimeCouldNotBeDetermined)
                        {
                            Lot = lot
                        });
                    return null;
                }

                Log(new CallbackParameters(CallbackReason.ProductionEndTimeFromProductionDate)
                    {
                        Lot = lot
                    });
                productionEnd = lot.ProductionDate.Value.ConvertLocalToUTC();
            }
            else
            {
                productionEnd = lot.BatchEndTime.Value.ConvertLocalToUTC();
            }

            return new LotProductionResults
                {
                    EmployeeId = employeeId,
                    TimeStamp = entryDate,

                    LotDateCreated = lotKey.LotKey_DateCreated,
                    LotDateSequence = lotKey.LotKey_DateSequence,
                    LotTypeId = lotKey.LotKey_LotTypeId,

                    ProductionLineLocationId = productionLine.Id,

                    ShiftKey = lot.Shift,
                    ProductionBegin = productionBegin,
                    ProductionEnd = productionEnd,
                    DateTimeEntered = entryDate,
                    
                    ProductionLineLocation = productionLine,

                    ResultItems = CreateLotProductionResultItems(lotKey, lot.tblIncomings).ToList()
                };
        }

        private IEnumerable<LotProductionResultItem> CreateLotProductionResultItems(ILotKey lotKey, IEnumerable<IncomingDTO> incomings)
        {
            var itemSequence = 0;
            foreach(var incoming in incomings)
            {
                _loadCount.AddRead(EntityTypes.LotProductionResultItem);

                var packaging = _newContextHelper.GetPackagingProduct(incoming.PkgID);
                if(packaging == null)
                {
                    Log(new CallbackParameters(CallbackReason.IncomingPackagingNotLoaded)
                        {
                            Incoming = incoming
                        });
                    continue;
                }

                var warehouseLocation = _newContextHelper.GetLocation(incoming.LocID);
                if(warehouseLocation == null)
                {
                    Log(new CallbackParameters(CallbackReason.IncomingWarehouseLocationNotLoaded)
                        {
                            Incoming = incoming
                        });
                    continue;
                }

                yield return new LotProductionResultItem
                    {
                        LotDateCreated = lotKey.LotKey_DateCreated,
                        LotDateSequence = lotKey.LotKey_DateSequence,
                        LotTypeId = lotKey.LotKey_LotTypeId,
                        ResultItemSequence = ++itemSequence,

                        Quantity = (int) incoming.Quantity,
                        PackagingProductId = packaging.Id,
                        LocationId = warehouseLocation.Id,
                        TreatmentId = _newContextHelper.NoTreatment.Id
                    };
            }
        }

        [Issue("Modified data source to match Access logic. Issue arose from previous logic *not* loading a M&W record that had had no results entered. -RI 2016-12-14",
            References = new[] { "RVCADMIN-1428" })]
        public static List<LotDTO> SelectMillAndWetdownToLoad(ObjectContext oldContext)
        {
            return oldContext.CreateObjectSet<tblLot>()
                .Where(l => l.PTypeID == 2 && l.ProdID != 9998 && l.ProdID != 9999)
                .Select(l => new LotDTO
                    {
                        EmployeeID = l.EmployeeID,
                        EntryDate = l.EntryDate,

                        Lot = l.Lot,
                        ProdID = l.ProdID,
                        Shift = l.Shift,
                        ProductionLine = l.ProductionLine,

                        BatchBegTime = l.BatchBegTime,
                        BatchEndTime = l.BatchEndTime,
                        ProductionDate = l.ProductionDate,

                        tblIncomings = l.tblIncomings.Select(i => new IncomingDTO
                            {
                                Lot = i.Lot,
                                ID = i.ID,
                                PkgID = i.PkgID,
                                LocID = i.LocID,
                                Quantity = i.Quantity
                            }),
                        tblOutgoingInputs = l.tblOutgoingInputs.Select(o => new OutgoingDTO
                            {
                                EmployeeID = o.EmployeeID,
                                EntryDate = o.EntryDate,

                                Lot = o.Lot,
                                PTypeID = o.tblLot.PTypeID,
                                ID = o.ID,
                                PkgID = o.PkgID,
                                Tote = o.Tote,
                                LocID = o.LocID,
                                TrtmtID = o.TrtmtID,
                                Quantity = o.Quantity,
                                CustProductCode = o.CustProductCode
                            })
                    }).ToList();
        }

        public class LotDTO : IEmployeeIdentifiableDTO
        {
            public int Lot { get; set; }
            public int? ProdID { get; set; }
            public string Shift { get; set; }
            public int? ProductionLine { get; set; }
            public DateTime? BatchBegTime { get; set; }
            public DateTime? BatchEndTime { get; set; }
            public DateTime? ProductionDate { get; set; }
            public DateTime? EntryDate { get; set; }
            public int? EmployeeID { get; set; }
            public DateTime? Timestamp { get { return EntryDate; } }
            public IEnumerable<IncomingDTO> tblIncomings { get; set; }
            public IEnumerable<OutgoingDTO> tblOutgoingInputs { get; set; }
        }

        public class IncomingDTO
        {
            public int Lot { get; set; }
            public int ID { get; set; }
            public int PkgID { get; set; }
            public int LocID { get; set; }
            public decimal? Quantity { get; set; }
        }

        public class OutgoingDTO : IEmployeeIdentifiableDTO
        {
            public int? EmployeeID { get; set; }
            public DateTime EntryDate { get; set; }
            public int Lot { get; set; }
            public int? PTypeID { get; set; }
            public int ID { get; set; }
            public int PkgID { get; set; }
            public string Tote { get; set; }
            public int LocID { get; set; }
            public int TrtmtID { get; set; }
            public decimal? Quantity { get; set; }
            public DateTime? Timestamp { get { return EntryDate; } }
            public string CustProductCode { get; set; }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            ChileLotNotLoaded,
            DefaultProductionLine,
            InvalidLotNumber,
            NullEntryDate,
            ProductionLineNotLoaded,
            NullProdID,
            ChileProductNotLoaded,
            ProductionBeginCouldNotBeDetermined,
            ProductionEndTimeCouldNotBeDetermined,
            OutgoingInvalidLotNumber,
            OutgoingPackagingNotLoaded,
            OutgoingWarehouseLocationNotLoaded,
            OutgoingTreatmentNotLoaded,
            IncomingPackagingNotLoaded,
            IncomingWarehouseLocationNotLoaded,
            OutgoingLotNotLoaded,
            ProductionBeginFromProductionDate,
            ProductionEndTimeFromProductionDate,
            DefaultEmployee,
            PickedInventoryUndeterminedTimestamp,
            PickedInventoryUsedDefaultEmployee,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public LotDTO Lot { get; set; }

            public LotKey LotKey { get; set; }

            public Location ProductionLine { get; set; }

            public int ProductionLineNumber { get; set; }

            public OutgoingDTO Outgoing { get; set; }

            public IncomingDTO Incoming { get; set; }

            public int EmployeeId { get; set; }

            protected override CallbackReason ExceptionReason { get { return MillAndWetdownMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return MillAndWetdownMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return MillAndWetdownMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case MillAndWetdownMother.CallbackReason.Exception: return ReasonCategory.Error;
                        
                    case MillAndWetdownMother.CallbackReason.ChileLotNotLoaded:
                    case MillAndWetdownMother.CallbackReason.InvalidLotNumber:
                    case MillAndWetdownMother.CallbackReason.NullEntryDate:
                    case MillAndWetdownMother.CallbackReason.ProductionLineNotLoaded:
                    case MillAndWetdownMother.CallbackReason.NullProdID:
                    case MillAndWetdownMother.CallbackReason.ChileProductNotLoaded:
                    case MillAndWetdownMother.CallbackReason.ProductionBeginCouldNotBeDetermined:
                    case MillAndWetdownMother.CallbackReason.ProductionEndTimeCouldNotBeDetermined:
                    case MillAndWetdownMother.CallbackReason.OutgoingInvalidLotNumber:
                    case MillAndWetdownMother.CallbackReason.OutgoingPackagingNotLoaded:
                    case MillAndWetdownMother.CallbackReason.OutgoingWarehouseLocationNotLoaded:
                    case MillAndWetdownMother.CallbackReason.OutgoingTreatmentNotLoaded:
                    case MillAndWetdownMother.CallbackReason.IncomingPackagingNotLoaded:
                    case MillAndWetdownMother.CallbackReason.IncomingWarehouseLocationNotLoaded:
                    case MillAndWetdownMother.CallbackReason.OutgoingLotNotLoaded:
                    case MillAndWetdownMother.CallbackReason.PickedInventoryUndeterminedTimestamp:
                        return ReasonCategory.RecordSkipped;

                    case MillAndWetdownMother.CallbackReason.PickedInventoryUsedDefaultEmployee:
                    case MillAndWetdownMother.CallbackReason.DefaultEmployee:
                        return ReasonCategory.Informational;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}