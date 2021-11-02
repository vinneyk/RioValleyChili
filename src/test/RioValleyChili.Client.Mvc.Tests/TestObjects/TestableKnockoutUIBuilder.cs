using System.Web.Mvc;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding;

namespace RioValleyChili.Tests.TestObjects
{
    public class TestableKnockoutUIBuilder<TModel> : KnockoutUIBuilder<TModel>
    {
        public TestableKnockoutUIBuilder(HtmlHelper<TModel> htmlHelper, IUIContainerBuilder containerBuilder)
            : base(htmlHelper, containerBuilder)
        {
        }

        public HtmlHelper<TModel> PublicHtmlHelper
        {
            get { return HtmlHelper; }
        }
    }
}