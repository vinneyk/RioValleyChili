using System;
using System.ComponentModel;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public abstract class ReportingService
    {
        public static TService ResolveService<TService>() where TService : class
        {
            var service = Application.Configuration.DependencyResolver.GetService(typeof(TService)) as TService;
            if (service == null) throw new ApplicationException(string.Format("Unable to resolve instance of service."));
            return service;
        }
    }
}