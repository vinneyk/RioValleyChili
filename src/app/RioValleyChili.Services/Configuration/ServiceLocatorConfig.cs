using Ninject;
using Ninject.Modules;
using RioValleyChili.Services.Configuration.NinjectModules;
using RioValleyChili.Services.Interfaces;
using Solutionhead.Core;

namespace RioValleyChili.Services.Configuration
{
    public static class ServiceLocatorConfig
    {
        public static void Configure(IKernel kernel)
        {
            kernel.Load(new INinjectModule[]
                {
                    new UnitOfWorkModule()
                });

            kernel.Bind<IInventoryService>().To<InventoryService>();
            kernel.Bind<IInventoryAdjustmentsService>().To<InventoryAdjustmentsService>();
            kernel.Bind<ITreatmentOrderService>().To<TreatmentOrderService>();
            kernel.Bind<IFacilityService>().To<FacilityService>();
            kernel.Bind<IIntraWarehouseOrderService>().To<IntraWarehouseOrderService>();
            kernel.Bind<IProductService>().To<ProductService>();
            kernel.Bind<IProductionResultsService>().To<ProductionResultsService>();
            kernel.Bind<IProductionService>().To<ProductionService>();
            kernel.Bind<ILotService>().To<LotService>();
            kernel.Bind<IMaterialsReceivedService>().To<MaterialsReceivedService>();
            kernel.Bind<IMillAndWetDownService>().To<MillAndWetdownService>();
            kernel.Bind<ISalesService>().To<SalesService>();
            kernel.Bind<IWarehouseOrderService>().To<WarehouseOrderService>();
            kernel.Bind<ICompanyService>().To<CompanyService>();
            kernel.Bind<INotebookService>().To<NotebookService>();
            kernel.Bind<IEmployeesService>().To<EmployeesService>();
            kernel.Bind<IInventoryShipmentOrderService>().To<InventoryShipmentOrderService>();
            kernel.Bind<ISampleOrderService>().To<SampleOrderService>();
            
            kernel.Bind<ITimeStamper>().To<UniversalTimeStamper>().InSingletonScope();
        }
    }
}