using System;
using System.Linq;

namespace RioValleyChili.Services.Utilities.Helpers
{
    public static class TypeExtensions
    {
        public static Type GetGenericInterfaceImplementation(this Type t, Type interfaceDefinition)
        {
            if(!interfaceDefinition.IsInterface || !interfaceDefinition.IsGenericTypeDefinition)
            {
                throw new ArgumentException("TDefinition must be a generic interface definition.");
            }

            var interfaces = t.GetInterfaces().Where(i => i.IsGenericType).ToList();
            if(t.IsInterface && t.IsGenericType)
            {
                interfaces.Insert(0, t);
            }

            return interfaces.FirstOrDefault(i => i.GetGenericTypeDefinition() == interfaceDefinition);
        }
    }
}