using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Helpers
{
    [ExtractIntoSolutionheadLibrary]
    public class ViewHelper
    {
        /// <summary>
        /// Generates an enumerable of PropetyInfo objects which make up a recursive account of view properties of the specified type and any nested types contained by the type. 
        /// Note: properties without a public Get accessor or properties decorated with a DisplayAttribute or HiddendInputAttribute are excluded.
        /// </summary>
        /// <param name="modelType">The type from which to build the PropertyInfo collection.</param>
        public static IEnumerable<PropertyInfo> GetViewPropertiesForModel(Type modelType)
        {
            return from prop in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                   where prop.CanRead && prop.GetGetMethod() != null
                   let autoGenerate = prop.GetCustomAttributes<DisplayAttribute>()
                   where autoGenerate == null || autoGenerate.All(a => a.GetAutoGenerateField() != false)
                   select prop;
        }

        /// <summary>
        /// Generates an enumerable of lambda expressions which contain member access expressions for all valid properties defined by the type. Note container properties with a null value will be omitted.
        /// </summary>
        /// <param name="expression">An expression indicating the member to evaluate.</param>
        /// <param name="model">The model which contains the property values.</param>
        /// <returns></returns>
        public static IEnumerable<LambdaExpression> GetLambdaExpressionsForViewProperties(Expression expression, object model)
        {
            return from prop in GetViewPropertiesForModel(expression.GetReturnValueType())
                   let lambda = (LambdaExpression) expression
                   let member = Expression.MakeMemberAccess(lambda.Body, prop)
                   let propertyAccess = Expression.Lambda(member, lambda.Parameters)
                   let isComplex = prop.PropertyType.IsComplexType()
                   where isComplex == false || (isComplex && propertyAccess.Compile().DynamicInvoke(model) != null)
                   select propertyAccess;
        }

        public static bool IsHiddenInput(LambdaExpression expression)
        {
            var memberInfo = ((MemberExpression) expression.Body).Member;
            return IsHiddenInput(memberInfo);
        }

        public static bool IsHiddenInput<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            if (expression.NodeType != ExpressionType.Lambda) { throw new NotSupportedException("IsHiddenInput expects a LambdaExpression."); }

            var member = (MemberExpression) expression.Body;
            return IsHiddenInput(member.Member);
        }

        private static bool IsHiddenInput(MemberInfo member)
        {
            if (member == null) { throw new ArgumentNullException("member"); }
            return member.GetCustomAttribute<HiddenInputAttribute>() != null;
        }
    }
}