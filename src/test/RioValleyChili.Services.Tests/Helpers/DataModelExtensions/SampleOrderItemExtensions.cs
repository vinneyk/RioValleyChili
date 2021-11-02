using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class SampleOrderItemExtensions
    {
        internal static void AssertEqual(this SampleOrderItem expected, ISampleOrderItemReturn result)
        {
            Assert.AreEqual(expected.ToSampleOrderItemKey().KeyValue, result.ItemKey);
            Assert.AreEqual(expected.CustomerProductName, result.CustomerProductName);
            Assert.AreEqual(expected.Lot == null ? null : expected.Lot.ToLotKey().KeyValue, result.LotKey);
            Assert.AreEqual(expected.Product == null ? null : expected.Product.ToProductKey().KeyValue, result.ProductKey);
            Assert.AreEqual(expected.Product == null ? (ProductTypeEnum?)null : expected.Product.ProductType, result.ProductType);
            Assert.AreEqual(expected.Quantity, result.Quantity);
            Assert.AreEqual(expected.Description, result.Description);

            expected.Spec.AssertEqual(result.CustomerSpec);
            expected.Match.AssertEqual(result.LabResults);
        }
    }
}