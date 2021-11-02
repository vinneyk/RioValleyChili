using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionResultsService
{
    public interface IProductionResultDetailReturn : IProductionResultSummaryReturn
    {
        IEnumerable<IProductionResultItemReturn> ResultItems { get; }
    }
}