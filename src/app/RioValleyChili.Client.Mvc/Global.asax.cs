using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Validation.Providers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RioValleyChili.Client.Mvc.App_Start;
using Newtonsoft.Json;
using RioValleyChili.Client.Mvc.Core;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Formatters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Core;
using RioValleyChili.Services;
using ApplicationException = Elmah.ApplicationException;

namespace RioValleyChili.Client.Mvc
{
    public class MvcApplication : HttpApplication
    {
        public static HttpContext MvcContext { get; private set; }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            return custom.Equals("user", StringComparison.OrdinalIgnoreCase)
                ? new VaryByClaimsPrincipalUserOutputCache().GetVaryByUserString(context)
                : base.GetVaryByCustomString(context, custom);
        }
    
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            NinjectConfiguration.Configure();
            AutoMapperConfiguration.Configure();
            KillSwitch.Instance = new AppSettingsKillSwitch();
            DataLoadResults.Instance = DataLoadResultImplementation.GetRVCDataLoadResultInstance();
            DBConnectionStatusImplementation.RegisterImplementations();
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.UserToken;
            OldContextSynchronizationSwitch.Instance = new AppSettingsOldContextSynchronization();

            ConfigureViewEngines();
            HttpConfiguration();

            Reporting.Configuration.ConfigureReporting(GlobalConfiguration.Configuration.DependencyResolver);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MvcContext = Context;
            Application["context"] = Context;
            Application["appInstance"] = Context.ApplicationInstance;
        }

        private static void ConfigureViewEngines()
        {
            var razorEngine = ViewEngines.Engines.OfType<RazorViewEngine>().First();
            var locationFormats = new List<string>(razorEngine.PartialViewLocationFormats)
            {
                "~/Views/Shared/ScriptTemplates/{0}.cshtml"
            };
            razorEngine.PartialViewLocationFormats = locationFormats.ToArray();
        }

        private static void HttpConfiguration()
        {
            var config = GlobalConfiguration.Configuration;

            // Prevents the "Value-typed properties marked as [Required] must 
            // also be marked with [DataMember(IsRequired=true)] to be 
            // recognized as required" error. 
            // As seen http://forums.asp.net/t/1834524.aspx/1 and
            // http://stackoverflow.com/questions/12305784/dataannotation-for-required-property
            config.Services.RemoveAll(
                typeof(System.Web.Http.Validation.ModelValidatorProvider),
                v => v is InvalidModelValidatorProvider);

            GlobalConfiguration.Configure(WebApiConfig.Register);

            config.Filters.Add(new KillSwitchEngagedApiFilter());
            
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.Insert(0, new CompanyDetailsJsonFormatter());
        }
    }

    public class ApplicationStartupException : ApplicationException
    {
        public ApplicationStartupException() { }

        public ApplicationStartupException(string message)
            : base(message) { }

        public ApplicationStartupException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}