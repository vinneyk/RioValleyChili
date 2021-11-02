using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production
{
    public class UpdateProductionScheduleRequest
    {
        public IEnumerable<SetProductionScheduleItemRequest> ScheduledItems { get; set; }
    }
}