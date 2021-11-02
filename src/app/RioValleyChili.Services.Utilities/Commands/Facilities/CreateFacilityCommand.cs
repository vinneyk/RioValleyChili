using System;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Facilities
{
    internal class CreateFacilityCommand
    {
        private readonly IFacilityUnitOfWork _facilityUnitOfWork;

        internal CreateFacilityCommand(IFacilityUnitOfWork facilityUnitOfWork)
        {
            if(facilityUnitOfWork == null) { throw new ArgumentNullException("facilityUnitOfWork"); }
            _facilityUnitOfWork = facilityUnitOfWork;
        }

        internal IResult<Facility> CreateFacility(ICreateFacilityParameters parameters)
        {
            var facility = _facilityUnitOfWork.FacilityRepository.Add(new Facility());
            var updateResult = new UpdateFacilityCommand(_facilityUnitOfWork).UpdateFacility(facility, parameters);
            if(!updateResult.Success)
            {
                return updateResult.ConvertTo<Facility>();
            }

            return new SuccessResult<Facility>(facility);
        }
    }
}