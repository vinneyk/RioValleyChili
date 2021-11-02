using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class ProductionScheduleEntityObjectMother : EntityMotherLogBase<ProductionSchedule, ProductionScheduleEntityObjectMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;
        

        public ProductionScheduleEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
            _loadCount = new MotherLoadCount<EntityTypes>();
        }

        public enum EntityTypes
        {
            ProductionSchedule,
            ProductionScheduleItem
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount;

        protected override IEnumerable<ProductionSchedule> BirthRecords()
        {
            _loadCount.Reset();

            foreach(var oldProductionSchedule in SelectProductionSchedulesToLoad(OldContext))
            {
                _loadCount.AddRead(EntityTypes.ProductionSchedule);

                var productionLine = _newContextHelper.GetProductionLine((int)oldProductionSchedule.LineNumber);
                if(productionLine == null)
                {
                    Log(new CallbackParameters(CallbackReason.ProductionLineNotLoaded)
                        {
                            ProductionSchedule = oldProductionSchedule,
                            Line = oldProductionSchedule.LineNumber
                        });
                    continue;
                }

                var productionSchedule = new ProductionSchedule
                    {
                        ProductionDate = oldProductionSchedule.ProductionDate,
                        ProductionLineLocationId = productionLine.Id,
                        ScheduledItems = new List<ProductionScheduleItem>(),

                        TimeStamp = oldProductionSchedule.DateCreated.Value.ConvertLocalToUTC(),
                        EmployeeId = oldProductionSchedule.CreatedBy.Value
                    };

                foreach(var oldScheduledItems in oldProductionSchedule.ScheduleItems.GroupBy(i => i.Index))
                {
                    var count = 0;
                    foreach(var oldScheduleItem in oldScheduledItems)
                    {
                        _loadCount.AddRead(EntityTypes.ProductionScheduleItem);

                        if(oldScheduleItem.PackSchedule == null)
                        {
                            Log(new CallbackParameters(CallbackReason.NullPackSchedule)
                                {
                                    ProductionSchedule = oldProductionSchedule,
                                    ProductionScheduleItem = oldScheduleItem
                                });
                            continue;
                        }

                        if(count > 0)
                        {
                            Log(new CallbackParameters(CallbackReason.DuplicateIndex)
                                {
                                    ProductionSchedule = oldProductionSchedule,
                                    ProductionScheduleItem = oldScheduleItem
                                });
                            continue;
                        }

                        var packSchedule = _newContextHelper.GetPackSchedule(oldScheduleItem.PackSchedule.PackSchID);
                        if(packSchedule == null)
                        {
                            Log(new CallbackParameters(CallbackReason.PackScheduleNotLoaded)
                                {
                                    ProductionSchedule = oldProductionSchedule,
                                    ProductionScheduleItem = oldScheduleItem
                                });
                            continue;
                        }

                        productionSchedule.ScheduledItems.Add(new ProductionScheduleItem
                            {
                                ProductionDate = productionSchedule.ProductionDate,
                                ProductionLineLocationId = productionSchedule.ProductionLineLocationId,
                                Index = (int) oldScheduleItem.Index,

                                FlushBefore = oldScheduleItem.FlushBefore,
                                FlushBeforeInstructions = oldScheduleItem.FlushBeforeInstructions,
                                FlushAfter = oldScheduleItem.FlushAfter,
                                FlushAfterInstructions = oldScheduleItem.FlushAfterInstructions,

                                PackScheduleDateCreated = packSchedule.DateCreated,
                                PackScheduleSequence = packSchedule.SequentialNumber
                            });

                        count += 1;
                    }
                }

                _loadCount.AddLoaded(EntityTypes.ProductionSchedule);
                _loadCount.AddLoaded(EntityTypes.ProductionScheduleItem, (uint) productionSchedule.ScheduledItems.Count);

                yield return productionSchedule;
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        public static List<ProductionScheduleDTO> SelectProductionSchedulesToLoad(ObjectContext objectContext)
        {
            var packSchedules = objectContext.CreateObjectSet<tblPackSch>();

            return objectContext.CreateObjectSet<tblProductionSchedule>().Select(s => new ProductionScheduleDTO
                {
                    ProductionDate = s.ProductionDate,
                    LineNumber = s.LineNumber,

                    DateCreated = s.DateCreated,
                    CreatedBy = s.CreatedBy,
                    
                    ScheduleItems = s.tblProductionScheduleGroups.Select(g => new ProductionScheduleItemDTO
                        {
                            PSNum = g.PSNum,
                            PackSchedule = packSchedules.Where(p => p.PSNum == g.PSNum)
                                .Select(p => new PackScheduleDTO
                                    {
                                        PackSchID = p.PackSchID
                                    })
                                .FirstOrDefault(),

                            Index = g.Index,
                            FlushBefore = g.FlushBefore,
                            FlushBeforeInstructions = g.FlushBeforeInstructions,
                            FlushAfter = g.FlushAfter,
                            FlushAfterInstructions = g.FlushAfterInstructions
                        })
                }).ToList();
        }

        #region DTOs

        public class ProductionScheduleDTO
        {
            public DateTime ProductionDate { get; set; }
            public float LineNumber { get; set; }

            public DateTime? DateCreated { get; set; }
            public int? CreatedBy { get; set; }

            public IEnumerable<ProductionScheduleItemDTO> ScheduleItems { get; set; }
        }

        public class ProductionScheduleItemDTO
        {
            public float? Index { get; set; }
            public bool FlushBefore { get; set; }
            public string FlushBeforeInstructions { get; set; }
            public bool FlushAfter { get; set; }
            public string FlushAfterInstructions { get; set; }

            public int PSNum { get; set; }
            public PackScheduleDTO PackSchedule { get; set; }
        }

        public class PackScheduleDTO
        {
            public DateTime PackSchID { get; set; }
        }

        #endregion

        public enum CallbackReason
        {
            Exception,
            Summary,
            NullPackSchedule,
            PackScheduleNotLoaded,
            ProductionLineNotLoaded,
            StringTruncated,
            DuplicateIndex
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public ProductionScheduleDTO ProductionSchedule { get; set; }
            public ProductionScheduleItemDTO ProductionScheduleItem { get; set; }

            public int NewInstructionLength { get; set; }
            public int OldInstructionLength { get; set; }
            public float Line { get; set; }

            protected override CallbackReason ExceptionReason { get { return ProductionScheduleEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return ProductionScheduleEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return ProductionScheduleEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case ProductionScheduleEntityObjectMother.CallbackReason.NullPackSchedule:
                    case ProductionScheduleEntityObjectMother.CallbackReason.PackScheduleNotLoaded:
                    case ProductionScheduleEntityObjectMother.CallbackReason.DuplicateIndex:
                    case ProductionScheduleEntityObjectMother.CallbackReason.ProductionLineNotLoaded: return ReasonCategory.RecordSkipped;

                    case ProductionScheduleEntityObjectMother.CallbackReason.Exception: return ReasonCategory.Error;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}