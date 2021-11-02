using NUnit.Framework;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ShippingLabelExtensions
    {
        internal static void AssertEquivalent(this ShippingLabel expected, ShippingLabel result)
        {
            expected = expected ?? new ShippingLabel();
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Phone, result.Phone);
            Assert.AreEqual(expected.EMail, result.EMail);
            Assert.AreEqual(expected.Fax, result.Fax);

            expected.Address.AssertEquivalent(result.Address);
        }
    }
}