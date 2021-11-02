using System.Web.Http.Dependencies;

namespace RioValleyChili.Client.Reporting.Core
{
    public class ReportingConfiguration
    {
        public IDependencyResolver DependencyResolver { get; set; }
    }
}