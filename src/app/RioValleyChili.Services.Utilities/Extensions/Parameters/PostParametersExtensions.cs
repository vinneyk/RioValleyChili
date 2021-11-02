using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class PostParametersExtensions
    {
        internal static IResult<PostInventoryShipmentOrderConductor.Parameters> ToParsedParameters(this IPostParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(parameters.OrderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<PostInventoryShipmentOrderConductor.Parameters>();
            }

            var itemParameters = new List<PostInventoryShipmentOrderConductor.Parameters.Item>();
            foreach(var item in parameters.PickedItemDestinations ?? new IPostItemParameters[0])
            {
                var itemKeyResult = KeyParserHelper.ParseResult<IPickedInventoryItemKey>(item.PickedInventoryItemKey);
                if(!itemKeyResult.Success)
                {
                    return itemKeyResult.ConvertTo<PostInventoryShipmentOrderConductor.Parameters>();
                }

                LocationKey locationKey = null;
                if(item.DestinationLocationKey != null)
                {
                    var locationKeyResult = KeyParserHelper.ParseResult<ILocationKey>(item.DestinationLocationKey);
                    if(!locationKeyResult.Success)
                    {
                        return locationKeyResult.ConvertTo<PostInventoryShipmentOrderConductor.Parameters>();
                    }
                    locationKey = new LocationKey(locationKeyResult.ResultingObject);
                }

                itemParameters.Add(new PostInventoryShipmentOrderConductor.Parameters.Item
                    {
                        ItemKey = itemKeyResult.ResultingObject.ToPickedInventoryItemKey(),
                        DestinationKey = locationKey
                    });
            }

            return new SuccessResult<PostInventoryShipmentOrderConductor.Parameters>(new PostInventoryShipmentOrderConductor.Parameters
                {
                    Params = parameters,
                    InventoryShipmentOrderKey = orderKeyResult.ResultingObject.ToInventoryShipmentOrderKey(),
                    ItemParams = itemParameters
                });
        }
    }
}