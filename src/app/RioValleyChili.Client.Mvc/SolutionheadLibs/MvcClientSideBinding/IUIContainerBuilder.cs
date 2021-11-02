using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public interface IUIContainerBuilder
    {
        TagBuilder BuildUIContainer(Expression expression, object additionalViewData = null);

        [Obsolete("Do not use the containerType parameter overload. This value should be provided in the constructor.")]
        TagBuilder BuildUIContainer(Expression expression, string containerType, object additionalViewData = null);
    }
}