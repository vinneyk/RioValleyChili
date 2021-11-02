using System;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IProductionResultsService
    {
        IResult<IQueryable<IProductionResultSummaryReturn>> GetProductionResultSummaries();
        IResult<IProductionResultDetailReturn> GetProductionResultDetail(string lotKey);

        [Obsolete("Use GetProductionResultDetail instead - Production results are keyed by LotKey now. -RI 6/16/2014")]
        IResult<IProductionResultDetailReturn> GetProductionResultDetailByProductionBatchKey(string lotKey);

        IResult<string> CreateProductionBatchResults(ICreateProductionBatchResultsParameters parameters);
        IResult UpdateProductionBatchResults(IUpdateProductionBatchResultsParameters parameters);
        IResult<IProductionRecapReportReturn> GetProductionRecapReport(DateTime startDate, DateTime endDate);
        IResult<IProductionAdditiveInputsReportReturn> GetProductionAdditiveInputsReport(DateTime startDate, DateTime endDate);
    }
}