using System;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public class KnockoutUIBuilder<TModel> : HtmlHelperUIBuilder<TModel>
    {
        #region fields & constructors

        private readonly DataBindingViewDataHelper _viewDataHelper = new DataBindingViewDataHelper();
        private readonly ControllerContext _controllerContext;

        //can the containerBuilder parameter be removed? -vk 5/14/13
        public KnockoutUIBuilder(HtmlHelper<TModel> htmlHelper, IUIContainerBuilder containerBuilder)
            : base(htmlHelper, containerBuilder)
        {
            _controllerContext = htmlHelper.ViewContext.Controller.ControllerContext;
        }

        #endregion

        private ClientBindingLevel bindingLevelCurrentElement = ClientBindingLevel.Self;

        public MvcHtmlString EditorFieldsetFor<TValue>(Expression<Func<TModel, TValue>> expression, ClientBindingLevel bindingLevel, object additionalViewData)
        {
            bindingLevelCurrentElement = bindingLevel;
            return EditorFieldsetFor(expression, additionalViewData);
        }

        public MvcHtmlString DisplayFieldsetFor<TValue>(Expression<Func<TModel, TValue>> expression, ClientBindingLevel bindingLevel, object additionalViewData)
        {
            bindingLevelCurrentElement = bindingLevel;
            return DisplayFieldsetFor(expression, additionalViewData);
        }

        public MvcHtmlString EditorCommandControls(string stateManagerName, EditorViewConfig config)
        {
            return EditorCommandControls(new EditStateManagerHelper(stateManagerName), config);
        }

        public MvcHtmlString EditorCommandControls(EditStateManagerHelper esmHelper, EditorViewConfig config)
        {
            var commandStringBuilder = new StringBuilder();
            commandStringBuilder.Append(esmHelper.CreateCommand(EditStateManagerHelper.Command.BeginEdit));
            if (!config.HideEndEditButton) { commandStringBuilder.Append(esmHelper.CreateCommand(EditStateManagerHelper.Command.EndEdit)); }
            commandStringBuilder.Append(esmHelper.CreateCommand(EditStateManagerHelper.Command.Reset));
            if (!config.HideSaveButton) { commandStringBuilder.Append(esmHelper.CreateCommand(EditStateManagerHelper.Command.Save)); }
            commandStringBuilder.Append(esmHelper.CreateCommand(EditStateManagerHelper.Command.Cancel));

            return new MvcHtmlString(commandStringBuilder.ToString());
        }

        public MvcHtmlString EditorFor<TValue>(Expression<Func<TModel, TValue>> expression, bool scopeBindingToProperty = false, object additionalViewData = null)
        {
            if (!scopeBindingToProperty) return EditorFor(expression, additionalViewData);

            var model = ContextSwitcher.SwitchContext(expression, HtmlHelper.ViewData.Model);
            var container = new BindingScopeUIContainerBuilder().BuildUIContainer(expression, "ol", additionalViewData);
            var newHelper = HtmlHelperFactory.BuildHtmlHelper(HtmlHelper.ViewContext, model);

            var builder = new KnockoutUIBuilder<TValue>(newHelper, ContainerBuilder);
            container.InnerHtml = builder.EditorForModel(false, additionalViewData).ToHtmlString();
            
            return new MvcHtmlString(container.ToString());
        }

        public MvcHtmlString EditorForModel(bool scopeBindingToProperty = false, object additionalViewData = null)
        {
            return EditorFor(m => m, scopeBindingToProperty, additionalViewData);
        }

        public MvcHtmlString DisplayFor<TValue>(Expression<Func<TModel, TValue>> expression, bool scopeBindingToProperty, object additionalViewData = null)
        {
            if (!scopeBindingToProperty) return DisplayFor(expression, additionalViewData);

            var model = ContextSwitcher.SwitchContext(expression, HtmlHelper.ViewData.Model);
            var container = new BindingScopeUIContainerBuilder().BuildUIContainer(expression, "div", additionalViewData);
            var newHelper = HtmlHelperFactory.BuildHtmlHelper(HtmlHelper.ViewContext, model);
            var builder = new KnockoutUIBuilder<TValue>(newHelper, ContainerBuilder);
            container.InnerHtml = builder.DisplayForModel(false, additionalViewData).ToHtmlString();
            return new MvcHtmlString(container.ToString());
        }

        public MvcHtmlString DisplayForModel(bool scopeBindingToProperty = false, object additionalViewData = null)
        {
            return DisplayFor(m => m, scopeBindingToProperty, additionalViewData);
        }

        public MvcHtmlString EditorViewFor<TValue>(Expression<Func<TModel, TValue>> expression, string stateManagerName, EditorViewConfig config, bool scopeBindingToProperty)
        {
            var esmHelper = new EditStateManagerHelper(stateManagerName);
            var model = ContextSwitcher.SwitchContext(expression, HtmlHelper.ViewData.Model);
            var newHelper = HtmlHelperFactory.BuildHtmlHelper(HtmlHelper.ViewContext, model);
            var knockoutBuilder = new KnockoutUIBuilder<TValue>(newHelper, ContainerBuilder);

            var editor = new TagBuilder("div");
            editor.MergeAttribute("data-bind", esmHelper.VisibleWhenEditingBinding);
            editor.InnerHtml = knockoutBuilder.EditorForModel().ToHtmlString();

            var display = new TagBuilder("div");
            display.MergeAttribute("data-bind", esmHelper.VisibleWhenReadonlyBinding);
            display.InnerHtml = knockoutBuilder.DisplayForModel().ToHtmlString();

            var footer = new TagBuilder("footer");
            footer.InnerHtml = EditorCommandControls(esmHelper, config).ToString();

            var containerHtmlStringBuilder = new StringBuilder();
            containerHtmlStringBuilder.Append(display);
            containerHtmlStringBuilder.Append(editor);
            containerHtmlStringBuilder.Append(footer);

            var containerBuilder = scopeBindingToProperty
                                       ? new BindingScopeUIContainerBuilder()
                                       : new UIContainerBuilder();

            var container = containerBuilder.BuildUIContainer(expression, "div");
            container.AddCssClass("editor-control");
            container.InnerHtml = containerHtmlStringBuilder.ToString();
            return new MvcHtmlString(container.ToString());
        }

        #region base member overrides

        protected override MvcHtmlString RenderEditorFor<TValue>(Expression<Func<TModel, TValue>> expression,
                                                                 object additionalViewData)
        {
            var htmlAttributeObject = _viewDataHelper.BuildHtmlAttributeObjectFor(expression, HtmlHelper.ViewData,
                                                                                  DataBindingMode.Editable,
                                                                                  bindingLevelCurrentElement,
                                                                                  additionalViewData);
            
            //todo: enable custom template overrides (see RenderDisplayFor<T> for example)
            return base.RenderEditorFor(expression, htmlAttributeObject);
        }

        protected override MvcHtmlString RenderDisplayFor<TValue>(Expression<Func<TModel, TValue>> expression,
                                                                  object additionalViewData)
        {
            var dataBindingObject = _viewDataHelper.BuildHtmlAttributeObjectFor(expression, HtmlHelper.ViewData,
                                                                                DataBindingMode.Readonly,
                                                                                bindingLevelCurrentElement,
                                                                                additionalViewData);
            var displayMvcHtml = base.RenderDisplayFor(expression, dataBindingObject);
            if (dataBindingObject == null)
            {
                return displayMvcHtml;
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, HtmlHelper.ViewData);
            
            // if a template has been supplied, we expect for the template to inject the data binding attributes
            // or, if no data binding attributes are found, just render the displayHtml without the extra data-bound container
            if (!string.IsNullOrWhiteSpace(metadata.TemplateHint))
            {
                var partialView = ViewEngines.Engines.FindPartialView(_controllerContext, string.Format("DisplayTemplates/{0}", metadata.TemplateHint));
                if (partialView.View != null)
                {
                    return displayMvcHtml;
                }
            }
            //todo: check for display template by type name if TemplateHint is not available?

            // create data-bound container and wrap around displayHtml
            var container = ContainerBuilder.BuildUIContainer(expression, additionalViewData);
            var dataBindingAttribute = dataBindingObject.DataBindingAttributes.GetHtmlAttributeKeyValuePair();
            container.MergeAttribute(dataBindingAttribute.Key, dataBindingAttribute.Value.ToString());
            container.InnerHtml = displayMvcHtml.ToHtmlString();
            return new MvcHtmlString(container.ToString());
        }

        #endregion

    }
}