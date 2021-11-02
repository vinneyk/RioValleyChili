using System;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISetDehydratedMaterialsReceivedParametersExtensions
    {
        internal static void AssertAsExpected(this ICreateChileMaterialsReceivedParameters parameters, ChileMaterialsReceived received)
        {
            Assert.AreEqual(parameters.DateReceived, received.DateReceived);
            Assert.AreEqual(parameters.LoadNumber, received.LoadNumber);
            Assert.AreEqual(parameters.PurchaseOrder, received.ChileLot.Lot.PurchaseOrderNumber);
            Assert.AreEqual(parameters.ShipperNumber, received.ChileLot.Lot.ShipperNumber);
            Assert.AreEqual(parameters.SupplierKey, received.ToCompanyKey().KeyValue);
            Assert.AreEqual(parameters.ChileProductKey, received.ToChileProductKey().KeyValue);

            var expectedItems = parameters.Items.ToList();
            var receivedItems = received.Items.ToList();
            Assert.AreEqual(expectedItems.Count, receivedItems.Count);
            foreach(var item in expectedItems)
            {
                var itemMatch = receivedItems.First(i =>
                    {
                        try
                        {
                            item.AssertAsExpected(i);
                            return true;
                        }
                        catch(Exception)
                        {
                            return false;
                        }
                    });
                receivedItems.Remove(itemMatch);
            }
        }
    }
}