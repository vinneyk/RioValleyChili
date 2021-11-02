using System.Collections;
using System.Linq;
using NUnit.Framework;
using System;

namespace RioValleyChili.Tests
{
    public static class Assess
    {
        public static void IsTrue(bool condition, string message = null)
        {
            if (!condition)
            {
                Inconclusive(message);
            }
        }

        public static void IsFalse(bool condition, string message = null)
        {
            IsTrue(!condition, message);
        }

        public static void IsNull(object x, string message = null)
        {
            IsTrue(x == null, message);
        }

        public static void IsNotNull(object x, string message = null)
        {
            IsTrue(x != null, message);
        }

        public static void AreNotEqual(string expected, string actual, string message = null)
        {
            IsFalse(expected.Equals(actual), message);
        }

        public static void AreNotEqual(int expected, int actual, string message = null)
        {
            IsFalse(expected.Equals(actual), message);
        }

        private static void Inconclusive(string message = null)
        {
            Assert.Inconclusive(message ?? "The test was determined to be inconclusive.");
        }

        public static void IsNotEmpty(IEnumerable collection, string message = null)
        {
            IsTrue(collection.Cast<object>().Any(), message);
        }

        public static void IsEmpty(IEnumerable collection, string message = null)
        {
            IsFalse(collection.Cast<object>().Any(), message);
        }
    }
}