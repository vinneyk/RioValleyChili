using System;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionResultsService
{
    public interface IProductionResultSummaryReturn : IProductionResultBaseReturn
    {
        string ProductionResultKey { get; }

        string ProductionBatchKey { get; }

        string OutputLotNumber { get; }

        string ProductionLineKey { get; }

        string ChileProductKey { get; }

        DateTime DateTimeEntered { get; }

        DateTime ProductionStartDate { get; }

        string ChileProductName { get; }
        
        double TargetBatchWeight { get; }

        string WorkType { get; }

        string BatchStatus { get; } // enum?
    }
}