using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISetSampleMatchParametersExtensions
    {
        internal static void AssertEqual(this ISetSampleMatchParameters expected, SampleOrderItemMatch result)
        {
            if(expected == null)
            {
                Assert.IsNull(result);
                return;
            }

            Assert.AreEqual(expected.SampleOrderItemKey, result.ToSampleOrderItemKey().KeyValue);
            Assert.AreEqual(expected.Notes, result.Notes);
            Assert.AreEqual(expected.Gran, result.Gran);
            Assert.AreEqual(expected.AvgAsta, result.AvgAsta);
            Assert.AreEqual(expected.AoverB, result.AoverB);
            Assert.AreEqual(expected.AvgScov, result.AvgScov);
            Assert.AreEqual(expected.H2O, result.H2O);
            Assert.AreEqual(expected.Scan, result.Scan);
            Assert.AreEqual(expected.Yeast, result.Yeast);
            Assert.AreEqual(expected.Mold, result.Mold);
            Assert.AreEqual(expected.Coli, result.Coli);
            Assert.AreEqual(expected.TPC, result.TPC );
            Assert.AreEqual(expected.EColi, result.EColi);
            Assert.AreEqual(expected.Sal, result.Sal);
            Assert.AreEqual(expected.InsPrts, result.InsPrts);
            Assert.AreEqual(expected.RodHrs, result.RodHrs);
        }
    }
}