using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class InventoryAdjustmentPredicateBuilder
    {
        internal static IResult<Expression<Func<InventoryAdjustment, bool>>> BuildPredicate(PredicateBuilderFilters parameters = null)
        {
            var predicate = PredicateBuilder.True<InventoryAdjustment>();

            if(parameters != null)
            {
                if(parameters.LotKey != null)
                {
                    predicate = predicate.And(InventoryAdjustmentPredicates.ByLotKey(parameters.LotKey).ExpandAll());
                }

                if(parameters.AdjustmentDateRangeStart != null || parameters.AdjustmentDateRangeEnd != null)
                {
                    predicate = predicate.And(InventoryAdjustmentPredicates.ByTimeStampRange(parameters.AdjustmentDateRangeStart, parameters.AdjustmentDateRangeEnd).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<InventoryAdjustment, bool>>>(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            internal LotKey LotKey = null;

            internal DateTime? AdjustmentDateRangeStart = null;

            internal DateTime? AdjustmentDateRangeEnd = null;
        }
    }
}