using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RioValleyChili.Services.Utilities.Helpers
{
    public static class LambdaExpressionHelper
    {
        public static Expression CreateAnonymousSelector(IEnumerable<PropertyInfo> sourceProperties)
        {
            var propDictionary = sourceProperties.ToDictionary(p => p.Name, p => p);
            var declaringType = propDictionary.First().Value.DeclaringType;
            var dynamicType = TypeBuilder.GetDynamicType(propDictionary.ToDictionary(p => p.Key, p => p.Value.PropertyType));

            var createMethod = CreateSelectorInfo.MakeGenericMethod(declaringType, dynamicType);
            return (Expression) createMethod.Invoke(null, new object[] { propDictionary });
        }

        public static Expression<Func<TSource, TResult>> CreateTypedSelector<TSource, TResult>(IDictionary<string, PropertyInfo> sourceProperties)
        {
            var sourceType = typeof(TSource);
            var resultType = typeof(TResult);

            var sourceParameter = Expression.Parameter(sourceType, "d");
            var bindings = resultType.GetFields().Select(f => Expression.Bind(f, Expression.Property(sourceParameter, sourceProperties[f.Name])));
            var constructor = resultType.GetConstructor(Type.EmptyTypes);
            if(constructor == null)
            {
                throw new Exception(string.Format("Type '{0}' does not have a public parameterless constructor defined.", resultType.Name));
            }

            var memberInit = Expression.MemberInit(Expression.New(constructor), bindings);
            return (Expression<Func<TSource, TResult>>)Expression.Lambda(memberInit, sourceParameter);
        }

        private static readonly MethodInfo CreateSelectorInfo = typeof(LambdaExpressionHelper).GetMethod("CreateTypedSelector", BindingFlags.Static | BindingFlags.Public);
    }
}