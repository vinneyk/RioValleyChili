using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production
{
    public class CreateProductionScheduleRequest
    {
        public DateTime ProductionDate { get; set; }
        public string ProductionLineLocationKey { get; set; }
    }
}