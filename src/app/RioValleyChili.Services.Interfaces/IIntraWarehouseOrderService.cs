using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IIntraWarehouseOrderService 
    {
        IResult<string> CreateIntraWarehouseOrder(ICreateIntraWarehouseOrderParameters parameters);

        IResult UpdateIntraWarehouseOrder(IUpdateIntraWarehouseOrderParameters parameters);

        IResult<IQueryable<IIntraWarehouseOrderSummaryReturn>> GetIntraWarehouseOrderSummaries();

        IResult<IQueryable<IIntraWarehouseOrderDetailReturn>> GetIntraWarehouseOrders();
    }
}