using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISetSampleOrderParametersExtensions
    {
        internal static void AssertEqual(this ISetSampleOrderParameters expected, SampleOrder result)
        {
            if(!string.IsNullOrWhiteSpace(expected.SampleOrderKey))
            {
                Assert.AreEqual(expected.SampleOrderKey, result.ToSampleOrderKey().KeyValue);
            }

            Assert.AreEqual(expected.DateDue, result.DateDue);
            Assert.AreEqual(expected.DateReceived, result.DateReceived);
            Assert.AreEqual(expected.DateCompleted, result.DateCompleted);
            Assert.AreEqual(expected.Status, result.Status);
            Assert.AreEqual(expected.Active, result.Active);
            Assert.AreEqual(expected.Comments, result.Comments);
            Assert.AreEqual(expected.PrintNotes, result.PrintNotes);
            Assert.AreEqual(expected.Volume, result.Volume);
            Assert.AreEqual(expected.ShipToCompany, result.ShipToCompany);
            Assert.AreEqual(expected.ShipmentMethod, result.ShipmentMethod);
            Assert.AreEqual(expected.FOB, result.FOB);

            if(!string.IsNullOrWhiteSpace(expected.BrokerKey))
            {
                Assert.AreEqual(expected.BrokerKey, result.Broker.ToCompanyKey().KeyValue);
            }
            else
            {
                Assert.IsNull(result.Broker);
            }

            if(!string.IsNullOrWhiteSpace(expected.RequestedByCompanyKey))
            {
                Assert.AreEqual(expected.RequestedByCompanyKey, result.RequestCustomer.ToCustomerKey().KeyValue);
            }
            else
            {
                Assert.IsNull(result.RequestCustomer);
            }

            expected.RequestedByShippingLabel.AssertEquivalent(result.Request);
            expected.ShipToShippingLabel.AssertEquivalent(result.ShipTo);

            Assert.AreEqual(expected.Items.Count(), result.Items.Count);
            foreach(var item in expected.Items)
            {
                Assert.IsTrue(result.Items.Any(i =>
                    {
                        try
                        {
                            item.AssertEqual(i);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }));
            }}
        }
    }