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
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class IntraWarehouseOrdersMother : EntityMotherLogBase<IntraWarehouseOrder, IntraWarehouseOrdersMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;

        public IntraWarehouseOrdersMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
        }

        private enum EntityTypes
        {
            IntraWarehouseOrder,
            PickedInventory,
            PickedInventoryItem
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<IntraWarehouseOrder> BirthRecords()
        {
            _loadCount.Reset();

            foreach(var rinconRecord in SelectRinconRecordsToLoad())
            {
                _loadCount.AddRead(EntityTypes.IntraWarehouseOrder);

                var dateCreated = rinconRecord.RinconID.Date.Date;
                var sequence = PickedInventoryKeyHelper.Singleton.GetNextSequence(dateCreated);

                var moveDate = rinconRecord.MoveDate ?? dateCreated;
                var timestamp = rinconRecord.RinconID.ConvertLocalToUTC();

                var employeeId = _newContextHelper.DefaultEmployee.EmployeeId;
                if(rinconRecord.EmployeeID == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullEmployeeID)
                        {
                            Rincon = rinconRecord
                        });
                }
                else
                {
                    employeeId = rinconRecord.EmployeeID.Value;
                }

                var intraWarehouseOrder = new IntraWarehouseOrder
                    {
                        DateCreated = dateCreated,
                        Sequence = sequence,
                        EmployeeId = employeeId,
                        TimeStamp = timestamp,

                        TrackingSheetNumber = rinconRecord.SheetNum,
                        OperatorName = rinconRecord.PrepBy,
                        MovementDate = moveDate,
                        RinconID = rinconRecord.RinconID
                    };

                intraWarehouseOrder.PickedInventory = CreatePickedInventory(intraWarehouseOrder, rinconRecord);
                if(intraWarehouseOrder.PickedInventory == null)
                {
                    continue;
                }

                _loadCount.AddLoaded(EntityTypes.IntraWarehouseOrder);
                _loadCount.AddLoaded(EntityTypes.PickedInventory);
                _loadCount.AddLoaded(EntityTypes.PickedInventoryItem, (uint) intraWarehouseOrder.PickedInventory.Items.Count);

                yield return intraWarehouseOrder;
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private PickedInventory CreatePickedInventory(IPickedInventoryKey pickedInventoryKey, tblRinconDTO rincon)
        {
            _loadCount.AddRead(EntityTypes.PickedInventory);
            
            var identifiable = rincon.tblRinconDetails.Cast<IEmployeeIdentifiableDTO>().ToList();
            identifiable.Add(rincon);
            
            int? employeeID;
            DateTime? timestamp;
            identifiable.GetLatest(out employeeID, out timestamp);

            if(timestamp == null)
            {
                Log(new CallbackParameters(CallbackReason.PickedInventoryUndeterminedTimestamp)
                    {
                        Rincon = rincon
                    });
                return null;
            }

            employeeID = employeeID ?? _newContextHelper.DefaultEmployee.EmployeeId;
            if(employeeID == _newContextHelper.DefaultEmployee.EmployeeId)
            {
                Log(new CallbackParameters(CallbackReason.PickedInventoryUsedDefaultEmployee)
                    {
                        Rincon = rincon,
                        DefaultEmployeeID = employeeID.Value
                    });
            }

            return new PickedInventory
                {
                    EmployeeId = employeeID.Value,
                    TimeStamp = timestamp.Value,
                    
                    DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                    Sequence = pickedInventoryKey.PickedInventoryKey_Sequence,

                    Archived = true,
                    PickedReason = PickedReason.IntraWarehouseMovement,
                    Items = CreatePickedInventoryItems(pickedInventoryKey, rincon).ToList()
                };
        }

        private IEnumerable<PickedInventoryItem> CreatePickedInventoryItems(IPickedInventoryKey pickedInventoryKey, tblRinconDTO rincon)
        {
            var sequence = 0;
            foreach(var detail in rincon.tblRinconDetails)
            {
                _loadCount.AddRead(EntityTypes.PickedInventoryItem);

                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(detail.Lot, out lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.InvalidLotKey)
                        {
                            Detail = detail
                        });
                    continue;
                }

                if(!_newContextHelper.LotLoaded(lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.LotNotLoaded)
                        {
                            Detail = detail
                        });
                    continue;
                }

                var packagingProduct = detail.PTypeID == (int?)LotTypeEnum.Packaging ? _newContextHelper.NoPackagingProduct : _newContextHelper.GetPackagingProduct(detail.PkgID);
                if(packagingProduct == null)
                {
                    Log(new CallbackParameters(CallbackReason.PackagingNotLoaded)
                        {
                            Detail = detail
                        });
                    continue;
                }
                
                var fromWarehouseLocation = _newContextHelper.GetLocation(detail.CurrLocID);
                if(fromWarehouseLocation == null)
                {
                    Log(new CallbackParameters(CallbackReason.FromWarehouseLocationNotLoaded)
                        {
                            Detail = detail
                        });
                    continue;
                }

                var treatment = _newContextHelper.GetInventoryTreatment(detail.TrtmtID);
                if(treatment == null)
                {
                    Log(new CallbackParameters(CallbackReason.TreatmentNotLoaded)
                        {
                            Detail = detail
                        });
                    continue;
                }

                var destinationLocation = _newContextHelper.GetLocation(detail.DestLocID);
                if(destinationLocation == null)
                {
                    Log(new CallbackParameters(CallbackReason.DestinationLocationNotLoaded)
                        {
                            Detail = detail
                        });
                    continue;
                }

                if(detail.Quantity == null)
                {
                    Log(new CallbackParameters(CallbackReason.MissingQuantity)
                        {
                            Detail = detail
                        });
                    continue;
                }

                yield return new PickedInventoryItem
                    {
                        DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                        Sequence = pickedInventoryKey.PickedInventoryKey_Sequence,
                        ItemSequence = ++sequence,

                        LotDateCreated = lotKey.LotKey_DateCreated,
                        LotDateSequence = lotKey.LotKey_DateSequence,
                        LotTypeId = lotKey.LotKey_LotTypeId,

                        PackagingProductId = packagingProduct.Id,
                        FromLocationId = fromWarehouseLocation.Id,
                        TreatmentId = treatment.Id,
                        CurrentLocationId = destinationLocation.Id,
                        ToteKey = "",
                        Quantity = (int) detail.Quantity
                    };
            }
        }

        private List<tblRinconDTO> SelectRinconRecordsToLoad()
        {
            return OldContext.CreateObjectSet<tblRincon>()
                .Where(r => r.Updated != null)
                .Select(r => new tblRinconDTO
                    {
                        EmployeeID = r.EmployeeID,
                        RinconID = r.RinconID,
                        MoveDate = r.MoveDate,
                        SheetNum = r.SheetNum,
                        PrepBy = r.PrepBy,

                        tblRinconDetails = r.tblRinconDetails.Select(d => new tblRinconDetailDTO
                            {
                                RDetailID = d.RDetailID,
                                EmployeeID = d.EmployeeID,
                                RinconID = d.RinconID,
                                Lot = d.Lot,
                                PTypeID = d.tblLot.PTypeID,
                                PkgID = d.PkgID,
                                TrtmtID = d.TrtmtID,
                                Quantity = d.Quantity,
                                CurrLocID = d.CurrLocID,
                                DestLocID = d.DestLocID
                            })
                    })
                .ToList();
        }

        public class tblRinconDTO : IEmployeeIdentifiableDTO
        {
            public int? EmployeeID { get; set; }

            public DateTime RinconID { get; set; }

            public DateTime? MoveDate { get; set; }

            public decimal SheetNum { get; set; }

            public string PrepBy { get; set; }

            public IEnumerable<tblRinconDetailDTO> tblRinconDetails { get; set; }

            public DateTime? Timestamp { get { return RinconID; } }
        }

        public class tblRinconDetailDTO : IEmployeeIdentifiableDTO
        {
            public int? EmployeeID { get; set; }

            public DateTime RDetailID { get; set; }

            public DateTime? RinconID { get; set; }

            public int Lot { get; set; }

            public int? PTypeID { get; set; }

            public int? PkgID { get; set; }

            public int TrtmtID { get; set; }

            public decimal? Quantity { get; set; }

            public int CurrLocID { get; set; }

            public int DestLocID { get; set; }

            public DateTime? Timestamp { get { return RDetailID; } }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            InvalidLotKey,
            FromWarehouseLocationNotLoaded,
            MissingQuantity,
            PackagingNotLoaded,
            TreatmentNotLoaded,
            DestinationLocationNotLoaded,
            LotNotLoaded,
            PickedInventoryUndeterminedTimestamp,
            PickedInventoryUsedDefaultEmployee,
            StringTruncated,
            NullEmployeeID
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public tblRinconDTO Rincon { get; set; }

            public tblRinconDetailDTO Detail { get; set; }

            public int DefaultEmployeeID { get; set; }

            protected override CallbackReason ExceptionReason { get { return IntraWarehouseOrdersMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return IntraWarehouseOrdersMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return IntraWarehouseOrdersMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case IntraWarehouseOrdersMother.CallbackReason.Exception: return ReasonCategory.Error;
                        
                    case IntraWarehouseOrdersMother.CallbackReason.InvalidLotKey:
                    case IntraWarehouseOrdersMother.CallbackReason.FromWarehouseLocationNotLoaded:
                    case IntraWarehouseOrdersMother.CallbackReason.MissingQuantity:
                    case IntraWarehouseOrdersMother.CallbackReason.PackagingNotLoaded:
                    case IntraWarehouseOrdersMother.CallbackReason.TreatmentNotLoaded:
                    case IntraWarehouseOrdersMother.CallbackReason.DestinationLocationNotLoaded:
                    case IntraWarehouseOrdersMother.CallbackReason.LotNotLoaded:
                    case IntraWarehouseOrdersMother.CallbackReason.PickedInventoryUndeterminedTimestamp:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}