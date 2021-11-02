using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISetSampleSpecsParametersExtensions
    {
        internal static void AssertEqual(this ISetSampleSpecsParameters expected, SampleOrderItemSpec result)
        {
            if(expected == null)
            {
                Assert.IsNull(result);
                return;
            }

            Assert.AreEqual(expected.SampleOrderItemKey, result.ToSampleOrderItemKey().KeyValue);
            Assert.AreEqual(expected.Notes, result.Notes);
            Assert.AreEqual(expected.AstaMin, result.AstaMin);
            Assert.AreEqual(expected.AstaMax, result.AstaMax);
            Assert.AreEqual(expected.MoistureMin, result.MoistureMin);
            Assert.AreEqual(expected.MoistureMax, result.MoistureMax);
            Assert.AreEqual(expected.WaterActivityMin, result.WaterActivityMin);
            Assert.AreEqual(expected.WaterActivityMax, result.WaterActivityMax);
            Assert.AreEqual(expected.Mesh, result.Mesh);
            Assert.AreEqual(expected.AoverB, result.AoverB);
            Assert.AreEqual(expected.ScovMin, result.ScovMin);
            Assert.AreEqual(expected.ScovMax, result.ScovMax);
            Assert.AreEqual(expected.ScanMin, result.ScanMin);
            Assert.AreEqual(expected.ScanMax, result.ScanMax);
            Assert.AreEqual(expected.TPCMin, result.TPCMin);
            Assert.AreEqual(expected.TPCMax, result.TPCMax);
            Assert.AreEqual(expected.YeastMin, result.YeastMin);
            Assert.AreEqual(expected.YeastMax, result.YeastMax);
            Assert.AreEqual(expected.MoldMin, result.MoldMin);
            Assert.AreEqual(expected.MoldMax, result.MoldMax);
            Assert.AreEqual(expected.ColiformsMin, result.ColiformsMin);
            Assert.AreEqual(expected.ColiformsMax, result.ColiformsMax);
            Assert.AreEqual(expected.EColiMin, result.EColiMin);
            Assert.AreEqual(expected.EColiMax, result.EColiMax);
            Assert.AreEqual(expected.SalMin, result.SalMin);
            Assert.AreEqual(expected.SalMax, result.SalMax);
        }
    }
}