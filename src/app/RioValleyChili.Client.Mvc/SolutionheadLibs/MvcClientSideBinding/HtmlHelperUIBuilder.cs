using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Helpers;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public class HtmlHelperUIBuilder<TModel> : UIBuilder<TModel>
    {
        #region fields & constructors

        protected readonly HtmlHelper<TModel> HtmlHelper;

        public HtmlHelperUIBuilder(HtmlHelper<TModel> htmlHelper, IUIContainerBuilder containerBuilder)
            : base(containerBuilder)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }
            HtmlHelper = htmlHelper;
        }

        #endregion

        public override TModel Model
        {
            get { return HtmlHelper.ViewData.Model; }
        }

        public override MvcHtmlString LabelFor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return HtmlHelper.LabelFor(expression);
        }

        public override MvcHtmlString ValidationMessageFor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return HtmlHelper.ValidationMessageFor(expression);
        }

        protected override MvcHtmlString RenderEditorFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData)
        {
            return HtmlHelper.EditorFor(expression, additionalViewData);
        }
        
        protected override MvcHtmlString RenderDisplayFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData)
        {
            return HtmlHelper.DisplayFor(expression, additionalViewData);
        }
    }
}