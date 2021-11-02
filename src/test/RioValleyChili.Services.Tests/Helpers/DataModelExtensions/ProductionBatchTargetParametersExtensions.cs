using System;
using NUnit.Framework;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    public static class ProductionBatchTargetParametersExtensions
    {
        internal static void SetTargetParameters(this PackSchedule packSchedule, double asta, double scan, double scoville, double weight)
        {
            if(packSchedule == null) { throw new ArgumentNullException("packSchedule"); }

            packSchedule.DefaultBatchTargetParameters.BatchTargetWeight = weight;
            packSchedule.DefaultBatchTargetParameters.BatchTargetAsta = asta;
            packSchedule.DefaultBatchTargetParameters.BatchTargetScan = scan;
            packSchedule.DefaultBatchTargetParameters.BatchTargetScoville = scoville;
        }

        internal static void SetTargetParameters(this ProductionBatch productionBatch, double asta, double scan, double scoville, double weight)
        {
            if(productionBatch == null) { throw new ArgumentNullException("productionBatch"); }

            productionBatch.TargetParameters.BatchTargetAsta = asta;
            productionBatch.TargetParameters.BatchTargetScan = scan;
            productionBatch.TargetParameters.BatchTargetScoville = scoville;
            productionBatch.TargetParameters.BatchTargetWeight = weight;
        }

        public static void AssertAsExpected(this IProductionBatchTargetParameters expectedParameters, IProductionBatchTargetParameters targetParameters)
        {
            if(expectedParameters == null) { throw new ArgumentNullException("expectedParameters"); }
            if(targetParameters == null) {  throw new ArgumentNullException("targetParameters"); }

            Assert.AreEqual(expectedParameters.BatchTargetWeight, targetParameters.BatchTargetWeight);
            Assert.AreEqual(expectedParameters.BatchTargetAsta, targetParameters.BatchTargetAsta);
            Assert.AreEqual(expectedParameters.BatchTargetScan, targetParameters.BatchTargetScan);
            Assert.AreEqual(expectedParameters.BatchTargetScoville, targetParameters.BatchTargetScoville);
        }
    }
}