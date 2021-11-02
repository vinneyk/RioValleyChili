using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class UpdateLotRequest
    {
        public string LotKey { get; set; }
        public string Notes { get; set; }
        public IEnumerable<LotAttributeRequest> Attributes { get; set; }
        public bool OverrideOldContextLotAsCompleted { get; set; }
    }
}