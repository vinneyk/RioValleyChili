using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Attributes;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Helpers
{
    public static class HtmlHelperExtensions
    {
        internal enum DisplayMode
        {
            Bound,
            Unbound
        }

        #region Select List Helpers

        [ExtractIntoSolutionheadLibrary]
        public static IEnumerable<SelectListItem> GetSelectListSource<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var selectListSourceName = BindSelectListsAttribute.BuildViewDataKey(metadata.PropertyName);
            return (IEnumerable<SelectListItem>) htmlHelper.ViewData[selectListSourceName];
        }

        [ExtractIntoSolutionheadLibrary]
        public static IEnumerable<SelectListItem> SetSelected(this IEnumerable<SelectListItem> selectList,
                                                              object selectedValue)
        {
            selectList = selectList ?? new List<SelectListItem>();
            if (selectedValue == null)
            {
                return selectList;
            }
            var value = selectedValue.ToString();
            return selectList.Select(i => new SelectListItem
                                              {
                                                  Selected = String.Equals(value, (string) i.Value),
                                                  Text = i.Text,
                                                  Value = i.Value,
                                              });
        }

        #endregion

        public static MvcHtmlString DisplayUIFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var containerBuilder = new UIContainerBuilder();
            var uiBuilder = new HtmlHelperUIBuilder<TModel>(htmlHelper, containerBuilder);
            return uiBuilder.DisplayFor(expression);
        }

        public static MvcHtmlString EditorUIFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var containerBuilder = new UIContainerBuilder();
            var uiBuilder = new HtmlHelperUIBuilder<TModel>(htmlHelper, containerBuilder);
            return uiBuilder.EditorFor(expression);
        }

        public static MvcHtmlString DisplayFieldsetFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var containerBuilder = new UIContainerBuilder();
            var uiBuilder = new HtmlHelperUIBuilder<TModel>(htmlHelper, containerBuilder);
            return uiBuilder.DisplayFieldsetFor(expression);
        }

        public static MvcHtmlString EditorFieldsetFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var containerBuilder = new UIContainerBuilder();
            var uiBuilder = new HtmlHelperUIBuilder<TModel>(htmlHelper, containerBuilder);
            return uiBuilder.EditorFieldsetFor(expression);
        }

        public static MvcHtmlString EditorViewFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var containerBuilder = new UIContainerBuilder();
            var uiBuilder = new HtmlHelperUIBuilder<TModel>(htmlHelper, containerBuilder);
            return uiBuilder.EditorViewFor(expression);
        }

    }
}