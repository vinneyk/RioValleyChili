using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class IntraWarehouseOrderConductor : PickedInventoryConductorBase<IIntraWarehouseOrderUnitOfWork>
    {
        internal IntraWarehouseOrderConductor(IIntraWarehouseOrderUnitOfWork intraWarehouseOrderUnitOfWork) : base(intraWarehouseOrderUnitOfWork) { }

        internal IResult<IntraWarehouseOrder> Create(DateTime timestamp, ICreateIntraWarehouseOrderParameters parameters)
        {
            var orderResult = GetUpdatedOrder(timestamp, null, parameters);
            if(!orderResult.Success)
            {
                return orderResult;
            }
            var order = orderResult.ResultingObject;
            order.TrackingSheetNumber = parameters.TrackingSheetNumber;

            if(parameters.PickedItems == null || !parameters.PickedItems.Any())
            {
                return new InvalidResult<IntraWarehouseOrder>(null, UserMessages.PickedItemsManifestRequired);
            }

            var items = parameters.PickedItems.ToParsedParameters();
            if(!items.Success)
            {
                return items.ConvertTo<IntraWarehouseOrder>();
            }

            var locationKey = items.ResultingObject.Select(i => new LocationKey(i.InventoryKey)).First();
            var location = UnitOfWork.LocationRepository.FindByKey(locationKey, l => l.Facility.Locations);
            if(location == null)
            {
                return new InvalidResult<IntraWarehouseOrder>(null, string.Format(UserMessages.LocationNotFound, locationKey));
            }

            var locationKeys = location.Facility.Locations.Select(l => new LocationKey(l)).ToList();
            if(items.ResultingObject.Any(i => locationKeys.All(k => !k.Equals(i.InventoryKey)) || locationKeys.All(k => !k.Equals(i.CurrentLocationKey))))
            {
                return new InvalidResult<IntraWarehouseOrder>(null, UserMessages.IntraWarehouseOrderDifferentWarehouses);
            }
            
            var pickedResult = UpdatePickedInventory(null, order.Employee, order.TimeStamp, order.PickedInventory, items.ResultingObject, true);
            if(!pickedResult.Success)
            {
                return pickedResult.ConvertTo<IntraWarehouseOrder>();
            }
            
            var transactionParameters = new InventoryTransactionParameters(order, order.TimeStamp, InventoryTransactionType.InternalMovement, order.TrackingSheetNumber.ToString());
            var createTransactionCommand = new CreateInventoryTransactionCommand(UnitOfWork);
            foreach(var item in items.ResultingObject)
            {
                var result = createTransactionCommand.Create(transactionParameters, item.InventoryKey, -item.Quantity);
                if(!result.Success)
                {
                    return result.ConvertTo<IntraWarehouseOrder>();
                }

                result = createTransactionCommand.Create(transactionParameters, new InventoryKey(item.InventoryKey, item.InventoryKey, item.CurrentLocationKey, item.InventoryKey, item.InventoryKey.InventoryKey_ToteKey), item.Quantity);
                if(!result.Success)
                {
                    return result.ConvertTo<IntraWarehouseOrder>();
                }
            }

            return new SuccessResult<IntraWarehouseOrder>(order);
        }

        internal IResult<IntraWarehouseOrder> Update(DateTime timestamp, IIntraWarehouseOrderKey orderKey, IIntraWarehouseOrderParameters parameters)
        {
            if(orderKey == null) { throw new ArgumentNullException("orderKey"); }
            return GetUpdatedOrder(timestamp, orderKey, parameters);
        }

        #region Private Parts

        private IResult<IntraWarehouseOrder> GetUpdatedOrder(DateTime timestamp, IIntraWarehouseOrderKey orderKey, IIntraWarehouseOrderParameters parameters)
        {
            var employeeResult = new GetEmployeeCommand(UnitOfWork).GetEmployee(parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<IntraWarehouseOrder>();
            }

            IntraWarehouseOrder order;

            if(orderKey != null)
            {
                var key = new IntraWarehouseOrderKey(orderKey);
                order = UnitOfWork.IntraWarehouseOrderRepository.FindByKey(key, o => o.PickedInventory.Items);
                if(order == null)
                {
                    return new InvalidResult<IntraWarehouseOrder>(null, string.Format(UserMessages.IntraWarehouseOrderNotFound, key));
                }
            }
            else
            {
                var pickedInventoryResult = new CreatePickedInventoryCommand(UnitOfWork).Execute(new CreatePickedInventoryCommandParameters
                    {
                        EmployeeKey = employeeResult.ResultingObject,
                        PickedReason = PickedReason.IntraWarehouseMovement,
                        TimeStamp = timestamp
                    });

                if(!pickedInventoryResult.Success)
                {
                    return pickedInventoryResult.ConvertTo<IntraWarehouseOrder>();
                }

                var pickedInventory = pickedInventoryResult.ResultingObject;
                pickedInventory.Archived = true;

                order = UnitOfWork.IntraWarehouseOrderRepository.Add(new IntraWarehouseOrder
                    {
                        DateCreated = pickedInventory.DateCreated,
                        Sequence = pickedInventory.Sequence,
                        PickedInventory = pickedInventoryResult.ResultingObject
                    });
            }

            order.TimeStamp = timestamp;
            order.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            order.Employee = employeeResult.ResultingObject;

            order.OperatorName = parameters.OperatorName;
            order.MovementDate = parameters.MovementDate;

            return new SuccessResult<IntraWarehouseOrder>(order);
        }

        #endregion
    }
}