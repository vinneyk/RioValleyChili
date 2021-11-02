using System.Linq.Expressions;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public class ContextSwitcherVisitor : ExpressionVisitor
    {
        // Converts expression to the child node such as:
        // m => m.Child to m => m
        // where m is of type Child
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var newContextType = node.ReturnType;
            var parameter = Expression.Parameter(newContextType, "m");
            var lambda = Expression.Lambda(
                parameter,
                new[] { parameter });
            return lambda;
        }
    }
}