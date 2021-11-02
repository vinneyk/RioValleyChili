using System;
using System.Linq.Expressions;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers
{
    public class KnockoutBindingContextHelper
    {
        public string BuildBindingContextFor(Expression expression, ClientBindingLevel bindingLevel = ClientBindingLevel.Self)
        {
            if (expression == null) { throw new ArgumentNullException("expression"); }
            
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                case ExpressionType.MemberAccess:
                    return RecursExpressionToBuildBindingContext(expression, bindingLevel);
                case ExpressionType.Lambda:
                    return RecursExpressionToBuildBindingContext(((LambdaExpression)expression).Body, bindingLevel);
            }

            throw new NotSupportedException(string.Format("The expression type '{0}' is not supported. Expression should be a Lambda or MemberAccess Expression.", expression.NodeType));
        }

        private static string RecursExpressionToBuildBindingContext(Expression expression, ClientBindingLevel bindingLevel, string contextChain = null)
        {
            contextChain = contextChain ?? string.Empty;

            if (expression.NodeType == ExpressionType.Parameter)
            {
                return AddScopeBindingContext(contextChain, bindingLevel);
            }

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                // NOTE: Because recursion will walk the chain from the outer-most nesting to the root,
                // the extendedChain prepends the member of the current node to the beginning of the string.
                var extendedChain = string.Format("{0}{1}{2}",
                                                  ((MemberExpression)expression).Member.Name,
                                                  string.IsNullOrWhiteSpace(contextChain) ? "" : ".",
                                                  contextChain);

                return RecursExpressionToBuildBindingContext(((MemberExpression)expression).Expression, bindingLevel, extendedChain);
            }

            throw new NotSupportedException("Expression is not supported.");
        }

        private static string AddScopeBindingContext(string context, ClientBindingLevel bindingLevel)
        {
            var contextScope = "";
            switch (bindingLevel)
            {
                case ClientBindingLevel.Parent:
                    contextScope = "$parent";
                    break;
                case ClientBindingLevel.Root:
                    contextScope = "$root";
                    break;
            }

            return string.Format("{0}{1}{2}",
                                 contextScope,
                                 string.IsNullOrEmpty(contextScope) ? string.Empty : ".",
                                 context);
        }
    }
}