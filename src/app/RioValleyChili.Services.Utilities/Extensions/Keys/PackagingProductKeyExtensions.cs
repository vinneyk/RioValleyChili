using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.Keys
{
    public static class PackagingProductKeyExtensions
    {
        #region Extension.

        public static string BuildKey(this PackagingProduct packagingProduct)
        {
            return new PackagingProductKey(packagingProduct).ToString();
        }

        public static PackagingProductKey GetKey(this PackagingProduct packagingProduct)
        {
            return new PackagingProductKey(packagingProduct);
        }

        public static PackagingProductKey GetPackagingProductKey(this PackSchedule packSchedule)
        {
            if(packSchedule == null) { throw new ArgumentNullException("packSchedule"); }
            return new PackagingProductKey(packSchedule);
        }

        #endregion
    }
}