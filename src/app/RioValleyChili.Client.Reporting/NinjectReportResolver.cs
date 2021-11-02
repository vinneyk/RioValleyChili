using System;
using Ninject;
using Ninject.Modules;
using Telerik.Reporting;
using Telerik.Reporting.Service;

namespace RioValleyChili.Client.Reporting
{
    public class NinjectReportResolver : IReportResolver
    {
        readonly IKernel _kernel;

        public NinjectReportResolver(INinjectSettings ninjectSettings, params INinjectModule[] modules)
            : this(new StandardKernel(ninjectSettings, modules)) { }

        public NinjectReportResolver(IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            _kernel = kernel;
        }

        public ReportSource Resolve(string report)
        {
            var reportDoc = (IReportDocument)_kernel.Get(Type.GetType(report));
            return new InstanceReportSource { ReportDocument = reportDoc };
        }
    }

    //public interface IServiceResolver
    //{
    //    object Get(Type type);
    //    T Get<T>();
    //}

    //public class ServiceResolver : IServiceResolver
    //{
    //    readonly IKernel _kernel;

    //    public ServiceResolver(INinjectSettings ninjectSettings, params INinjectModule[] modules)
    //        : this(new StandardKernel(ninjectSettings, modules)) { }

    //    public ServiceResolver(IKernel kernel)
    //    {
    //        if(kernel == null) throw new ArgumentNullException("kernel");
    //        _kernel = kernel;
    //    }

    //    public object Get(Type type)
    //    {
    //        return _kernel.Get(type);
    //    }

    //    public T Get<T>()
    //    {
    //        return _kernel.Get<T>();
    //    }
    //}
}
