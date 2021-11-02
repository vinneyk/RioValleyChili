using System;

namespace RioValleyChili.Services.Utilities.Helpers
{
    public static class ReflectionHelper
    {
        public static bool Implements(this Type type, Type baseType, out Type[] genericArguments)
        {
            genericArguments = null;
            if(type == null || type.IsGenericTypeDefinition)
            {
                return false;
            }

            if(baseType.IsGenericTypeDefinition)
            {
                if(type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                {
                    genericArguments = type.GetGenericArguments();
                    return true;
                }
            }
            else if(baseType.IsAssignableFrom(type))
            {
                return true;
            }

            return Implements(type.BaseType, baseType, out genericArguments);
        }
    }
}