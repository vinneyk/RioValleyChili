using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.Core.Utilities
{
    public class CompanyDetailsBuilder
    {
        private static readonly PropertyInfo _customerPropertiesNode = typeof(ICompanyDetailReturn).GetProperties().First(p => p.PropertyType == typeof(ICustomerCompanyReturn));
        
        public IDictionary<string, object> BuildCompanyDetailsObject(ICompanyDetailReturn source)
        {
            if(source == null) return new Dictionary<string, object>();

            var dictionary = new RouteValueDictionary(source);
            
            if (!source.CompanyTypes.Contains(CompanyType.Customer))
            {
                dictionary.Remove(_customerPropertiesNode.Name);
            }

            return dictionary;
        }
    }
}