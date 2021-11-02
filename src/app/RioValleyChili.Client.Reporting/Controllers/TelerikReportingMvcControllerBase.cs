using System.Collections;
using System.Web.Mvc;
using System.Web.Routing;
using RioValleyChili.Client.Reporting.Models;
using Telerik.Reporting.Processing;

namespace RioValleyChili.Client.Reporting.Controllers
{
    public abstract class TelerikReportingMvcControllerBase : Controller
    {
        public ActionResult Pdf(string reportName, object reportParameters)
        {
            var processor = new ReportProcessor();
            var deviceInfo = new Hashtable();

            var typeReportSource = new Telerik.Reporting.TypeReportSource { TypeName = reportName, Parameters = { }};
            var parameters = new RouteValueDictionary(reportParameters);
            parameters.Keys.ForEach(k => typeReportSource.Parameters.Add(k, parameters[k]));

            var result = processor.RenderReport("PDF", typeReportSource, deviceInfo);

            var fileName = string.Format("{0}.{1}", result.DocumentName, result.Extension);
            return File(result.DocumentBytes, "application/pdf", fileName);
        }
    }
}