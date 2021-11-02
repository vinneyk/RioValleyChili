using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class InventoryAdjustmentsMother : EntityMotherLogBase<InventoryAdjustment, InventoryAdjustmentsMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;
        private readonly NotebookFactory _notebookFactory;

        public InventoryAdjustmentsMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            
            _newContextHelper = new NewContextHelper(newContext);
            _notebookFactory = NotebookFactory.Create(newContext);
        }

        private enum EntityTypes
        {
            InventoryAdjustment,
            InventoryAdjustmentItem,
            Notebook,
            Note
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<InventoryAdjustment> BirthRecords()
        {
            _loadCount.Reset();
            var adjustments = SelectAdjustmentsToLoad(OldContext);
            var sequences = new Dictionary<DateTime, int>();

            foreach(var adjustment in adjustments)
            {
                _loadCount.AddRead(EntityTypes.InventoryAdjustment);

                var timeStamp = adjustment.AdjustID.ConvertLocalToUTC();
                var adjustmentDate = timeStamp.Date;
                if(!sequences.ContainsKey(adjustmentDate))
                {
                    sequences.Add(adjustmentDate, 0);
                }
                var sequence = sequences[adjustmentDate] += 1;

                var employeeId = adjustment.EmployeeID ?? _newContextHelper.DefaultEmployee.EmployeeId;
                if(adjustment.EmployeeID == null)
                {
                    Log(new CallbackParameters(CallbackReason.DefaultEmployee)
                        {
                            Adjustment = adjustment,
                            EmployeeID = employeeId
                        });
                }
                
                _loadCount.AddRead(EntityTypes.Notebook);
                if(!string.IsNullOrWhiteSpace(adjustment.Reason))
                {
                    _loadCount.AddRead(EntityTypes.Note);
                }
                var notebook = _notebookFactory.BirthNext(timeStamp, employeeId, adjustment.Reason);
                var newAdjustment = new InventoryAdjustment
                    {
                        AdjustmentDate = adjustmentDate,
                        Sequence = sequence,

                        TimeStamp = timeStamp,
                        EmployeeId = employeeId,

                        NotebookDate = notebook.Date,
                        NotebookSequence = notebook.Sequence,
                        Notebook = notebook
                    };

                var adjustmentItems = GetAdjustmentItems(newAdjustment, adjustment.tblOutgoings ?? new List<OutgoingDTO>()).ToList();
                if(!adjustmentItems.Any())
                {
                    Log(new CallbackParameters(CallbackReason.NoOutgoingItems)
                        {
                            Adjustment = adjustment
                        });
                    continue;
                }

                newAdjustment.Items = adjustmentItems;

                _loadCount.AddLoaded(EntityTypes.InventoryAdjustment);
                _loadCount.AddLoaded(EntityTypes.InventoryAdjustmentItem, (uint) newAdjustment.Items.Count);
                _loadCount.AddLoaded(EntityTypes.Notebook);
                _loadCount.AddLoaded(EntityTypes.Note, (uint) newAdjustment.Notebook.Notes.Count);
                yield return newAdjustment;
            }
        }

        private IEnumerable<InventoryAdjustmentItem> GetAdjustmentItems(InventoryAdjustment newAdjustment, IEnumerable<OutgoingDTO> outgoings)
        {
            var adjustmentSequence = 1;

            foreach(var outgoing in outgoings)
            {
                _loadCount.AddRead(EntityTypes.InventoryAdjustmentItem);

                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(outgoing.Lot, out lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.InvalidLotNumber)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                if(!_newContextHelper.LotLoaded(lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.LotNotLoaded)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                var packagingProduct = _newContextHelper.GetPackagingProduct(outgoing.PkgID);
                if(packagingProduct == null)
                {
                    Log(new CallbackParameters(CallbackReason.PackagingNotLoaded)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                var warehouseLocation = _newContextHelper.GetLocation(outgoing.LocID);
                if(warehouseLocation == null)
                {
                    Log(new CallbackParameters(CallbackReason.WarehouseLocationNotLoaded)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                var inventoryTreatment = _newContextHelper.GetInventoryTreatment(outgoing.TrtmtID);
                if(inventoryTreatment == null)
                {
                    Log(new CallbackParameters(CallbackReason.TreatmentNotLoaded)
                        {
                            Outgoing = outgoing
                        });
                    continue;
                }

                var adjustmentItem = new InventoryAdjustmentItem
                    {
                        AdjustmentDate = newAdjustment.AdjustmentDate,
                        Sequence = newAdjustment.Sequence,
                        ItemSequence = adjustmentSequence++,

                        TimeStamp = newAdjustment.TimeStamp,
                        EmployeeId = newAdjustment.EmployeeId,
                        
                        QuantityAdjustment = (int) -outgoing.Quantity,
                        LotDateCreated = lotKey.LotKey_DateCreated,
                        LotDateSequence = lotKey.LotKey_DateSequence,
                        LotTypeId = lotKey.LotKey_LotTypeId,
                        PackagingProductId = packagingProduct.Id,
                        LocationId = warehouseLocation.Id,
                        TreatmentId = inventoryTreatment.Id,
                        ToteKey = outgoing.Tote ?? " "
                    };
                
                yield return adjustmentItem;
            }
        }

        private static List<AdjustmentDTO> SelectAdjustmentsToLoad(ObjectContext oldContext)
        {
            return oldContext.CreateObjectSet<tblAdjust>().Select(a => new AdjustmentDTO
                {
                    AdjustID = a.AdjustID,
                    EmployeeID = a.EmployeeID,
                    Reason = a.Reason,
                    tblOutgoings = a.tblOutgoings.Select(o => new OutgoingDTO
                        {
                            ID = o.ID,
                            AdjustID = o.AdjustID,
                            Quantity = o.Quantity,
                            Lot = o.Lot,
                            PkgID = o.PkgID,
                            LocID = o.LocID,
                            TrtmtID = o.TrtmtID,
                            Tote = o.Tote,
                        })
                }).ToList();
        }

        public class AdjustmentDTO
        {
            public DateTime AdjustID { get; set; }

            public int? EmployeeID { get; set; }

            public string Reason { get; set; }

            public IEnumerable<OutgoingDTO> tblOutgoings { get; set; }
        }

        public class OutgoingDTO
        {
            public int ID { get; set; }

            public DateTime? AdjustID { get; set; }

            public decimal? Quantity { get; set; }

            public int Lot { get; set; }

            public int PkgID { get; set; }

            public int LocID { get; set; }

            public int TrtmtID { get; set; }

            public string Tote { get; set; }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            NoOutgoingItems,
            LotNotLoaded,
            WarehouseLocationNotLoaded,
            InvalidLotNumber,
            PackagingNotLoaded,
            TreatmentNotLoaded,
            DefaultEmployee,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public AdjustmentDTO Adjustment { get; set; }

            public OutgoingDTO Outgoing { get; set; }

            public int EmployeeID { get; set; }

            protected override CallbackReason ExceptionReason { get { return InventoryAdjustmentsMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return InventoryAdjustmentsMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return InventoryAdjustmentsMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case InventoryAdjustmentsMother.CallbackReason.Exception: return ReasonCategory.Error;
                        
                    case InventoryAdjustmentsMother.CallbackReason.NoOutgoingItems:
                    case InventoryAdjustmentsMother.CallbackReason.LotNotLoaded:
                    case InventoryAdjustmentsMother.CallbackReason.WarehouseLocationNotLoaded:
                    case InventoryAdjustmentsMother.CallbackReason.PackagingNotLoaded:
                    case InventoryAdjustmentsMother.CallbackReason.TreatmentNotLoaded:
                    case InventoryAdjustmentsMother.CallbackReason.InvalidLotNumber: return ReasonCategory.RecordSkipped;

                    case InventoryAdjustmentsMother.CallbackReason.DefaultEmployee: return ReasonCategory.Informational;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}