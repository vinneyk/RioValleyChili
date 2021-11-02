using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateInventoryAdjustmentParameters : ICreateInventoryAdjustmentParameters
    {
        public string UserToken { get; set; }
        public string Comment { get; set; }
        public IEnumerable<IInventoryAdjustmentParameters> InventoryAdjustments { get; set; }
    }
}