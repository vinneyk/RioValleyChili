using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal abstract class InventoryShipmentOrderPickInventoryConductor : PickedInventoryConductorBase<IInventoryShipmentOrderUnitOfWork>
    {
        protected abstract IResult<IInventoryValidator> GetInventoryValidator(InventoryShipmentOrder order);

        internal InventoryShipmentOrderPickInventoryConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentUnitOfWork) : base(inventoryShipmentUnitOfWork) { }

        internal IResult<InventoryShipmentOrder> SetPickedInventory(DateTime timeStamp, Parameters parameters)
        {
            var inventoryShipmentOrder = UnitOfWork.InventoryShipmentOrderRepository.FindByKey(parameters.OrderKey,
                i => i.ShipmentInformation,
                i => i.SourceFacility,
                i => i.PickedInventory.Items.Select(t => t.FromLocation),
                i => i.PickedInventory.Items.Select(t => t.CurrentLocation));
            if(inventoryShipmentOrder == null)
            {
                return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, parameters.OrderKey));
            }

            var employeeResult = new GetEmployeeCommand(UnitOfWork).GetEmployee(parameters.User);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<InventoryShipmentOrder>();
            }

            if(inventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Scheduled && inventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Unscheduled)
            {
                return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.CannotPickShipment, inventoryShipmentOrder.ShipmentInformation.Status));
            }

            var validator = GetInventoryValidator(inventoryShipmentOrder);
            if(!validator.Success)
            {
                return validator.ConvertTo<InventoryShipmentOrder>();
            }

            var pickedResult = UpdatePickedInventory(validator.ResultingObject, employeeResult.ResultingObject, timeStamp, inventoryShipmentOrder.PickedInventory, parameters.PickedInventoryParameters);
            if(!pickedResult.Success)
            {
                return pickedResult.ConvertTo<InventoryShipmentOrder>();
            }

            return new SuccessResult<InventoryShipmentOrder>(inventoryShipmentOrder);
        }

        internal class Parameters
        {
            internal IUserIdentifiable User;
            internal InventoryShipmentOrderKey OrderKey;
            internal List<PickedInventoryParameters> PickedInventoryParameters;
        }
    }

    internal class TreatmentOrderPickedInventoryConductor : InventoryShipmentOrderPickInventoryConductor
    {
        public TreatmentOrderPickedInventoryConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentUnitOfWork) : base(inventoryShipmentUnitOfWork) { }
        protected override IResult<IInventoryValidator> GetInventoryValidator(InventoryShipmentOrder order)
        {
            var predicate = TreatmentOrderPredicates.ByInventoryShipmentOrder(order);
            var treatmentOrder = UnitOfWork.TreatmentOrderRepository.Filter(predicate).FirstOrDefault();
            if(treatmentOrder == null)
            {
                return new InvalidResult<IInventoryValidator>(null, string.Format(UserMessages.TreatmentOrderNotFound, new InventoryShipmentOrderKey(order)));
            }

            treatmentOrder.InventoryShipmentOrder = order;

            return new SuccessResult<IInventoryValidator>(PickedInventoryValidator.ForTreatmentOrder(treatmentOrder.InventoryShipmentOrder.SourceFacility, treatmentOrder, UnitOfWork));
        }
    }
}