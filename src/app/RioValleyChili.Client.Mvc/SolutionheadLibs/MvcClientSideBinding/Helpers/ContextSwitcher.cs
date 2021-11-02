using System;
using System.Linq.Expressions;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers
{
    public static class ContextSwitcher
    {
        static readonly ContextSwitcherVisitor _visitor = new ContextSwitcherVisitor();

        public static TNewContext SwitchContext<TOldContext, TNewContext>(Expression<Func<TOldContext, TNewContext>> expression, TOldContext contextObject)
        {
            var visitedExpression = _visitor.Visit(expression) as LambdaExpression;
            var newContextObject = expression.Compile().Invoke(contextObject);
            return (TNewContext)visitedExpression.Compile().DynamicInvoke(newContextObject);
        }
    }
}