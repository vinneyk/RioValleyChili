using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse
{
    public abstract class CreateLocationParameter
    {
        public abstract LocationType LocationType { get; }

        [Required]
        public string FacilityKey { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public int Row { get; set; }

        public LocationStatus Status { get; set; }
    }

    public class CreateWarehouseLocationParameter : CreateLocationParameter
    {
        public override LocationType LocationType { get { return LocationType.Warehouse; } }
    }
}