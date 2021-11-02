using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class SetChileProductAttributeRangesRequest
    {
        public IEnumerable<AttributeRangeRequest> AttributeRanges { get; set; }
    }
}