using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public class UIContainerBuilder : IUIContainerBuilder
    {
        #region fields and constructors

        private readonly string _containerTag;

        public UIContainerBuilder()
            : this("div") { }

        public UIContainerBuilder(string containerTag)
        {
            if (containerTag == null) { throw new ArgumentNullException("containerTag"); }
            _containerTag = containerTag;
        }

        #endregion
        
        public virtual TagBuilder BuildUIContainer(Expression expression, string containerType, object additionalViewData = null)
        {
            return BuildUIContainer(expression, additionalViewData);
        }

        public virtual TagBuilder BuildUIContainer(Expression expression, object additionalViewData = null)
        {
            var container = new TagBuilder(_containerTag);
            var viewData = additionalViewData as ViewDataDictionary;
            if (viewData != null && viewData.ContainsKey("class"))
            {
                container.AddCssClass(viewData["class"].ToString());
            }
            return container;
        }
    }

    public class BindingScopeUIContainerBuilder : UIContainerBuilder
    {
        private readonly KnockoutBindingContextHelper _bindingContextHelper = new KnockoutBindingContextHelper(); 

        public override TagBuilder BuildUIContainer(Expression expression, string containerType, object additionalViewData = null)
        {
            return BuildUIContainer(expression, additionalViewData);
        }

        public override TagBuilder BuildUIContainer(Expression expression, object additionalViewData = null)
        {
            if (expression.NodeType != ExpressionType.Lambda) { throw new ArgumentException("Only Lambda expressions are valid.", "expression"); }

            var lambdaExpression = (LambdaExpression)expression;
            if (lambdaExpression.Body.NodeType == ExpressionType.Parameter)
            {
                return base.BuildUIContainer(expression, additionalViewData);
            }

            var tag = base.BuildUIContainer(expression, additionalViewData);
            var memberName = _bindingContextHelper.BuildBindingContextFor(expression);
            tag.MergeAttribute("data-bind", string.Format("with: {0}", memberName));

            return tag;
        }
    }
}