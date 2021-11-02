using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ProductionSchedulePredicateBuilder
    {
        internal static IResult<Expression<Func<ProductionSchedule, bool>>> BuildPredicate(PredicateBuilderFilters filters)
        {
            var predicate = PredicateBuilder.True<ProductionSchedule>();

            if(filters != null)
            {
                if(filters.ProductionDate != null)
                {
                    predicate = predicate.And(ProductionSchedulePredicates.ByProductionDate(filters.ProductionDate.Value).ExpandAll());
                }

                if(filters.ProductionLineLocationKey != null)
                {
                    predicate = predicate.And(ProductionSchedulePredicates.ByProductionLineLocationKey(filters.ProductionLineLocationKey).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<ProductionSchedule, bool>>>(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            internal DateTime? ProductionDate { get; set; }
            internal LocationKey ProductionLineLocationKey { get; set; }
        }
    }
}