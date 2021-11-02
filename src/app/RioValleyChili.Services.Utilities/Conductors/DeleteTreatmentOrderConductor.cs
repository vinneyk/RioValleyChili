using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class DeleteTreatmentOrderConductor : DeleteInventoryShipmentOrderConductorBase
    {
        internal DeleteTreatmentOrderConductor(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        internal IResult Execute(TreatmentOrderKey key, out int? moveNum)
        {
            if(key == null) { throw new ArgumentNullException("key"); }
            moveNum = null;

            var treatmentOrder = UnitOfWork.TreatmentOrderRepository.FindByKey(key,
                t => t.InventoryShipmentOrder.ShipmentInformation,
                t => t.InventoryShipmentOrder.PickedInventory.Items.Select(i => i.CurrentLocation),
                t => t.InventoryShipmentOrder.InventoryPickOrder.Items);
            if(treatmentOrder == null)
            {
                return new InvalidResult(string.Format(UserMessages.TreatmentOrderNotFound, key));
            }
            moveNum = treatmentOrder.InventoryShipmentOrder.MoveNum;

            var deleteShipmentOrder = DeleteInventoryShipmentOrder(treatmentOrder.InventoryShipmentOrder);
            if(!deleteShipmentOrder.Success)
            {
                return deleteShipmentOrder;
            }
            
            UnitOfWork.TreatmentOrderRepository.Remove(treatmentOrder);

            return new SuccessResult();
        }
    }
}