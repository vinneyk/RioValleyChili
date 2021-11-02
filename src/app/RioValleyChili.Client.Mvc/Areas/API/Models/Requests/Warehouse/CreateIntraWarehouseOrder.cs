using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse
{
    public class CreateIntraWarehouseOrder
    {
        public decimal TrackingSheetNumber { get; set; }
        public string OperatorName { get; set; }
        public DateTime MovementDate { get; set; }
        public IEnumerable<PickedInventoryItemWithDestinationDto> PickedInventoryItems { get; set; }
    }
}