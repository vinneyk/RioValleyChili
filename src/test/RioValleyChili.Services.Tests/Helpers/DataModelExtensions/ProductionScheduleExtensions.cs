using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ProductionScheduleExtensions
    {
        internal static ProductionSchedule SetProductionLine(this ProductionSchedule productionSchedule, ILocationKey location)
        {
            productionSchedule.ProductionLineLocation = null;
            productionSchedule.ProductionLineLocationId = location.LocationKey_Id;
            return productionSchedule;
        }

        internal static void AssertEquivalent(this ProductionSchedule expected, IProductionScheduleSummaryReturn result)
        {
            Assert.AreEqual(expected.ToProductionScheduleKey().KeyValue, result.ProductionScheduleKey);
            Assert.AreEqual(expected.ProductionDate, result.ProductionDate);
            expected.ProductionLineLocation.AssertEqual(result.ProductionLine);
        }

        internal static void AssertEquivalent(this ProductionSchedule expected, IProductionScheduleDetailReturn result)
        {
            Assert.AreEqual(expected.ToProductionScheduleKey().KeyValue, result.ProductionScheduleKey);
            expected.ScheduledItems.AssertEquivalent(result.ScheduledItems, e => e.Index, r => r.Index,
                (e, r) =>
                    {
                        Assert.AreEqual(e.FlushBefore, r.FlushBefore);
                        Assert.AreEqual(e.FlushBeforeInstructions, r.FlushBeforeInstructions);
                        Assert.AreEqual(e.FlushAfter, r.FlushAfter);
                        Assert.AreEqual(e.FlushAfterInstructions, r.FlushAfterInstructions);

                        Assert.AreEqual(e.PackSchedule.ToPackScheduleKey().KeyValue, r.PackSchedule.PackScheduleKey);
                        Assert.AreEqual(e.PackSchedule.ProductionDeadline, r.PackSchedule.ProductionDeadline);
                        Assert.AreEqual(e.PackSchedule.SummaryOfWork, r.PackSchedule.Instructions);
                        Assert.AreEqual(e.PackSchedule.ChileProduct.Mesh, r.PackSchedule.AverageGranularity);
                        Assert.AreEqual(e.PackSchedule.GetAverage(StaticAttributeNames.AB), r.PackSchedule.AverageAoverB, 0.01);
                        Assert.AreEqual(e.PackSchedule.GetAverage(StaticAttributeNames.Scoville), r.PackSchedule.AverageScoville, 0.01);
                        Assert.AreEqual(e.PackSchedule.GetAverage(StaticAttributeNames.Scan), r.PackSchedule.AverageScan, 0.01);

                        e.PackSchedule.ChileProduct.AssertEqual(r.PackSchedule.ChileProduct);
                    });
        }

        private static double GetAverage(this PackSchedule packSchedule, IAttributeNameKey attributeName)
        {
            return packSchedule.ProductionBatches.SelectMany(b => b.Production.ResultingChileLot.Lot.Attributes).Where(a => a.AttributeShortName == attributeName.AttributeNameKey_ShortName)
                .Select(a => a.AttributeValue)
                .DefaultIfEmpty(0.0)
                .Average();
        }
    }
}