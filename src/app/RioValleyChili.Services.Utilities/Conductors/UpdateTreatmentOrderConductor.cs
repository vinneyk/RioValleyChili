using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;
using RioValleyChili.Services.Utilities.Conductors.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class UpdateTreatmentOrderConductor : SetInventoryShipmentOrderConductorBase<ITreatmentOrderUnitOfWork>
    {
        internal UpdateTreatmentOrderConductor(ITreatmentOrderUnitOfWork treatmentOrderUnitOfWork) : base(treatmentOrderUnitOfWork) { }

        internal IResult<TreatmentOrder> Update<TParams>(DateTime timeStamp, UpdateTreatmentOrderConductorParameters<TParams> parameters)
            where TParams : IUpdateTreatmentOrderParameters
        {
            var treatmentOrder = UnitOfWork.TreatmentOrderRepository.FindByKey(parameters.TreatmentOrderKey,
                o => o.InventoryShipmentOrder.DestinationFacility.Locations,
                o => o.InventoryShipmentOrder.SourceFacility.Locations,
                o => o.InventoryShipmentOrder.ShipmentInformation,
                o => o.InventoryShipmentOrder.PickedInventory.Items.Select(i => i.CurrentLocation),
                o => o.InventoryShipmentOrder.PickedInventory.Items.Select(i => i.FromLocation),
                o => o.InventoryShipmentOrder.InventoryPickOrder.Items.Select(i => i.Customer));
            if(treatmentOrder == null)
            {
                return new InvalidResult<TreatmentOrder>(null, string.Format(UserMessages.TreatmentOrderNotFound, parameters.TreatmentOrderKey));
            }

            var updateResult = Update(treatmentOrder, timeStamp, parameters);
            if(updateResult.Success)
            {
                var itemCodesResult = SetPickedItemCodes(treatmentOrder.InventoryShipmentOrder, parameters.SetPickedInventoryItemCodes);
                if(!itemCodesResult.Success)
                {
                    return itemCodesResult.ConvertTo<TreatmentOrder>();
                }
            }

            return updateResult;
        }

        internal IResult<TreatmentOrder> Update<TParams>(TreatmentOrder treatmentOrder, DateTime timeStamp, CreateTreatmentOrderConductorParameters<TParams> parameters)
            where TParams : ICreateTreatmentOrderParameters
        {
            if(parameters.TreatmentKey != null)
            {
                treatmentOrder.InventoryTreatmentId = parameters.TreatmentKey.InventoryTreatmentKey_Id;
            }

            return SetOrder(treatmentOrder.InventoryShipmentOrder, timeStamp, parameters).ConvertTo(treatmentOrder);
        }

        protected override IResult ValidateDestinationFacility(Facility facility)
        {
            if(facility.FacilityType != FacilityType.Treatment)
            {
                return new InvalidResult(string.Format(UserMessages.FacilityNotOfType, new FacilityKey(facility), FacilityType.Treatment));
            }

            return base.ValidateDestinationFacility(facility);
        }
    }
}