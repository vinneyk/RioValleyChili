using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ISetPickedInventoryItemParameterExtensions
    {
        internal static IResult<PickedInventoryParameters> ToParsedParameters(this IPickedInventoryItemParameters item, bool requireOrderItemKey = false)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            IInventoryPickOrderItemKey orderItemKey = null;
            if(requireOrderItemKey || item.OrderItemKey != null)
            {
                var orderItemKeyResult = KeyParserHelper.ParseResult<IInventoryPickOrderItemKey>(item.OrderItemKey);
                if(!orderItemKeyResult.Success)
                {
                    return orderItemKeyResult.ConvertTo<PickedInventoryParameters>();
                }
                orderItemKey = orderItemKeyResult.ResultingObject;
            }

            var inventoryKeyResult = KeyParserHelper.ParseResult<IInventoryKey>(item.InventoryKey);
            if(!inventoryKeyResult.Success)
            {
                return inventoryKeyResult.ConvertTo<PickedInventoryParameters>();
            }

            return new SuccessResult<PickedInventoryParameters>(new PickedInventoryParameters(orderItemKey, inventoryKeyResult.ResultingObject, item.Quantity, item.CustomerLotCode, item.CustomerProductCode));
        }

        internal static IResult<List<PickedInventoryParameters>> ToParsedParameters(this IEnumerable<IPickedInventoryItemParameters> parameters)
        {
            var result = new List<PickedInventoryParameters>();

            if(parameters != null)
            {
                foreach(var item in parameters)
                {
                    var itemResult = item.ToParsedParameters();
                    if(!itemResult.Success)
                    {
                        return itemResult.ConvertTo<List<PickedInventoryParameters>>();
                    }

                    result.Add(itemResult.ResultingObject);
                }
            }

            return new SuccessResult<List<PickedInventoryParameters>>(result);
        }

        internal static IResult<List<PickedInventoryParameters>> ToParsedParameters(this IEnumerable<IIntraWarehouseOrderPickedItemParameters> parameters)
        {
            var result = new List<PickedInventoryParameters>();

            if(parameters != null)
            {
                foreach(var item in parameters)
                {
                    var itemResult = item.ToParsedParameters();
                    if(!itemResult.Success)
                    {
                        return itemResult.ConvertTo<List<PickedInventoryParameters>>();
                    }

                    var keyResult = KeyParserHelper.ParseResult<ILocationKey>(item.DestinationLocationKey);
                    if(!keyResult.Success)
                    {
                        return keyResult.ConvertTo<List<PickedInventoryParameters>>();
                    }

                    itemResult.ResultingObject.CurrentLocationKey = new LocationKey(keyResult.ResultingObject);

                    result.Add(itemResult.ResultingObject);
                }
            }

            return new SuccessResult<List<PickedInventoryParameters>>(result);
        }
    }
}