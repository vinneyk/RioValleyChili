using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class LotInputTraceResponse
    {
        public IEnumerable<string> LotPath { get; set; }
        public string Treatment { get; set; }
    }
}