using System;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Attributes
{
    public class ClientBoundMultiProperty : ClientBoundUIHintAttribute
    {
        public ClientBoundMultiProperty(string attributes, DataBindingMode validForModes = DataBindingMode.Readonly | DataBindingMode.Editable, string templateName = "")
            : base(BuildDataBindingAttributeDictionary(attributes), validForModes, templateName)
        { }

        private static DataBindingAttributeDictionary BuildDataBindingAttributeDictionary(string attributes)
        {
            if (attributes == null) { throw new ArgumentNullException("attributes"); }

            var dictionary = new DataBindingAttributeDictionary();
            foreach (var a in attributes.Split(','))
            {
                var attr = a.Split(':');
                dictionary.SetAttribute(attr[0], attr[1]);
            }
            return dictionary;
        }
    }

    public class ClientBoundPropertyAttribute : ClientBoundUIHintAttribute
    {
        public ClientBoundPropertyAttribute(string attributeName, string attributeValue, DataBindingMode validForModes = DataBindingMode.Readonly | DataBindingMode.Editable, string templateName = "")
            : base(BuildDataBindingAttributeDictionary(attributeName, attributeValue), validForModes, templateName)
        { }

        private static DataBindingAttributeDictionary BuildDataBindingAttributeDictionary(string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) { throw new ArgumentNullException("attributeName"); }
            if(string.IsNullOrWhiteSpace(attributeValue)) { throw new ArgumentNullException("attributeValue"); }

            var dictionary = new DataBindingAttributeDictionary();
            dictionary.SetAttribute(attributeName, attributeValue);

            return dictionary;
        }
    }
}