using System;
using System.Linq.Expressions;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public interface IDataBindingCustomization
    {
        bool IsValidForContext<TModel, TValue>(DataBindingMode mode, Expression<Func<TModel, TValue>> expression);
        string ApplyCustomization(string dataBindingContext);
    }
}