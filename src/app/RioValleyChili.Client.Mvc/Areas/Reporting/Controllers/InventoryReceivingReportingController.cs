using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Controllers;
using RioValleyChili.Client.Reporting.Reports;

namespace RioValleyChili.Client.Mvc.Areas.Reporting.Controllers
{
    public partial class InventoryReceivingReportingController : RvcReportingMvcControllerBase
    {
        public virtual ActionResult ChileMaterialsReceivedRecap(string key)
        {
            return Pdf(
                ReportTypes.MaterialsReceivedReports.ChileRecapReport,
                new { lotKey = key });
        }
    }
}