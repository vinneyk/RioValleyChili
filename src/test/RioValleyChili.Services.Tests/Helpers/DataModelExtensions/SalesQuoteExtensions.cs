using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class SalesQuoteExtensions
    {
        internal static SalesQuote SetCustomer(this SalesQuote salesQuote, ICustomerKey customer)
        {
            salesQuote.Customer = null;
            salesQuote.CustomerId = customer == null ? (int?) null : customer.CustomerKey_Id;
            return salesQuote;
        }

        internal static SalesQuote SetBroker(this SalesQuote salesQuote, ICompanyKey broker)
        {
            salesQuote.Broker = null;
            salesQuote.BrokerId = broker == null ? (int?) null : broker.CompanyKey_Id;
            return salesQuote;
        }

        internal static void AssertEqual(this SalesQuote expected, ISalesQuoteSummaryReturn result)
        {
            Assert.AreEqual(expected.ToSalesQuoteKey().KeyValue, result.SalesQuoteKey);
            Assert.AreEqual(expected.QuoteNum, result.QuoteNumber);
            Assert.AreEqual(expected.QuoteDate, result.QuoteDate);
            Assert.AreEqual(expected.ShipmentInformation.ShipmentDate, result.ShipmentDate);
            Assert.AreEqual(expected.Customer == null ? null : expected.Customer.Company.Name, result.CustomerName);
            Assert.AreEqual(expected.Broker == null ? null : expected.Broker.Name, result.BrokerName);
            Assert.AreEqual(expected.SourceFacility == null ? null : expected.SourceFacility.Name, result.SourceFacilityName);
        }

        internal static void AssertEqual(this SalesQuote expected, ISalesQuoteDetailReturn result)
        {
            Assert.AreEqual(expected.ToSalesQuoteKey().KeyValue, result.SalesQuoteKey);
            Assert.AreEqual(expected.QuoteNum, result.QuoteNumber);
            Assert.AreEqual(expected.QuoteDate, result.QuoteDate);
            Assert.AreEqual(expected.DateReceived, result.DateReceived);
            Assert.AreEqual(expected.CalledBy, result.CalledBy);
            Assert.AreEqual(expected.TakenBy, result.TakenBy);
            expected.ShipmentInformation.AssertEqual(result.Shipment);

            if(expected.SourceFacility == null)
            {
                Assert.IsNull(result.SourceFacility);
            }
            else
            {
                expected.SourceFacility.AssertEqual(result.SourceFacility);
            }

            if(expected.Customer == null)
            {
                Assert.IsNull(result.Customer);
            }
            else
            {
                expected.Customer.Company.AssertEqual(result.Customer);
            }

            if(expected.Broker == null)
            {
                Assert.IsNull(result.Broker);
            }
            else
            {
                expected.Broker.AssertEqual(result.Broker);
            }

            expected.Items.AssertEquivalent(result.Items, e => e.ToSalesQuoteItemKey().KeyValue, r => r.SalesQuoteItemKey, SalesQuoteItemExtensions.AssertEqual);
        }
    }
}