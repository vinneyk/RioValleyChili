﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RioValleyChili.Services.Tests.Helpers
{
    public static class EquivalentHelper
    {
        public static void AssertEquivalent<TExpected, TResult, TKey>(this IEnumerable<TExpected> expectedItems, IEnumerable<TResult> resultItems, Func<TExpected, TKey> selectExpectedKey, Func<TResult, TKey> selectResultKey, Action<TExpected, TResult> assert = null)
        {
            if(expectedItems == null)
            {
                Assert.IsNull(resultItems);
                return;
            }

            Assert.IsNotNull(resultItems);
            Assert.AreEqual(expectedItems.Count(), resultItems.Count());

            var indexedResults = resultItems.ToDictionary(selectResultKey);
            foreach(var expected in expectedItems)
            {
                var eKey = selectExpectedKey(expected);
                TResult match;
                if(!indexedResults.TryGetValue(eKey, out match))
                {
                    Assert.Fail("No match found for expected key [{0}].", eKey);
                }

                if(assert != null)
                {
                    assert(expected, match);
                }
            }
        }

        public static void AssertEquivalent<TExpected, TResult>(this IEnumerable<TExpected> expectedItems, IEnumerable<TResult> resultItems, Action<TExpected, TResult> assert = null)
        {
            if(expectedItems == null)
            {
                Assert.IsNull(resultItems);
                return;
            }

            Assert.IsNotNull(resultItems);
            Assert.AreEqual(expectedItems.Count(), resultItems.Count());

            if(assert != null)
            {
                expectedItems.Zip(resultItems,
                    (expected, result) => new { expected, results = result })
                    .ToList()
                    .ForEach(select => assert(select.expected, select.results));
            }
        }

        public static bool AreEquivalent(object expected, object result)
        {
            if(expected == null)
            {
                return result == null;
            }

            if(result == null)
            {
                return false;
            }

            var expectedEnumerable = expected as IEnumerable;
            if(expectedEnumerable != null)
            {
                return AreEquivalent(((IEnumerable)expected).Cast<object>().ToList(), ((IEnumerable)result).Cast<object>().ToList());
            }

            var expectedType = expected.GetType();
            var resultType = result.GetType();

            if(!expectedType.IsComplexType())
            {
                return expected.Equals(result);
            }

            var expectedProperties = expectedType.GetProperties().Select(p => new
                {
                    Property = p,
                    GetMethod = p.GetGetMethod()
                }).Where(m => m.GetMethod != null).ToList();

            var resultProperties = expectedProperties.Select(e => resultType.GetProperty(e.Property.Name))
                .Where(p => p != null)
                .Select(p => new
                {
                    Property = p,
                    GetMethod = p.GetGetMethod()
                }).Where(m => m.GetMethod != null)
                .ToDictionary(m => m.Property.Name, m => m.GetMethod);

            if(expectedProperties.Count != resultProperties.Count)
            {
                return false;
            }
            if(expectedProperties.Any(e => !AreEquivalent(e.GetMethod.Invoke(expected, null), resultProperties[e.Property.Name].Invoke(result, null))))
            {
                return false;
            }

            var expectedFields = expectedType.GetFields().ToList();
            var resultFields = expectedFields.Select(e => resultType.GetField(e.Name)).Where(p => p != null).ToDictionary(m => m.Name, m => m);
            if(expectedFields.Count != resultFields.Count)
            {
                return false;
            }
            return expectedFields.All(e => AreEquivalent(e.GetValue(expected), resultFields[e.Name].GetValue(result)));
        }

        private static bool AreEquivalent(IReadOnlyCollection<object> expected, IReadOnlyCollection<object> result)
        {
            if(expected.Count != result.Count)
            {
                return false;
            }

            return !expected.Any(e => result.All(r => !AreEquivalent(e, r)));
        }

        private static bool IsComplexType(this Type type)
        {
            if(type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(DateTime))
            {
                return false;
            }

            if(type.IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();
                if(genericDefinition == typeof(Nullable<>))
                {
                    return IsComplexType(type.GetGenericArguments().Single());
                }
            }

            return type.IsClass || type.IsValueType || type.IsInterface;
        }
    }
}
