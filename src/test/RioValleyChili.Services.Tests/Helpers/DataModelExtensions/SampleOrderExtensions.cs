using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class SampleOrderExtensions
    {
        internal static SampleOrder SetRequestCustomer(this SampleOrder sampleOrder, ICustomerKey customer)
        {
            sampleOrder.RequestCustomer = null;
            sampleOrder.RequestCustomerId = customer == null ? (int?) null : customer.CustomerKey_Id;
            return sampleOrder;
        }

        internal static SampleOrder SetBroker(this SampleOrder sampleOrder, ICompanyKey broker)
        {
            sampleOrder.Broker = null;
            sampleOrder.BrokerId = broker == null ? (int?) null : broker.CompanyKey_Id;
            return sampleOrder;
        }

        internal static void AssertEqual(this SampleOrder expected, ISampleOrderSummaryReturn result)
        {
            Assert.AreEqual(expected.ToSampleOrderKey().KeyValue, result.SampleRequestKey);
            Assert.AreEqual(expected.DateDue, result.DateDue);
            Assert.AreEqual(expected.DateReceived, result.DateReceived);
            Assert.AreEqual(expected.DateCompleted, result.DateCompleted);
            Assert.AreEqual(expected.Status, result.Status);

            var requestCompany = expected.RequestCustomer == null ? null : expected.RequestCustomer.Company;
            requestCompany.AssertEqual(result.RequestedByCompany);
            expected.Broker.AssertEqual(result.Broker);
            expected.Employee.AssertEqual(result.CreatedByUser);
        }

        internal static void AssertEqual(this SampleOrder expected, ISampleOrderDetailReturn result)
        {
            Assert.AreEqual(expected.ToSampleOrderKey().KeyValue, result.SampleRequestKey);
            Assert.AreEqual(expected.DateDue, result.DateDue);
            Assert.AreEqual(expected.DateReceived, result.DateReceived);
            Assert.AreEqual(expected.DateCompleted, result.DateCompleted);
            Assert.AreEqual(expected.Status, result.Status);
            Assert.AreEqual(expected.Active, result.Active);
            Assert.AreEqual(expected.FOB, result.FOB);
            Assert.AreEqual(expected.ShipmentMethod, result.ShipVia);
            Assert.AreEqual(expected.Comments, result.Comments);
            Assert.AreEqual(expected.PrintNotes, result.NotesToPrint);

            var requestCompany = expected.RequestCustomer == null ? null : expected.RequestCustomer.Company;
            requestCompany.AssertEqual(result.RequestedByCompany);
            expected.Request.AssertEqual(result.RequestedByShippingLabel);

            Assert.AreEqual(expected.ShipToCompany, result.ShipToCompany);
            expected.ShipTo.AssertEqual(result.ShipToShippingLabel);

            expected.Broker.AssertEqual(result.Broker);
            expected.Employee.AssertEqual(result.CreatedByUser);

            expected.Items.AssertEquivalent(result.Items, e => e.ToSampleOrderItemKey().KeyValue, r => r.ItemKey,
                (e, r) => e.AssertEqual(r));
            expected.JournalEntries.AssertEquivalent(result.JournalEntries, j => j.ToSampleOrderJournalEntryKey().KeyValue, r => r.JournalEntryKey,
                (e, r) => e.AssertEqual(r));
        }
    }
}