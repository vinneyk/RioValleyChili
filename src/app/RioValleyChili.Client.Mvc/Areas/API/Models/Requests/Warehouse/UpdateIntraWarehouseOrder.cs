using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse
{
    public class UpdateIntraWarehouseOrder 
    {
        public string IntraWarehouseOrderKey { get; internal set; }
        public decimal TrackingSheetNumber { get; set; }
        public string OperatorName { get; set; }
        public DateTime MovementDate { get; set; }
        public IEnumerable<PickedInventoryItemDto> PickedInventoryItems { get; set; }
        public IEnumerable<SetPickedInventoryItemCodesRequestParameter> PickedInventoryItemCodes { get; set; }
    }
}