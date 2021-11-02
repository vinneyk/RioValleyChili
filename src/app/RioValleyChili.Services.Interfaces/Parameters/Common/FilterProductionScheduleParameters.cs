using System;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public class FilterProductionScheduleParameters
    {
        public DateTime? ProductionDate { get; set; }
        public string ProductionLineLocationKey { get; set; }
    }
}