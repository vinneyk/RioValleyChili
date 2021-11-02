using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class PickedInventoryItemDto
    {
        [Required]
        public string InventoryKey { get; set; }

        public string OrderItemKey { get; set; }
        
        [Range(0, int.MaxValue)]
        public int QuantityPicked { get; set; }

        [StringLength(Constants.StringLengths.CustomerLotCode)]
        public string CustomerLotCode { get; set; }
        [StringLength(Constants.StringLengths.CustomerProductCode)]
        public string CustomerProductCode { get; set; }
    }

    public class PickedInventoryItemWithDestinationDto : PickedInventoryItemDto
    {
        public string DestinationLocationKey { get; set; }
    }
}