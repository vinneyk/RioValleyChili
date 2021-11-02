using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotCustomerOrderAllowanceExtensions
    {
        internal static void AssertAreEqual(this LotSalesOrderAllowance expected, ILotCustomerOrderAllowanceReturn result)
        {
            Assert.AreEqual(expected.ToSalesOrderKey().KeyValue, result.OrderKey);
            Assert.AreEqual(expected.SalesOrder.InventoryShipmentOrder.MoveNum, result.OrderNumber);
        }
    }
}