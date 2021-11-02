// ReSharper disable RedundantExtendsListEntry
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface ITreatmentOrderUnitOfWork : IUnitOfWork,
        ICompanyUnitOfWork,
        IInventoryPickOrderUnitOfWork,
        IPickedInventoryUnitOfWork,
        IShipmentUnitOfWork,
        IFacilityUnitOfWork,
        IInventoryUnitOfWork
    {
        IRepository<TreatmentOrder> TreatmentOrderRepository { get; }
        IRepository<InventoryTreatment> InventoryTreatmentRepository { get; }
        IRepository<InventoryShipmentOrder> InventoryShipmentOrderRepository { get; }
    }
}
// ReSharper restore RedundantExtendsListEntry