using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class AddLotAttributesRequest
    {
        public IEnumerable<string> LotKeys { get; set; }
        public IEnumerable<LotAttributeRequest> Attributes { get; set; }
        public bool OverrideOldContextLotAsCompleted { get; set; }
    }
}