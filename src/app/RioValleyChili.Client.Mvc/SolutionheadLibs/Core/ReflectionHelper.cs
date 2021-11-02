using System;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.Core
{
    [ExtractIntoSolutionheadLibrary]
    public static class ReflectionHelper
    {
        public static bool IsComplexType(this Type type)
        {
            if (type == typeof (string)
                || type.IsPrimitive
                || type.IsEnum) return false;

            return type.IsGenericType 
                ? type.IsGenericWithComplexTypeArgument() 
                : type.IsClass || type.IsInterface || type.IsCustomStruct();
        }
        
        private static bool IsCustomStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum && !type.Namespace.StartsWith("System");
        }

        private static bool IsGenericWithComplexTypeArgument(this Type type)
        {
            return type.IsGenericType && type.GetGenericArguments().Any(IsComplexType);
        }

        public static Type GetReturnValueType(this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    return ((LambdaExpression)expression).ReturnType;
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).Member.ReflectedType;
                default: throw new ArgumentException("The expression is not supported.");
            }
        }
    }
}