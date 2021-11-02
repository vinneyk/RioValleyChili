using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class IDehydratedMaterialsReceivedItemParametersExtensions
    {
        internal static void AssertAsExpected(this ICreateChileMaterialsReceivedItemParameters parameters, ChileMaterialsReceivedItem item)
        {
            Assert.AreEqual(parameters.GrowerCode, item.GrowerCode);
            Assert.AreEqual(parameters.ToteKey, item.ToteKey);
            Assert.AreEqual(parameters.Quantity, item.Quantity);
            Assert.AreEqual(parameters.Variety, item.ChileVariety);
            Assert.AreEqual(parameters.PackagingProductKey, new PackagingProductKey(item).KeyValue);
            Assert.AreEqual(parameters.LocationKey, new LocationKey(item).KeyValue);
        }
    }
}