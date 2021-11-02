using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;
using RioValleyChili.Services.Interfaces.ServiceCompositions;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IWarehouseOrderService : IPickInventoryServiceComponent
    {
        /// <summary>
        /// Creates a new Inventory Warehouse Order.
        /// </summary>
        /// <param name="parameters">Specifics of the inventory warehouse order.</param>
        IResult<string> CreateWarehouseOrder(ISetOrderParameters parameters);
        IResult UpdateInterWarehouseOrder(IUpdateInterWarehouseOrderParameters parameters);
        IResult<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>> GetWarehouseOrder(string orderKey);
        IResult<IQueryable<IInventoryShipmentOrderSummaryReturn>> GetWarehouseOrders(FilterInterWarehouseOrderParameters parameters = null);
    }
}