using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Data.Models.StaticRecords
{
    public static class StaticFacilities
    {
        public static readonly Facility Rincon = new Facility
            {
                Id = Constants.StaticKeyValues.RinconFacilityKey,
                Active = true,
                Name = "Rincon",
                FacilityType = FacilityType.Internal
            };

        public static readonly List<Facility> Warehouses = new List<Facility>
            {
                Rincon
            };
    }
}