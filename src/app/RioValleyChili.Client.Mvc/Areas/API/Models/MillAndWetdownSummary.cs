using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class MillAndWetdownSummary 
    {
        public string MillAndWetdownKey { get; set; }
        public string OutputChileLotKey { get; set; }
        public string ChileProductKey { get; set; }
        public string ProductionLineKey { get; set; }
        public string ShiftKey { get; set; }
        public string ProductionLineDescription { get; set; }
        public string ChileProductName { get; set; }
        public DateTime ProductionBegin { get; set; }
        public DateTime ProductionEnd { get; set; }
        public int TotalProductionTimeMinutes { get; set; }
        public int TotalWeightProduced { get; set; }
        public int TotalWeightPicked { get; set; }
    }
}