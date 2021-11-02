using System;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production
{
    public class ProductionScheduleSummaryResponse
    {
        public string ProductionScheduleKey { get; set; }
        public DateTime ProductionDate { get; set; }
        public FacilityLocationResponse ProductionLine { get; set; }
    }
}