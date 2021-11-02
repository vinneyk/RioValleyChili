using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class ClientBoundUIHintAttribute : UIHintAttribute, IClientBoundProperty
    {
        protected readonly DataBindingAttributeDictionary DataBindingAttributeDictionary;
        private readonly DataBindingMode _validForModes;

        protected ClientBoundUIHintAttribute(DataBindingAttributeDictionary dataBindingAttributeDictionary, DataBindingMode validForModes, string uiHint)
            : base(uiHint)
        {
            if (dataBindingAttributeDictionary == null) { throw new ArgumentNullException("dataBindingAttributeDictionary"); }
            DataBindingAttributeDictionary = dataBindingAttributeDictionary;

            _validForModes = validForModes;
        }
        
        public bool IsValidForMode(DataBindingMode currentMode)
        {
            return _validForModes.HasFlag(currentMode);
        }

        public DataBindingAttributeDictionary GetDatabindingAttributes()
        {
            return DataBindingAttributeDictionary;
        }

        protected static string GetBindingScope(ClientBindingLevel bindingLevel)
        {
            switch (bindingLevel)
            {
                case ClientBindingLevel.Self: return "$data";
                case ClientBindingLevel.Parent: return "$parent";
                default: return "$root";
            }
        }

        public virtual bool IsValidForProperty(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Property)
            {
                return false;
            }

            var property = (PropertyInfo)member;
            return !property.PropertyType.IsClass || property.PropertyType == typeof(string);
        }
    }
}