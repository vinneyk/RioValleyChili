using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class EmployeeExtensions
    {
        internal static void AssertEqual(this Employee expected, IUserSummaryReturn result)
        {
            Assert.AreEqual(expected.ToEmployeeKey().KeyValue, result.EmployeeKey);
            Assert.AreEqual(expected.UserName, result.Name);
        }
    }
}