using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Client.Mvc.DataBinding.BindingCustomizations;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public class DataBindingInfoFactory
    {
        private readonly IList<IDataBindingCustomization> _customizations = new List<IDataBindingCustomization>();
        private readonly KnockoutBindingContextHelper _bindingContextHelper = new KnockoutBindingContextHelper();

        public DataBindingInfoFactory()
        {
            _customizations.Add(new ISODateDataBindingCustomization());
        }

        public DataBindingInfo CreateDataBindingAttributeInfoFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression, DataBindingMode mode, ClientBindingLevel bindingLevel)
        {
            if (typeof (TValue).IsComplexType())
            {
                throw new NotSupportedException("Creation of data binding attributes is not supported for complex types.");
            }

            if (expression.Body.NodeType == ExpressionType.Parameter)
            {
                throw new ArgumentException("DataBindingAttributeInfo cannot be created from a self referencing Expression (such as m => m).");
            }

            var bindingContext = _bindingContextHelper.BuildBindingContextFor(expression, bindingLevel);
            _customizations.Where(c => c.IsValidForContext(mode, expression)).ToList()
                .ForEach(c => bindingContext = c.ApplyCustomization(bindingContext));
            var dataBindingInfo = DataBindingInfo.Create(mode, bindingContext);

            //todo: enable customization of DataBinding expression. Examples: click, hasFocus, valueUpdate

            return dataBindingInfo;
        }
    }
}