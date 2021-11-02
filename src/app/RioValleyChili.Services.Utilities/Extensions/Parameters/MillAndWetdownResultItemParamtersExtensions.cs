using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class MillAndWetdownResultItemParamtersExtensions
    {
        internal static IResult<CreateMillAndWetdownResultItemCommandParameters> ToCreateMillAndWetdownResultItemCommandParameters(this IMillAndWetdownResultItemParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.Quantity <= 0)
            {
                return new InvalidResult<CreateMillAndWetdownResultItemCommandParameters>(null, UserMessages.QuantityNotGreaterThanZero);
            }

            var warehouseLocationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.LocationKey);
            if(!warehouseLocationKeyResult.Success)
            {
                return warehouseLocationKeyResult.ConvertTo((CreateMillAndWetdownResultItemCommandParameters) null);
            }

            var packagingProductKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingProductKey);
            if(!packagingProductKeyResult.Success)
            {
                return packagingProductKeyResult.ConvertTo((CreateMillAndWetdownResultItemCommandParameters) null);
            }

            return new SuccessResult<CreateMillAndWetdownResultItemCommandParameters>(new CreateMillAndWetdownResultItemCommandParameters
                {
                    LocationKey = new LocationKey(warehouseLocationKeyResult.ResultingObject),
                    PackagingProductKey = new PackagingProductKey(packagingProductKeyResult.ResultingObject),
                    Quantity = parameters.Quantity
                });
        }
    }
}