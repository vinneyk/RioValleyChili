using Telerik.Reporting.Cache.Interfaces;
using Telerik.Reporting.Services.WebApi;
using IReportResolver = Telerik.Reporting.Services.Engine.IReportResolver;

namespace RioValleyChili.Client.Reporting.Controllers
{
    public abstract class TelerikReportsApiControllerBase : ReportsControllerBase
    {
        protected override IReportResolver CreateReportResolver()
        {
            return new ReportTypeResolver();
        }

        protected override ICache CreateCache()
        {
            return Telerik.Reporting.Services.Engine.CacheFactory.CreateFileCache();
        }
    }
}