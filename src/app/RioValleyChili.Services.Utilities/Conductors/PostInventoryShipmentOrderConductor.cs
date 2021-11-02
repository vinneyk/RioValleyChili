using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class PostInventoryShipmentOrderConductor
    {
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;

        internal PostInventoryShipmentOrderConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;
        }

        internal IResult Post(Parameters parameters, DateTime timestamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_inventoryShipmentOrderUnitOfWork).GetEmployee(parameters.Params);
            if(!employeeResult.Success)
            {
                return employeeResult;
            }

            var order = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.FindByKey(parameters.InventoryShipmentOrderKey,
                    o => o.ShipmentInformation,
                    o => o.PickedInventory.Items,
                    o => o.DestinationFacility.Locations);
            if(order == null)
            {
                return new InvalidResult(string.Format(UserMessages.InventoryShipmentOrderNotFound, parameters.InventoryShipmentOrderKey));
            }

            var moveResult = PostOrder(parameters, order, employeeResult.ResultingObject);
            if(!moveResult.Success)
            {
                return moveResult;
            }

            var sourceReference = parameters.InventoryShipmentOrderKey.KeyValue;
            InventoryTransactionType transactionType;
            switch(order.OrderType)
            {
                case InventoryShipmentOrderTypeEnum.InterWarehouseOrder:
                    order.OrderStatus = OrderStatus.Fulfilled;
                    order.PickedInventory.Archived = true;
                    transactionType = InventoryTransactionType.PostedInterWarehouseOrder;
                    break;

                case InventoryShipmentOrderTypeEnum.TreatmentOrder:
                    transactionType = InventoryTransactionType.PostedTreatmentOrder;
                    break;

                case InventoryShipmentOrderTypeEnum.SalesOrder:
                    order.OrderStatus = OrderStatus.Fulfilled;
                    order.PickedInventory.Archived = true;
                    transactionType = InventoryTransactionType.PostedCustomerOrder;
                    if(order.MoveNum != null)
                    {
                        sourceReference = order.MoveNum.ToString();
                    } 
                    break;

                case InventoryShipmentOrderTypeEnum.MiscellaneousOrder:
                    order.OrderStatus = OrderStatus.Fulfilled;
                    order.PickedInventory.Archived = true;
                    transactionType = InventoryTransactionType.PostedMiscellaneousOrder;
                    if(order.MoveNum != null)
                    {
                        sourceReference = order.MoveNum.ToString();
                    } 
                    break;

                default: throw new ArgumentOutOfRangeException("order.OrderType");
            }
            
            var transactionParameters = new InventoryTransactionParameters(employeeResult.ResultingObject, timestamp, transactionType, sourceReference);
            var modifyResult = new ModifyInventoryCommand(_inventoryShipmentOrderUnitOfWork).Execute(moveResult.ResultingObject, transactionParameters);
            if(!modifyResult.Success)
            {
                return modifyResult;
            }

            return new CreateInventoryTransactionCommand(_inventoryShipmentOrderUnitOfWork).LogPickedInventory(transactionParameters, order.PickedInventory.Items);
        }

        private static IResult<List<ModifyInventoryParameters>> PostOrder(Parameters parameters, InventoryShipmentOrder order, IEmployeeKey employee)
        {
            if(order == null) { throw new ArgumentNullException("order"); }
            if(employee == null) { throw new ArgumentNullException("order"); }

            if(order.OrderStatus == OrderStatus.Fulfilled)
            {
                switch(order.OrderType)
                {
                    case InventoryShipmentOrderTypeEnum.InterWarehouseOrder:
                        return new InvalidResult<List<ModifyInventoryParameters>>(null, string.Format(UserMessages.InterWarehouseOrderAlreadyFulfilled, order.ToInventoryShipmentOrderKey()));

                    case InventoryShipmentOrderTypeEnum.TreatmentOrder:
                        return new InvalidResult<List<ModifyInventoryParameters>>(null, string.Format(UserMessages.TreatmentOrderAlreadyFulfilled, order.ToInventoryShipmentOrderKey()));

                    case InventoryShipmentOrderTypeEnum.SalesOrder:
                    case InventoryShipmentOrderTypeEnum.MiscellaneousOrder:
                        return new InvalidResult<List<ModifyInventoryParameters>>(null, string.Format(UserMessages.SalesOrderAlreadyFulfilled, order.ToInventoryShipmentOrderKey()));
                    
                    default: throw new ArgumentOutOfRangeException("order.OrderType");
                }
            }

            if(order.ShipmentInformation.Status == ShipmentStatus.Shipped)
            {
                return new InvalidResult<List<ModifyInventoryParameters>>(null, string.Format(UserMessages.ShipmentStatusCannotPost, order.ShipmentInformation.Status));
            }

            var inventoryModifications = new List<ModifyInventoryParameters>();
            foreach(var item in order.PickedInventory.Items)
            {
                var pickedItemKey = new PickedInventoryItemKey(item);
                var itemParameters = parameters.ItemParams.FirstOrDefault(i => i.ItemKey.Equals(pickedItemKey));
                if(itemParameters == null)
                {
                    if(order.DestinationFacility != null)
                    {
                        return new InvalidResult<List<ModifyInventoryParameters>>(null, string.Format(UserMessages.DestinationLocationRequiredForPicked, pickedItemKey));
                    }
                }
                else if(itemParameters.DestinationKey != null)
                {
                    if(order.DestinationFacility != null && !order.DestinationFacility.Locations.Any(l => itemParameters.DestinationKey.Equals(l)))
                    {
                        return new InvalidResult<List<ModifyInventoryParameters>>(null, string.Format(UserMessages.DestinationLocationMustBelongToFacility, new FacilityKey(order.DestinationFacility)));
                    }

                    item.CurrentLocationId = itemParameters.DestinationKey.LocationKey_Id;
                    inventoryModifications.Add(item.ToModifyInventoryDestinationParameters(itemParameters.DestinationKey));
                }
            }

            order.ShipmentInformation.Status = ShipmentStatus.Shipped;
            order.EmployeeId = employee.EmployeeKey_Id;

            return new SuccessResult<List<ModifyInventoryParameters>>(inventoryModifications);
        }

        internal class Parameters
        {
            internal IPostParameters Params;
            internal InventoryShipmentOrderKey InventoryShipmentOrderKey;
            internal List<Item> ItemParams;

            internal class Item
            {
                internal PickedInventoryItemKey ItemKey;
                internal LocationKey DestinationKey;
            }
        }
    }
}