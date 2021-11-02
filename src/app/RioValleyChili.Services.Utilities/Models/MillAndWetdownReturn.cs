using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class MillAndWetdownReturn : IMillAndWetdownSummaryReturn, IMillAndWetdownDetailReturn
    {
        public string MillAndWetdownKey { get { return OutputChileLotKeyReturn.LotKey; } }

        public string OutputChileLotKey { get { return OutputChileLotKeyReturn.LotKey; } }

        public string ChileProductKey { get { return ChileProductKeyReturn.ProductKey; } }

        public string ProductionLineKey { get { return ProductionLineLocationKeyReturn.LocationKey; } }

        public string ShiftKey { get; internal set; }

        public string ProductionLineDescription { get; internal set; }

        public string ChileProductName { get; internal set; }

        public DateTime ProductionBegin { get; internal set; }

        public DateTime ProductionEnd { get; internal set; }

        public int TotalProductionTimeMinutes { get; internal set; }

        public int TotalWeightProduced { get; internal set; }

        public int TotalWeightPicked { get; internal set; }

        public IEnumerable<IMillAndWetdownResultItemReturn> ResultItems { get; internal set; }

        public IEnumerable<IMillAndWetdownPickedItemReturn> PickedItems { get; internal set; }

        #region Internal Parts

        internal LotKeyReturn OutputChileLotKeyReturn { get; set; }

        internal ProductKeyReturn ChileProductKeyReturn { get; set; }

        internal LocationKeyReturn ProductionLineLocationKeyReturn { get; set; }

        #endregion
    }

    internal class MillAndWetdownProductionReturn
    {
        internal string ShiftKey { get; set; }

        internal string ProductionLineDescription { get; set; }

        internal DateTime ProductionBegin { get; set; }

        internal DateTime ProductionEnd { get; set; }
    }
}