using System;
using System.Linq.Expressions;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    public static class LocationPredicates
    {
        public static Expression<Func<Location, bool>> ProductionLinesFilter
        {
            get { return l => l.LocationType == LocationType.ProductionLine; }
        }

        public static Expression<Func<Location, bool>> ByFacilityKey(IFacilityKey facilityKey)
        {
            return l => l.FacilityId == facilityKey.FacilityKey_Id;
        }
    }
}