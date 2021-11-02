using System;
using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    [Obsolete("Use SetInventoryPickOrderItem for write operations.")]
    public class InventoryPickOrderItem : ISetInventoryPickOrderItemParameters
    {
        [Required]
        public string ProductKey { get; set; }

        [Required]
        public string PackagingKey { get; set; }

        public string TreatmentKey { get; set; }

        public string CustomerKey { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [StringLength(Constants.StringLengths.CustomerLotCode)]
        public string CustomerLotCode { get; set; }

        [StringLength(Constants.StringLengths.CustomerLotCode)]
        public string CustomerProductCode { get; set; }
    }
}