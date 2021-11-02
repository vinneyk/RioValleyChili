using NUnit.Framework;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class AddressExtensions
    {
        internal static void AssertEquivalent(this Address expected, Address result)
        {
            expected = expected ?? new Address();
            Assert.AreEqual(expected.AddressLine1, result.AddressLine1);
            Assert.AreEqual(expected.AddressLine2, result.AddressLine2);
            Assert.AreEqual(expected.AddressLine3, result.AddressLine3);
            Assert.AreEqual(expected.City, result.City);
            Assert.AreEqual(expected.State, result.State);
            Assert.AreEqual(expected.PostalCode, result.PostalCode);
            Assert.AreEqual(expected.Country, result.Country);
        }
    }
}