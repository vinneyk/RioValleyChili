using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse
{
    public class UpdateLocationParameter
    {
        public string LocationKey { get; internal set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public int Row { get; set; }
        
        public LocationStatus? Status { get; set; }
    }
}