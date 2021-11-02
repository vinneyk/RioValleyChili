using System;
using System.Linq;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService;
using RioValleyChili.Services.Utilities.Conductors.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class UpdateInterWarehouseOrderConductor : SetInventoryShipmentOrderConductorBase<ISalesUnitOfWork>
    {
        internal UpdateInterWarehouseOrderConductor(ISalesUnitOfWork interWarehouseOrderUnitOfWork) : base(interWarehouseOrderUnitOfWork) { }

        internal IResult<InventoryShipmentOrder> Update<TParams>(DateTime timeStamp, UpdateInterWarehouseOrderConductorParameters<TParams> parameters)
            where TParams : IUpdateInterWarehouseOrderParameters
        {
            var inventoryShipmentOrder = UnitOfWork.InventoryShipmentOrderRepository.FindByKey(parameters.InventoryShipmentOrderKey,
                o => o.DestinationFacility.Locations,
                o => o.SourceFacility.Locations,
                o => o.ShipmentInformation,
                o => o.PickedInventory.Items.Select(i => i.CurrentLocation),
                o => o.PickedInventory.Items.Select(i => i.FromLocation),
                o => o.InventoryPickOrder.Items.Select(i => i.Customer));
            if(inventoryShipmentOrder == null)
            {
                return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, parameters.InventoryShipmentOrderKey));
            }

            var updateResult = Update(inventoryShipmentOrder, timeStamp, parameters);
            if(!updateResult.Success)
            {
                return updateResult;
            }

            return SetPickedItemCodes(inventoryShipmentOrder, parameters.SetPickedInventoryItemCodes);
        }

        internal IResult<InventoryShipmentOrder> Update<TParams>(InventoryShipmentOrder inventoryShipmentOrder, DateTime timeStamp, SetInventoryShipmentOrderConductorParameters<TParams> parameters)
            where TParams : ISetOrderParameters
        {
            return SetOrder(inventoryShipmentOrder, timeStamp, parameters);
        }
    }
}