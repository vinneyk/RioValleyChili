using System.Collections.Generic;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models
{
    public class HtmlAttributeDictionary
    {
        private readonly DataBindingAttributeDictionary _dataBindingAttributes;
        private readonly IDictionary<string, object> _htmlAttributes;

        public HtmlAttributeDictionary() { }

        public HtmlAttributeDictionary(object htmlAttributes, DataBindingAttributeDictionary dataBindingAttributes)
            : this(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), dataBindingAttributes) { }

        public HtmlAttributeDictionary(IDictionary<string, object> htmlAttributes, DataBindingAttributeDictionary dataBindingAttributes)
        {
            _dataBindingAttributes = dataBindingAttributes ?? new DataBindingAttributeDictionary();
            _htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();
        }


        public IDictionary<string, object> ToDictionary()
        {
            var dataBindAttribute = _dataBindingAttributes.GetHtmlAttributeKeyValuePair();
            return new Dictionary<string, object>(_htmlAttributes)
                       {
                           { dataBindAttribute.Key, dataBindAttribute.Value },
                       };
        }

        public DataBindingAttributeDictionary DataBindingAttributeDictionary
        {
            get { return _dataBindingAttributes; }
        }
    }
}