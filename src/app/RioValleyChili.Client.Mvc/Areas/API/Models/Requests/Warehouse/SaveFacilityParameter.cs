using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse
{
    public class SaveFacilityParameter
    {
        public FacilityType FacilityType { get; set; }
        [Required]
        public string FacilityName { get; set; }
        public bool Active { get; set; }
        public string PhoneNumber { get; set; }
        public string EMailAddress { get; set; }
        public string ShippingLabelName { get; set; }
        public Address Address { get; set; }
    }
}