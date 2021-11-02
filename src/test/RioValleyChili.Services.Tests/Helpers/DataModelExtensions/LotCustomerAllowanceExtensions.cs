using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotCustomerAllowanceExtensions
    {
        internal static void AssertAreEqual(this LotCustomerAllowance expected, ILotCustomerAllowanceReturn result)
        {
            Assert.AreEqual(expected.ToCustomerKey().KeyValue, result.CustomerKey);
            Assert.AreEqual(expected.Customer.Company.Name, result.CustomerName);
        }
    }
}