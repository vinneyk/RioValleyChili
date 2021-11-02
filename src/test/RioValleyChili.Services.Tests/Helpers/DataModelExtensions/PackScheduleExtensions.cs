using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class PackScheduleExtensions
    {
        internal static PackSchedule SetChileProduct(this PackSchedule packSchedule, IChileProductKey chileProductKey)
        {
            if(packSchedule == null)
            {
                throw new ArgumentNullException("packSchedule");
            }

            packSchedule.ChileProduct = null;
            packSchedule.ChileProductId = chileProductKey.ChileProductKey_ProductId;

            return packSchedule;
        }

        internal static PackSchedule SetPackagingProduct(this PackSchedule packSchedule, IPackagingProductKey packagingProductKey)
        {
            if(packSchedule == null)
            {
                throw new ArgumentNullException("packSchedule");
            }

            packSchedule.PackagingProduct = null;
            packSchedule.PackagingProductId = packagingProductKey.PackagingProductKey_ProductId;

            return packSchedule;
        }

        internal static PackSchedule SetWorkType(this PackSchedule packSchedule, IWorkTypeKey workTypeKey)
        {
            if(packSchedule == null)
            {
                throw new ArgumentNullException("packSchedule");
            }
            if(workTypeKey == null)
            {
                throw new ArgumentNullException("workTypeKey");
            }

            packSchedule.WorkType = null;
            packSchedule.WorkTypeId = workTypeKey.WorkTypeKey_WorkTypeId;

            return packSchedule;
        }

        internal static PackSchedule SetCustomerKey(this PackSchedule packSchedule, ICustomerKey customerKey)
        {
            if(packSchedule == null)
            {
                throw new ArgumentNullException("packSchedule");
            }

            packSchedule.Customer = null;
            packSchedule.CustomerId = customerKey == null ? (int?)null : customerKey.CustomerKey_Id;

            return packSchedule;
        }

        internal static PackSchedule SetProductionLine(this PackSchedule packSchedule, ILocationKey productionLine)
        {
            if(packSchedule == null) { throw new ArgumentNullException("packSchedule"); }

            packSchedule.ProductionLineLocation = null;
            packSchedule.ProductionLineLocationId = productionLine.LocationKey_Id;

            return packSchedule;
        }
    }
}