using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService
{
    public interface ICreateInventoryAdjustmentParameters : IUserIdentifiable
    {
        /// <summary>
        /// A comment supplied by the users describing the details regarding the reason for the adjustment.
        /// </summary>
        string Comment { get; }

        /// <summary>
        /// An enumerable of adjustments to make by Warehouse Location and Packaging Type.
        /// </summary>
        IEnumerable<IInventoryAdjustmentParameters> InventoryAdjustments { get; }
    }
}