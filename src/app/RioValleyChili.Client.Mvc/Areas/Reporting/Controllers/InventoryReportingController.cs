using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Controllers;
using RioValleyChili.Client.Reporting.Reports;

namespace RioValleyChili.Client.Mvc.Areas.Reporting.Controllers
{
    public partial class InventoryReportingController : RvcReportingMvcControllerBase
    {
        // */Reporting/InventoryReporting/CycleCount
        public virtual ActionResult CycleCount(string facilityKey, string groupName)
        {
            return ReportView(
                "Inventory Cycle Count Report",
                ReportTypes.InventoryReports.InventoryCycleCountReport,
                new
                {
                    FacilityKey = facilityKey,
                    GroupName = groupName
                });
        }
    }
}