using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class LotPredicateBuilder
    {
#warning Implement ProductionStart, ProductionEnd, and ProductionStatus filtering
        internal static IResult<Expression<Func<Lot, bool>>> BuildPredicate(ILotUnitOfWork lotUnitOfWork, FilterLotParameters parameters = null)
        {
            var predicate = PredicateBuilder.True<Lot>();

            if(parameters != null)
            {
                if(parameters.LotType != null)
                {
                    predicate = predicate.And(LotPredicates.FilterByLotType(parameters.LotType.Value).ExpandAll());
                }

                if(parameters.ProductType != null)
                {
                    predicate = predicate.And(LotPredicates.FilterByProductType(lotUnitOfWork, parameters.ProductType.Value).ExpandAll());
                }

                if (!string.IsNullOrWhiteSpace(parameters.ProductKey))
                {
                    var productKeyResult = KeyParserHelper.ParseResult<IProductKey>(parameters.ProductKey);
                    if (!productKeyResult.Success)
                    {
                        return productKeyResult.ConvertTo<Expression<Func<Lot, bool>>>(null);
                    }
                    predicate = predicate.And(LotPredicates.FilterByProductKey(lotUnitOfWork, new ProductKey(productKeyResult.ResultingObject)).ExpandAll());
                }
                
                if(parameters.ProductionStartRangeStart != null || parameters.ProductionStartRangeEnd != null)
                {
                    predicate = predicate.And(LotPredicates.FilterByLotProductionStart(lotUnitOfWork, parameters.ProductionStartRangeStart, parameters.ProductionStartRangeEnd).ExpandAll());
                }

                if(parameters.ProductionStatus != null)
                {
                    predicate = predicate.And(LotPredicates.FilterByProductionStatus(parameters.ProductionStatus.Value).ExpandAll());
                }

                if(parameters.QualityStatus != null)
                {
                    predicate = predicate.And(LotPredicates.FilterByQualityStatus(parameters.QualityStatus.Value).ExpandAll());
                }

                if(parameters.ProductSpecComplete != null)
                {
                    predicate = predicate.And(LotPredicates.FilterByProductSpecComplete(parameters.ProductSpecComplete.Value).ExpandAll());
                }

                if(parameters.ProductSpecOutOfRange != null)
                {
                    predicate = predicate.And(LotPredicates.FilterByProductSpecOutOfRange(parameters.ProductSpecOutOfRange.Value).ExpandAll());
                }

                if(!string.IsNullOrWhiteSpace(parameters.StartingLotKey))
                {
                    var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.StartingLotKey);
                    if(!lotKeyResult.Success)
                    {
                        return lotKeyResult.ConvertTo<Expression<Func<Lot, bool>>>(null);
                    }

                    predicate = predicate.And(LotPredicates.FilterByStartingLotKey(lotKeyResult.ResultingObject).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<Lot, bool>>>(predicate.ExpandAll());
        }
    }
}