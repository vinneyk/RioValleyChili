using System.Web.Mvc;
using RioValleyChili.Client.Reporting.Controllers;

namespace RioValleyChili.Client.Mvc.Controllers
{
    public abstract partial class RvcReportingMvcControllerBase : TelerikReportingMvcControllerBase
    {
        /// <summary>
        /// Sets up the ViewBag and returns the common view for rendering Telerik ReportViewer.
        /// </summary>
        /// <param name="pageTitle">The HTML title to be set for the page</param>
        /// <param name="reportType">Fully-qualified assembly name of the Telerik Report</param>
        /// <param name="reportParameters">Optional parameters to be passed into the ReportViewer initialization</param>
        /// <returns></returns>
        protected virtual ActionResult ReportView(string pageTitle, string reportType, object reportParameters = null)
        {
            ViewBag.Title = pageTitle;
            ViewBag.ReportType = reportType;

            // If the user gets here by clicking a ActionLink, the reportParameters value is apparently in an
            // array of strings. So far, I've only seen a single array index with all values stringified within.
            var jsonValues = reportParameters as string[];
            ViewBag.ReportParameters = jsonValues != null
                ? jsonValues[0]
                : Newtonsoft.Json.JsonConvert.SerializeObject(reportParameters);

            return View(MVC.Reporting.Shared.Views.ReportViewer);
        }
    }
}