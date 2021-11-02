using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Attributes;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public class DataBindingViewDataHelper
    {
        //todo: write tests for public methods

        public void StoreDataBindingAttributesFor(string propertyName, ViewDataDictionary viewData, DataBindingAttributeDictionary attributes)
        {
            var dataBindingAttributes = GetDataBindingAttributesForCurrentContext(viewData);
            dataBindingAttributes.MergeAttributes(attributes);
        }

        public DataBindingAttributeDictionary GetDataBindingAttributesForCurrentContext(ViewDataDictionary viewData)
        {
            return GetOrCreateDataBindingAttributeViewDataEntry(DataBindingObjectViewDataKey, viewData);
        }

        /// <summary>
        /// Constructs an anonymous object containing the data Knockout.JS binding property and all data binding attributes appropriate for the expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="viewData"></param>
        /// <param name="mode"></param>
        /// <param name="bindingLevel"></param>
        /// <param name="additionalViewData"></param>
        /// <returns></returns>
        public KnockoutDataBindingObject BuildHtmlAttributeObjectFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression, ViewDataDictionary<TModel> viewData, DataBindingMode mode, ClientBindingLevel bindingLevel, object additionalViewData)
        {   
            var dataBindingAttributes = BuildDataBindingAttributesFor(expression, viewData, mode, bindingLevel);

            return dataBindingAttributes == null || !dataBindingAttributes.Any()
                       ? null
                       : new KnockoutDataBindingObject(dataBindingAttributes, additionalViewData);
        }

        /// <summary>
        /// Constructs a DataBindingAttributeDictionary consisting of all data binding attributes for supplied expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression">An expression indicating the property for which the build the DataBindingAttributeDictionary.</param>
        /// <param name="viewData"></param>
        /// <param name="mode"></param>
        /// <param name="bindingLevel"></param>
        /// <returns></returns>
        public DataBindingAttributeDictionary BuildDataBindingAttributesFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression, ViewDataDictionary<TModel> viewData, DataBindingMode mode, ClientBindingLevel bindingLevel)
        {
            var dataBindingAttributes = new DataBindingAttributeDictionary();
            
            var dataBindingInfoFactory = new DataBindingInfoFactory();
            var bindingInfo = dataBindingInfoFactory.CreateDataBindingAttributeInfoFor(expression, mode, bindingLevel);
            dataBindingAttributes.MergeAttribute(bindingInfo);

            //GetClientSideDataBindingAttributesFor(dataBindingAttributes, expression, mode);
            GetClientSideDataBindingAttributesFor(dataBindingAttributes, expression, mode);
            //dataBindingAttributes.MergeAttributes(clientSideDataBindingAttributes);

            return dataBindingAttributes;
        }

        private static DataBindingAttributeDictionary GetClientSideDataBindingAttributesFor<TModel, TValue>(DataBindingAttributeDictionary dataBindingAttributes, Expression<Func<TModel, TValue>> expression, DataBindingMode mode)
        {
            if (dataBindingAttributes == null) { throw new ArgumentNullException("dataBindingAttributes"); }
            //var dataBindingAttributes = new DataBindingAttributeDictionary();

            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var member = ((MemberExpression)expression.Body).Member;
                if (member.MemberType != MemberTypes.Property) { throw new NotSupportedException("The expression's return type must be a Property type."); }

                foreach (var bindingAttribute in member.GetCustomAttributes<ClientBoundUIHintAttribute>().Where(a => a.IsValidForMode(mode)))
                {
                    if (!bindingAttribute.IsValidForProperty(member))
                    {
                        throw new ArgumentException(
                            String.Format(
                                "The ClientBoundUIHintAttribute, '{0}' does not support binding to the property '{1}' which is of type '{2}' and is a member of '{3}.",
                                bindingAttribute.GetType().FullName,
                                member.Name,
                                member.ReflectedType.FullName,
                                typeof(TModel).FullName));
                    }

                    dataBindingAttributes.MergeAttributes(bindingAttribute.GetDatabindingAttributes());
                }

                ApplyDataBindingExtensions(member, dataBindingAttributes, mode);
            }

            return dataBindingAttributes;
        }

        private static DataBindingAttributeDictionary ApplyDataBindingExtensions(MemberInfo property, DataBindingAttributeDictionary dataBindingAttributes, DataBindingMode mode)
        {
            foreach (var extensionAttribute in property.GetCustomAttributes<ClientBoundExtensionAttribute>())
            {
                extensionAttribute.ApplyExtension(dataBindingAttributes, mode);
            }

            return dataBindingAttributes;
        }

        private static DataBindingAttributeDictionary GetOrCreateDataBindingAttributeViewDataEntry(string viewDataKey, ViewDataDictionary viewData)
        {
            return (DataBindingAttributeDictionary) (viewData[viewDataKey] = viewData[viewDataKey] ?? new DataBindingAttributeDictionary());
        }

        //todo: move into static helper class
        private static string DataBindingObjectViewDataKey
        {
            get
            {
                return
                    typeof(KnockoutDataBindingObject).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                      .Single(p => p.PropertyType == typeof(DataBindingAttributeDictionary))
                                                      .Name;
            }
        }
    }
}