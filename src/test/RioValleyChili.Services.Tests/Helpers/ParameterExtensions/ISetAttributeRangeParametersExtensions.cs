using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISetAttributeRangeParametersExtensions
    {
        internal static void AssertEquivalent(this ISetAttributeRangeParameters expected, ChileProductAttributeRange actual)
        {
            Assert.AreEqual(expected.AttributeNameKey, actual.ToAttributeNameKey().KeyValue);
            Assert.AreEqual(expected.RangeMin, actual.RangeMin);
            Assert.AreEqual(expected.RangeMax, actual.RangeMax);
        }
    }
}