using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;

namespace RioValleyChili.Services.Utilities.Helpers
{
    [ExtractIntoSolutionheadLibrary]
    internal static class PredicateHelper
    {
        internal static Expression<Func<T, bool>> AndExpanded<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> and)
            where T : class
        {
            return predicate.And(and).ExpandAll();
        }

        internal static Expression<Func<T, bool>> OrPredicates<T>(IEnumerable<Expression<Func<T, bool>>> predicateExpressions) where T : class
        {
            if(predicateExpressions == null) { throw new ArgumentNullException("predicateExpressions"); }

            var predicate = PredicateBuilder.False<T>();
            predicateExpressions.ForEach(key => predicate = predicate.Or(key));
            return predicate.ExpandAll();
        }

        internal static Expression<Func<DateTime, bool>> DateTimeInRange(DateTime? start, DateTime? end)
        {
            return dateTime => (start == null || dateTime >= start) && (end == null || dateTime <= end);
        }

        internal static Expression<Func<DateTime, DateTime?, DateTime?, bool>> DateTimeInRange()
        {
            return (dateTime, start, end) => (start == null || dateTime >= start) && (end == null || dateTime <= end);
        }

        internal static Expression<Func<DateTime?, bool>> DateInRange(DateTime? start, DateTime? end)
        {
            start = start.GetDate();
            end = end == null ? (DateTime?)null : end.Value.Date.AddDays(1);

            if(start == null)
            {
                if(end == null)
                {
                    return date => true;
                }

                return date => date != null && date < end;
            }

            if(end == null)
            {
                return date => date != null && date >= start;
            }

            return date => date != null && date >= start && date < end;
        }

        [EdmFunction("Edm", "TruncateTime")]
        internal static DateTime? TruncateTime(DateTime? dateValue)
        {
            return dateValue == null ? (DateTime?)null : dateValue.Value.Date;
        }
    }
}