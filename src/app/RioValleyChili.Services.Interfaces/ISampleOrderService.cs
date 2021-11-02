using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface ISampleOrderService
    {
        IResult<string> SetSampleOrder(ISetSampleOrderParameters parameters);
        IResult DeleteSampleOrder(string sampleOrderKey);
        IResult SetSampleSpecs(ISetSampleSpecsParameters parameters);
        IResult SetSampleMatch(ISetSampleMatchParameters parameters);
        IResult<ISampleOrderJournalEntryReturn> SetJournalEntry(ISetSampleOrderJournalEntryParameters parameters);
        IResult DeleteJournalEntry(string journalEntryKey);
        IResult<IEnumerable<string>> GetCustomerProducNames();

        IResult<ISampleOrderDetailReturn> GetSampleOrder(string sampleOrderKey);
        IResult<IQueryable<ISampleOrderSummaryReturn>> GetSampleOrders(FilterSampleOrdersParameters parameters = null);

        IResult<ISampleOrderMatchingSummaryReportReturn> GetSampleOrderMatchingSummaryReport(string sampleOrderKey, string itemKey);
        IResult<ISampleOrderRequestReportReturn> GetSampleOrderRequestReport(string sampleOrderKey);
    }
}