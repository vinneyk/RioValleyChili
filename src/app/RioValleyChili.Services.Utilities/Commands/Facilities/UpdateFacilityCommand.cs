using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Facilities
{
    internal class UpdateFacilityCommand
    {
        private readonly IFacilityUnitOfWork _facilityUnitOfWork;

        internal UpdateFacilityCommand(IFacilityUnitOfWork facilityUnitOfWork)
        {
            if(facilityUnitOfWork == null) { throw new ArgumentNullException("facilityUnitOfWork"); }
            _facilityUnitOfWork = facilityUnitOfWork;
        }

        internal IResult UpdateFacility(IFacilityKey facilityKey, ICreateFacilityParameters parameters)
        {
            var key = facilityKey.ToFacilityKey();
            var facility = _facilityUnitOfWork.FacilityRepository.FindByKey(key);
            if(facility == null)
            {
                return new InvalidResult(string.Format(UserMessages.FacilityNotFound, key));
            }

            UpdateFacility(facility, parameters);

            return new SuccessResult();
        }

        internal IResult UpdateFacility(Facility facility, ICreateFacilityParameters parameters)
        {
            facility.FacilityType = parameters.FacilityType;
            facility.Name = parameters.Name;
            facility.Active = parameters.Active;
            facility.PhoneNumber = parameters.PhoneNumber;
            facility.EMailAddress = parameters.EMailAddress;
            facility.ShippingLabelName = parameters.ShippingLabelName;
            facility.Address = parameters.Address;

            return new SuccessResult();
        }
    }
}