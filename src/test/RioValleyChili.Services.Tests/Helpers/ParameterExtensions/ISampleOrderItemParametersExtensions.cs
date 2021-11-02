using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISampleOrderItemParametersExtensions
    {
        internal static void AssertEqual(this ISampleOrderItemParameters expected, SampleOrderItem result)
        {
            if(!string.IsNullOrWhiteSpace(expected.SampleOrderItemKey))
            {
                Assert.AreEqual(expected.SampleOrderItemKey, result.ToSampleOrderItemKey().KeyValue);
            }

            Assert.AreEqual(expected.Quantity, result.Quantity);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.CustomerProductName, result.CustomerProductName);

            if(!string.IsNullOrWhiteSpace(expected.ProductKey))
            {
                Assert.AreEqual(expected.ProductKey, result.Product.ToProductKey().KeyValue);
            }
            else
            {
                Assert.IsNull(result.Product);
                
                if(!string.IsNullOrWhiteSpace(expected.LotKey))
                {
                    Assert.AreEqual(expected.LotKey, result.Lot.ToLotKey().KeyValue);
                }
                else
                {
                    Assert.IsNull(result.Lot);
                }
            }
        }
    }
}