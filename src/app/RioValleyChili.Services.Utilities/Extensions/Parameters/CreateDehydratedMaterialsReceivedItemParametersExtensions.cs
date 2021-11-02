using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CreateDehydratedMaterialsReceivedItemParametersExtensions
    {
        internal static IResult<SetChileMaterialsReceivedItemParameters> ToParsedParameters(this ICreateChileMaterialsReceivedItemParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.Quantity <= 0)
            {
                return new InvalidResult<SetChileMaterialsReceivedItemParameters>(null, UserMessages.QuantityNotGreaterThanZero);
            }

            var toteKeyResult = ToteKeyParser.Parse(parameters.ToteKey);
            if(!toteKeyResult.Success)
            {
                return toteKeyResult.ConvertTo<SetChileMaterialsReceivedItemParameters>();
            }
            
            var packagingProductKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingProductKey);
            if(!packagingProductKeyResult.Success)
            {
                return packagingProductKeyResult.ConvertTo<SetChileMaterialsReceivedItemParameters>();
            }

            var locationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.LocationKey);
            if(!locationKeyResult.Success)
            {
                return locationKeyResult.ConvertTo<SetChileMaterialsReceivedItemParameters>();
            }

            return new SuccessResult<SetChileMaterialsReceivedItemParameters>(new SetChileMaterialsReceivedItemParameters
                {
                    GrowerCode = parameters.GrowerCode,
                    ToteKey = toteKeyResult.ResultingObject,
                    ChileVariety = parameters.Variety,
                    Quantity = parameters.Quantity,

                    PackagingProductKey = packagingProductKeyResult.ResultingObject.ToPackagingProductKey(),
                    LocationKey = locationKeyResult.ResultingObject.ToLocationKey()
                });
        }

        internal static IResult<SetChileMaterialsReceivedItemParameters> ToParsedParameters(this IUpdateChileMaterialsReceivedItemParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.Quantity <= 0)
            {
                return new InvalidResult<SetChileMaterialsReceivedItemParameters>(null, UserMessages.QuantityNotGreaterThanZero);
            }

            var toteKeyResult = ToteKeyParser.Parse(parameters.ToteKey);
            if(!toteKeyResult.Success)
            {
                return toteKeyResult.ConvertTo<SetChileMaterialsReceivedItemParameters>();
            }

            ChileMaterialsReceivedItemKey itemKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.ItemKey))
            {
                var itemKeyResult = KeyParserHelper.ParseResult<IChileMaterialsReceivedItemKey>(parameters.ItemKey);
                if(!itemKeyResult.Success)
                {
                    return itemKeyResult.ConvertTo<SetChileMaterialsReceivedItemParameters>();
                }

                itemKey = itemKeyResult.ResultingObject.ToChileMaterialsReceivedItemKey();
            }
            
            var packagingProductKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingProductKey);
            if(!packagingProductKeyResult.Success)
            {
                return packagingProductKeyResult.ConvertTo<SetChileMaterialsReceivedItemParameters>();
            }

            var locationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.LocationKey);
            if(!locationKeyResult.Success)
            {
                return locationKeyResult.ConvertTo<SetChileMaterialsReceivedItemParameters>();
            }

            return new SuccessResult<SetChileMaterialsReceivedItemParameters>(new SetChileMaterialsReceivedItemParameters
                {
                    GrowerCode = parameters.GrowerCode,
                    ToteKey = toteKeyResult.ResultingObject,
                    ChileVariety = parameters.Variety,
                    Quantity = parameters.Quantity,

                    ItemKey = itemKey,
                    PackagingProductKey = packagingProductKeyResult.ResultingObject.ToPackagingProductKey(),
                    LocationKey = locationKeyResult.ResultingObject.ToLocationKey()
                });
        }
    }
}