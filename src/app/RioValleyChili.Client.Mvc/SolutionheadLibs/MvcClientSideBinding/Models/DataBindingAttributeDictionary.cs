using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models
{
    [ExtractIntoSolutionheadLibrary]
    public class DataBindingAttributeDictionary : IEnumerable<KeyValuePair<string, object>> 
    {
        public DataBindingAttributeDictionary(DataBindingAttributeDictionary initialAttributes = null)
        {
            if (initialAttributes != null && initialAttributes.Any())
            {
                initialAttributes.ToList().ForEach(_dataBindingAttributes.Add);
            }
        }

        private readonly IDictionary<string, object> _dataBindingAttributes = new Dictionary<string, object>();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dataBindingAttributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public KeyValuePair<string, object> GetHtmlAttributeKeyValuePair()
        {
            return new KeyValuePair<string, object>(HtmlAttributeName, RenderDataBindAttributes());
        }

        private string RenderDataBindAttributes()
        {
            return String.Join(", ", _dataBindingAttributes.Select(a => String.Format("{0}: {1}", a.Key, a.Value ?? "")));
        }

        public string AsHtmlAttributeString()
        {
            var dataBindAttribute = GetHtmlAttributeKeyValuePair();
            return String.Format("{0}=\"{1}\"", dataBindAttribute.Key, dataBindAttribute.Value);
        }

        public override string ToString()
        {
            return AsHtmlAttributeString();
        }

        public object SetAttribute(string key, object value)
        {
            return _dataBindingAttributes[key] = value;
        }

        public object SetAttribute(KeyValuePair<string, object> attribute)
        {
            return SetAttribute(attribute.Key, attribute.Value);
        }

        public object GetAttribute(string key)
        {
            return _dataBindingAttributes[key];
        }

        public void MergeAttributes(DataBindingAttributeDictionary source)
        {
            if (source == null) { return; }

            foreach (var attr in source)
            {
                SetAttribute(attr.Key, attr.Value);
            }
        }

        public void MergeAttributes(IEnumerable<DataBindingInfo> source)
        {
            if (source == null) { return; }

            foreach (var attr in source)
            {
                MergeAttribute(attr);
            }
        }

        public void MergeAttribute(DataBindingInfo attribute)
        {
            if (attribute == null) { return; }

            var attr = attribute.AsKeyValuePair();
            SetAttribute(attr.Key, attr.Value);
        }

        public static string HtmlAttributeName
        {
            get { return "data-bind"; }
        }
    }
}