using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ChileMaterialsReceivedFiltersExtensions
    {
        internal static IResult<ChileMaterialsReceivedPredicateBuilder.Parameters> ToParsedParameters(this ChileMaterialsReceivedFilters filters)
        {
            var parameters = new ChileMaterialsReceivedPredicateBuilder.Parameters();

            if(filters != null)
            {
                parameters.ChileMaterialsType = filters.ChileMaterialsType;

                if(!string.IsNullOrWhiteSpace(filters.SupplierKey))
                {
                    var supplierKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(filters.SupplierKey);
                    if(!supplierKeyResult.Success)
                    {
                        return supplierKeyResult.ConvertTo<ChileMaterialsReceivedPredicateBuilder.Parameters>();
                    }

                    parameters.SupplierKey = supplierKeyResult.ResultingObject.ToCompanyKey();
                }

                if(!string.IsNullOrWhiteSpace(filters.ChileProductKey))
                {
                    var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(filters.ChileProductKey);
                    if(!chileProductKeyResult.Success)
                    {
                        return chileProductKeyResult.ConvertTo<ChileMaterialsReceivedPredicateBuilder.Parameters>();
                    }

                    parameters.ChileProductKey = chileProductKeyResult.ResultingObject.ToChileProductKey();
                }
            }

            return new SuccessResult<ChileMaterialsReceivedPredicateBuilder.Parameters>(parameters);
        }
    }
}