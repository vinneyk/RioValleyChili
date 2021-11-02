using System.Web.Http;
using System.Web.Http.Dependencies;
using Telerik.Reporting.Services.WebApi;

namespace RioValleyChili.Client.Reporting
{
    public class Configuration
    {
        public static void ConfigureReporting(IDependencyResolver dependencyResolver)
        {
            ReportsControllerConfiguration.RegisterRoutes(GlobalConfiguration.Configuration);
            Application.SetDependencyResolver(dependencyResolver);
        }
    }
}
