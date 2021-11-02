using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Commands.Shipment;
using RioValleyChili.Services.Utilities.Conductors.Parameters;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class CreateInterWarehouseOrderConductor
    {
        private readonly ISalesUnitOfWork _interWarehouseOrderUnitOfWork;

        public CreateInterWarehouseOrderConductor(ISalesUnitOfWork interWarehouseOrderUnitOfWork)
        {
            if(interWarehouseOrderUnitOfWork == null) { throw new ArgumentNullException("interWarehouseOrderUnitOfWork"); }
            _interWarehouseOrderUnitOfWork = interWarehouseOrderUnitOfWork;
        }

        internal IResult<InventoryShipmentOrder> Create<TParams>(DateTime timestamp, SetInventoryShipmentOrderConductorParameters<TParams> parameters)
            where TParams : ISetOrderParameters
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_interWarehouseOrderUnitOfWork).GetEmployee(parameters.Params);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<InventoryShipmentOrder>();
            }

            var pickedInventoryResult = new CreatePickedInventoryCommand(_interWarehouseOrderUnitOfWork).Execute(new CreatePickedInventoryCommandParameters
                {
                    EmployeeKey = new EmployeeKey(employeeResult.ResultingObject),
                    TimeStamp = timestamp,
                    PickedReason = PickedReason.InterWarehouseMovement
                });
            if(!pickedInventoryResult.Success)
            {
                return pickedInventoryResult.ConvertTo<InventoryShipmentOrder>();
            }

            var pickedOrderResult = new CreateInventoryPickOrderCommand(_interWarehouseOrderUnitOfWork).Execute(pickedInventoryResult.ResultingObject);
            if(!pickedOrderResult.Success)
            {
                return pickedOrderResult.ConvertTo<InventoryShipmentOrder>();
            }

            var shipmentInfoResult = new CreateShipmentInformationCommand(_interWarehouseOrderUnitOfWork).Execute(timestamp, parameters.Params.SetShipmentInformation);
            if(!shipmentInfoResult.Success)
            {
                return shipmentInfoResult.ConvertTo<InventoryShipmentOrder>();
            }

            var order = new InventoryShipmentOrder
                {
                    OrderType = InventoryShipmentOrderTypeEnum.InterWarehouseOrder,
                    OrderStatus = OrderStatus.Scheduled,
                    DateCreated = pickedInventoryResult.ResultingObject.DateCreated,
                    Sequence = pickedInventoryResult.ResultingObject.Sequence,
                    PickedInventory = pickedInventoryResult.ResultingObject,
                    InventoryPickOrder = pickedOrderResult.ResultingObject,
                    ShipmentInformation = shipmentInfoResult.ResultingObject,
                    ShipmentInfoDateCreated = shipmentInfoResult.ResultingObject.DateCreated,
                    ShipmentInfoSequence = shipmentInfoResult.ResultingObject.Sequence,
                    MoveNum = new GetMoveNum(_interWarehouseOrderUnitOfWork.InventoryShipmentOrderRepository).Get(pickedInventoryResult.ResultingObject.DateCreated.Year)
                };
            if(parameters.Params.HeaderParameters != null)
            {
                order.SetHeaderParameters(parameters.Params.HeaderParameters);
            }

            order = _interWarehouseOrderUnitOfWork.InventoryShipmentOrderRepository.Add(order);

            return new UpdateInterWarehouseOrderConductor(_interWarehouseOrderUnitOfWork).Update(order, timestamp, parameters);
        }
    }
}