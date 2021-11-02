using System.Linq.Expressions;
using System.Reflection;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.Core.Utilities.Expressions
{
    /// <summary>
    /// LINQ has a Convert function which it shims into expressions returning object or dynamic
    /// to enable expression binding. For example, The ResultPager&lt;T&gt; class exposes an 
    /// OrderByPredicate property which is defined as Expression&lt;Func&lt;T, dynamic&gt;&gt;. Because 
    /// the return type is dynamic (same would apply for object), LINQ will translate this expression
    /// from p => p.MySortProperty to p => Convert(p.MySortProperty). This was causing LINQ to Entities
    /// to fail with an InvalidOperationExcpetion when MySortProperty was of type DateTime. This visitor
    /// will replace occurrences of the Convert UnaryExpression with a strongly typed reference to the 
    /// desired property.
    /// </summary>
    public class UnaryConversionExpressionRebinder : ExpressionVisitor
    {
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var lambda = (LambdaExpression)node;
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                var memberExpression = (MemberExpression)VisitUnary((UnaryExpression)node.Body);
                var propertyInfo = (PropertyInfo) memberExpression.Member;
                var parameter = Expression.Parameter(lambda.Parameters[0].Type);
                var memberAccessExpression = Expression.MakeMemberAccess(parameter, propertyInfo);
                var orderExpression = Expression.Lambda(memberAccessExpression, parameter);

                var gmethod = GetType().BaseType.GetMethod("VisitLambda", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(orderExpression.Type);
                return (Expression) gmethod.Invoke(this, new object[] {orderExpression});
            }

            return base.VisitLambda(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert)
            {
                var memberExpression = (MemberExpression)node.Operand;
                var visited = base.VisitMember(memberExpression);
                return visited;
            }

            return base.VisitUnary(node);
        }

        public static Expression ReplaceConvertedLambdaExpressions(Expression expression)
        {
            return new UnaryConversionExpressionRebinder().Visit(expression);
        }
    }
}