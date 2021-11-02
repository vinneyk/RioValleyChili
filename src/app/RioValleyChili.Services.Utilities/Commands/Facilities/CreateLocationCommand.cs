using System;
using System.Linq;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Facilities
{
    internal class CreateLocationCommand
    {
        private readonly IFacilityUnitOfWork _facilityUnitOfWork;

        public CreateLocationCommand(IFacilityUnitOfWork facilityUnitOfWork)
        {
            if(facilityUnitOfWork == null) { throw new ArgumentNullException("facilityUnitOfWork"); }
            _facilityUnitOfWork = facilityUnitOfWork;
        }

        internal IResult<Location> CreateLocation(CreateLocationParameters parameters)
        {
            var location = new Location();
            var updateResult = UpdateLocation(location, parameters);
            if(!updateResult.Success)
            {
                return updateResult.ConvertTo<Location>();
            }

            return new SuccessResult<Location>(_facilityUnitOfWork.LocationRepository.Add(location));
        }

        private IResult UpdateLocation(Location location, CreateLocationParameters parameters)
        {
            if(string.IsNullOrWhiteSpace(parameters.Params.Description))
            {
                return new InvalidResult(string.Format(UserMessages.LocationDescriptionRequired));
            }

            var facility = _facilityUnitOfWork.FacilityRepository.FindByKey(parameters.FacilityKey, f => f.Locations);
            if(facility == null)
            {
                return new InvalidResult(string.Format(UserMessages.FacilityNotFound, parameters.FacilityKey));
            }

            if(facility.Locations.Any(l => l.Description == parameters.Params.Description))
            {
                return new InvalidResult(string.Format(UserMessages.FacilityContainsLocationDescription, facility.Name, parameters.Params.Description));
            }

            location.LocationType = parameters.Params.LocationType;
            location.Description = parameters.Params.Description;
            location.Active = parameters.Params.Active;
            location.Locked = parameters.Params.Locked;
            location.FacilityId = parameters.FacilityKey.FacilityKey_Id;

            return new SuccessResult();
        }
    }
}