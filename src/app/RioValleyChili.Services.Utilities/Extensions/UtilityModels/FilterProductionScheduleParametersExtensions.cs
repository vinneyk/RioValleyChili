using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.UtilityModels
{
    internal static class FilterProductionScheduleParametersExtensions
    {
        internal static IResult<ProductionSchedulePredicateBuilder.PredicateBuilderFilters> ToParsedParameters(this FilterProductionScheduleParameters parameters)
        {
            var filterParameters = new ProductionSchedulePredicateBuilder.PredicateBuilderFilters();
            if(parameters != null)
            {
                filterParameters.ProductionDate = parameters.ProductionDate;

                if(!string.IsNullOrWhiteSpace(parameters.ProductionLineLocationKey))
                {
                    var locationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.ProductionLineLocationKey);
                    if(!locationKeyResult.Success)
                    {
                        return locationKeyResult.ConvertTo<ProductionSchedulePredicateBuilder.PredicateBuilderFilters>();
                    }

                    filterParameters.ProductionLineLocationKey = locationKeyResult.ResultingObject.ToLocationKey();
                }
            }

            return new SuccessResult<ProductionSchedulePredicateBuilder.PredicateBuilderFilters>(filterParameters);
        }
    }
}