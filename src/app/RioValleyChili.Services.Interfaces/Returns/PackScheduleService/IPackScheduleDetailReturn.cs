using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IPackScheduleDetailReturn : IPackScheduleSummaryReturn
    {
        string PackagingProductKey { get; }

        string PackagingProductName { get; }

        double PackagingWeight { get; }

        string SummaryOfWork { get; }

        IEnumerable<IProductionBatchSummaryReturn> ProductionBatches { get; }
    }
}