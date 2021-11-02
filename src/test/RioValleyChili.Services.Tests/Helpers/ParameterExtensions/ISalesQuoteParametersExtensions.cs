using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISalesQuoteParametersExtensions
    {
        internal static void AssertEqual(this ISalesQuoteParameters expected, SalesQuote result)
        {
            if(expected.SalesQuoteNumber != null)
            {
                Assert.AreEqual(expected.SalesQuoteNumber, result.QuoteNum);
            }

            Assert.AreEqual(expected.QuoteDate, result.QuoteDate);
            Assert.AreEqual(expected.DateReceived, result.DateReceived);
            Assert.AreEqual(expected.CalledBy, result.CalledBy);
            Assert.AreEqual(expected.TakenBy, result.TakenBy);
            Assert.AreEqual(expected.UserToken, result.Employee.UserName);

            if(!string.IsNullOrWhiteSpace(expected.SourceFacilityKey))
            {
                Assert.AreEqual(expected.SourceFacilityKey, result.SourceFacility.ToFacilityKey().KeyValue);
            }
            else
            {
                Assert.IsNull(result.SourceFacility);
            }

            if(!string.IsNullOrWhiteSpace(expected.CustomerKey))
            {
                Assert.AreEqual(expected.CustomerKey, result.Customer.ToCustomerKey().KeyValue);
            }
            else
            {
                Assert.IsNull(result.Customer);
            }

            if(!string.IsNullOrWhiteSpace(expected.BrokerKey))
            {
                Assert.AreEqual(expected.BrokerKey, result.Broker.ToCompanyKey().KeyValue);
            }
            else
            {
                Assert.IsNull(result.Broker);
            }

            expected.ShipmentInformation.AssertEqual(result.ShipmentInformation, result.SoldTo);

            var resultItems = result.Items.ToList();
            (expected.Items ?? new ISalesQuoteItemParameters[0]).All(e =>
                {
                    var match = resultItems.FirstOrDefault(r =>
                        (e.SalesQuoteItemKey == null || (e.SalesQuoteItemKey == r.ToSalesQuoteItemKey().KeyValue)) &&
                        e.ProductKey == r.Product.ToProductKey().KeyValue &&
                        e.PackagingKey == r.PackagingProduct.ToPackagingProductKey().KeyValue &&
                        e.TreatmentKey == r.Treatment.ToInventoryTreatmentKey().KeyValue &&
                        e.Quantity == r.Quantity &&
                        e.PriceBase == r.PriceBase &&
                        e.PriceFreight == r.PriceFreight &&
                        e.PriceTreatment == r.PriceTreatment &&
                        e.PriceWarehouse == r.PriceWarehouse &&
                        e.PriceRebate == r.PriceRebate);
                    Assert.IsNotNull(match);
                    resultItems.Remove(match);
                    return true;
                });
        }
    }
}