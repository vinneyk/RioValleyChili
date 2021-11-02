using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Controllers;
using RioValleyChili.Client.Reporting.Reports;

namespace RioValleyChili.Client.Mvc.Areas.Reporting.Controllers
{
    public partial class QualityControlReportingController : RvcReportingMvcControllerBase
    {
        public virtual ActionResult LabResults()
        {
            return ReportView("Lab Results Report", ReportTypes.QualityControlReports.LabResultsReport);
        }
    }
}