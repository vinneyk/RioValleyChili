using System;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public class IntraWarehouseOrderDetails
    {
        public string OrderKey { get; set; }

        public decimal TrackingSheetNumber { get; set; }

        public string OperatorName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime MovementDate { get; set; }

        public PickedInventoryWithDestination PickedInventoryDetail { get; set; }
    }
}