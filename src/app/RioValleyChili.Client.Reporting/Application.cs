using System.Web.Http.Dependencies;
using RioValleyChili.Client.Reporting.Core;

namespace RioValleyChili.Client.Reporting
{
    public static class Application
    {
        static Application()
        {
            Configuration = new ReportingConfiguration();
            AutoMapperConfiguration.Configure();
        }

        public static void SetDependencyResolver(IDependencyResolver bindingConfigurations)
        {
            Configuration.DependencyResolver = bindingConfigurations;
        }

        public static ReportingConfiguration Configuration { get; set; }
    }
}