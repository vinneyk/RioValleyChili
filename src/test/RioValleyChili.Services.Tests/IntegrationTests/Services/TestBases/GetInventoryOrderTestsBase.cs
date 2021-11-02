using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using Solutionhead.Services;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases
{
    [TestFixture]
    public abstract class GetInventoryOrderTestsBase<TService, TParentRecord, TResult> : ServiceIntegrationTestBase<TService> where TService : class
    {
        protected abstract Func<TParentRecord, IResult<TResult>> MethodUnderTest { get; }
        protected abstract Func<TParentRecord> CreateParentRecord { get; }
        protected abstract Func<TParentRecord, PickedInventory> GetPickedInventoryRecordFromParent { get; }
        protected abstract Func<TResult, List<IPickedInventoryItemReturn>> GetPickedInventoryItemsFromResult { get; }
            
        [Test]
        public void Returns_PickedInventoryItems_with_AstaCalc_properties_as_expected_on_success()
        {
            //Arrange
            StartStopwatch();

            const double originalAsta = 120.0;
            var now = DateTime.UtcNow;
            var productionEnd = now.AddDays(-200);
            var expectedAstaCalc = AstaCalculator.CalculateAsta(originalAsta, productionEnd, productionEnd, now);
            var asta = StaticAttributeNames.Asta;

            var lot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot());
            var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot());
            var astaChileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Production.Results.EmptyItems().ProductionEnd = productionEnd);
            TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(astaChileLot, asta, originalAsta).AttributeDate = productionEnd);
            
            var parentRecord = CreateParentRecord();
            var pickedInventory = GetPickedInventoryRecordFromParent(parentRecord);
            var picked0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(pickedInventory, lot));
            var picked1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(pickedInventory, chileLot));
            var picked2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(pickedInventory, astaChileLot));

            TestHelper.ResetContext();
            StopWatchAndWriteTime("Arrange");

            //Act
            var result = TimedExecution(() => MethodUnderTest(parentRecord));

            //Assert
            result.AssertSuccess();
            var items = GetPickedInventoryItemsFromResult(result.ResultingObject);
            var pickedKey0 = new PickedInventoryItemKey(picked0);
            var pickedKey1 = new PickedInventoryItemKey(picked1);
            var pickedKey2 = new PickedInventoryItemKey(picked2);

            Assert.IsNull(items.Single(i => i.PickedInventoryItemKey == pickedKey0.KeyValue).AstaCalc);
            Assert.IsNull(items.Single(i => i.PickedInventoryItemKey == pickedKey1.KeyValue).AstaCalc);
            Assert.AreEqual(expectedAstaCalc, items.Single(i => i.PickedInventoryItemKey == pickedKey2.KeyValue).AstaCalc);
        }

        [Test]
        public void Returns_PickedInventoryItems_with_LoBac_properties_as_expected_on_success()
        {
            //Arrange
            StartStopwatch();

            var nullLoBacLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().NullDerivedLots());
            var notLoBacLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.AllAttributesAreLoBac = false, c => c.SetLotType().Lot.EmptyLot());
            var loBacLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.AllAttributesAreLoBac = true, c => c.SetLotType().Lot.EmptyLot());

            var parentRecord = CreateParentRecord();
            var pickedInventory = GetPickedInventoryRecordFromParent(parentRecord);
            var pickedKey0 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(pickedInventory, nullLoBacLot)));
            var pickedKey1 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(pickedInventory, notLoBacLot)));
            var pickedKey2 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(pickedInventory, loBacLot)));

            StopWatchAndWriteTime("Arrange");

            //Act
            var result = TimedExecution(() => MethodUnderTest(parentRecord));

            //Assert
            result.AssertSuccess();
            var items = GetPickedInventoryItemsFromResult(result.ResultingObject);
            Assert.IsNull(items.Single(i => i.PickedInventoryItemKey == pickedKey0.KeyValue).LoBac);
            Assert.IsFalse((bool) items.Single(i => i.PickedInventoryItemKey == pickedKey1.KeyValue).LoBac);
            Assert.IsTrue((bool) items.Single(i => i.PickedInventoryItemKey == pickedKey2.KeyValue).LoBac);
        }

        [Test]
        public void Returns_PickedInventoryItems_with_Lot_status_properties_as_expected_on_success()
        {
            //Arrange
            StartStopwatch();

            const LotQualityStatus lotStatus0 = LotQualityStatus.Contaminated;
            const bool onHold0 = false;
            const LotProductionStatus productionStatus0 = LotProductionStatus.Batched;

            const LotQualityStatus lotStatus1 = LotQualityStatus.Pending;
            const bool onHold1 = true;
            const LotProductionStatus status1 = LotProductionStatus.Produced;

            var lotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot(), l => l.QualityStatus = lotStatus0, l => l.ProductionStatus = productionStatus0, l => l.Hold = null));
            var lotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot(), l => l.QualityStatus = lotStatus1, l => l.ProductionStatus = status1, l => l.Hold = LotHoldType.HoldForAdditionalTesting));

            var parentRecord = CreateParentRecord();
            var pickedInventory = GetPickedInventoryRecordFromParent(parentRecord);
            var pickedKey0 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(pickedInventory, lotKey0)));
            var pickedKey1 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(pickedInventory, lotKey1)));

            StopWatchAndWriteTime("Arrange");

            //Act
            var result = TimedExecution(() => MethodUnderTest(parentRecord));

            //Assert
            result.AssertSuccess();
            var items = GetPickedInventoryItemsFromResult(result.ResultingObject);

            var inventory = items.Single(r => r.PickedInventoryItemKey == pickedKey0.KeyValue);
            Assert.AreEqual(lotStatus0, inventory.QualityStatus);
            Assert.AreEqual(onHold0, inventory.HoldType != null);
            Assert.AreEqual(productionStatus0, inventory.ProductionStatus);

            inventory = items.Single(r => r.PickedInventoryItemKey == pickedKey1.KeyValue);
            Assert.AreEqual(lotStatus1, inventory.QualityStatus);
            Assert.AreEqual(onHold1, inventory.HoldType != null);
            Assert.AreEqual(status1, inventory.ProductionStatus);
        }
    }
}