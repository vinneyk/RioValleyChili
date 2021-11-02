using System.Linq.Expressions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core.Utilities.Expressions;
using RioValleyChili.Client.Mvc.Utilities;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.Core.Helpers
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// Replaces Convert expressions with strongly typed Func&lt;Expression&lt;T,TParam&gt;&gt;.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression ReplaceUnaryConvertExpressions(this Expression expression)
        {
            return UnaryConversionExpressionRebinder.ReplaceConvertedLambdaExpressions(expression);
        }
    }
}