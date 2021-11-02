using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Controllers;
using RioValleyChili.Client.Reporting.Reports;

namespace RioValleyChili.Client.Mvc.Areas.Reporting.Controllers
{
    public partial class SampleOrderReportingController : RvcReportingMvcControllerBase
    {
        public virtual ActionResult SampleMatchingSummary(string sampleOrderKey, string itemKey = null)
        {
            return Pdf(
                ReportTypes.SampleOrderReports.MatchSummaryReport,
                new { sampleOrderKey = sampleOrderKey, itemKey = itemKey });
        }

        public virtual ActionResult SampleRequest(string sampleOrderKey)
        {
            return Pdf(
                ReportTypes.SampleOrderReports.SampleRequestReport,
                new { sampleOrderKey = sampleOrderKey });
        }
    }
}