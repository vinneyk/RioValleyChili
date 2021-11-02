using Ninject.Modules;
using RioValleyChili.Client.Core.ObjectMapping;
using RioValleyChili.Client.Mvc.Core;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Core;
using RioValleyChili.Services.Configuration;

namespace RioValleyChili.Client.Mvc.App_Start
{
    internal class RvcAppKernelConfiguration : NinjectModule
    {
        #region Overrides of NinjectModule

        public override void Load()
        {
            Kernel.Bind<IExceptionLogger>().To<ElmahExceptionLogger>();
            Kernel.Bind<IUserIdentityProvider>().To<ClaimsPrincipalUserIdentityProvider>();
            Kernel.Bind<IObjectMapper>().ToConstant(new AutoMapperObjectMapper());
            
            ServiceLocatorConfig.Configure(Kernel);
        }

        #endregion
    }
}