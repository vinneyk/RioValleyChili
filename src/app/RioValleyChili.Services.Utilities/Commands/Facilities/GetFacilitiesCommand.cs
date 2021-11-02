using System;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Facilities
{
    internal class GetFacilitiesCommand
    {
        #region fields and constructors

        private readonly IFacilityUnitOfWork _facilityUnitOfWork;

        public GetFacilitiesCommand(IFacilityUnitOfWork facilityUnitOfWork)
        {
            if (facilityUnitOfWork == null) { throw new ArgumentNullException("facilityUnitOfWork"); }
            _facilityUnitOfWork = facilityUnitOfWork;
        }

        #endregion

        #region Implementation of IResultWithInputCommand<out Warehouse,in IWarehouseKey>

        public IResult<IQueryable<FacilityReturn>> Execute(FacilityKey facilityKey, bool includeLocations, bool includeShippingLabel)
        {
            facilityKey = facilityKey ?? FacilityKey.Null.ToFacilityKey();
            var filterFacilityKey = !facilityKey.Equals(FacilityKey.Null);
            var selector = FacilityProjectors.Select(includeLocations, includeShippingLabel);

            var result = _facilityUnitOfWork.FacilityRepository.Filter(w => (!filterFacilityKey || w.Id == facilityKey.FacilityKey_Id));
            var results = result.OrderBy(w => w.Name).AsExpandable().Select(w => selector.Invoke(w));

            return new SuccessResult<IQueryable<FacilityReturn>>(results);
        }

        #endregion
    }
}
