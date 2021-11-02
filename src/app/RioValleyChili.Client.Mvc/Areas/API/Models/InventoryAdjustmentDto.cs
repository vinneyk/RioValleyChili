using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class InventoryAdjustmentDto : ICreateInventoryAdjustmentParameters
    {
        public string UserToken { get; set; }

        public string Comment { get; set; }

        public IEnumerable<InventoryAdjustmentItemDto> InventoryAdjustments {get; set; }

        IEnumerable<IInventoryAdjustmentParameters> ICreateInventoryAdjustmentParameters.InventoryAdjustments { get { return InventoryAdjustments; } }
    }

    public class InventoryAdjustmentItemDto : IInventoryAdjustmentParameters
    {
        public string LotKey { get; set; }

        public string WarehouseLocationKey { get; set; }

        public string PackagingProductKey { get; set; }

        public string TreatmentKey { get; set; }

        public string ToteKey { get; set; }

        public int Adjustment { get; set; }
    }
}