using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class ReceiveTreatmentOrderConductor
    {
        private readonly ITreatmentOrderUnitOfWork _treatmentOrderUnitOfWork;

        public ReceiveTreatmentOrderConductor(ITreatmentOrderUnitOfWork treatmentOrderUnitOfWork)
        {
            if(treatmentOrderUnitOfWork == null) { throw new ArgumentNullException("treatmentOrderUnitOfWork"); }
            _treatmentOrderUnitOfWork = treatmentOrderUnitOfWork;
        }

        public IResult Execute(DateTime timestamp, ReceiveTreatmentOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_treatmentOrderUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult;
            }

            var order = _treatmentOrderUnitOfWork.TreatmentOrderRepository.FindByKey(parameters.TreatmentOrderKey, o => o.InventoryShipmentOrder.PickedInventory.Items.Select(i => i.CurrentLocation));
            if(order == null)
            {
                return new FailureResult(string.Format(UserMessages.TreatmentOrderNotFound, parameters.TreatmentOrderKey.KeyValue));
            }

            if(order.InventoryShipmentOrder.OrderStatus == OrderStatus.Fulfilled)
            {
                return new FailureResult<PickedInventory>(null, string.Format(UserMessages.TreatmentOrderAlreadyFulfilled, new TreatmentOrderKey(order).KeyValue));
            }

            var inventoryModifications = new List<ModifyInventoryParameters>();
            foreach(var item in order.InventoryShipmentOrder.PickedInventory.Items)
            {
                inventoryModifications.Add(new ModifyInventoryParameters(item, item, item.CurrentLocation, item, item.ToteKey, -item.Quantity));
                inventoryModifications.Add(item.ToModifyInventoryDestinationParameters(parameters.DestinationLocationKey, order));
                item.CurrentLocationId = parameters.DestinationLocationKey.LocationKey_Id;
            }

            var modificationsResult = new ModifyInventoryCommand(_treatmentOrderUnitOfWork).Execute(inventoryModifications,
                new InventoryTransactionParameters(employeeResult.ResultingObject, timestamp, InventoryTransactionType.ReceiveTreatmentOrder, parameters.TreatmentOrderKey));
            if(!modificationsResult.Success)
            {
                return modificationsResult;
            }

            order.InventoryShipmentOrder.OrderStatus = OrderStatus.Fulfilled;
            order.InventoryShipmentOrder.PickedInventory.Archived = true;
            order.Returned = DateTime.UtcNow;

            return new SuccessResult<PickedInventory>(order.InventoryShipmentOrder.PickedInventory);
        }
    }
}