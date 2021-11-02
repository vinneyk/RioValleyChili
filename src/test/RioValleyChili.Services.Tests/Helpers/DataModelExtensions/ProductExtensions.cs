using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ProductExtensions
    {
        internal static void AssertEqual(this Product expected, IProductReturn result)
        {
            Assert.AreEqual(expected.ToProductKey().KeyValue, result.ProductKey);
            Assert.AreEqual(expected.ProductCode, result.ProductCode);
            Assert.AreEqual(expected.IsActive, result.IsActive);
        }
    }
}