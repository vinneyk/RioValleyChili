using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.Keys
{
    public static class AdditiveProductKeyExtensions
    {
        #region Extension.

        public static string BuildKey(this AdditiveProduct additiveProduct)
        {
            return new AdditiveProductKey(additiveProduct).ToString();
        }

        public static AdditiveProductKey GetKey(this AdditiveProduct additiveProduct)
        {
            return new AdditiveProductKey(additiveProduct);
        }

        #endregion
    }
}