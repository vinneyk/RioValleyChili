using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;
using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IInventoryAdjustmentsService
    {
        IResult<string> CreateInventoryAdjustment(ICreateInventoryAdjustmentParameters parameters);

        IResult<IQueryable<IInventoryAdjustmentReturn>> GetInventoryAdjustments(FilterInventoryAdjustmentParameters parameters = null);

        IResult<IInventoryAdjustmentReturn> GetInventoryAdjustment(string inventoryAdjustmentKey);
    }
}