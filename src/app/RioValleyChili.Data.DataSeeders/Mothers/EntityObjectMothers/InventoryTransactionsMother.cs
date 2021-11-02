using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class InventoryTransactionsMother : EntityMotherLogBase<InventoryTransaction, InventoryTransactionsMother.CallbackParameters>
    {
        private static readonly DateTime? CutoffDate = new DateTime(2014, 1, 1);
        private readonly NewContextHelper _newContextHelper;

        public InventoryTransactionsMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
        }

        private enum EntityTypes
        {
            InventoryTransactions
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();
        private Dictionary<DateTime, int> _dateSequences;

        protected override IEnumerable<InventoryTransaction> BirthRecords()
        {
            _loadCount.Reset();
            _dateSequences = new Dictionary<DateTime, int>();

            foreach(var incoming in GetIncoming())
            {
                _loadCount.AddRead(EntityTypes.InventoryTransactions);
                var result = Process(incoming);

                if(result != null)
                {
                    _loadCount.AddLoaded(EntityTypes.InventoryTransactions);
                    yield return result;
                }
            }

            foreach(var outgoing in GetOutgoing())
            {
                _loadCount.AddRead(EntityTypes.InventoryTransactions);
                var result = Process(outgoing);

                if(result != null)
                {
                    _loadCount.AddLoaded(EntityTypes.InventoryTransactions);
                    yield return result;
                }
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private InventoryTransaction Process(TransactionDTO transaction)
        {
            var employeeId = _newContextHelper.DefaultEmployee.EmployeeId;
            if(transaction.EmployeeID == null)
            {
                Log(new CallbackParameters(CallbackReason.NullEmployeeID)
                    {
                        Transaction = transaction
                    });
            }
            else
            {
                employeeId = transaction.EmployeeID.Value;
            }

            LotKey lotKey;
            if(!LotNumberParser.ParseLotNumber(transaction.Lot, out lotKey))
            {
                Log(new CallbackParameters(CallbackReason.CannotParseLotNumber)
                    {
                        Transaction = transaction
                    });
                return null;
            }

            if(!_newContextHelper.LotLoaded(lotKey))
            {
                Log(new CallbackParameters(CallbackReason.LotNotLoaded)
                    {
                        Transaction = transaction
                    });
                return null;
            }

            LotKey newLotKey = null;
            if(transaction.NewLot != null)
            {
                if(!LotNumberParser.ParseLotNumber(transaction.NewLot.Value, out newLotKey))
                {
                    Log(new CallbackParameters(CallbackReason.CannotParseDestLotNumber)
                        {
                            Transaction = transaction
                        });
                    return null;
                }

                if(!_newContextHelper.LotLoaded(newLotKey))
                {
                    Log(new CallbackParameters(CallbackReason.DestLotNotLoaded)
                        {
                            Transaction = transaction
                        });
                    return null;
                }
            }

            PackagingProduct packaging;
            if(_newContextHelper.GetPackagingLotWithProduct(lotKey) != null)
            {
                packaging = _newContextHelper.NoPackagingProduct;
            }
            else
            {
                packaging = _newContextHelper.GetPackagingProduct(transaction.PkgID);
                if(packaging == null)
                {
                    Log(new CallbackParameters(CallbackReason.PackagingNotLoaded)
                        {
                            Transaction = transaction
                        });
                    return null;
                }
            }

            var location = _newContextHelper.GetLocation(transaction.LocID);
            if(location == null)
            {
                Log(new CallbackParameters(CallbackReason.LocationNotLoaded)
                    {
                        Transaction = transaction
                    });
                return null;
            }

            var treatment = _newContextHelper.GetInventoryTreatment(transaction.TrtmtID);
            if(treatment == null)
            {
                Log(new CallbackParameters(CallbackReason.TreatmentNotLoaded)
                    {
                        Transaction = transaction
                    });
                return null;
            }

            var date = transaction.EntryDate.Date;
            return SetTransactionType(new InventoryTransaction
                {
                    DateCreated = date,
                    Sequence = GetSequence(date),

                    TimeStamp = transaction.EntryDate.ConvertLocalToUTC(),
                    EmployeeId = employeeId,

                    SourceLotDateCreated = lotKey.LotKey_DateCreated,
                    SourceLotDateSequence = lotKey.LotKey_DateSequence,
                    SourceLotTypeId = lotKey.LotKey_LotTypeId,

                    PackagingProductId = packaging.Id,
                    LocationId = location.Id,
                    TreatmentId = treatment.Id,
                    ToteKey = transaction.Tote ?? "",
                    Quantity = (int) (transaction.Quantity ?? 0),

                    DestinationLotDateCreated = newLotKey != null ? newLotKey.LotKey_DateCreated : (DateTime?)null,
                    DestinationLotDateSequence = newLotKey != null ? newLotKey.LotKey_DateSequence : (int?)null,
                    DestinationLotTypeId = newLotKey != null ? newLotKey.LotKey_LotTypeId : (int?)null
                }, transaction);
        }

        [Issue("There was an issue regarding consignment order transaction failing to load due to looking up the order as an InterWarehouseOrder and not finding it." +
               "Fixed the issue by modifying the logic to look for ConsignmentOrders. -RI 2016-9-19",
               References = new[]{ "RVCADMIN-1300" })]
        private InventoryTransaction SetTransactionType(InventoryTransaction transaction, TransactionDTO oldTransaction)
        {
            switch(oldTransaction.TTypeID.ToTransType())
            {
                case TransType.DeHy:
                    transaction.TransactionType = InventoryTransactionType.ReceivedDehydratedMaterials;
                    transaction.SourceReference = new LotKey(transaction);
                    break;
                    
                case TransType.MnW:
                    transaction.TransactionType = InventoryTransactionType.CreatedMillAndWetdown;
                    transaction.SourceReference = new LotKey(transaction);
                    break;

                case TransType.Batching:
                case TransType.Production:
                    transaction.TransactionType = InventoryTransactionType.ProductionResults;

                    var packSchedule = _newContextHelper.GetPackSchedule(oldTransaction.PackSchId);
                    var packScheduleKey = packSchedule != null ? new PackScheduleKey(packSchedule) : _newContextHelper.GetLotPackScheduleKey(transaction);
                    if(packScheduleKey != null)
                    {
                        transaction.SourceReference = packScheduleKey;
                    }
                    else
                    {
                        transaction.SourceReference = new LotKey(transaction);
                    }
                    break;

                case TransType.Rincon:
                    transaction.TransactionType = InventoryTransactionType.InternalMovement;
                    var order = _newContextHelper.GetIntraWarehouseOrder(oldTransaction.RinconID);
                    if(order == null)
                    {
                        Log(new CallbackParameters(CallbackReason.IntraWarehouseOrderNotLoaded)
                            {
                                Transaction = oldTransaction
                            });
                        return null;
                    }
                    transaction.SourceReference = order.TrackingSheetNumber.ToString();
                    break;

                case TransType.Move:
                case TransType.OnConsignment:
                    transaction.TransactionType = InventoryTransactionType.PostedInterWarehouseOrder;
                    var shipmentOrder = _newContextHelper.GetInventoryShipmentOrder(oldTransaction.MoveNum, InventoryShipmentOrderTypeEnum.InterWarehouseOrder, InventoryShipmentOrderTypeEnum.ConsignmentOrder);
                    if(shipmentOrder == null)
                    {
                        Log(new CallbackParameters(CallbackReason.InventoryShipmentOrderNotLoaded)
                            {
                                OrderType = InventoryShipmentOrderTypeEnum.InterWarehouseOrder,
                                Transaction = oldTransaction
                            });
                        return null;
                    }
                    transaction.SourceReference = shipmentOrder.ToInventoryShipmentOrderKey();
                    break;

                case TransType.Sale:
                    transaction.TransactionType = InventoryTransactionType.PostedCustomerOrder;
                    shipmentOrder = _newContextHelper.GetInventoryShipmentOrder(oldTransaction.OrderNum, InventoryShipmentOrderTypeEnum.SalesOrder);
                    if(shipmentOrder == null)
                    {
                        Log(new CallbackParameters(CallbackReason.InventoryShipmentOrderNotLoaded)
                            {
                                OrderType = InventoryShipmentOrderTypeEnum.SalesOrder,
                                Transaction = oldTransaction
                            });
                        return null;
                    }
                    transaction.SourceReference = oldTransaction.OrderNum.ToString();
                    break;

                case TransType.MiscInvoice:
                    transaction.TransactionType = InventoryTransactionType.PostedMiscellaneousOrder;
                    shipmentOrder = _newContextHelper.GetInventoryShipmentOrder(oldTransaction.OrderNum, InventoryShipmentOrderTypeEnum.MiscellaneousOrder);
                    if(shipmentOrder == null)
                    {
                        Log(new CallbackParameters(CallbackReason.InventoryShipmentOrderNotLoaded)
                            {
                                OrderType = InventoryShipmentOrderTypeEnum.MiscellaneousOrder,
                                Transaction = oldTransaction
                            });
                        return null;
                    }
                    transaction.SourceReference = oldTransaction.OrderNum.ToString();
                    break;

                case TransType.ToTrmt:
                    transaction.TransactionType = InventoryTransactionType.PostedTreatmentOrder;
                    shipmentOrder = _newContextHelper.GetInventoryShipmentOrder(oldTransaction.MoveNum, InventoryShipmentOrderTypeEnum.TreatmentOrder);
                    if(shipmentOrder == null)
                    {
                        Log(new CallbackParameters(CallbackReason.InventoryShipmentOrderNotLoaded)
                            {
                                OrderType = InventoryShipmentOrderTypeEnum.TreatmentOrder,
                                Transaction = oldTransaction
                            });
                        return null;
                    }
                    transaction.SourceReference = new InventoryShipmentOrderKey(shipmentOrder);
                    break;

                case TransType.FrmTrmt:
                    transaction.TransactionType = InventoryTransactionType.ReceiveTreatmentOrder;
                    shipmentOrder = _newContextHelper.GetInventoryShipmentOrder(oldTransaction.MoveNum, InventoryShipmentOrderTypeEnum.TreatmentOrder);
                    if(shipmentOrder == null)
                    {
                        Log(new CallbackParameters(CallbackReason.InventoryShipmentOrderNotLoaded)
                            {
                                OrderType = InventoryShipmentOrderTypeEnum.TreatmentOrder,
                                Transaction = oldTransaction
                            });
                        return null;
                    }
                    transaction.SourceReference = new InventoryShipmentOrderKey(shipmentOrder);
                    break;

                case TransType.InvAdj:
                    transaction.TransactionType = InventoryTransactionType.InventoryAdjustment;
                    var adjustment = _newContextHelper.GetInventoryAdjustment(oldTransaction.AdjustID.ConvertLocalToUTC());
                    if(adjustment == null)
                    {
                        Log(new CallbackParameters(CallbackReason.InventoryAdjustmentNotLoaded)
                            {
                                Transaction = oldTransaction
                            });
                        return null;
                    }
                    transaction.SourceReference = new InventoryAdjustmentKey(adjustment);
                    break;

                case TransType.Other:
                case TransType.GRP:
                case TransType.Ingredients:
                case TransType.Packaging:
                    transaction.TransactionType = InventoryTransactionType.ReceiveInventory;
                    transaction.SourceReference = null;
                    break;

                //case TransType.Rework:
                //case TransType.ConsignmentSale:
                //case TransType.MiscInvoice:
                //case TransType.Quote:
                default:
                    Log(new CallbackParameters(CallbackReason.UnmappedTTypeID)
                        {
                            Transaction = oldTransaction
                        });
                    return null;
            }

            transaction.Description = InventoryTransactionsHelper.GetDescription(transaction.TransactionType);
            return transaction;
        }

        private int GetSequence(DateTime date)
        {
            date = date.Date;

            int sequence;
            if(_dateSequences.TryGetValue(date, out sequence))
            {
                _dateSequences[date] = (sequence += 1);
            }
            else
            {
                _dateSequences.Add(date, sequence = 1);
            }

            return sequence;
        }

        private IEnumerable<TransactionDTO> GetIncoming()
        {
            return OldContext.CreateObjectSet<tblIncoming>()
                .OrderByDescending(i => i.EntryDate)
                .Where(i => CutoffDate == null || i.EntryDate > CutoffDate)
                .Select(i => new TransactionDTO
                    {
                        EmployeeID = i.EmployeeID,
                        EntryDate = i.EntryDate,
                        TTypeID = i.TTypeID,

                        Lot = i.Lot,
                        PkgID = i.PkgID,
                        LocID = i.LocID,
                        TrtmtID = i.TrtmtID,
                        Tote = i.Tote,
                        Quantity = i.Quantity,

                        IncomingID = i.ID,
                    });
        }

        private IEnumerable<TransactionDTO> GetOutgoing()
        {
            return OldContext.CreateObjectSet<tblOutgoing>()
                .OrderByDescending(o => o.EntryDate)
                .Where(o => CutoffDate == null || o.EntryDate > CutoffDate)
                .Select(o => new TransactionDTO
                    {
                        EmployeeID = o.EmployeeID,
                        EntryDate = o.EntryDate,
                        TTypeID = o.TTypeID,

                        Lot = o.Lot,
                        PkgID = o.PkgID,
                        LocID = o.LocID,
                        TrtmtID = o.TrtmtID,
                        Tote = o.Tote,
                        Quantity = -o.Quantity,
                        NewLot = o.NewLot,

                        OutgoingID = o.ID,
                        RinconID = o.RinconID,
                        MoveNum = o.MoveNum,
                        OrderNum = o.OrderNum,
                        AdjustID = o.AdjustID,
                        PackSchId = o.PackSchID
                    });
        }

        public class TransactionDTO
        {
            public int? OutgoingID { get; set; }
            public int? IncomingID { get; set; }
            public int? TTypeID { get; set; }

            public int? EmployeeID { get; set; }
            public DateTime EntryDate { get; set; }

            public int Lot { get; set; }
            public int PkgID { get; set; }
            public int LocID { get; set; }
            public int TrtmtID { get; set; }
            public string Tote { get; set; }
            public decimal? Quantity { get; set; }
            public int? NewLot { get; set; }

            public DateTime? RinconID { get; set; }
            public int? MoveNum { get; set; }
            public int? OrderNum { get; set; }
            public DateTime? AdjustID { get; set; }
            public DateTime? PackSchId { get; set; }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,

            UnmappedTTypeID,
            NullEmployeeID,
            CannotParseLotNumber,
            CannotParseDestLotNumber,
            LotNotLoaded,
            DestLotNotLoaded,
            PackagingNotLoaded,
            LocationNotLoaded,
            TreatmentNotLoaded,
            IntraWarehouseOrderNotLoaded,
            InventoryShipmentOrderNotLoaded,
            InventoryAdjustmentNotLoaded,
            PackScheduleNotLoaded
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public InventoryShipmentOrderTypeEnum OrderType { get; set; }
            public TransactionDTO Transaction { get; set; }

            protected override CallbackReason ExceptionReason { get { return InventoryTransactionsMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return InventoryTransactionsMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return InventoryTransactionsMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case InventoryTransactionsMother.CallbackReason.UnmappedTTypeID:
                    case InventoryTransactionsMother.CallbackReason.CannotParseLotNumber:
                    case InventoryTransactionsMother.CallbackReason.CannotParseDestLotNumber:
                    case InventoryTransactionsMother.CallbackReason.LotNotLoaded:
                    case InventoryTransactionsMother.CallbackReason.DestLotNotLoaded:
                    case InventoryTransactionsMother.CallbackReason.PackagingNotLoaded:
                    case InventoryTransactionsMother.CallbackReason.LocationNotLoaded:
                    case InventoryTransactionsMother.CallbackReason.TreatmentNotLoaded:
                    case InventoryTransactionsMother.CallbackReason.IntraWarehouseOrderNotLoaded:
                    case InventoryTransactionsMother.CallbackReason.InventoryShipmentOrderNotLoaded:
                    case InventoryTransactionsMother.CallbackReason.InventoryAdjustmentNotLoaded:
                    case InventoryTransactionsMother.CallbackReason.PackScheduleNotLoaded:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}