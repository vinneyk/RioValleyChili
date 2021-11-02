using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal class ChileMaterialsReceivedPredicateBuilder
    {
        public static IResult<Expression<Func<ChileMaterialsReceived, bool>>> BuildPredicate(Parameters parameters)
        {
            var predicate = PredicateBuilder.True<ChileMaterialsReceived>();

            if(parameters.ChileMaterialsType != null)
            {
                predicate = predicate.And(ChileMaterialsReceivedPredicates.ByChileMaterialsType(parameters.ChileMaterialsType).ExpandAll());
            }

            if(parameters.SupplierKey != null)
            {
                predicate = predicate.And(ChileMaterialsReceivedPredicates.BySupplierKey(parameters.SupplierKey).ExpandAll());
            }

            if(parameters.ChileProductKey != null)
            {
                predicate = predicate.And(ChileMaterialsReceivedPredicates.ByChileProductKey(parameters.ChileProductKey).ExpandAll());
            }

            return new SuccessResult<Expression<Func<ChileMaterialsReceived, bool>>>(predicate.ExpandAll());
        }

        internal class Parameters
        {
            internal ChileMaterialsReceivedType? ChileMaterialsType { get; set; }
            internal CompanyKey SupplierKey { get; set; }
            internal ChileProductKey ChileProductKey { get; set; }
        }
    }
}