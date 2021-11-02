using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.TreatmentOrderService;
using RioValleyChili.Services.Interfaces.ServiceCompositions;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface ITreatmentOrderService : IPickInventoryServiceComponent
    {
        IResult<string> CreateInventoryTreatmentOrder(ICreateTreatmentOrderParameters parameters);
        IResult UpdateTreatmentOrder(IUpdateTreatmentOrderParameters parameters);
        IResult DeleteTreatmentOrder(string orderKey);
        IResult ReceiveOrder(IReceiveTreatmentOrderParameters parameters);
        IResult<IEnumerable<IInventoryTreatmentReturn>> GetInventoryTreatments();
        IResult<ITreatmentOrderDetailReturn> GetTreatmentOrder(string orderKey);
        IResult<IQueryable<ITreatmentOrderSummaryReturn>> GetTreatmentOrders();
    }
}