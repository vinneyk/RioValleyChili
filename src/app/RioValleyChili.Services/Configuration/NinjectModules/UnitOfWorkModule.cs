using Ninject.Modules;
using RioValleyChili.Data;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;

namespace RioValleyChili.Services.Configuration.NinjectModules
{
    public class UnitOfWorkModule : NinjectModule
    {
        #region Overrides of NinjectModule

        public override void Load()
        {
            Kernel.Bind<IRVCUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IIntraWarehouseOrderUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IInventoryPickOrderUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IInventoryUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<ILotUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IPickedInventoryUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IProductionUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IShipmentUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<ITreatmentOrderUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IFacilityUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IProductUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IMaterialsReceivedUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IChileLotProductionUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<ISalesUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<ICompanyUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<INotebookUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<ICoreUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<IInventoryShipmentOrderUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
            Kernel.Bind<ISampleOrderUnitOfWork>().To<EFRVCUnitOfWork>().InTransientScope();
        }

        #endregion
    }
}