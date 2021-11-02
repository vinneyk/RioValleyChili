using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Attributes
{
    public abstract class ClientBoundExtensionAttribute : Attribute
    {
        public abstract DataBindingAttributeDictionary ApplyExtension(DataBindingAttributeDictionary bindings, DataBindingMode currentMode);
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SimpleClientBoundExtensionAttribute : ClientBoundExtensionAttribute
    {
        private readonly string _attributeName;
        private readonly string _extensionValue;
        private readonly DataBindingMode _validForModes;

        public SimpleClientBoundExtensionAttribute(string attributeName, string extensionValue, DataBindingMode validForModes)
        {
            if (attributeName == null) { throw new ArgumentNullException("attributeName"); }
            _attributeName = attributeName;

            if (extensionValue == null) { throw new ArgumentNullException("extensionValue"); }
            _extensionValue = extensionValue;

            if (validForModes == null) { throw new ArgumentNullException("validForModes"); }
            _validForModes = validForModes;
        }

        public override DataBindingAttributeDictionary ApplyExtension(DataBindingAttributeDictionary bindings, DataBindingMode currentMode)
        {
            if (!_validForModes.HasFlag(currentMode) || !bindings.Any(b => b.Key == _attributeName)) return bindings;
            var valueBinding = bindings.First(b => b.Key == _attributeName);
            var extendedValueBinding = BuildExtensionString(valueBinding.Value.ToString());
            bindings.SetAttribute(valueBinding.Key, extendedValueBinding);
            return bindings;
        }

        protected string BuildExtensionString(string originalValue)
        {
            return string.Format("{0}.{1}", originalValue, _extensionValue);
        }
    }

    public class NumericValueBindingExtensionAttribute : SimpleClientBoundExtensionAttribute
    {
        public NumericValueBindingExtensionAttribute(DataBindingMode validForModes)
            : base("value", "formattedNumber", validForModes) { }
    }
}