using System;
using System.Linq.Expressions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding;

namespace RioValleyChili.Client.Mvc.DataBinding.BindingCustomizations
{
    public class ISODateDataBindingCustomization : IDataBindingCustomization
    {
        public bool IsValidForContext<TModel, TValue>(DataBindingMode mode, Expression<Func<TModel, TValue>> expression)
        {
            return typeof (TValue) == typeof (DateTime) 
                   || typeof (TValue) == typeof (DateTime?);
        }

        public string ApplyCustomization(string dataBindingContext)
        {
            return string.Format("{0}.formattedDate", dataBindingContext);
        }
    }
}