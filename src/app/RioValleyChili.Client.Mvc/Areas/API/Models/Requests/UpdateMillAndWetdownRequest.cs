using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class UpdateMillAndWetdownRequest
    {
        public string UserToken { get; set; }
        public string LotKey { get; set; }
        public string ChileProductKey { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionBegin { get; set; }
        public DateTime ProductionEnd { get; set; }

        public IEnumerable<MillAndWetdownResultItemRequest> ResultItems { get; set; }
        public IEnumerable<MillAndWetdownPickedItemRequest> PickedItems { get; set; }
    }
}