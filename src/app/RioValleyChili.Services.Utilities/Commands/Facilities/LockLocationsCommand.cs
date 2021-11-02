using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Facilities
{
    internal class LockLocationsCommand
    {
        private readonly IFacilityUnitOfWork _facilityUnitOfWork;

        internal LockLocationsCommand(IFacilityUnitOfWork facilityUnitOfWork)
        {
            if(facilityUnitOfWork == null) { throw new ArgumentNullException("facilityUnitOfWork"); }
            _facilityUnitOfWork = facilityUnitOfWork;
        }

        internal IResult Execute(List<LocationKey> keys, bool @lock)
        {
            if(keys == null) { throw new ArgumentNullException("keys"); }

            var predicate = PredicateHelper.OrPredicates(keys.Select(k => k.FindByPredicate));
            var locations = _facilityUnitOfWork.LocationRepository.Filter(predicate).ToList();
            var missingKey = keys.FirstOrDefault(k => !locations.Any(k.FindByPredicate.Compile()));
            if(missingKey != null)
            {
                return new InvalidResult(string.Format(UserMessages.LocationNotFound, missingKey));
            }

            locations.ForEach(l => l.Locked = @lock);

            return new SuccessResult();
        }
    }
}