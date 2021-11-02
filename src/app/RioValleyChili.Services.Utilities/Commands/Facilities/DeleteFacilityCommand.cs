using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Facilities
{
    internal class DeleteFacilityCommand
    {
        private readonly IFacilityUnitOfWork _facilityUnitOfWork;

        internal DeleteFacilityCommand(IFacilityUnitOfWork facilityUnitOfWork)
        {
            if(facilityUnitOfWork == null) { throw new ArgumentNullException("facilityUnitOfWork"); }
            _facilityUnitOfWork = facilityUnitOfWork;
        }

        internal IResult<Facility> DeleteFacility(IFacilityKey facilityKey)
        {
            var key = new FacilityKey(facilityKey);
            var facility = _facilityUnitOfWork.FacilityRepository.FindByKey(key);
            if(facility == null)
            {
                return new InvalidResult<Facility>(null, string.Format(UserMessages.FacilityNotFound, key));
            }

            _facilityUnitOfWork.FacilityRepository.Remove(facility);

            return new SuccessResult<Facility>(facility);
        }
    }
}