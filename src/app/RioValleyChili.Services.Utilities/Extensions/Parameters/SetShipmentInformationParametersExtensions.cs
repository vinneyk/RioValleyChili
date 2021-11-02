using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SetShipmentInformationParametersExtensions
    {
        internal static IResult<SetShipmentInformationConductor.Parameters> ToParsedParameters(this ISetInventoryShipmentInformationParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var shipmentOrderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(parameters.InventoryShipmentOrderKey);
            if(!shipmentOrderKeyResult.Success)
            {
                return shipmentOrderKeyResult.ConvertTo<SetShipmentInformationConductor.Parameters>(null);
            }

            return new SuccessResult<SetShipmentInformationConductor.Parameters>(new SetShipmentInformationConductor.Parameters
                {
                    SetShipmentInformation = parameters,
                    ShipmentOrderKey = new InventoryShipmentOrderKey(shipmentOrderKeyResult.ResultingObject)
                });
        }
    }
}