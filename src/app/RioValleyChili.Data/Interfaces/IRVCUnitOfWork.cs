// ReSharper disable RedundantExtendsListEntry
// ReSharper disable PossibleInterfaceMemberAmbiguity

using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces
{
    public interface IRVCUnitOfWork : IUnitOfWork,
        IIntraWarehouseOrderUnitOfWork,
        IInventoryPickOrderUnitOfWork,
        IInventoryUnitOfWork,
        ILotUnitOfWork,
        IPickedInventoryUnitOfWork,
        IProductionUnitOfWork,
        IShipmentUnitOfWork,
        ITreatmentOrderUnitOfWork,
        IFacilityUnitOfWork,
        IProductUnitOfWork,
        IMaterialsReceivedUnitOfWork,
        ISalesUnitOfWork,
        ICompanyUnitOfWork,
        ICoreUnitOfWork,
        IInventoryShipmentOrderUnitOfWork,
        ISampleOrderUnitOfWork
    { }
}

// ReSharper restore PossibleInterfaceMemberAmbiguity
// ReSharper restore RedundantExtendsListEntry