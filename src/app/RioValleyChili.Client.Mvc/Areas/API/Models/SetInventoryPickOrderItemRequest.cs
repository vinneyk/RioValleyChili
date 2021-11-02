using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class SetInventoryPickOrderItemRequest 
    {
        [Required]
        public string ProductKey { get; set; }

        [Required]
        public string PackagingKey { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public string TreatmentKey { get; set; }

        [StringLength(Constants.StringLengths.CustomerLotCode)]
        public string CustomerLotCode { get; set; }

        [StringLength(Constants.StringLengths.CustomerLotCode)]
        public string CustomerProductCode { get; set; }

        public string CustomerKey { get; set; }
    }
}