using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService
{
    public interface IMillAndWetdownDetailReturn
    {
        string MillAndWetdownKey { get; }

        string OutputChileLotKey { get; }

        string ChileProductKey { get; }

        string ProductionLineKey { get; }

        string ShiftKey { get; }

        string ProductionLineDescription { get; }

        string ChileProductName { get; }

        DateTime ProductionBegin { get; }

        DateTime ProductionEnd { get; }

        int TotalProductionTimeMinutes { get; }

        int TotalWeightProduced { get; }

        int TotalWeightPicked { get; }

        IEnumerable<IMillAndWetdownResultItemReturn> ResultItems { get; }

        IEnumerable<IMillAndWetdownPickedItemReturn> PickedItems { get; }
    }
}