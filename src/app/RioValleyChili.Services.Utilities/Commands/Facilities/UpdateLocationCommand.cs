using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Facilities
{
    internal class UpdateLocationCommand
    {
        private readonly IFacilityUnitOfWork _facilityUnitOfWork;

        internal UpdateLocationCommand(IFacilityUnitOfWork facilityUnitOfWork)
        {
            if(facilityUnitOfWork == null) { throw new ArgumentNullException("facilityUnitOfWork"); }
            _facilityUnitOfWork = facilityUnitOfWork;
        }

        internal IResult UpdateLocation(UpdateLocationParameters parameters)
        {
            var location = _facilityUnitOfWork.LocationRepository.FindByKey(parameters.LocationKey);
            if(location == null)
            {
                return new InvalidResult(string.Format(UserMessages.LocationNotFound, parameters.LocationKey));
            }

            return UpdateLocation(location, new UpdateLocationParameters
                {
                    Params = parameters.Params,
                });
        }

        internal IResult UpdateLocation(Location location, UpdateLocationParameters parameters)
        {
            location.Description = parameters.Params.Description;
            location.Active = parameters.Params.Active;
            location.Locked = parameters.Params.Locked;

            return new SuccessResult();
        }
    }
}