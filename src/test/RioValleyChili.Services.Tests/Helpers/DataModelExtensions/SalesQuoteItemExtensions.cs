using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class SalesQuoteItemExtensions
    {
        internal static void AssertEqual(this SalesQuoteItem expected, ISalesQuoteItemReturn result)
        {
            Assert.AreEqual(expected.ToSalesQuoteItemKey().KeyValue, result.SalesQuoteItemKey);
            Assert.AreEqual(expected.Quantity, result.Quantity);
            Assert.AreEqual(expected.CustomerProductCode, result.CustomerProductCode);
            Assert.AreEqual(expected.PriceBase, result.PriceBase);
            Assert.AreEqual(expected.PriceFreight, result.PriceFreight);
            Assert.AreEqual(expected.PriceTreatment, result.PriceTreatment);
            Assert.AreEqual(expected.PriceWarehouse, result.PriceWarehouse);
            Assert.AreEqual(expected.PriceRebate, result.PriceRebate);
            Assert.AreEqual(expected.Treatment.ToInventoryTreatmentKey().KeyValue, result.TreatmentKey);
            expected.Product.AssertEqual(result.Product);
            expected.PackagingProduct.AssertEqual(result.Packaging);
        }
    }
}