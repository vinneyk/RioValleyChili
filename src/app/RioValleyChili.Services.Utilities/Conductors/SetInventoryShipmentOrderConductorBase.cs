using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Conductors.Parameters;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal abstract class SetInventoryShipmentOrderConductorBase<TUnitOfWork>
        where TUnitOfWork : class, ICoreUnitOfWork, IFacilityUnitOfWork, IInventoryPickOrderUnitOfWork, IPickedInventoryUnitOfWork
    {
        internal SetInventoryShipmentOrderConductorBase(TUnitOfWork unitOfWork)
        {
            if(unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }
            UnitOfWork = unitOfWork;
        }

        protected TUnitOfWork UnitOfWork;

        protected virtual IResult ValidateDestinationFacility(Facility facility)
        {
            return new SuccessResult();
        }

        protected IResult<InventoryShipmentOrder> SetOrder<TParams>(InventoryShipmentOrder inventoryShipmentOrder, DateTime timeStamp, SetInventoryShipmentOrderConductorParameters<TParams> parameters)
            where TParams : ISetOrderParameters
        {
            if(inventoryShipmentOrder == null) { throw new ArgumentNullException("inventoryShipmentOrder"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(UnitOfWork).GetEmployee(parameters.Params);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<InventoryShipmentOrder>();
            }

            inventoryShipmentOrder.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            inventoryShipmentOrder.TimeStamp = timeStamp;

            if(parameters.Params.HeaderParameters != null)
            {
                inventoryShipmentOrder.SetHeaderParameters(parameters.Params.HeaderParameters);
            }

            if(parameters.DestinationFacilityKey != null)
            {
                var destinationFacility = UnitOfWork.FacilityRepository.FindByKey(parameters.DestinationFacilityKey, f => f.Locations);
                if(destinationFacility == null)
                {
                    return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.FacilityNotFound, parameters.DestinationFacilityKey));
                }

                var validateFacility = ValidateDestinationFacility(destinationFacility);
                if(!validateFacility.Success)
                {
                    return validateFacility.ConvertTo<InventoryShipmentOrder>();
                }

                inventoryShipmentOrder.DestinationFacility = destinationFacility;
                inventoryShipmentOrder.DestinationFacilityId = destinationFacility.Id;
            }

            if(parameters.SourceFacilityKey != null)
            {
                var sourceFacility = UnitOfWork.FacilityRepository.FindByKey(parameters.SourceFacilityKey, f => f.Locations);
                if(sourceFacility == null)
                {
                    return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.FacilityNotFound, parameters.SourceFacilityKey));
                }
                
                var currentKeys = inventoryShipmentOrder.PickedInventory.Items.Select(i => new LocationKey(i.FromLocation)).ToList();
                if(currentKeys.Any(l => sourceFacility.Locations.All(c => !l.Equals(c))))
                {
                    return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.SourceLocationMustBelongToFacility, parameters.SourceFacilityKey));
                }

                inventoryShipmentOrder.SourceFacility = sourceFacility;
                inventoryShipmentOrder.SourceFacilityId = sourceFacility.Id;

            }

            if(parameters.Params.SetShipmentInformation != null)
            {
                inventoryShipmentOrder.ShipmentInformation.SetShipmentInformation(parameters.Params.SetShipmentInformation);
            }

            if(parameters.Params.InventoryPickOrderItems != null)
            {
                var pickOrderResult = new InventoryPickOrderCommand(UnitOfWork).Execute(inventoryShipmentOrder.InventoryPickOrder, parameters.PickOrderItems);
                if(!pickOrderResult.Success)
                {
                    return pickOrderResult.ConvertTo<InventoryShipmentOrder>();
                }
            }

            return new SuccessResult<InventoryShipmentOrder>(inventoryShipmentOrder);
        }

        protected IResult<InventoryShipmentOrder> SetPickedItemCodes(InventoryShipmentOrder inventoryShipmentOrder, IEnumerable<SetPickedInventoryItemCodesParameters> setItemCodesParameters)
        {
            if(setItemCodesParameters != null)
            {
                foreach(var setItem in setItemCodesParameters)
                {
                    var item = inventoryShipmentOrder.PickedInventory.Items.FirstOrDefault(setItem.PickedInventoryItemKey.FindByPredicate.Compile());
                    if(item == null)
                    {
                        return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.PickedInventoryItemNotFound, setItem.PickedInventoryItemKey));
                    }
                    item.CustomerProductCode = setItem.Parameters.CustomerProductCode;
                    item.CustomerLotCode = setItem.Parameters.CustomerLotCode;
                }
            }

            return new SuccessResult<InventoryShipmentOrder>(inventoryShipmentOrder);
        }
    }
}