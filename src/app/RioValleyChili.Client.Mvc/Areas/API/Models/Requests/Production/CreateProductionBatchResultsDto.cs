using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production
{
    public class CreateProductionBatchResultsDto 
    {
        public string ProductionBatchKey { get; set; }
        public string ProductionShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionStartTimestamp { get; set; }
        public DateTime ProductionEndTimestamp { get; set; }

        public IEnumerable<ProductionResultItemDto> InventoryItems { get; set; }
        public IEnumerable<PickedInventoryItemDto> PickedInventoryItemChanges { get; set; }
    }
}