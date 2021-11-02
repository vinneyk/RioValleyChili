using System.Linq.Expressions;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers;

namespace RioValleyChili.Tests.TestObjects
{
    public class TestableHtmlHelperUIBuilder<TModel> : HtmlHelperUIBuilder<TModel>
    {
        public TestableHtmlHelperUIBuilder(HtmlHelper<TModel> htmlHelper, IUIContainerBuilder containerBuilder)
            : base(htmlHelper, containerBuilder)
        {
        }

        public HtmlHelper<TModel> PublicHtmlHelper
        {
            get { return HtmlHelper; }
        }

        public MvcHtmlString RenderEditorForType(Expression expression)
        {
            return base.RenderEditorForType(expression, null);
        }

        public MvcHtmlString RenderDisplayForType(Expression expression)
        {
            return base.RenderDisplayForType(expression, null);
        }
    }
}