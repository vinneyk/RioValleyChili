using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.Keys
{
    public static class WarehouseKeyExtensions
    {
        #region Extensions.

        public static string BuildKey(this Facility facility)
        {
            return new FacilityKey(facility).ToString();
        }

        public static FacilityKey GetKey(this Facility facility)
        {
            return new FacilityKey(facility);
        }

        #endregion
    }
}
