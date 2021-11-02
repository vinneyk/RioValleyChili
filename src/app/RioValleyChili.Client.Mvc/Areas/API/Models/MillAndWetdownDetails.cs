using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    internal class MillAndWetdownDetails 
    {
        public bool Editable { get; set; }

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

        public IEnumerable<MillAndWetdownResultItem> ResultItems { get; set; }

        public IEnumerable<MillAndWetdownPickedItem> PickedItems { get; set; }
    }
}