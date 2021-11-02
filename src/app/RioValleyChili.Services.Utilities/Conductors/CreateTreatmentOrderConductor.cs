using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Commands.Shipment;
using RioValleyChili.Services.Utilities.Conductors.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class CreateTreatmentOrderConductor
    {
        private readonly ITreatmentOrderUnitOfWork _treatmentOrderUnitOfWork;

        public CreateTreatmentOrderConductor(ITreatmentOrderUnitOfWork treatmentOrderUnitOfWork)
        {
            if(treatmentOrderUnitOfWork == null) { throw new ArgumentNullException("treatmentOrderUnitOfWork"); }
            _treatmentOrderUnitOfWork = treatmentOrderUnitOfWork;
        }

        public IResult<TreatmentOrder> CreateTreatmentOrder<TParams>(DateTime timestamp, CreateTreatmentOrderConductorParameters<TParams> parameters)
            where TParams : ICreateTreatmentOrderParameters
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_treatmentOrderUnitOfWork).GetEmployee(parameters.Params);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<TreatmentOrder>();
            }

            var dateCreated = timestamp.Date;
            var pickedInventoryResult = new CreatePickedInventoryCommand(_treatmentOrderUnitOfWork).Execute(new CreatePickedInventoryCommandParameters
                {
                    EmployeeKey = new EmployeeKey(employeeResult.ResultingObject),
                    TimeStamp = dateCreated,
                    PickedReason = PickedReason.TreatmentOrder
                });
            if(!pickedInventoryResult.Success)
            {
                return pickedInventoryResult.ConvertTo<TreatmentOrder>();
            }

            var pickedOrderResult = new CreateInventoryPickOrderCommand(_treatmentOrderUnitOfWork).Execute(pickedInventoryResult.ResultingObject);
            if(!pickedOrderResult.Success)
            {
                return pickedOrderResult.ConvertTo<TreatmentOrder>();
            }

            var shipmentInfoResult = new CreateShipmentInformationCommand(_treatmentOrderUnitOfWork).Execute(dateCreated);
            if(!shipmentInfoResult.Success)
            {
                return shipmentInfoResult.ConvertTo<TreatmentOrder>();
            }

            var treatmentOrder = _treatmentOrderUnitOfWork.TreatmentOrderRepository.Add(new TreatmentOrder
                {
                    DateCreated = pickedInventoryResult.ResultingObject.DateCreated,
                    Sequence = pickedInventoryResult.ResultingObject.Sequence,
                    InventoryTreatmentId = parameters.TreatmentKey.InventoryTreatmentKey_Id,

                    InventoryShipmentOrder = new InventoryShipmentOrder
                        {
                            DateCreated = pickedInventoryResult.ResultingObject.DateCreated,
                            Sequence = pickedInventoryResult.ResultingObject.Sequence,

                            OrderType = InventoryShipmentOrderTypeEnum.TreatmentOrder,
                            OrderStatus = OrderStatus.Scheduled,

                            ShipmentInfoDateCreated = shipmentInfoResult.ResultingObject.DateCreated,
                            ShipmentInfoSequence = shipmentInfoResult.ResultingObject.Sequence,
                            ShipmentInformation = shipmentInfoResult.ResultingObject,

                            InventoryPickOrder = pickedOrderResult.ResultingObject,
                            PickedInventory = pickedInventoryResult.ResultingObject,
                            MoveNum = new GetMoveNum(_treatmentOrderUnitOfWork.InventoryShipmentOrderRepository).Get(pickedInventoryResult.ResultingObject.DateCreated.Year)
                        }
                });

            return new UpdateTreatmentOrderConductor(_treatmentOrderUnitOfWork).Update(treatmentOrder, timestamp, parameters);
        }
    }
}
