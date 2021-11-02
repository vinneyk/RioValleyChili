using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IShipmentUnitOfWork : IUnitOfWork
    {
        IRepository<ShipmentInformation> ShipmentInformationRepository { get; }
        IRepository<InventoryShipmentOrder> InventoryShipmentOrderRepository { get; }
    }
}