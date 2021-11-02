using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;

namespace RioValleyChili.Tests.Helpers
{
    internal static class ApiControllerTestHelpers
    {
        #region AssertAntiForgeryTokenValidationForMethod

        public static void AssertAntiForgeryTokenValidationForMethod<TResult>(Func<TResult> methodDelegate)
        {
            AssertAntiForgeryTokenValidationForMethod(methodDelegate.Method);
        }

        public static void AssertAntiForgeryTokenValidationForMethod<T0, TResult>(Func<T0, TResult> methodDelegate)
        {
            AssertAntiForgeryTokenValidationForMethod(methodDelegate.Method);
        }

        public static void AssertAntiForgeryTokenValidationForMethod<T0, T1, TResult>(
            Func<T0, T1, TResult> methodDelegate)
        {
            AssertAntiForgeryTokenValidationForMethod(methodDelegate.Method);
        }

        public static void AssertAntiForgeryTokenValidationForMethod<T0, T1, T2, TResult>(
            Func<T0, T1, T2, TResult> methodDelegate)
        {
            AssertAntiForgeryTokenValidationForMethod(methodDelegate.Method);
        }

        public static void AssertAntiForgeryTokenValidationForMethod<T0, T1, T2, T3, TResult>(
            Func<T0, T1, T2, T3, TResult> methodDelegate)
        {
            AssertAntiForgeryTokenValidationForMethod(methodDelegate.Method);
        }

        public static void AssertAntiForgeryTokenValidationForMethod<T0, T1, T2, T3, T4, TResult>(
            Func<T0, T1, T2, T3, T4, TResult> methodDelegate)
        {
            AssertAntiForgeryTokenValidationForMethod(methodDelegate.Method);
        }

        public static void AssertAntiForgeryTokenValidationForMethod<T0, T1, T2, T3, T4, T5, TResult>(
            Func<T0, T1, T2, T3, T4, T5, TResult> methodDelegate)
        {
            AssertAntiForgeryTokenValidationForMethod(methodDelegate.Method);
        }

        public static void AssertAntiForgeryTokenValidationForMethod<T0, T1, T2, T3, T4, T5, T6, TResult>(
            Func<T0, T1, T2, T3, T4, T5, T6, TResult> methodDelegate)
        {
            AssertAntiForgeryTokenValidationForMethod(methodDelegate.Method);
        }

        private static void AssertAntiForgeryTokenValidationForMethod(MemberInfo memberInfo)
        {
            var antiForgeryTokenAttribute = memberInfo.GetCustomAttribute<ValidateAntiForgeryTokenFromCookieAttribute>();
            Assert.IsNotNull(antiForgeryTokenAttribute, string.Format("Expected the method {0} to be decorated with {1}.", memberInfo.Name, typeof(ValidateAntiForgeryTokenFromCookieAttribute).Name));
        }

        internal static void AssertAntiForgeryTokenValidationForMethod(this Type type, string methodName)
        {
            AssertAntiForgeryTokenValidationForMethod(type.GetMethod(methodName));
        }

        #endregion

        #region AssertClaimsForMethod

        public static void AssertClaimsForMethod<TResult>(Func<TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }

        public static void AssertClaimsForMethod<T0, TResult>(Func<T0, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, TResult>(Func<T0, T1, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, TResult>(Func<T0, T1, T2, T3, T4, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, TResult>(Func<T0, T1, T2, T3, T4, T5, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        public static void AssertClaimsForMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> methodDelegate, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(methodDelegate.Method, claimResources, claimType);
        }
        
        public static void AssertClaimsForMethod(this Type type, string methodName, IEnumerable<string> claimResources, string claimType)
        {
            AssertClaimsForMethodByMemberInfo(type.GetMethod(methodName), claimResources, claimType);
        }

        private static void AssertClaimsForMethodByMemberInfo(MemberInfo memberInfo, IEnumerable<string> claimResources, string claimType)
        {
            var isValid = ContainsExpectedClaims(memberInfo.GetCustomAttributes<ClaimsAuthorizeAttribute>(), claimResources, claimType);
            Assert.IsTrue(isValid, string.Format("Expected claims validation to be defined for the method \"{0}\". Claim expected: \"{1}\" for resource(s): \"{2}\".", 
                memberInfo.Name, 
                claimType,
                string.Join(", ", claimResources)));
        }

        private static bool ContainsExpectedClaims(IEnumerable<ClaimsAuthorizeAttribute> claims, IEnumerable<string> claimResources, string claimType)
        {
            return claimResources.All(r =>
                claims.Any(a =>
                    a.GetClaims().Any(c => c.Value == r && c.Type == claimType )));
        }

        #endregion

        internal static void AssertClaimsForType(this Type type, IEnumerable<string> claimResources, string claimType)
        {
            var claimsAttributes = type.GetCustomAttributes<ClaimsAuthorizeAttribute>();
            Assert.IsTrue(ContainsExpectedClaims(claimsAttributes, claimResources, claimType));
        }
    }
}