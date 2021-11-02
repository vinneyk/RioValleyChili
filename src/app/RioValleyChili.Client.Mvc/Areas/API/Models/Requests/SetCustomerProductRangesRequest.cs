using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetCustomerProductRangesRequest
    {
        public string CustomerKey { get; set; }
        public string ChileProductKey { get; set; }
        public IEnumerable<CustomerProductRangeRequest> AttributeRanges { get; set; }
    }

    public class CustomerProductRangeRequest
    {
        [Required]
        public string AttributeNameKey { get; set; }
        public double RangeMin { get; set; }
        public double RangeMax { get; set; }
    }
}