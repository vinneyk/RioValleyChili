using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Helpers
{
    public abstract class UIBuilder<TModel>
    {
        #region fields & constructors

        protected readonly IUIContainerBuilder ContainerBuilder;

        protected UIBuilder(IUIContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException("containerBuilder");
            }
            ContainerBuilder = containerBuilder;
        }

        #endregion

        /// <summary>
        /// Gets the model for the view
        /// </summary>
        public abstract TModel Model { get; }

        public MvcHtmlString EditorFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData = null)
        {
            return expression.ReturnType.IsComplexType()
                       ? RenderEditorForType(expression, additionalViewData)
                       : RenderEditorFor(expression, additionalViewData);
        }

        public MvcHtmlString DisplayFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData = null)
        {
            return typeof (TValue).IsComplexType()
                       ? RenderDisplayForType(expression, additionalViewData)
                       : RenderDisplayFor(expression, additionalViewData);
        }

        public abstract MvcHtmlString LabelFor<TValue>(Expression<Func<TModel, TValue>> expression);
        
        public abstract MvcHtmlString ValidationMessageFor<TValue>(Expression<Func<TModel, TValue>> expression);

        public MvcHtmlString InputButton(string value, string[] cssClassNames)
        {
            var inputButton = new TagBuilder("input");
            var buttonAttributes = new Dictionary<string, string>
                                       {
                                           { "type", "button" },
                                           { "value", String.IsNullOrWhiteSpace(value) ? "Button" : value },
                                       };
            inputButton.MergeAttributes(buttonAttributes);
            if (cssClassNames != null && cssClassNames.Any())
            {
                cssClassNames.ToList().ForEach(inputButton.AddCssClass);
            }

            return new MvcHtmlString(inputButton.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// Renders the label text value inside a div tag with CSS class attribute.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression">An expression indicating the property of the model for which to render the label.</param>
        /// <returns></returns>
        public virtual MvcHtmlString TemplatedLabelFor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            if (expression.ReturnType.IsComplexType())
            {
                throw new ArgumentException(UserMessages.ExpressionCannotReturnContainerObject, typeof(TValue).FullName);
            }

            if(ViewHelper.IsHiddenInput(expression)) { return new MvcHtmlString(""); }

            var labelBuilder = new TagBuilder("div");
            labelBuilder.AddCssClass("editor-label");
            labelBuilder.InnerHtml = LabelFor(expression).ToHtmlString();

            return new MvcHtmlString(labelBuilder.ToString());
        }

        /// <summary>
        /// Renders the Display for the supplied expression wrapped inside an html div tag with CSS class attribute. NOTICE: This property is not valid for complex objects. Use DisplayFor instead.
        /// </summary>
        /// <param name="expression">An expression indicating the property of the model to be rendered into the display template. The property cannot be a container object.</param>
        /// <param name="additionalViewData">Additional data to be passed into the view.</param>
        /// <returns></returns>
        public virtual MvcHtmlString TemplatedDisplayFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData = null)
        {
            if (expression.ReturnType.IsComplexType())
            {
                throw new ArgumentException(UserMessages.ExpressionCannotReturnContainerObject, typeof(TValue).FullName);
            }

            if (ViewHelper.IsHiddenInput(expression))
            {
                
                return DisplayFor(expression, additionalViewData);
            }

            var templateBuilder = new TagBuilder("div");
            templateBuilder.AddCssClass("display-field");
            templateBuilder.InnerHtml = DisplayFor(expression, additionalViewData).ToHtmlString();
            return new MvcHtmlString(templateBuilder.ToString());
        }

        /// <summary>
        /// Renders the Editor for the supplied expression wrapped inside an html div tag with CSS class attribute.
        /// </summary>
        /// <param name="expression">An expression indicating the property of the model to be rendered into the editor template. The property cannot be a container object.</param>
        /// <param name="additionalViewData">Additional data to be passed into the view.</param>
        /// <returns></returns>
        public virtual MvcHtmlString TemplatedEditorFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData = null)
        {
            if (expression.ReturnType.IsComplexType())
            {
                throw new ArgumentException(UserMessages.ExpressionCannotReturnContainerObject, typeof(TValue).FullName);
            }

            if (ViewHelper.IsHiddenInput(expression))
            {
                return EditorFor(expression, additionalViewData);
            }

            var templateBuilder = new TagBuilder("div");
            templateBuilder.AddCssClass("editor-field");
            templateBuilder.InnerHtml = EditorFor(expression, additionalViewData).ToHtmlString();
            
            var validationMessage = ValidationMessageFor(expression);
            if (validationMessage != null)
            {
                templateBuilder.InnerHtml += ValidationMessageFor(expression).ToHtmlString();
            }
            
            return new MvcHtmlString(templateBuilder.ToString());
        }

        /// <summary>
        /// Renders the an editor fieldset, consisting of a templated label and templated editor, for the indicated property.
        /// </summary>
        /// <param name="expression">An expression indicating the property of the model to be rendered into the template. The property cannot be a container object.</param>
        /// <param name="additionalViewData">Additional data to be passed into the view.</param>
        /// <returns></returns>
        public MvcHtmlString EditorFieldsetFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData = null)
        {
            if (expression.ReturnType.IsComplexType())
            {
                throw new ArgumentException(UserMessages.ExpressionCannotReturnContainerObject, typeof(TValue).FullName);
            }

            var inputBuilder = new StringBuilder();
            inputBuilder.Append(LabelFor(expression));
            inputBuilder.Append(EditorFor(expression, additionalViewData).ToHtmlString());
            return new MvcHtmlString(inputBuilder.ToString());
        }
        
        /// <summary>
        /// Renders the a display fieldset, consisting of a templated label and templated editor, for the indicated property.
        /// </summary>
        /// <param name="expression">An expression indicating the property of the model to be rendered into the template. The property cannot be a container object.</param>
        /// <param name="additionalViewData">Additional data to be passed into the view.</param>
        /// <returns></returns>
        public MvcHtmlString DisplayFieldsetFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData = null)
        {
            if (expression.ReturnType.IsComplexType())
            {
                throw new ArgumentException(string.Format(UserMessages.ExpressionCannotReturnContainerObject, expression.ReturnType.FullName));
            }

            var fieldsetBuilder = new StringBuilder();
            fieldsetBuilder.Append(TemplatedLabelFor(expression).ToHtmlString());
            fieldsetBuilder.Append(TemplatedDisplayFor(expression, additionalViewData).ToHtmlString());

            return new MvcHtmlString(fieldsetBuilder.ToString());
        }
        
        /// <summary>
        /// Creates an SHEditorView (jQuery plugin).
        /// </summary>
        [Obsolete("Use the KnockoutUI version instead.")]
        public virtual MvcHtmlString EditorViewFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData = null)
        {
            var isComplexObject = typeof (TValue).IsComplexType();

            var container = new TagBuilder("div");
            container.AddCssClass("editor-control");

            var displayContainer = new TagBuilder("div");
            displayContainer.AddCssClass("editor-readonly-view");
            displayContainer.InnerHtml += isComplexObject
                                              ? RenderDisplayForType(expression, additionalViewData)
                                              : DisplayFieldsetFor(expression, additionalViewData);

            displayContainer.InnerHtml += InputButton("Edit", new[] {"editor-state-command", "edit-command"});
            container.InnerHtml += displayContainer.ToString();

            var editorContainer = new TagBuilder("div");
            editorContainer.AddCssClass("editor-editable-view");
            editorContainer.InnerHtml += isComplexObject
                                             ? RenderEditorForType(expression, additionalViewData)
                                             : EditorFieldsetFor(expression, additionalViewData);

            editorContainer.InnerHtml += InputButton("Save", new[] {"editor-state-command", "save-command"});
            editorContainer.InnerHtml += InputButton("Cancel", new[] {"editor-state-command", "cancel-command"});
            container.InnerHtml += editorContainer.ToString();

            return new MvcHtmlString(container.ToString());
        }

        protected abstract MvcHtmlString RenderEditorFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData);

        protected abstract MvcHtmlString RenderDisplayFor<TValue>(Expression<Func<TModel, TValue>>  expression, object additionalViewData);

        protected virtual MvcHtmlString RenderEditorForType(Expression expression, object additionalViewData)
        {
            if (!expression.GetReturnValueType().IsComplexType())
            {
                throw new ArgumentException(string.Format(UserMessages.ExpressionMustReturnContainerObject, expression.GetReturnValueType().FullName));
            }

            var container = new TagBuilder("ol");
            var childContainerBuilder = new UIContainerBuilder("li");

            var viewProperties = ViewHelper.GetLambdaExpressionsForViewProperties(expression, Model);
            foreach (var propertyLambda in viewProperties)
            {
                container.InnerHtml += BuildEditorUIFromLambdaExpression(propertyLambda, additionalViewData, childContainerBuilder);
            }
            
            container.AddCssClass("tabular");
            return new MvcHtmlString(container.ToString());
        }

        protected virtual MvcHtmlString RenderDisplayForType(Expression expression, object additionalViewData)
        {
            if (!expression.GetReturnValueType().IsComplexType())
            {
                throw new ArgumentException(string.Format(UserMessages.ExpressionMustReturnContainerObject, expression.GetReturnValueType().FullName));
            }
            
            var container = new TagBuilder("ol");
            var childContainerBuilder = new UIContainerBuilder("li");

            var viewProperties = ViewHelper.GetLambdaExpressionsForViewProperties(expression, Model);
            foreach (var propertyLambda in viewProperties.Where(propertyLambda => !ViewHelper.IsHiddenInput(propertyLambda)))
            {
                container.InnerHtml += BuildDisplayUIFromLambdaExpression(propertyLambda, additionalViewData, childContainerBuilder);
            }

            var viewData = new RouteValueDictionary(additionalViewData);
            if (viewData.ContainsKey("class"))
            {
                container.AddCssClass(viewData["class"].ToString());
            }

            return new MvcHtmlString(container.ToString());
        }

        #region private members

