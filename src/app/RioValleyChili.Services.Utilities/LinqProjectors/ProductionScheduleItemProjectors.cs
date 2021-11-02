using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ProductionScheduleItemProjectors
    {
        internal static IEnumerable<Expression<Func<ProductionScheduleItem, ProductionScheduleItemReturn>>> Select()
        {
            var packSchedule = PackScheduleProjectors.SelectScheduled();

            return new Projectors<ProductionScheduleItem, ProductionScheduleItemReturn>
                {
                    i => new ProductionScheduleItemReturn
                        {
                            Index = i.Index,
                            FlushBefore = i.FlushBefore,
                            FlushBeforeInstructions = i.FlushBeforeInstructions,
                            FlushAfter = i.FlushAfter,
                            FlushAfterInstructions = i.FlushAfterInstructions,
                        },
                    { packSchedule, p => i => new ProductionScheduleItemReturn
                        {
                            PackSchedule = p.Invoke(i.PackSchedule)
                        }
                    }
                };
        }
    }
}