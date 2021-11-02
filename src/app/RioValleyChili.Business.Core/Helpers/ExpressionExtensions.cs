using System;
using System.Linq.Expressions;
using LinqKit;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class ExpressionExtensions
    {
        public static Expression<T> ExpandAll<T>(this Expression<T> expression)
        {
            if(expression == null) { throw new ArgumentNullException("expression"); }

            while(expression.ToString().Contains("Invoke"))
            {
                expression = expression.Expand();
            }

            return expression;
        }
    }
}