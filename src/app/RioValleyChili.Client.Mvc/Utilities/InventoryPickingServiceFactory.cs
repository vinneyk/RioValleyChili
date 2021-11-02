using System;
using System.Web.Mvc;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.ServiceCompositions;

namespace RioValleyChili.Client.Mvc.Utilities
{
    public static class InventoryPickingServiceFactory
    {
        public static IPickInventoryServiceComponent ResolveComponent(InventoryOrderEnum pickingContext)
        {
            switch (pickingContext)
            {
                case InventoryOrderEnum.TransWarehouseMovements:
                    return DependencyResolver.Current.GetService<IWarehouseOrderService>();
                case InventoryOrderEnum.Treatments:
                    return DependencyResolver.Current.GetService<ITreatmentOrderService>();
                case InventoryOrderEnum.ProductionBatch:
                    return DependencyResolver.Current.GetService<IProductionService>();
                case InventoryOrderEnum.CustomerOrder:
                    return DependencyResolver.Current.GetService<ISalesService>();
                default: throw new NotSupportedException(string.Format("Unsupported picking context: \"{0}\". The requested InventoryOrderEnum value is not registered as a valid factory option.", pickingContext));
            }
        }
    }
}