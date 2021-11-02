using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService;
using RioValleyChili.Services.Utilities.Helpers;

namespace RioValleyChili.Services.Utilities.Conductors.Parameters
{
    internal class UpdateInterWarehouseOrderConductorParameters<TParams> : SetInventoryShipmentOrderConductorParameters<TParams>
        where TParams : IUpdateInterWarehouseOrderParameters
    {
        public InventoryShipmentOrderKey InventoryShipmentOrderKey { get; set; }
        public List<SetPickedInventoryItemCodesParameters> SetPickedInventoryItemCodes { get; set; }

        public UpdateInterWarehouseOrderConductorParameters(TParams parameters) : base(parameters)
        {
            if(Result.Success)
            {
                var keyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(parameters.InventoryShipmentOrderKey);
                if(!keyResult.Success)
                {
                    Result = keyResult;
                    return;
                }
                InventoryShipmentOrderKey = new InventoryShipmentOrderKey(keyResult.ResultingObject);

                var setItemCodesResult = ParseParameters(parameters.PickedInventoryItemCodes);
                if(!setItemCodesResult.Success)
                {
                    Result = setItemCodesResult;
                    return;
                }

                SetPickedInventoryItemCodes = setItemCodesResult.ResultingObject;
            }
        }
    }
}