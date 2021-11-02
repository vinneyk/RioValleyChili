// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LocationProjectors
    {
        internal static Expression<Func<Location, LocationKeyReturn>> SelectLocationKey()
        {
            return l => new LocationKeyReturn
                {
                    LocationKey_Id = l.Id
                };
        }

        internal static Expression<Func<Location, LocationReturn>> SelectLocation()
        {
            var locationKey = SelectLocationKey();
            var facilityKey = FacilityProjectors.SelectFacilityKey();

            return Projector<Location>.To(l => new LocationReturn
                {
                    LocationKeyReturn = locationKey.Invoke(l),
                    Description = l.Description,
                    Active = l.Facility.Active,
                    Status = !l.Active ? LocationStatus.InActive : l.Locked ? LocationStatus.Locked : LocationStatus.Available,

                    FacilityKeyReturn = facilityKey.Invoke(l.Facility),
                    FacilityName = l.Facility.Name
                });
        }

        internal static Expression<Func<Location, LocationGroupNameReturn>> SelectGroupName()
        {
            return Projector<Location>.To(l => new LocationGroupNameReturn
                {
                    Description = l.Description,
                });
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup