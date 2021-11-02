using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Extensions
{
    public static class KnockoutHtmlHelperExtensions
    {
        public static MvcHtmlString BoundEditorFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, bool scopeBindingToProperty = false, object additionalViewData = null)
        {
            return htmlHelper.ToKnockoutUIBuilder().EditorFor(expression, scopeBindingToProperty, additionalViewData);
        }

        public static MvcHtmlString BoundDisplayFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, bool recurseChildObjects = false, object additionalViewData = null)
        {
            return htmlHelper.ToKnockoutUIBuilder().DisplayFor(expression, additionalViewData);
        }

        public static MvcHtmlString BoundEditorFieldsetFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, ClientBindingLevel bindingLevel = ClientBindingLevel.Self, object additionalViewData = null)
        {
            return htmlHelper.ToKnockoutUIBuilder().EditorFieldsetFor(expression, bindingLevel, additionalViewData);
        }

        public static MvcHtmlString BoundDisplayFieldsetFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, ClientBindingLevel bindingLevel = ClientBindingLevel.Self, object additionalViewData = null)
        {
            return htmlHelper.ToKnockoutUIBuilder().DisplayFieldsetFor(expression, bindingLevel, additionalViewData);
        }

        public static MvcHtmlString BoundEditorViewFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string stateManagerName, EditorViewConfig config = null, bool scopeBindingToProperty = false)
        {
            return htmlHelper.ToKnockoutUIBuilder().EditorViewFor(expression, stateManagerName, config ?? EditorViewConfig.Default, scopeBindingToProperty);
        }

        public static MvcHtmlString EditorCommandControls<TModel>(this HtmlHelper<TModel> htmlHelper, string stateManagerName, EditorViewConfig config = null)
        {
            return htmlHelper.ToKnockoutUIBuilder().EditorCommandControls(stateManagerName, config ?? EditorViewConfig.Default);
        }

        public static MvcHtmlString EditorStateManagerDebug(this HtmlHelper htmlHelper, string editorStateManagerName)
        {
            var innerHtmlBuilder = new StringBuilder();
            innerHtmlBuilder.Append(BuildDebugElementDisplay("Is Editing", "isEditing"));
            innerHtmlBuilder.Append(BuildDebugElementDisplay("Is Dirty", "isDirty"));
            innerHtmlBuilder.Append(BuildDebugElementDisplay("Cache === Current State", "cache() === currentHash"));
            innerHtmlBuilder.Append(BuildDebugElementDisplay("Cache", "cache"));
            innerHtmlBuilder.Append(BuildDebugElementDisplay("Current State", "currentHash"));

            var container = new TagBuilder("div");
            container.MergeAttribute("data-bind", string.Format("with: {0}.debug", editorStateManagerName));
            container.InnerHtml = innerHtmlBuilder.ToString();
            return new MvcHtmlString(container.ToString());
        }

        
        public static IDictionary<string, object> BuildHtmlAttributesForView<TModel>(this HtmlHelper<TModel> htmlHelper, object additionalHtmlAttributes = null)
        {
            var htmlAttributes = additionalHtmlAttributes == null
                                     ? new Dictionary<string, object>()
                                     : additionalHtmlAttributes.BuildHtmlAttributeDictionary();

            var dataBindingAttributes = GetDataBindingHtmlAttributesForModel(htmlHelper);

            return dataBindingAttributes == null
                       ? htmlAttributes
                       : htmlAttributes.Concat(dataBindingAttributes).ToDictionary();
        }
        
        private static KnockoutUIBuilder<TModel> ToKnockoutUIBuilder<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new KnockoutUIBuilder<TModel>(htmlHelper, new UIContainerBuilder());
        }

        private static IEnumerable<KeyValuePair<string, object>> GetDataBindingHtmlAttributesForModel(this HtmlHelper htmlHelper)
        {
            var viewDataHelper = new DataBindingViewDataHelper();
            var dataBindingAttributes = viewDataHelper.GetDataBindingAttributesForCurrentContext(htmlHelper.ViewData);
            if(dataBindingAttributes == null) { return new Dictionary<string, object>(); }
            var htmlAttribute = dataBindingAttributes.GetHtmlAttributeKeyValuePair();

            return new Dictionary<string, object>()
                       {
                           { htmlAttribute.Key, htmlAttribute.Value }
                       };
        }

        private static string BuildDebugElementDisplay(string labelText, string propertyName)
        {
            var fieldHtmlBuilder = new StringBuilder(string.Format("<label>{0}:</label>", labelText.ToUpper()));
            fieldHtmlBuilder.Append(string.Format("<span data-bind=\"text: {0}\"></span>", propertyName));
            var fieldContainer = new TagBuilder("div");
            fieldContainer.InnerHtml = fieldHtmlBuilder.ToString();

            return fieldContainer.ToString();
        }
    }
}