#warning replace anonymous expressions with the visitor helpers.

        private string BuildEditorUIFromLambdaExpression(LambdaExpression expression, object additionalViewData, IUIContainerBuilder containerBuilder = null)
        {
            if (expression.ReturnType.IsComplexType())
            {
                return RenderEditorForType(expression, additionalViewData).ToHtmlString();
            }

            var methodInfo = GetType().GetMethods().First(m => m.Name == "EditorFieldsetFor" && m.GetParameters().Length == 2);
            var genericMethod = methodInfo.MakeGenericMethod(expression.ReturnType);
            var editorFieldsetHtml =((MvcHtmlString) genericMethod.Invoke(this, new[] {expression, additionalViewData})).ToHtmlString();
            
            if (containerBuilder != null)
            {
                var container = containerBuilder.BuildUIContainer(null, additionalViewData);
                container.InnerHtml = editorFieldsetHtml;
                return container.ToString();
            }
            return editorFieldsetHtml;
        }

        private string BuildDisplayUIFromLambdaExpression(LambdaExpression expression, object additionalViewData, IUIContainerBuilder containerBuilder = null)
        {
            if (expression.ReturnType.IsComplexType())
            {
                return RenderDisplayForType(expression, additionalViewData).ToHtmlString();
            }

            var methodInfo = GetType().GetMethods().First(m => m.Name == "DisplayFieldsetFor" && m.GetParameters().Count() == 2);
            var genericMethod = methodInfo.MakeGenericMethod(expression.ReturnType);
            var displayFieldsetHtml = ((MvcHtmlString)genericMethod.Invoke(this, new[] { expression, additionalViewData })).ToHtmlString();
            
            if (containerBuilder != null)
            {
                var container = containerBuilder.BuildUIContainer(null, additionalViewData);
                container.InnerHtml = displayFieldsetHtml;
                return container.ToString();
            }

            return displayFieldsetHtml;
        }

        #endregion
    }
}