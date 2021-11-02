using System;
using System.ComponentModel;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public class SampleOrderReportingService : ReportingService
    {
        #region initialization
        
        private readonly ISampleOrderService _sampleOrderService;

        public SampleOrderReportingService() : this(ResolveService<ISampleOrderService>()) { }

        public SampleOrderReportingService(ISampleOrderService sampleOrderService)
        {
            if (sampleOrderService == null) throw new ArgumentNullException("sampleOrderService");
            _sampleOrderService = sampleOrderService;
        }

        #endregion

        public SampleOrderMatchingSummaryReportModel GetSummaryReport(string sampleOrderKey, string itemKey = null)
        {
            var result = _sampleOrderService.GetSampleOrderMatchingSummaryReport(sampleOrderKey, itemKey);
            var mapped = result.ResultingObject.Map().To<SampleOrderMatchingSummaryReportModel>();
            return mapped;
        }

        public SampleOrderRequestReportModel GetSampleOrderRequestReport(string sampleOrderKey)
        {
            var result = _sampleOrderService.GetSampleOrderRequestReport(sampleOrderKey);
            var mapped = result.ResultingObject.Map().To<SampleOrderRequestReportModel>();
            return mapped;
        }
    }
}