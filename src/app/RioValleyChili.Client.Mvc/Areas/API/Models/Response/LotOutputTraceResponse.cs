using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class LotOutputTraceResponse
    {
        public IEnumerable<string> LotPath { get; set; }
        public IEnumerable<LotOutputTraceInputResponse> Inputs { get; set; }
        public IEnumerable<LotOutputTraceOrdersResponse> Orders { get; set; }
    }

    public class LotOutputTraceInputResponse
    {
        public string LotKey { get; set; }
        public string Treatment { get; set; }
    }

    public class LotOutputTraceOrdersResponse
    {
        public string Treatment { get; set; }
        public int? OrderNumber { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string CustomerName { get; set; }
    }
}