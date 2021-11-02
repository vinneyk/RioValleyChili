using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetShipmentInformationConductor
    {
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;

        internal SetShipmentInformationConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;
        }

        internal IResult<InventoryShipmentOrder> SetShipmentInformation(Parameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var shipmentOrder = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository
                .FindByKey(parameters.ShipmentOrderKey, o => o.ShipmentInformation);
            if(shipmentOrder == null)
            {
                return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, parameters.ShipmentOrderKey));
            }

            shipmentOrder.ShipmentInformation.SetShipmentInformation(parameters.SetShipmentInformation);

            return new SuccessResult<InventoryShipmentOrder>(shipmentOrder);
        }

        internal class Parameters
        {
            public ISetInventoryShipmentInformationParameters SetShipmentInformation { get; set; }
            public InventoryShipmentOrderKey ShipmentOrderKey { get; set; }
        }
    }
}