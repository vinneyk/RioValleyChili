using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ProductionBatchExtensions
    {
        internal static ProductionBatch ConstrainByKeys(this ProductionBatch productionBatch, IPackScheduleKey packScheduleKey)
        {
            if(productionBatch == null) { throw new ArgumentNullException("productionBatch"); }
            if(packScheduleKey == null) { throw new ArgumentNullException("packScheduleKey"); }

            productionBatch.PackSchedule = null;
            productionBatch.PackScheduleDateCreated = packScheduleKey.PackScheduleKey_DateCreated;
            productionBatch.PackScheduleSequence = packScheduleKey.PackScheduleKey_DateSequence;

            return productionBatch;
        }

        internal static ProductionBatch SetOutputLot(this ProductionBatch productionBatch, ILotKey lotKey)
        {
            if(productionBatch == null) { throw new ArgumentNullException("productionBatch"); }
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            productionBatch.Production.ResultingChileLot = null;
            productionBatch.LotDateCreated = lotKey.LotKey_DateCreated;
            productionBatch.LotDateSequence = lotKey.LotKey_DateSequence;
            productionBatch.LotTypeId = lotKey.LotKey_LotTypeId;

            return productionBatch;
        }

        internal static ProductionBatch EmptyItems(this ProductionBatch productionBatch)
        {
            if(productionBatch == null) { throw new ArgumentNullException("productionBatch"); }

            if(productionBatch.Production.PickedInventory != null)
            {
                productionBatch.Production.PickedInventory.EmptyItems();
            }

            if(productionBatch.Production.ResultingChileLot != null)
            {
                productionBatch.Production.ResultingChileLot.Lot.EmptyLot();
            }

            return productionBatch;
        }

        internal static ProductionBatch SetToNotCompleted(this ProductionBatch productionBatch)
        {
            if(productionBatch == null) { throw new ArgumentNullException("productionBatch"); }

            productionBatch.ProductionHasBeenCompleted = false;
            productionBatch.Production.Results = null;

            return productionBatch;
        }

        internal static void AssertEqual(this ProductionBatch productionBatch, IProductionResultSummaryReturn summaryReturn)
        {
            if(productionBatch == null) { throw new ArgumentNullException("productionBatch"); }
            if(summaryReturn == null) { throw new ArgumentNullException("summaryReturn"); }

            var productionResult = productionBatch.Production.Results;
            var lotKeyValue = new LotKey(productionResult).KeyValue;
            Assert.AreEqual(lotKeyValue, summaryReturn.ProductionResultKey);
            Assert.AreEqual(lotKeyValue, summaryReturn.ProductionBatchKey);
            Assert.AreEqual(lotKeyValue, summaryReturn.OutputLotNumber);

            Assert.AreEqual(new LocationKey(productionResult).KeyValue, summaryReturn.ProductionLineKey);
            Assert.AreEqual(new ChileProductKey(productionResult.Production.ResultingChileLot).KeyValue, summaryReturn.ChileProductKey);
            Assert.AreEqual(productionResult.Production.ResultingChileLot.ChileProduct.Product.Name, summaryReturn.ChileProductName);

            Assert.AreEqual(productionBatch.TargetParameters.BatchTargetWeight, summaryReturn.TargetBatchWeight);
            Assert.AreEqual(productionResult.ShiftKey, summaryReturn.ProductionShiftKey);
            Assert.AreEqual(productionBatch.PackSchedule.WorkType.Description, summaryReturn.WorkType);
            if(productionBatch.ProductionHasBeenCompleted)
            {
                Assert.AreEqual("Completed", summaryReturn.BatchStatus);
            }
            else
            {
                Assert.AreEqual("NotCompleted", summaryReturn.BatchStatus);
            }
        }
    }
}