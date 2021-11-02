using System;
using System.Linq;
using System.Reflection;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Attributes
{
    public class ClientBoundSelectListAttribute : ClientBoundUIHintAttribute
    {
        /// <summary>
        /// Marks the property to be rendered as a DropDownList with client-side binding attributes.
        /// </summary>
        /// <param name="dataSourceName">The name of the property from which the DropDownList will populate it's options.</param>
        /// <param name="displayPropertyName">The name of the property on the data source object which contains the values to be displayed as the select list option text value.</param>
        /// <param name="valuePropertyName">The optional name of the property on the data source object which contains the values to be bound to the select list's value. When empty, null, or whitespace, the associated attribute will be left off. In Knockout, this will result in the property being bound to the JS object.</param>
        /// <param name="optionsCaption">The optional caption to be displayed as the default (empty) selection of the select list. If null or empty, no caption item will be added.</param>
        /// <param name="bindingLevel">The level at which the view model will find the data source object. By default, the framework will look for the property within the view model's own members.</param>
        public ClientBoundSelectListAttribute(string dataSourceName, string displayPropertyName, 
                                              string valuePropertyName = null, string optionsCaption = null,
                                              ClientBindingLevel bindingLevel = ClientBindingLevel.Self)
            : this(BuildDataBindingAttributeDictionary(dataSourceName, valuePropertyName, bindingLevel, displayPropertyName, optionsCaption), "SelectList")
        { }

        private ClientBoundSelectListAttribute(DataBindingAttributeDictionary dataBindingAttributeDictionary, string templateName)
            : base(dataBindingAttributeDictionary, DataBindingMode.Editable | DataBindingMode.Readonly, templateName) { }

        private static DataBindingAttributeDictionary BuildDataBindingAttributeDictionary(string dataSourceName, string valuePropertyName, ClientBindingLevel bindingLevel, string displayPropertyName, string optionsCaption)
        {
            if (dataSourceName == null) { throw new ArgumentNullException("dataSourceName"); }
            if (string.IsNullOrWhiteSpace(dataSourceName)) { throw new ArgumentException("Data source name cannot be empty.", "dataSourceName"); }

            if (displayPropertyName == null) { throw new ArgumentNullException("displayPropertyName"); }
            if (string.IsNullOrWhiteSpace(displayPropertyName)) { throw new ArgumentException("Display property name cannot be empty.", "displayPropertyName"); }

            var dataBindingAttributeDictionary = new DataBindingAttributeDictionary();
            dataBindingAttributeDictionary.SetAttribute("options", string.Format("{0}.{1}", GetBindingScope(bindingLevel), dataSourceName));
            dataBindingAttributeDictionary.SetAttribute("optionsText", string.Format("'{0}'", displayPropertyName));

            if (!string.IsNullOrWhiteSpace(valuePropertyName))
            {
                dataBindingAttributeDictionary.SetAttribute("optionsValue", string.Format("'{0}'", valuePropertyName));
            }

            if (!string.IsNullOrEmpty(optionsCaption))
            {
                dataBindingAttributeDictionary.SetAttribute("optionsCaption", string.Format("'{0}'", optionsCaption));
            }

            return dataBindingAttributeDictionary;
        }
    }
}