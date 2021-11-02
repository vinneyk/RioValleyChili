using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.Keys
{
    public static class AdditiveTypeKeyExtensions
    {
        #region Extensions.

        public static string BuildKey(this AdditiveType additiveType)
        {
            return new AdditiveTypeKey(additiveType).ToString();
        }

        #endregion
    }
}
