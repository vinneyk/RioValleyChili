using System;

namespace RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService
{
    public interface IMillAndWetdownSummaryReturn
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
    }
}