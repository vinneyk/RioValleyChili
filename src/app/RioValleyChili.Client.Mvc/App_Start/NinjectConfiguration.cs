using System.Web.Http;
using System.Web.Mvc;
using Ninject;
using Ninject.Web.Mvc;
using RioValleyChili.Client.Mvc.Core;

namespace RioValleyChili.Client.Mvc.App_Start
{
    public class NinjectConfiguration
    {
        public static IKernel Configure()
        {
            var kernel = new StandardKernel(new RvcAppKernelConfiguration());
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
            return kernel;
        }
    }
}