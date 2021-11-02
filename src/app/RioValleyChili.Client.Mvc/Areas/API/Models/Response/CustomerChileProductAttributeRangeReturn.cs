using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class CustomerChileProductAttributeRangeReturn 
    {
        public string CustomerChileProductAttributeRangeKey { get; set; }
        public string AttributeShortName { get; set; }
        public double RangeMin { get; set; }
        public double RangeMax { get; set; }
    }

    public class CustomerChileProductAttributeRangesReturn
    {
        public ChileProductResponse ChileProduct { get; set; }
        public IEnumerable<CustomerChileProductAttributeRangeReturn> AttributeRanges { get; set; }
    }
}