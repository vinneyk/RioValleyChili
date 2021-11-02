using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using Newtonsoft.Json;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class LotServiceTests : ServiceIntegrationTestBase<LotService>
    {
        [TestFixture]
        public class GetLotSummaries : LotServiceTests
        {
            protected override bool SetupStaticRecords { get { return false; } }

            [Test]
            public void Returns_AttributeNamesByProductType_as_expected()
            {
                //Arrange
                const string chileAttribute = "ChileA";
                const string additiveAttribute = "AdditiveA";
                const string packagingAttribute = "PackagingA";

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetValues(null, chileAttribute, true, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetValues(null, additiveAttribute, true, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetValues(null, packagingAttribute, true, false, false, true));

                //Act
                var result = TimedExecution(() => Service.GetLotSummaries());

                //Assert
                result.AssertSuccess();
                Assert.NotNull(result.ResultingObject.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Chile).Value.FirstOrDefault(a => a.Key == chileAttribute));
                Assert.NotNull(result.ResultingObject.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Additive).Value.FirstOrDefault(a => a.Key == additiveAttribute));
                Assert.NotNull(result.ResultingObject.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Packaging).Value.FirstOrDefault(a => a.Key == packagingAttribute));
            }

            [Test]
            public void Returns_all_Lot_keys()
            {
                //Arrange
                var lotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));
                var lotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));
                var lotKey2 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries();
                var lots = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.IsNotNull(lots.SingleOrDefault(l => l.LotKey == lotKey0.KeyValue));
                Assert.IsNotNull(lots.SingleOrDefault(l => l.LotKey == lotKey1.KeyValue));
                Assert.IsNotNull(lots.SingleOrDefault(l => l.LotKey == lotKey2.KeyValue));
            }

            [Test]
            public void Returns_only_Lots_of_specified_ProductType()
            {
                //Arrange
                const int expectedCount = 2;
                var chileLotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot().SetChileLot(), c => c.Production = null, c => c.ChileProduct.Product.ProductType = ProductTypeEnum.Chile));
                var chileLotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot().SetChileLot(), c => c.Production = null, c => c.ChileProduct.Product.ProductType = ProductTypeEnum.Chile));
                var unexpectedLotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>(a => a.Lot.EmptyLot().SetAdditiveLot(), a => a.AdditiveProduct.Product.ProductType = ProductTypeEnum.Additive));
                var unexpectedLotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingLot>(p => p.Lot.EmptyLot().SetPackagingLot(), p => p.PackagingProduct.Product.ProductType = ProductTypeEnum.Packaging));

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries(new FilterLotParameters
                    {
                        ProductType = ProductTypeEnum.Chile
                    });
                var lots = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedCount, lots.Count);
                Assert.IsNotNull(lots.SingleOrDefault(l => l.LotKey == chileLotKey0.KeyValue));
                Assert.IsNotNull(lots.SingleOrDefault(l => l.LotKey == chileLotKey1.KeyValue));
                Assert.IsNull(lots.SingleOrDefault(l => l.LotKey == unexpectedLotKey0.KeyValue));
                Assert.IsNull(lots.SingleOrDefault(l => l.LotKey == unexpectedLotKey1.KeyValue));
            }

            [Test]
            public void Results_will_be_filtered_by_AstaCalc_property_as_expected()
            {
                //Arrange
                const int minAsta = 100;
                var now = DateTime.UtcNow;
                var asta = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetKey(GlobalKeyHelpers.AstaAttributeNameKey).SetValues(null, "Asta", true));
                var expectedKeysAndAsta = new Dictionary<string, int>();

                var lot0ProductionEnd = now;
                var lotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Production.PickedInventory.EmptyItems(), c => c.Production.Results.ProductionEnd = lot0ProductionEnd));
                var asta0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey0, asta, 120).AttributeDate = lot0ProductionEnd);
                var astaCalc0 = AstaCalculator.CalculateAsta(asta0.AttributeValue, asta0.AttributeDate, lot0ProductionEnd, now);
                Assert.GreaterOrEqual(astaCalc0, minAsta);
                expectedKeysAndAsta.Add(lotKey0.KeyValue, astaCalc0);

                var lot1ProductionEnd = now.AddDays(-10);
                var lotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Production.PickedInventory.EmptyItems(), c => c.Production.Results.ProductionEnd = lot1ProductionEnd));
                var asta1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey1, asta, 110).AttributeDate = now.AddDays(-5));
                var astaCalc1 = AstaCalculator.CalculateAsta(asta1.AttributeValue, asta1.AttributeDate, lot1ProductionEnd, now);
                Assert.GreaterOrEqual(astaCalc1, minAsta);
                expectedKeysAndAsta.Add(lotKey1.KeyValue, astaCalc1);

                var lot2ProductionEnd = now.AddDays(-500);
                var lotKey2 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Production.PickedInventory.EmptyItems(), c => c.Production.Results.ProductionEnd = lot2ProductionEnd));
                var asta2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey2, asta, 120).AttributeDate = lot2ProductionEnd);
                var astaCalc2 = AstaCalculator.CalculateAsta(asta2.AttributeValue, asta2.AttributeDate, lot2ProductionEnd, now);
                Assert.Less(astaCalc2, minAsta);

                var lotKey3 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));
                var asta3 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey3, asta, 120).AttributeDate = now);
                var astaCalc3 = AstaCalculator.CalculateAsta(asta3.AttributeValue, asta3.AttributeDate, asta3.AttributeDate, now);
                Assert.GreaterOrEqual(astaCalc3, minAsta);
                expectedKeysAndAsta.Add(lotKey3.KeyValue, astaCalc3);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot());

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries();
                var lots = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.Where(l => l.AstaCalc > minAsta).ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();

                Assert.AreEqual(expectedKeysAndAsta.Count, lots.Count);
                expectedKeysAndAsta.ForEach(e => Assert.AreEqual(e.Value, lots.Single(l => l.LotKey == e.Key).AstaCalc));
            }

            [Test]
            public void Returns_only_Lots_of_specified_ProductionStatus()
            {
                //Arrange
                const int expectedLots = 2;
                const LotProductionStatus lotProductionStatus = LotProductionStatus.Produced;

                var expectedLotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.ProductionStatus = lotProductionStatus));
                var expectedLotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.ProductionStatus = lotProductionStatus));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.ProductionStatus = LotProductionStatus.Batched);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.ProductionStatus = LotProductionStatus.Batched);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.ProductionStatus = LotProductionStatus.Batched);

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries(new FilterLotParameters { ProductionStatus = lotProductionStatus });
                var results = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(results.Count, expectedLots);
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey0.KeyValue));
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey1.KeyValue));
            }

            [Test]
            public void Returns_only_Lots_of_specified_LotStatus()
            {
                //Arrange
                const int expectedLots = 2;
                const LotQualityStatus lotStatus = LotQualityStatus.Released;

                var expectedLotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.QualityStatus = lotStatus));
                var expectedLotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.QualityStatus = lotStatus));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.QualityStatus = LotQualityStatus.Pending);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.QualityStatus = LotQualityStatus.Contaminated);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.QualityStatus = LotQualityStatus.Rejected);

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries(new FilterLotParameters { QualityStatus = lotStatus });
                var results = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedLots, results.Count);
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey0.KeyValue));
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey1.KeyValue));
            }

            [Test]
            public void Returns_only_Lots_with_associated_ProductionResults_with_specified_ProductionStart_date_if_start_and_end_dates_are_equal()
            {
                //Arrange
                const int expectedLots = 2;
                var productionStart = new DateTime(2012, 3, 29);

                var expectedLotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionStart));
                var expectedLotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionStart));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionStart.AddDays(-1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionStart.AddDays(1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries(new FilterLotParameters { ProductionStartRangeStart = productionStart, ProductionStartRangeEnd = productionStart });
                var results = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(results.Count, expectedLots);
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey0.KeyValue));
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey1.KeyValue));
            }

            [Test]
            public void Returns_only_Lots_with_associated_ProductionResults_with_ProductionStart_date_in_the_specified_range()
            {
                //Arrange
                const int expectedLots = 3;
                var productionStart = new DateTime(2012, 3, 29);
                var productionEnd = new DateTime(2012, 4, 12);

                var expectedLotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionStart));
                var expectedLotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionStart.AddDays(5)));
                var expectedLotKey2 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionEnd));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionStart.AddDays(-1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results.ProductionBegin = productionEnd.AddDays(1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production.Results = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries(new FilterLotParameters { ProductionStartRangeStart = productionStart, ProductionStartRangeEnd = productionEnd});
                var results = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(results.Count, expectedLots);
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey0.KeyValue));
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey1.KeyValue));
                Assert.AreEqual(1, results.Count(l => l.LotKey == expectedLotKey2.KeyValue));
            }

            [Test]
            public void Returns_LotSummaries_with_expected_ValidLotQualityStatuses()
            {
                //Arrange
                var expectedStatuses0 = new List<LotQualityStatus>
                    {
                        LotQualityStatus.Rejected,
                        LotQualityStatus.Pending,
                        LotQualityStatus.Released,
                        LotQualityStatus.Contaminated
                    };
                var lotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot().SetProductSpec(true, true).QualityStatus = LotQualityStatus.Pending));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(lotKey0, DefectTypeEnum.ActionableDefect).Resolution = null);
                
                var expectedStatuses1 = new List<LotQualityStatus>
                    {
                        LotQualityStatus.Contaminated,
                        LotQualityStatus.Rejected
                    };
                var lotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot().SetProductSpec(true, true).QualityStatus = LotQualityStatus.Contaminated));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(lotKey1, DefectTypeEnum.BacterialContamination).Resolution = null);

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries();
                var lots = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();

                var lotSummary0 = lots.Single(l => l.LotKey == lotKey0);
                Assert.AreEqual(expectedStatuses0.Count, lotSummary0.ValidLotQualityStatuses.Count());
                expectedStatuses0.ForEach(s => Assert.IsNotNull(lotSummary0.ValidLotQualityStatuses.FirstOrDefault(r => r == s)));

                var lotSummary1 = lots.Single(l => l.LotKey == lotKey1);
                Assert.AreEqual(expectedStatuses1.Count, lotSummary1.ValidLotQualityStatuses.Count());
                expectedStatuses1.ForEach(s => Assert.IsNotNull(lotSummary1.ValidLotQualityStatuses.FirstOrDefault(r => r == s)));
            }

            [Test]
            public void Returns_LotSummaries_with_Notes_as_expected()
            {
                //Arrange
                var chileLot0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                var chileLot1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                var chileLot2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries();
                var lots = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(chileLot0.Lot.Notes, lots.Single(l => l.LotKey == new LotKey(chileLot0).KeyValue).Notes);
                Assert.AreEqual(chileLot1.Lot.Notes, lots.Single(l => l.LotKey == new LotKey(chileLot1).KeyValue).Notes);
                Assert.AreEqual(chileLot2.Lot.Notes, lots.Single(l => l.LotKey == new LotKey(chileLot2).KeyValue).Notes);
            }                                   

            [Test]
            public void Returns_expected_Lots_given_StartingLotKey()
            {
                const LotTypeEnum lotType = LotTypeEnum.WIP;
                var lotStartDate = new DateTime(2014, 3, 29);
                const int lotSequence = 20;

                var expectedLots = new List<Lot>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetLotKey(lotType, lotStartDate, lotSequence)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetLotKey(lotType, lotStartDate, lotSequence + 10)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetLotKey(lotType, lotStartDate.AddDays(10), lotSequence)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetLotKey(lotType, lotStartDate.AddDays(10), lotSequence + 10))
                    };

                var unexpectedLots = new List<Lot>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetLotKey(LotTypeEnum.FinishedGood, lotStartDate, lotSequence + 1)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetLotKey(lotType, lotStartDate.AddDays(-1), lotSequence)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetLotKey(lotType, lotStartDate.AddDays(-1), lotSequence - 10))
                    };

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries(new FilterLotParameters
                    {
                        StartingLotKey = new LotKey(expectedLots[0])
                    });
                var lots = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();

                Assert.AreEqual(expectedLots.Count, lots.Count);
                expectedLots.Select(l => new LotKey(l)).ForEach(k => Assert.AreEqual(1, lots.Count(r => r.LotKey == k.KeyValue)));
                unexpectedLots.Select(l => new LotKey(l)).ForEach(k => Assert.AreEqual(0, lots.Count(r => r.LotKey == k.KeyValue)));
            }

            [Test]
            public void Returns_Lots_in_expected_order()
            {
                //Arrange
                var expectedOrderedLots = new List<Lot>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot())
                    }.OrderBy(l => l.LotTypeId).ThenBy(l => l.LotDateCreated).ThenBy(l => l.LotDateSequence).ToList();

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries();
                var lots = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.IsTrue(expectedOrderedLots.Select((e, i) => e.ToLotKey().KeyValue == lots[i].LotKey).All(b => b));
            }

            [Test, Issue(References = new[] { "RVCADMIN-1169" })]
            public void Returns_Lots_with_allowances_as_expected()
            {
                //Arrange
                var lot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l =>
                    {
                        l.CustomerAllowances = TestHelper.List<LotCustomerAllowance>(1);
                        l.SalesOrderAllowances = TestHelper.List<LotSalesOrderAllowance>(1);
                        l.ContractAllowances = TestHelper.List<LotContractAllowance>(1);
                    });

                //Act
                var result = Service.GetLotSummaries();

                //Assert
                result.AssertSuccess();

                var lotReturn = result.ResultingObject.LotSummaries.Single();
                lot.CustomerAllowances.Single().AssertAreEqual(lotReturn.CustomerAllowances.Single());
                lot.SalesOrderAllowances.Single().AssertAreEqual(lotReturn.CustomerOrderAllowances.Single());
                lot.ContractAllowances.Single().AssertAreEqual(lotReturn.ContractAllowances.Single());
            }

            [Test, Issue("Test simulates controller-level filtering (following Inventory ProductSubType filtering) -RI 2016-12-27",
                References = new[] { "RVCADMIN-1442" })]
            public void Can_be_filtered_by_ProductSubType_as_expected()
            {
                //Arrange
                var subType = "GRP";
                var expectedOrderedLots = new List<Lot>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.ChileProduct.ChileType.Description = subType).Lot,
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.ChileProduct.ChileType.Description = subType).Lot,
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.ChileProduct.ChileType.Description = subType).Lot
                    }.OrderBy(l => l.LotTypeId).ThenBy(l => l.LotDateCreated).ThenBy(l => l.LotDateSequence).ToList();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.ChileProduct.ChileType.Description = "Paprika");
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();

                //Act
                StartStopwatch();
                var result = Service.GetLotSummaries();
                var lots = result.ResultingObject == null ? null : result.ResultingObject.LotSummaries.Where(l => l.LotProduct.ProductSubType == subType).ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.IsTrue(expectedOrderedLots.Select((e, i) => e.ToLotKey().KeyValue == lots[i].LotKey).All(b => b));
            }
        }

        [TestFixture]
        public class GetLotSummary : LotServiceTests
        {
            protected override bool SetupStaticRecords { get { return false; } }

            [Test]
            public void Returns_LotSummary_with_expected_AttributeNamesByProductType()
            {
                //Arrange
                const string chileAttribute = "ChileA";
                const string additiveAttribute = "AdditiveA";
                const string packagingAttribute = "PackagingA";

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey), a => a.AttributeName.SetValues(null, chileAttribute, true, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey), a => a.AttributeName.SetValues(null, additiveAttribute, true, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey), a => a.AttributeName.SetValues(null, packagingAttribute, true, false, false, true));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.NotNull(result.ResultingObject.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Chile).Value.FirstOrDefault(a => a.Key == chileAttribute));
                Assert.NotNull(result.ResultingObject.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Additive).Value.FirstOrDefault(a => a.Key == additiveAttribute));
                Assert.NotNull(result.ResultingObject.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Packaging).Value.FirstOrDefault(a => a.Key == packagingAttribute));
            }

            [Test]
            public void Returns_LotSummary_for_an_empty_lot_as_expected()
            {
                //Arrange
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(lotKey.KeyValue, result.ResultingObject.LotSummary.LotKey);
                Assert.IsEmpty(result.ResultingObject.LotSummary.Attributes);
                Assert.IsEmpty(result.ResultingObject.LotSummary.Defects);
            }

            [Test]
            public void Returns_LotSummary_with_expected_Product_information()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.SetChileLot().EmptyLot());
                var lotKey = new LotKey(chileLot);

                //Act
                var result = TimedExecution(() => Service.GetLotSummary(lotKey.KeyValue));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(chileLot.ChileProduct.Product.Name, result.ResultingObject.LotSummary.LotProduct.ProductName);
                Assert.AreEqual(new ProductKey((IChileProductKey)chileLot.ChileProduct).KeyValue, result.ResultingObject.LotSummary.LotProduct.ProductKey);
            }

            [Test]
            public void Returns_LotSummary_with_expected_LotAttributes()
            {
                //Arrange
                const double attributeValue0 = 0;
                const double attributeValue1 = 0.1;
                const double attributeValue2 = 0.2;

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));
                var lotAttribute0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey, null, attributeValue0));
                var lotAttribute1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey, null, attributeValue1));
                var lotAttribute2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey, null, attributeValue2));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(attributeValue0, result.ResultingObject.LotSummary.Attributes.Single(a => a.Name == lotAttribute0.AttributeName.Name).Value);
                Assert.AreEqual(attributeValue1, result.ResultingObject.LotSummary.Attributes.Single(a => a.Name == lotAttribute1.AttributeName.Name).Value);
                Assert.AreEqual(attributeValue2, result.ResultingObject.LotSummary.Attributes.Single(a => a.Name == lotAttribute2.AttributeName.Name).Value);
            }

            [Test]
            public void Returns_LotSummary_with_expected_LotDefects()
            {
                //Arrange
                const DefectTypeEnum attributeDefectType = DefectTypeEnum.BacterialContamination;
                const ResolutionTypeEnum attributeDefectResolution = ResolutionTypeEnum.Treated;
                const double attributeDefectValue = 123.0;
                const double attributeDefectMin = 0.0;
                const double attributeDefectMax = 10.0;

                const DefectTypeEnum defectType = DefectTypeEnum.InHouseContamination;

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));
                var lotDefectKey0 = new LotDefectKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(a => a.SetValues(lotKey, null, attributeDefectType, attributeDefectValue, attributeDefectMin, attributeDefectMax).LotDefect.Resolution.ResolutionType = attributeDefectResolution));
                var lotDefectKey1 = new LotDefectKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(lotKey, defectType).Resolution = null));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();

                var attributeDefect = result.ResultingObject.LotSummary.Defects.Single(d => d.LotDefectKey == lotDefectKey0.KeyValue);
                Assert.AreEqual(attributeDefectType, attributeDefect.DefectType);
                Assert.AreEqual(attributeDefectResolution, attributeDefect.Resolution.ResolutionType);
                Assert.AreEqual(attributeDefectValue, attributeDefect.AttributeDefect.OriginalValue);
                Assert.AreEqual(attributeDefectMin, attributeDefect.AttributeDefect.OriginalMinLimit);
                Assert.AreEqual(attributeDefectMax, attributeDefect.AttributeDefect.OriginalMaxLimit);

                var defect = result.ResultingObject.LotSummary.Defects.Single(d => d.LotDefectKey == lotDefectKey1.KeyValue);
                Assert.AreEqual(defectType, defect.DefectType);
                Assert.IsNull(defect.Resolution);
            }

            [Test]
            public void Returns_LotSummary_with_expected_ProductionStatus_and_LotStatus()
            {
                //Arrange
                const LotQualityStatus expectedLotStatus = LotQualityStatus.Contaminated;
                const LotProductionStatus expectedProductionStatus = LotProductionStatus.Produced;

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.ProductionStatus = expectedProductionStatus, l => l.QualityStatus = expectedLotStatus));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                Assert.AreEqual(expectedLotStatus, result.ResultingObject.LotSummary.QualityStatus);
                Assert.AreEqual(expectedProductionStatus, result.ResultingObject.LotSummary.ProductionStatus);
            }

            [Test]
            public void Returns_LotSummary_with_expected_Hold_properties()
            {
                //Arrange
                const string expectedDescription = "HOLDING";
                const LotHoldType expectedHoldType = LotHoldType.HoldForCustomer;

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.HoldDescription = expectedDescription, l => l.Hold = expectedHoldType));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                Assert.AreEqual(expectedDescription, result.ResultingObject.LotSummary.HoldDescription);
                Assert.AreEqual(expectedHoldType, result.ResultingObject.LotSummary.HoldType);
            }

            [Test]
            public void Returns_LotSummary_with_expected_LotStatus()
            {
                //Arrange
                const LotQualityStatus expected = LotQualityStatus.Pending;
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.QualityStatus = expected));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                Assert.AreEqual(expected, result.ResultingObject.LotSummary.QualityStatus);
            }

            [Test]
            public void If_Lot_has_an_attribute_with_a_name_of_Asta_will_return_AstaCalc_property_as_expected()
            {
                //Arrange
                const double originalAsta = 120.0;
                var now = DateTime.UtcNow;
                var productionEnd = now.AddDays(-200);
                var expectedAstaCalc = AstaCalculator.CalculateAsta(originalAsta, productionEnd, productionEnd, now);

                var asta = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetKey(GlobalKeyHelpers.AstaAttributeNameKey).SetValues(null, "Asta", true));
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Production.PickedInventory.EmptyItems(), c => c.Production.Results.ProductionEnd = productionEnd));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey, asta, originalAsta).AttributeDate = productionEnd);

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedAstaCalc, result.ResultingObject.LotSummary.AstaCalc);
            }

            [Test]
            public void If_Lot_has_an_attribute_with_a_name_of_Asta_but_no_ProductionResults_then_AstaCalc_propety_will_be_as_expected()
            {
                //Arrange
                const double originalAsta = 120.0;
                var now = DateTime.UtcNow;
                var testDate = now.AddDays(-200);
                var expectedAstaCalc = AstaCalculator.CalculateAsta(originalAsta, testDate, testDate, now);

                var asta = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetKey(GlobalKeyHelpers.AstaAttributeNameKey).SetValues(null, "Asta", true));
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Production.PickedInventory.EmptyItems(), c=> c.Production.Results = null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey, asta, originalAsta).AttributeDate = testDate);

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedAstaCalc, result.ResultingObject.LotSummary.AstaCalc);
            }

            [Test]
            public void If_Lot_has_no_attribute_with_name_of_Asta_will_return_AstaCalc_property_as_null()
            {
                //Arrange
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(result.ResultingObject.LotSummary.AstaCalc);
            }

            [Test]
            public void If_Lot_is_not_of_ChileLot_type_then_LoBac_property_will_be_null()
            {
                //Arrange
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.SetAdditiveLot().Inventory = null));

                //Act
                var result = TimedExecution(() => Service.GetLotSummary(lotKey.KeyValue));

                //Assert
                result.AssertSuccess();
                Assert.IsNull(result.ResultingObject.LotSummary.LoBac);
            }

            [Test]
            public void If_Lot_is_of_ChileLot_type_then_LoBac_property_will_be_as_expected()
            {
                //Arrange
                const bool expectedLoBac = true;
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Inventory = null, c => c.AllAttributesAreLoBac = expectedLoBac));

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedLoBac, result.ResultingObject.LotSummary.LoBac);
            }

            [Test]
            public void Returns_LotSummary_with_expected_ValidLotQualityStatuses()
            {
                //Arrange
                var expectedStatuses = new List<LotQualityStatus>
                    {
                        LotQualityStatus.Rejected,
                        LotQualityStatus.Pending,
                        LotQualityStatus.Released,
                        LotQualityStatus.Contaminated
                    };
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot().QualityStatus = LotQualityStatus.Pending));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(lotKey, DefectTypeEnum.ActionableDefect).Resolution = null);

                //Act
                var result = Service.GetLotSummary(lotKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedStatuses.Count, result.ResultingObject.LotSummary.ValidLotQualityStatuses.Count());
                expectedStatuses.ForEach(s => Assert.IsNotNull(result.ResultingObject.LotSummary.ValidLotQualityStatuses.FirstOrDefault(r => r == s)));
            }

            [Test]
            public void Returns_LotSummary_with_Notes_as_expected_on_success()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();

                //Act
                var result = Service.GetLotSummary(new LotKey(chileLot));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(chileLot.Lot.Notes, result.ResultingObject.LotSummary.Notes);
            }

            [Test]
            public void Returns_LotSummary_with_CustomerName_as_expected()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var batch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.PackSchedule.SetCustomerKey(customer));

                //Act
                var result = Service.GetLotSummary(new LotKey(batch));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(customer.Company.Name, result.ResultingObject.LotSummary.Customer.Name);
            }
        }

        [TestFixture]
        public class SetLotAttributes : LotServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Lot_could_not_be_found()
            {
                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>()
                    });
                
                //Assert
                result.AssertNotSuccess(UserMessages.LotNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_Employee_could_not_be_found()
            {
                //Arrange
                var dateTested = new DateTime(2013, 3, 29);
                const double expectedValue0 = 10.0f;
                const double expectedValue1 = 20.0f;

                var attributeNameKey0 = new AttributeNameKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>());
                var attributeNameKey1 = new AttributeNameKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>());

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot()));
                

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = "GI JOOooeee!",
                        LotKey = lotKey.KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey0.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = expectedValue0, Date = dateTested } } },
                                { attributeNameKey1.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = expectedValue1, Date = dateTested } } },
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.EmployeeByTokenNotFound);
            }

            [Test]
            public void Creates_new_LotAttributes_on_success()
            {
                //Arrange
                var dateTested = new DateTime(2013, 3, 29);
                const double expectedValue0 = 10.0f;
                const double expectedValue1 = 20.0f;

                var attributeNameKey0 = new AttributeNameKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>());
                var attributeNameKey1 = new AttributeNameKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>());

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot()));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey.KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey0.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = expectedValue0, Date = dateTested } } },
                                { attributeNameKey1.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = expectedValue1, Date = dateTested } } },
                            }
                    });

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.Attributes.Select(a => a.AttributeName));
                var attributes = lot.Attributes.ToList();

                var attribute = attributes.Single(a => attributeNameKey0.FindByPredicate.Invoke(a.AttributeName));
                Assert.AreEqual(expectedValue0, attribute.AttributeValue);
                Assert.AreEqual(dateTested, attribute.AttributeDate);
                Assert.False(attribute.Computed);

                attribute = attributes.Single(a => attributeNameKey1.FindByPredicate.Invoke(a.AttributeName));
                Assert.AreEqual(expectedValue1, attribute.AttributeValue);
                Assert.AreEqual(dateTested, attribute.AttributeDate);
                Assert.False(attribute.Computed);
            }

            [Test]
            public void Updates_values_of_existing_LotAttributes_on_success()
            {
                //Arrange
                var originalDateTested = new DateTime(2012, 12, 1);
                var dateTested = new DateTime(2013, 3, 29);
                const double originalValue0 = 10;
                const double originalValue1 = 20;
                const double newValue1 = 22.1;

                var attributeNameKey0 = new AttributeNameKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>());
                var attributeNameKey1 = new AttributeNameKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>());

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Lot.Attributes = new List<LotAttribute>
                    {
                        TestHelper.CreateObjectGraph<LotAttribute>(l => l.SetValues(c, attributeNameKey0, originalValue0).Computed = true, l => l.AttributeDate = originalDateTested),
                        TestHelper.CreateObjectGraph<LotAttribute>(l => l.SetValues(c, attributeNameKey1, originalValue1).Computed = true, l => l.AttributeDate = originalDateTested)
                    }));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey.KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey0.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = originalValue0, Date = originalDateTested } } },
                                { attributeNameKey1.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = newValue1, Date = dateTested } } },
                            }
                });

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.Attributes.Select(a => a.AttributeName));
                var attributes = lot.Attributes.ToList();

                var attribute = attributes.Single(a => attributeNameKey0.FindByPredicate.Invoke(a.AttributeName));
                Assert.AreEqual(originalValue0, attribute.AttributeValue);
                Assert.AreEqual(originalDateTested, attribute.AttributeDate);
                Assert.IsTrue(attribute.Computed);

                attribute = attributes.Single(a => attributeNameKey1.FindByPredicate.Invoke(a.AttributeName));
                Assert.AreEqual(newValue1, attribute.AttributeValue);
                Assert.AreEqual(dateTested, attribute.AttributeDate);
                Assert.False(attribute.Computed);
            }
            
            [Test]
            public void Returns_non_successful_result_if_not_all_existing_LotAttributes_are_accounted_for()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Inventory = null, c => c.Lot.LotDefects = null,
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.Lot = null)
                        });
                var lotKey = new LotKey(chileLot);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey.KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.NotAllLotAttributesAccountedFor);
            }

            [Test]
            public void Returns_non_successful_result_if_passing_null_value_without_resolution_for_an_attribute_with_a_defect()
            {
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Inventory = null, c => c.Lot.LotDefects = null, c => c.Lot.Attributes = null);
                var attribute = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attribute, DefectTypeEnum.ProductSpec).LotDefect.Resolution = null);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attribute.ToAttributeNameKey(), new AttributeValueParameters() }
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.AttributeDefectRequiresResolution);
            }

            [Test]
            public void Will_create_a_LotAttributeDefect_if_a_LotAttribute_value_violates_a_ChileProductRange()
            {
                //Arrange
                const string attributeName = "badStuff";
                const double min = 0.0f;
                const double max = 10.0f;
                const double attributeValue = 20.0f;
                const DefectTypeEnum expectedDefectType = DefectTypeEnum.BacterialContamination;

                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(
                    r => r.SetValues(min, max, attributeName), r => r.AttributeName.DefectType = expectedDefectType);
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(attributeRange.ChileProduct), c => c.Lot.EmptyLot()));
                var attributeNameKey = attributeRange.ToAttributeNameKey();

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey.KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = attributeValue, Date = DateTime.UtcNow } } }
                            }
                    });

                //Assert
                result.AssertSuccess();

                var lotDefect = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.LotDefects).LotDefects.Single();
                Assert.AreEqual(expectedDefectType, lotDefect.DefectType);
                Assert.IsNull(lotDefect.Resolution);

                var lotAttributeDefectKey = new LotAttributeDefectKey(lotDefect, attributeNameKey);
                var lotAttributeDefect = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(lotAttributeDefectKey);
                Assert.AreEqual(min, lotAttributeDefect.OriginalAttributeMinLimit);
                Assert.AreEqual(max, lotAttributeDefect.OriginalAttributeMaxLimit);
                Assert.AreEqual(attributeValue, lotAttributeDefect.OriginalAttributeValue);
            }

            [Test]
            public void Updates_existing_unresolved_LotAttributeDefect_record_with_new_values_if_they_remain_out_of_range()
            {
                //Arrange
                const double rangeMin = 0.0;
                const double rangeMax = 1.0;
                const double currentValue = 2.0;
                const double newValue = 3.0;

                var productRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.SetValues(null, rangeMin, rangeMax));
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(productRange.ChileProduct));
                var lotAttributeDefectKey = new LotAttributeDefectKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, productRange.AttributeName, productRange.AttributeName.DefectType, currentValue, rangeMin, rangeMax).LotDefect.Resolution = null));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(productRange.AttributeName).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = newValue, Date = DateTime.UtcNow } } }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var lotAttributeDefect = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(lotAttributeDefectKey);
                Assert.AreEqual(newValue, lotAttributeDefect.OriginalAttributeValue);
                Assert.AreEqual(rangeMin, lotAttributeDefect.OriginalAttributeMinLimit);
                Assert.AreEqual(rangeMax, lotAttributeDefect.OriginalAttributeMaxLimit);
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_resolve_a_defect_without_providing_resolution_data()
            {
                //Arrange
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.SetValues(50.0, 55.0, "Asta").AttributeName = null);
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(attributeRange.ChileProduct));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot.Lot, attributeRange.AttributeName, DefectTypeEnum.ProductSpec, 49.0, 50.0).LotDefect.Resolution = null);

                var lotKey = new LotKey(chileLot.Lot);
                var attributeNameKey = new AttributeNameKey(attributeRange.AttributeName);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey.KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 52.5, Date = DateTime.UtcNow } } }
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.AttributeDefectRequiresResolution);
            }

            [Test]
            public void Returns_non_successful_result_if_attemping_to_resolve_defect_with_invalid_ResolutionType()
            {
                //Arrange
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(
                    r => r.SetValues(0.0, 0.1, "salmon"), r => r.AttributeName.DefectType = DefectTypeEnum.BacterialContamination);
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(attributeRange.ChileProduct));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot.Lot, attributeRange.AttributeName, DefectTypeEnum.BacterialContamination, 0.2, 0.1).LotDefect.Resolution = null);

                var lotKey = new LotKey(chileLot.Lot);
                var attributeNameKey = new AttributeNameKey(attributeRange.AttributeName);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey.KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0.0, Date = DateTime.UtcNow }, Resolution = new DefectResolutionParameters
                                    {
                                        ResolutionType = ResolutionTypeEnum.ReworkPerformed,
                                        Description = "Bears."
                                    }
                                } }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.InvalidDefectResolutionType);
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_resolve_a_BacterialContamination_defect_with_a_Resolution_of_Treated_and_not_all_Inventory_for_the_Lot_has_treatment_valid_for_that_defect()
            {
                //Arrange
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(
                    r => r.SetValues(0.0, 0.1, "salmon").AttributeName.DefectType = DefectTypeEnum.BacterialContamination,
                    r => r.AttributeName.ValidTreatments = null);
                var invalidTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = null);
                var validTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = null,
                    a => a.AttributesTreated = new List<InventoryTreatmentForAttribute>
                        {
                            TestHelper.CreateObjectGraph<InventoryTreatmentForAttribute>(f => f.ConstrainKeys(a, attributeRange.AttributeName))
                        });

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(attributeRange.ChileProduct));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attributeRange.AttributeName, DefectTypeEnum.BacterialContamination, 0.5, 0.0).LotDefect.Resolution = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, null, null, validTreatment));
                var expectedInvalidInventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, null, null, invalidTreatment)));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(chileLot).KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    new AttributeNameKey(attributeRange).KeyValue, new AttributeValueParameters
                                    {
                                        AttributeInfo = new AttributeInfoParameters
                                            {
                                                Value = 0.0,
                                                Date = DateTime.UtcNow
                                            },
                                        Resolution = new DefectResolutionParameters
                                            {
                                                Description = "Treated it oh so good.",
                                                ResolutionType = ResolutionTypeEnum.Treated
                                            }
                                    }
                                }
                            }
                    });

                //Assert
                result.AssertNotSuccess(string.Format(UserMessages.TreatmentOnInventoryDoesNotResolveDefect, "{0}", expectedInvalidInventoryKey.KeyValue, "{0}"));
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_resolve_a_BacterialContamination_as_Treated_and_there_is_an_unarchived_PickedInventory_record_for_that_lot_without_valid_treatment()
            {
                //Arrange
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(
                    r => r.SetValues(0.0, 0.1, "salmon").AttributeName.DefectType = DefectTypeEnum.BacterialContamination,
                    r => r.AttributeName.ValidTreatments = null);
                var invalidTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = null);
                var validTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = null,
                    a => a.AttributesTreated = new List<InventoryTreatmentForAttribute>
                        {
                            TestHelper.CreateObjectGraph<InventoryTreatmentForAttribute>(f => f.ConstrainKeys(a, attributeRange.AttributeName))
                        });

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(attributeRange.ChileProduct));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attributeRange.AttributeName, DefectTypeEnum.BacterialContamination, 0.5, 0.0).LotDefect.Resolution = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(null, chileLot, null, validTreatment).PickedInventory.Archived = false);
                var invalidPickedInventoryKey = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(null, chileLot, null, invalidTreatment).PickedInventory.Archived = false));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = new LotKey(chileLot).KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    new AttributeNameKey(attributeRange).KeyValue, new AttributeValueParameters
                                    {
                                        AttributeInfo = new AttributeInfoParameters
                                            {
                                                Value = 0.0,
                                                Date = DateTime.UtcNow
                                            },
                                        Resolution = new DefectResolutionParameters
                                            {
                                                Description = "Treated it oh so good.",
                                                ResolutionType = ResolutionTypeEnum.Treated
                                            }
                                    }
                                }
                            }
                });

                //Assert
                result.AssertNotSuccess(string.Format(UserMessages.TreatmentOnUnarchivedPickedInventoryDoesNotResolveDefect, "{0}", invalidPickedInventoryKey.KeyValue, "{0}"));
            }

            [Test]
            public void Returns_successful_result_if_attempting_to_resolve_a_BacterialContamination_as_Treated_and_there_is_an_archived_PickedInventory_record_for_that_lot_without_valid_treatment()
            {
                //Arrange
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(
                    r => r.SetValues(0.0, 0.1, "salmon").AttributeName.DefectType = DefectTypeEnum.BacterialContamination,
                    r => r.AttributeName.ValidTreatments = null);
                var invalidTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = null);
                var validTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = null,
                    a => a.AttributesTreated = new List<InventoryTreatmentForAttribute>
                        {
                            TestHelper.CreateObjectGraph<InventoryTreatmentForAttribute>(f => f.ConstrainKeys(a, attributeRange.AttributeName))
                        });

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(attributeRange.ChileProduct));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attributeRange.AttributeName, DefectTypeEnum.BacterialContamination, 0.5, 0.0).LotDefect.Resolution = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(null, chileLot, null, validTreatment).PickedInventory.Archived = false);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(null, chileLot, null, invalidTreatment).PickedInventory.Archived = true);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = new LotKey(chileLot).KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    new AttributeNameKey(attributeRange).KeyValue, new AttributeValueParameters
                                    {
                                        AttributeInfo = new AttributeInfoParameters
                                            {
                                                Value = 0.0,
                                                Date = DateTime.UtcNow
                                            },
                                        Resolution = new DefectResolutionParameters
                                            {
                                                Description = "Treated it oh so good.",
                                                ResolutionType = ResolutionTypeEnum.Treated
                                            }
                                    }
                                }
                            }
                });

                //Assert
                result.AssertSuccess();
            }

            [Test]
            public void Returns_successful_result_and_creates_expected_LotDefectResolution_if_attempting_modify_Attribute_with_Treated_Resolution_and_all_Inventory_has_valid_Treatment_for_that_Attribute()
            {
                //Arrange
                const string expectedResolutionDescription = "Eskimos.";
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(
                    r => r.SetValues(0.0, 0.1, "salmon").AttributeName.DefectType = DefectTypeEnum.BacterialContamination,
                    r => r.AttributeName.ValidTreatments = null);
                var validTreatment0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = null,
                    a => a.AttributesTreated = new List<InventoryTreatmentForAttribute> { TestHelper.CreateObjectGraph<InventoryTreatmentForAttribute>(f => f.ConstrainKeys(a, attributeRange.AttributeName)) });
                var validTreatment1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = null,
                    a => a.AttributesTreated = new List<InventoryTreatmentForAttribute> { TestHelper.CreateObjectGraph<InventoryTreatmentForAttribute>(f => f.ConstrainKeys(a, attributeRange.AttributeName)) });

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(attributeRange.ChileProduct));
                var attributeDefect = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attributeRange.AttributeName, DefectTypeEnum.BacterialContamination, 0.5, 0.0).LotDefect.Resolution = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, treatment: validTreatment0));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, treatment: validTreatment1));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = new LotKey(chileLot).KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    new AttributeNameKey(attributeRange).KeyValue, new AttributeValueParameters
                                    {
                                        AttributeInfo = new AttributeInfoParameters
                                            {
                                                Value = 0.0,
                                                Date = DateTime.UtcNow
                                            },
                                        Resolution = new DefectResolutionParameters
                                            {
                                                Description = expectedResolutionDescription,
                                                ResolutionType = ResolutionTypeEnum.Treated
                                            }
                                    }
                                }
                            }
                });

                //Assert
                result.AssertSuccess();

                var resolution = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(new LotAttributeDefectKey(attributeDefect), d => d.LotDefect.Resolution).LotDefect.Resolution;
                Assert.AreEqual(expectedResolutionDescription, resolution.Description);
                Assert.AreEqual(ResolutionTypeEnum.Treated, resolution.ResolutionType);
            }

            [Test, Issue("Modified to accept null Description on LotDefectorResolutions. Implemeneted by coallescing null values with blank strings to prevent need to create new migration" +
                         "(to remove 'Required' attribute in the data model). -RI 2016-12-05",
                         References = new [] { "RVCADMIN-1410" })]
            public void Returns_successful_result_and_creates_expected_LotDefectResolution_if_Description_is_null()
            {
                //Arrange
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.SetValues(0.0, 0.1, "salmon").AttributeName.DefectType = DefectTypeEnum.BacterialContamination);
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(attributeRange.ChileProduct));
                var attributeDefect = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attributeRange.AttributeName, DefectTypeEnum.BacterialContamination, 0.5, 0.0).LotDefect.Resolution = null);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    attributeRange.ToAttributeNameKey(), new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters
                                                {
                                                    Value = 0.0,
                                                    Date = DateTime.UtcNow
                                                },
                                            Resolution = new DefectResolutionParameters
                                                {
                                                    Description = null,
                                                    ResolutionType = ResolutionTypeEnum.InvalidValue
                                                }
                                        }
                                }
                            }
                    });

                //Assert
                result.AssertSuccess();

                var resolution = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(attributeDefect.ToLotAttributeDefectKey(), d => d.LotDefect.Resolution).LotDefect.Resolution;
                Assert.AreEqual("", resolution.Description);
            }

            [Test]
            public void Returns_successful_result_when_resolving_a_ProductRange_defect_with_a_Reworked_resolution_and_creates_expected_record()
            {
                //Arrange
                const string attributeName = "Asta";
                const double rangeMin = 50.0;
                const double rangeMax = 60.0;
                const double currentValue = 70.0;
                const double newValue = 55.5;

                var productRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.SetValues(rangeMin, rangeMax, attributeName).AttributeName = null);
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.Lot.EmptyLot(), c => c.SetProduct(productRange.ChileProduct));
                var attributeDefectKey = new LotAttributeDefectKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, productRange.AttributeName, productRange.AttributeName.DefectType, currentValue, rangeMax).LotDefect.Resolution = null));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(chileLot).KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    new AttributeNameKey(productRange.AttributeName).KeyValue, new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters
                                                {
                                                    Value = newValue,
                                                    Date = DateTime.UtcNow
                                                },
                                            Resolution = new DefectResolutionParameters
                                                {
                                                    ResolutionType = ResolutionTypeEnum.ReworkPerformed,
                                                    Description = "Did some stuff to it"
                                                }
                                        }
                                }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var attributeDefect = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(attributeDefectKey, d => d.LotDefect.Resolution);
                var attribute = RVCUnitOfWork.LotAttributeRepository.Filter(a => a.AttributeShortName == attributeDefectKey.AttributeNameKey_ShortName).FirstOrDefault();
                Assert.AreEqual(ResolutionTypeEnum.ReworkPerformed, attributeDefect.LotDefect.Resolution.ResolutionType);
                Assert.AreEqual(newValue, attribute.AttributeValue);
            }

            [Test]
            public void Removes_LotAttribute_record_if_passing_null_value_without_resolution_for_an_attribute_without_an_open_defect()
            {
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Inventory = null, c => c.Lot.LotDefects = null, c => c.Lot.Attributes = null);
                var lotKey = new LotKey(chileLot);
                var attribute = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot));
                var attributeKey = new LotAttributeKey(attribute);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attribute, DefectTypeEnum.ProductSpec));

                

                //Act
                
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(attribute), new AttributeValueParameters() }
                            }
                });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.LotAttributeRepository.FindByKey(attributeKey));
            }

            [Test]
            public void Does_not_remove_existing_LotAttributeDefect_and_Resolution_records_if_passing_null_value_for_LotAttribute()
            {
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Inventory = null, c => c.Lot.LotDefects = null, c => c.Lot.Attributes = null);
                var lotKey = new LotKey(chileLot);
                var attribute = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot));
                var attributeDefectKey = new LotAttributeDefectKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attribute, DefectTypeEnum.ProductSpec)));

                

                //Act

                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(attribute), new AttributeValueParameters() }
                            }
                });

                //Assert
                result.AssertSuccess();
                var attributeDefect = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(attributeDefectKey, a => a.LotDefect.Resolution);
                Assert.IsNotNull(attributeDefect);
                Assert.IsNotNull(attributeDefect.LotDefect);
                Assert.IsNotNull(attributeDefect.LotDefect.Resolution);
            }

            [Test]
            public void Removes_LotAttribute_record_and_creates_Resolution_when_passing_null_value_for_LotAttribute_with_open_defect()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Inventory = null, c => c.Lot.LotDefects = null, c => c.Lot.Attributes = null);
                var lotKey = chileLot.ToLotKey();
                var attribute = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot));
                var attributeKey = attribute.ToLotAttributeKey();
                var attributeDefectKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attribute, DefectTypeEnum.ProductSpec).LotDefect.Resolution = null).ToLotAttributeDefectKey();

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                                {
                                    {
                                        new AttributeNameKey(attribute), new AttributeValueParameters
                                        {
                                            Resolution = new DefectResolutionParameters
                                                {
                                                    ResolutionType = ResolutionTypeEnum.DataEntryCorrection,
                                                    Description = "spppoooooo!"
                                                }
                                        }
                                    }
                                }
                    });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.LotAttributeRepository.FindByKey(attributeKey));
                Assert.IsNotNull(RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(attributeDefectKey, a => a.LotDefect.Resolution).LotDefect.Resolution);
            }

            [Test]
            public void Will_successfuly_update_Lot_status_to_Contaminated_if_any_LotDefects_of_Contaminated_type_are_created_without_a_resolution()
            {
                //Arrange
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.SetValues(0.0, 1.0, "badstuff").AttributeName.DefectType = DefectTypeEnum.BacterialContamination);
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.SetProduct(attributeRange.ChileProduct), c => c.Lot.QualityStatus = LotQualityStatus.Pending));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey.KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(attributeRange.AttributeName).KeyValue, new AttributeValueParameters
                                    {
                                        AttributeInfo = new AttributeInfoParameters
                                            {
                                                Value = 2.0,
                                                Date = DateTime.UtcNow
                                            }
                                    }
                                }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey);
                Assert.AreEqual(LotQualityStatus.Contaminated, lot.QualityStatus);
            }

            [Test]
            public void Will_set_ChileLot_LoBac_flag_to_true_if_all_attributes_are_set_within_their_LoBac_limit()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.NullProduction(), c => c.ChileProduct.ProductAttributeRanges = null, c => c.AllAttributesAreLoBac = false);
                var lotKey = new LotKey(chileLot);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, StaticAttributeNames.TPC, 500001));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey.KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    new AttributeNameKey(StaticAttributeNames.TPC).KeyValue, new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters
                                                {
                                                    Value = 50000,
                                                    Date = DateTime.UtcNow
                                                },
                                            Resolution = new DefectResolutionParameters { ResolutionType = ResolutionTypeEnum.DataEntryCorrection }
                                        }
                                },
                                { new AttributeNameKey(StaticAttributeNames.Yeast).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 100, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.Mold).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 100, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.ColiForms).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 10, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.EColi).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 3, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.Salmonella).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0, Date = DateTime.UtcNow } } }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.IsTrue(RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey).AllAttributesAreLoBac);
            }

            [Test]
            public void Will_set_ChileLot_LoBac_flag_to_true_if_Scan_is_set_less_than_or_equal_to_30()
            {
                //Arrange
                var lotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.NullProduction(), c => c.ChileProduct.ProductAttributeRanges = null, c => c.AllAttributesAreLoBac = false));
                var lotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.NullProduction(), c => c.ChileProduct.ProductAttributeRanges = null, c => c.AllAttributesAreLoBac = false));

                //Act
                var result0 = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey0.KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(StaticAttributeNames.Scan).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 30, Date = DateTime.UtcNow } } }
                            }
                });

                var result1 = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey1.KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(StaticAttributeNames.Scan).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 25, Date = DateTime.UtcNow } } }
                            }
                });

                //Assert
                result0.AssertSuccess();
                Assert.IsTrue(RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey0).AllAttributesAreLoBac);

                result1.AssertSuccess();
                Assert.IsTrue(RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey1).AllAttributesAreLoBac);
            }

            [Test]
            public void Will_set_ChileLot_LoBac_flag_to_false_if_removing_an_attribute_with_a_LoBac_limit()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.NullProduction(), c => c.ChileProduct.ProductAttributeRanges = null, c => c.AllAttributesAreLoBac = true);
                var lotKey = new LotKey(chileLot);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, StaticAttributeNames.TPC, 500000));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, StaticAttributeNames.Yeast, 100));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, StaticAttributeNames.Mold, 100));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, StaticAttributeNames.ColiForms, 10));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, StaticAttributeNames.EColi, 3));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, StaticAttributeNames.Salmonella, 0));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey.KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                        {
                            { new AttributeNameKey(StaticAttributeNames.TPC).KeyValue, new AttributeValueParameters
                                {
                                    Resolution = new DefectResolutionParameters { ResolutionType = ResolutionTypeEnum.DataEntryCorrection }
                                }
                            },
                            { new AttributeNameKey(StaticAttributeNames.Yeast).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 100, Date = DateTime.UtcNow } } },
                            { new AttributeNameKey(StaticAttributeNames.Mold).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 100, Date = DateTime.UtcNow } } },
                            { new AttributeNameKey(StaticAttributeNames.ColiForms).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 10, Date = DateTime.UtcNow } } },
                            { new AttributeNameKey(StaticAttributeNames.EColi).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 3, Date = DateTime.UtcNow } } },
                            { new AttributeNameKey(StaticAttributeNames.Salmonella).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0, Date = DateTime.UtcNow } } }
                        }
                });

                //Assert
                result.AssertSuccess();
                Assert.IsFalse(RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey).AllAttributesAreLoBac);
            }

            [Test]
            public void Will_set_ChileLot_LoBac_flag_to_false_if_not_all_attributes_are_set_within_their_LoBac_limit()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.NullProduction(), c => c.ChileProduct.ProductAttributeRanges = null, c => c.AllAttributesAreLoBac = true);
                var lotKey = new LotKey(chileLot);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey.KeyValue,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(StaticAttributeNames.TPC).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 50001, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.Yeast).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 100, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.Mold).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 100, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.ColiForms).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 10, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.EColi).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 3, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(StaticAttributeNames.Salmonella).KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0, Date = DateTime.UtcNow } } }
                            }
                });

                //Assert
                result.AssertSuccess();
                Assert.IsFalse(RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey).AllAttributesAreLoBac);
            }

            [Test]
            public void Will_set_LotProductSpecStatus_to_Incomplete_if_LotAttributes_are_missing()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 1.0));
                var productSpec = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 2.0));
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).NullProduction().Lot.EmptyLot()));

                

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(productSpec.AttributeName), new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 1.0, Date = DateTime.UtcNow } } }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.False(RVCUnitOfWork.LotRepository.FindByKey(lotKey).ProductSpecComplete);
            }

            [Test]
            public void Will_set_ProductSpecOutOfRange_to_true_if_any_LotAttribute_is_out_of_range()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 1.0));
                var productSpec = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 2.0));
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).NullProduction().Lot.EmptyLot()));

                

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(productSpec.AttributeName), new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 3.0, Date = DateTime.UtcNow } } }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.True(RVCUnitOfWork.LotRepository.FindByKey(lotKey).ProductSpecOutOfRange);
            }

            [Test]
            public void Will_set_ProductSpecComplete_to_true_and_ProductSpecOutOfRange_to_false_if_all_LotAttributes_are_entered_in_range()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct());
                var productSpec0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 1.0));
                var productSpec1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 2.0));
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).NullProduction().Lot.EmptyLot()));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { new AttributeNameKey(productSpec0.AttributeName), new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0.5, Date = DateTime.UtcNow } } },
                                { new AttributeNameKey(productSpec1.AttributeName), new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 1.0, Date = DateTime.UtcNow } } }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey);
                Assert.True(lot.ProductSpecComplete);
                Assert.False(lot.ProductSpecOutOfRange);
            }

            [Test, Issue("Previously we were automatically resvoling actionable defects when accepting a lot." +
                         "Defects and resolutions should now be informational so we see more benefit in only being" +
                         "able to resolve defects if attribute values are actually placed in range. -RI 2016-06-22" +
                         "Spec has changed to now resolve _all_ defects if override flag is set. -RI 2016-08-11",
                         References = new[] { "RVCADMIN-1166", "RVCADMIN-1223" })]
            public void If_OverrideOldContextLotAsCompleted_then_Lot_will_be_Accepted_and_actionable_defects_be_resolved()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct());
                var productSpec0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 1.0).AttributeName.DefectType = DefectTypeEnum.ActionableDefect);
                var lotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).NullProduction().Lot.EmptyLot().QualityStatus = LotQualityStatus.Pending).ToLotKey();

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                                {
                                    {
                                        new AttributeNameKey(productSpec0.AttributeName), new AttributeValueParameters
                                            {
                                                AttributeInfo = new AttributeInfoParameters
                                                    {
                                                        Value = 1.5,
                                                        Date = DateTime.UtcNow
                                                    }
                                            }
                                    }
                                },
                        OverrideOldContextLotAsCompleted = true
                    });

                //Assert
                result.AssertSuccess();

                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.LotDefects.Select(d => d.Resolution));
                Assert.AreEqual(LotQualityStatus.Released, lot.QualityStatus);

                var defect = lot.LotDefects.Single();
                Assert.AreEqual(DefectTypeEnum.ActionableDefect, defect.DefectType);
                Assert.IsNotNull(defect.Resolution);
            }

            [Test, Issue("Spec has changed to now resolve _all_ defects if override flag is set. -RI 2016-08-11",
                         References = new[] { "RVCADMIN-1223" })]
            public void Returns_non_successful_result_if_OverrideOldContextLotAsCompleted_and_new_attribute_creates_a_contamination_defect()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct());
                var productSpec0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 1.0).AttributeName.DefectType = DefectTypeEnum.BacterialContamination);
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetLotType().SetProduct(chileProduct).NullProduction().Lot.EmptyLot().SetChileLot().QualityStatus = LotQualityStatus.Pending);
                var lotKey = chileLot.ToLotKey();

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                                {
                                    {
                                        new AttributeNameKey(productSpec0.AttributeName), new AttributeValueParameters
                                            {
                                                AttributeInfo = new AttributeInfoParameters
                                                    {
                                                        Value = 1.5,
                                                        Date = DateTime.UtcNow
                                                    }
                                            }
                                        }
                                },
                        OverrideOldContextLotAsCompleted = true
                    });

                //Assert
                result.AssertSuccess();
                var defect = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.LotDefects.Select(d => d.Resolution)).LotDefects.Single();
                Assert.IsNotNull(defect.Resolution);
            }

            [Test, Issue("Previous implementation was such that resolving BacterialContamination defects would preserve a lot's Contaminated status if" +
            "the resulting determined status ended up being RequiresAttention (i.e. if an ActionableDefect remained unresolved)." +
            "We've changed the logic to set the lot status to RequiresAttention in that case. - RI 2016-06-14",
            References = new[] { "RVCADMIN-1150" })]
            public void Resolving_all_contamination_defects_will_change_Contamination_status()
            {
                //Arrange
                ChileProductAttributeRange contaminationRange = null;
                ChileProductAttributeRange actionableRange = null;
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(
                    c => c.ChileProduct.ProductAttributeRanges = TestHelper.List<ChileProductAttributeRange>(2,
                        r =>
                            {
                                (contaminationRange = r[0]).AttributeName.DefectType = DefectTypeEnum.BacterialContamination;
                                (actionableRange = r[1]).AttributeName.DefectType = DefectTypeEnum.ActionableDefect;
                            }),
                    c => c.Lot.Attributes = TestHelper.List<LotAttribute>(2,
                        a =>
                            {
                                a[0].SetValues(contaminationRange, contaminationRange.RangeMax * 2, false);
                                a[1].SetValues(actionableRange, contaminationRange.RangeMax * 2, false);
                            }),
                    c => c.Lot.QualityStatus = LotQualityStatus.Contaminated);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    contaminationRange.AttributeShortName, new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters { Value = (contaminationRange.RangeMin + contaminationRange.RangeMax) * 0.5, Date = DateTime.UtcNow },
                                            Resolution = new DefectResolutionParameters
                                                {
                                                    ResolutionType = ResolutionTypeEnum.Treated
                                                }
                                        }
                                },
                                {
                                    actionableRange.AttributeShortName, new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters { Value = actionableRange.RangeMax * 2, Date = DateTime.UtcNow },
                                        }
                                }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreNotEqual(LotQualityStatus.Contaminated, RVCUnitOfWork.LotRepository.FindByKey(chileLot.ToLotKey()).QualityStatus);
            }

            [Test, Issue("Other defect resolutions should require the value to actually change within range." +
            "InvalidValue resolution type is specifically for resolving a defect while preserving the out-of-range value" +
            "in cases where control knows the value will change but will not actually be tested for.",
            References = new [] { "RVCADMIN-1179"} )]
            public void Will_resolve_defect_without_change_in_value_if_resolution_type_is_InvalidValue()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct());
                var productSpec0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct).SetValues(null, 0.0, 1.0).AttributeName.DefectType = DefectTypeEnum.ActionableDefect);
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Inventory = null, c => c.Lot.LotDefects = null, c => c.Lot.Attributes = null);
                var lotKey = chileLot.ToLotKey();
                var attribute = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, productSpec0, 2.0f, false));
                var attributeDefectKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, attribute, DefectTypeEnum.ProductSpec).LotDefect.Resolution = null).ToLotAttributeDefectKey();

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                {
                    UserToken = TestUser.UserName,
                    LotKey = lotKey,
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                        {
                            {
                                attribute.ToAttributeNameKey(), new AttributeValueParameters
                                    {
                                        AttributeInfo = new AttributeInfoParameters
                                            {
                                                Value = attribute.AttributeValue,
                                                Date = attribute.AttributeDate
                                            },
                                        Resolution = new DefectResolutionParameters
                                            {
                                                ResolutionType = ResolutionTypeEnum.InvalidValue,
                                                Description = "don't care"
                                            }
                                    }
                            }
                        }
                });

                //Assert
                result.AssertSuccess();
                var defect = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(attributeDefectKey, d => d.LotDefect.Resolution);
                Assert.AreEqual(ResolutionTypeEnum.InvalidValue, defect.LotDefect.Resolution.ResolutionType);
            }

            [Test, Issue("Task mentioned waiting for client confirmation regarding the resolution of contamination defects." +
                         "Contamination defects _will_ be resolved by use of the flag, for now." +
                         "-RI 2016-8-10",
                References = new[] { "RVCADMIN-1223 "})]
            public void Will_resolve_all_defects_if_OverrideOldContextLotAsCompleted_is_set_to_true()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ProductAttributeRanges = TestHelper.List<ChileProductAttributeRange>(2,
                    l =>
                        {
                            l[0].SetValues(StaticAttributeNames.Salmonella, 0.0, 1.0);
                            l[1].SetValues(StaticAttributeNames.Asta, 0.0, 1.0);

                        }));
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.SetChileLot().ChileLot.SetProduct(chileProduct),
                    l => l.AttributeDefects = TestHelper.List<LotAttributeDefect>(1, d =>
                        {
                            d[0].SetValues(null, StaticAttributeNames.Asta, DefectTypeEnum.ProductSpec, 2.0f, 0.0f, 1.0f).LotDefect.Resolution = null;
                        }));

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        OverrideOldContextLotAsCompleted = true,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    StaticAttributeNames.Salmonella.ToAttributeNameKey(), new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters
                                                {
                                                    Value = 2.0,
                                                    Date = DateTime.UtcNow
                                                }
                                        }
                                }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var resultLot = RVCUnitOfWork.LotRepository.FindByKey(chileLot.ToLotKey(), l => l.LotDefects.Select(d => d.Resolution));
                var defects = resultLot.LotDefects.ToList();
                Assert.AreEqual(2, defects.Count);
                Assert.IsTrue(defects.All(d => d.Resolution.ResolutionType == ResolutionTypeEnum.AcceptedByUser));
            }

            [Test, Issue("Resolution needs to be created even if attribute is entered for the first time. -RI 2016-8-16",
                References = new[] { "RVCADMIN-1225"})]
            public void Will_create_a_LotAttributeDefect_with_resolution_if_provided()
            {
                //Arrange
                const string attributeName = "badStuff";
                const double min = 0.0f;
                const double max = 10.0f;
                const double attributeValue = 20.0f;
                const DefectTypeEnum expectedDefectType = DefectTypeEnum.ProductSpec;

                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(
                    r => r.SetValues(min, max, attributeName), r => r.AttributeName.DefectType = expectedDefectType);
                var lotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(attributeRange.ChileProduct), c => c.Lot.EmptyLot()).ToLotKey();
                var attributeNameKey = attributeRange.ToAttributeNameKey();

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey.KeyValue,
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                                {
                                    { attributeNameKey.KeyValue, new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters
                                                {
                                                    Value = attributeValue,
                                                    Date = DateTime.UtcNow
                                                },
                                            Resolution = new DefectResolutionParameters
                                                {
                                                    ResolutionType = ResolutionTypeEnum.InvalidValue,
                                                    Description = ":D"
                                                }
                                        }
                                    }
                                }
                    });

                //Assert
                result.AssertSuccess();

                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.LotDefects);
                var lotDefect = lot.LotDefects.Single();
                Assert.AreEqual(expectedDefectType, lotDefect.DefectType);
                Assert.IsNull(lotDefect.Resolution);

                var lotAttributeDefectKey = new LotAttributeDefectKey(lotDefect, attributeNameKey);
                var lotAttributeDefect = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(lotAttributeDefectKey, d => d.LotDefect.Resolution);
                Assert.AreEqual(min, lotAttributeDefect.OriginalAttributeMinLimit);
                Assert.AreEqual(max, lotAttributeDefect.OriginalAttributeMaxLimit);
                Assert.AreEqual(attributeValue, lotAttributeDefect.OriginalAttributeValue);
                Assert.AreEqual(ResolutionTypeEnum.InvalidValue, lotAttributeDefect.LotDefect.Resolution.ResolutionType);
            }

            [Test, Issue("Treatment on inventory and unarchived picked inventory must be able to resolve defect for attribute, otherwise method will fail. -RI 2016-8-29",
                References = new[]{ "RVCADMIN-1250" })]
            public void Will_resolve_defect_with_treatment_if_all_inventory_and_unarchived_picked_inventory_has_treatment_that_applies_to_attribute()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Yeast, 1, 2))
                    });

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Yeast, value: 3, computed: false))
                        },
                    c => c.Lot.AttributeDefects = new List<LotAttributeDefect>
                        {
                            TestHelper.CreateObjectGraph<LotAttributeDefect>(a => a.SetValues(attributeNameKey: StaticAttributeNames.Yeast, rangeMin: 1, rangeMax: 2, value: 3, defectType: DefectTypeEnum.BacterialContamination).LotDefect.Resolution = null)
                        });

                var treatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(t => t.AttributesTreated = new List<InventoryTreatmentForAttribute>
                    {
                        TestHelper.CreateObjectGraph<InventoryTreatmentForAttribute>(f => f.ConstrainKeys(attributeNameKey: StaticAttributeNames.Yeast))
                    });

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, treatment: treatment));

                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.SalesOrderPickedItems = new List<SalesOrderPickedItem>
                    {
                        TestHelper.CreateObjectGraph<SalesOrderPickedItem>(i => i.PickedInventoryItem.ConstrainByKeys(lotKey: chileLot, treatmentKey: treatment).Quantity = 10)
                    });
                customerOrder.InventoryShipmentOrder.PickedInventory.Archived = true;
                TestHelper.Context.SaveChanges();

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    StaticAttributeNames.Yeast.ShortName, new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters
                                                {
                                                    Value = 3,
                                                    Date = DateTime.UtcNow
                                                },
                                            Resolution = new DefectResolutionParameters
                                                {
                                                    ResolutionType = ResolutionTypeEnum.Treated,
                                                    Description = "Treated test"
                                                }
                                        }
                                }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(chileLot.ToLotKey(), c => c.AttributeDefects.Select(d => d.LotDefect.Resolution));
                Assert.AreEqual(ResolutionTypeEnum.Treated, lot.AttributeDefects.Select(d => d.LotDefect.Resolution).Single().ResolutionType);
            }

            [Test, Issue("Client has specified need to enter attribute data for AdditiveLots in order to have AdditiveLots affect produced Lot computed attribute values. -RI 2016-12-6",
                References = new[] { "RVCADMIN-1412" })]
            public void Sets_LotAttributes_as_expected_for_an_AdditiveLot()
            {
                //Arrange
                var additiveLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>();
                var attributes = new Dictionary<string, IAttributeValueParameters>
                    {
                        {
                            StaticAttributeNames.Asta.ShortName, new AttributeValueParameters
                                {
                                    AttributeInfo = new AttributeInfoParameters
                                        {
                                            Value = 1,
                                            Date = DateTime.UtcNow
                                        }
                                }
                        },
                        {
                            StaticAttributeNames.Scan.ShortName, new AttributeValueParameters
                                {
                                    AttributeInfo = new AttributeInfoParameters
                                        {
                                            Value = 2,
                                            Date = DateTime.UtcNow
                                        }
                                }
                        }
                    };

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = additiveLot.ToLotKey(),
                        Attributes = attributes
                    });

                //Assert
                result.AssertSuccess();
                var resultingAttributes = RVCUnitOfWork.LotRepository.FindByKey(additiveLot.ToLotKey(), l => l.Attributes);
                attributes.AssertEquivalent(resultingAttributes.Attributes, e => e.Key, r => r.AttributeShortName,
                    (e, r) =>
                        {
                            Assert.AreEqual(e.Value.AttributeInfo.Value, r.AttributeValue);
                            Assert.AreEqual(e.Value.AttributeInfo.Date.Date, r.AttributeDate);
                        });
            }
        }

        [TestFixture]
        public class AddLotAttributes : LotServiceTests
        {
            [Test]
            public void Creates_new_LotAttribute_records_as_expected()
            {
                //Arrange
                var lots = new[]
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>().Lot,
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>().Lot,
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>().Lot
                    };

                var attributeNameKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>().ToAttributeNameKey();
                var attributeNameKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>().ToAttributeNameKey();

                //Act
                var result = Service.AddLotAttributes(new AddLotAttributesParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKeys = lots.Select(l => l.ToLotKey().KeyValue).ToArray(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey0, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0.1, Date = new DateTime(2016, 1, 1) } } },
                                { attributeNameKey1, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0.2, Date = new DateTime(2016, 1, 2) } } }
                            }
                    });

                //Assert
                result.AssertSuccess();

                TestHelper.ResetContext();
                foreach(var lot in lots)
                {
                    var lotResult = RVCUnitOfWork.LotRepository.FindByKey(lot.ToLotKey(), l => l.Attributes);
                    Assert.AreEqual(0.1, lotResult.Attributes.FirstOrDefault(attributeNameKey0.Equals).AttributeValue);
                    Assert.AreEqual(0.2, lotResult.Attributes.FirstOrDefault(attributeNameKey1.Equals).AttributeValue);
                }
            }

            [Test]
            public void Does_not_remove_previous_LotAttribute_records_if_they_are_not_specified_to_be_added()
            {
                //Arrange
                var lots = new[]
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Attributes = TestHelper.List<LotAttribute>(1)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Attributes = TestHelper.List<LotAttribute>(2))
                    };

                var attributeNameKey0 = new AttributeNameKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>());
                var attributeNameKey1 = new AttributeNameKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>());

                //Act
                var result = Service.AddLotAttributes(new AddLotAttributesParameters
                {
                    UserToken = TestUser.UserName,
                    LotKeys = lots.Select(l => l.ToLotKey().KeyValue).ToArray(),
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey0.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0.1, Date = new DateTime(2016, 1, 1) } } },
                                { attributeNameKey1.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = 0.2, Date = new DateTime(2016, 1, 2) } } }
                            }
                });

                //Assert
                result.AssertSuccess();
                
                foreach(var lot in lots)
                {
                    var lotResult = RVCUnitOfWork.LotRepository.FindByKey(lot.ToLotKey(), l => l.Attributes);
                    if(lot.Lot.Attributes != null)
                    {
                        foreach(var attribute in lot.Lot.Attributes)
                        {
                            Assert.AreEqual(attribute.AttributeValue, lotResult.Attributes.Single(a => attribute.ToAttributeNameKey().Equals(a)).AttributeValue);
                        }
                    }
                }
            }

            [Test]
            public void Will_create_a_LotAttributeDefects_if_a_LotAttribute_value_violates_a_ChileProductRange()
            {
                //Arrange
                const string attributeName = "badStuff";
                const double min = 0.0f;
                const double max = 10.0f;
                const double attributeValue = 20.0f;
                const DefectTypeEnum expectedDefectType = DefectTypeEnum.BacterialContamination;

                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(
                    r => r.SetValues(min, max, attributeName), r => r.AttributeName.DefectType = expectedDefectType);
                var lots = new[]
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(attributeRange.ChileProduct), c => c.Lot.EmptyLot()),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(attributeRange.ChileProduct), c => c.Lot.EmptyLot()),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(attributeRange.ChileProduct), c => c.Lot.EmptyLot())
                    };
                var attributeNameKey = attributeRange.ToAttributeNameKey();

                //Act
                var result = Service.AddLotAttributes(new AddLotAttributesParameters
                {
                    UserToken = TestUser.UserName,
                    LotKeys = lots.Select(l => l.ToLotKey().KeyValue).ToArray(),
                    Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                { attributeNameKey.KeyValue, new AttributeValueParameters { AttributeInfo = new AttributeInfoParameters { Value = attributeValue, Date = DateTime.UtcNow } } }
                            }
                });

                //Assert
                result.AssertSuccess();

                foreach(var lot in lots)
                {
                    var lotDefect = RVCUnitOfWork.LotRepository.FindByKey(lot.ToLotKey(), l => l.LotDefects).LotDefects.Single();
                    Assert.AreEqual(expectedDefectType, lotDefect.DefectType);
                    Assert.IsNull(lotDefect.Resolution);

                    var lotAttributeDefectKey = new LotAttributeDefectKey(lotDefect, attributeNameKey);
                    var lotAttributeDefect = RVCUnitOfWork.LotAttributeDefectRepository.FindByKey(lotAttributeDefectKey);
                    Assert.AreEqual(min, lotAttributeDefect.OriginalAttributeMinLimit);
                    Assert.AreEqual(max, lotAttributeDefect.OriginalAttributeMaxLimit);
                    Assert.AreEqual(attributeValue, lotAttributeDefect.OriginalAttributeValue);
                }
            }
        }

        [TestFixture]
        public class GetDefectResolutions : LotServiceTests
        {
            [Test]
            public void Returns_expected_results()
            {
                //Act
                var result = Service.GetDefectResolutions();

                //Assert
                result.AssertSuccess();

                var productSpecResolutions = result.ResultingObject.DefectResolutions.Single(d => d.Key == DefectTypeEnum.ProductSpec).Value.ToList();
                Assert.IsTrue(productSpecResolutions.Contains(ResolutionTypeEnum.DataEntryCorrection));
                Assert.IsTrue(productSpecResolutions.Contains(ResolutionTypeEnum.ReworkPerformed));
                Assert.IsFalse(productSpecResolutions.Contains(ResolutionTypeEnum.Treated));

                var contaminatedResolutions = result.ResultingObject.DefectResolutions.Single(d => d.Key == DefectTypeEnum.BacterialContamination).Value.ToList();
                Assert.IsTrue(contaminatedResolutions.Contains(ResolutionTypeEnum.DataEntryCorrection));
                Assert.IsFalse(contaminatedResolutions.Contains(ResolutionTypeEnum.ReworkPerformed));
                Assert.IsTrue(contaminatedResolutions.Contains(ResolutionTypeEnum.Treated));

                var inHouseContaminatedResolutions = result.ResultingObject.DefectResolutions.Single(d => d.Key == DefectTypeEnum.InHouseContamination).Value.ToList();
                Assert.IsTrue(inHouseContaminatedResolutions.Contains(ResolutionTypeEnum.DataEntryCorrection));
                Assert.IsFalse(inHouseContaminatedResolutions.Contains(ResolutionTypeEnum.ReworkPerformed));
                Assert.IsFalse(inHouseContaminatedResolutions.Contains(ResolutionTypeEnum.Treated));
            }
        }

        [TestFixture]
        public class SetLotHoldStatus : LotServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Lot_key_could_not_be_parsed()
            {
                //Act
                var result = Service.SetLotHoldStatus(new SetLotHoldStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = "your-mother"
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.InvalidLotKey);
            }

            [Test]
            public void Returns_non_successful_result_if_Lot_could_not_be_found()
            {
                //Act
                var result = Service.SetLotHoldStatus(new SetLotHoldStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.LotNotFound);
            }

            [Test]
            public void Sets_Lot_hold_properties_as_expected_on_success()
            {
                //Arrange
                var lotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()).ToLotKey();

                //Act
                var parameters = new SetLotHoldStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Hold = new LotHold
                            {
                                HoldType = LotHoldType.HoldForAdditionalTesting,
                                Description = "Dont pick from this Lot bro"
                            }
                    };
                var result = Service.SetLotHoldStatus(parameters);

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey);
                Assert.AreEqual(parameters.Hold.HoldType, lot.Hold);
                Assert.AreEqual(parameters.Hold.Description, lot.HoldDescription);
            }

            [Test]
            public void Nulls_out_Lot_hold_properties_if_Hold_is_set_to_null()
            {
                //Arrange
                var lotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()).ToLotKey();

                //Act
                var parameters = new SetLotHoldStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Hold = null
                    };
                var result = Service.SetLotHoldStatus(parameters);

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey);
                Assert.IsNull(lot.Hold);
                Assert.IsNull(lot.HoldDescription);
            }
        }

        [TestFixture]
        public class CreateLotDefect : LotServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Lot_key_could_not_be_parsed()
            {
                //Act
                var result = Service.CreateLotDefect(new CreateLotDefectParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = "20120329-something or other - I give up",
                        DefectType = DefectTypeEnum.InHouseContamination
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.InvalidLotKey);
            }

            [Test]
            public void Returns_non_successful_result_if_Lot_could_not_be_found()
            {
                //Act
                var result = Service.CreateLotDefect(new CreateLotDefectParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(),
                        DefectType = DefectTypeEnum.InHouseContamination
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.LotNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_DefectType_is_not_InHouseContamination()
            {
                //Act
                var result = Service.CreateLotDefect(new CreateLotDefectParameters
                    {
                        LotKey = new LotKey(LotKey.Null).KeyValue,
                        DefectType = DefectTypeEnum.ProductSpec
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.OnlyInHouseContaminationValid);
            }

            [Test]
            public void Creates_LotDefect_as_expected_on_success()
            {
                //Arrange
                const string expectedDescription = "Little Jimmy fell into the vat.";
                var lotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()).ToLotKey();

                //Act
                var result = Service.CreateLotDefect(new CreateLotDefectParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey.KeyValue,
                        DefectType = DefectTypeEnum.InHouseContamination,
                        Description = expectedDescription
                    });

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.LotDefects.Select(d => d.Resolution));
                var defect = lot.LotDefects.Single();

                Assert.AreEqual(expectedDescription, defect.Description);
                Assert.AreEqual(DefectTypeEnum.InHouseContamination, defect.DefectType);
                Assert.IsNull(defect.Resolution);
            }
        }

        [TestFixture]
        public class RemoveLotDefectResolution : LotServiceTests
        {
            [Test]
            public void Returns_succesful_result_if_defect_has_no_resolution()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot());
                var defect = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(chileLot), d => d.Resolution = null);

                //Act
                var result = Service.RemoveLotDefectResolution(new RemoveLotDefectResolutionParameters
                    {
                        UserToken = TestUser.UserName,
                        LotDefectKey = new LotDefectKey(defect)
                    });

                //Assert
                result.AssertSuccess();
            }

            [Test]
            public void Removes_LotDefectResolution_from_database_on_success()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot());
                var defect = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(chileLot));

                //Act
                var result = Service.RemoveLotDefectResolution(new RemoveLotDefectResolutionParameters
                    {
                        UserToken = TestUser.UserName,
                        LotDefectKey = new LotDefectKey(defect)
                    });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.LotDefectRepository.FindByKey(new LotDefectKey(defect), d => d.Resolution).Resolution);
            }

            [Test]
            public void Updates_LotQualityStatus_as_expected_on_success()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot().QualityStatus = LotQualityStatus.Released);
                var defect = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(chileLot, DefectTypeEnum.BacterialContamination));

                //Act
                var result = Service.RemoveLotDefectResolution(new RemoveLotDefectResolutionParameters
                    {
                        UserToken = TestUser.UserName,
                        LotDefectKey = new LotDefectKey(defect)
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(LotQualityStatus.Contaminated, RVCUnitOfWork.LotRepository.FindByKey(new LotKey(defect)).QualityStatus);
            }
        }

        [TestFixture]
        public class SetLotStatus : LotServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_attempting_to_set_LotStatus_to_non_Contaminated_when_unresolved_Contaminated_LotDefect_exists()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(
                    c => c.SetLotType().Lot.LotDefects = null,
                    c => c.Lot.ProductionStatus = LotProductionStatus.Produced,
                    c => c.ChileProduct.ProductAttributeRanges = new List<ChileProductAttributeRange>
                        {
                            TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.ChileProduct = null, r => r.SetValues(StaticAttributeNames.Salmonella, 0.0, 1.0).AttributeName = null)
                        },
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.Lot = null, a => a.SetValues(null, StaticAttributeNames.Salmonella, 2.0))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Salmonella, DefectTypeEnum.BacterialContamination, 2.0, 0.0, 1.0).LotDefect.Resolution = null);

                //Act
                var result = Service.SetLotQualityStatus(new SetLotStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(chileLot),
                        QualityStatus = LotQualityStatus.Released
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CannotSetLotStatus);
            }

            [Test]
            public void Successfully_Sets_Contaminated_Lot_to_Rejected()
            {
                //Arrange
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(
                    c => c.Lot.ProductionStatus = LotProductionStatus.Produced,
                    c => c.Lot.QualityStatus = LotQualityStatus.Contaminated,
                    c => c.ChileProduct.ProductAttributeRanges = new List<ChileProductAttributeRange>
                        {
                            TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Salmonella, 0.0, 1.0).AttributeName = null)
                        },
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Salmonella, 2.0))
                        }));

                //Act
                var parameters = new SetLotStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        QualityStatus = LotQualityStatus.Rejected
                    };
                var result = Service.SetLotQualityStatus(parameters);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(parameters.QualityStatus, RVCUnitOfWork.LotRepository.FindByKey(lotKey).QualityStatus);
            }

            [Test, Issue("Use to be for a Lot that is in RequiresAttention state, but removed said state and am using " +
                         "Undetermined state instead. -RI 2016-06-22",
                         References = new[] { "RVCADMIN-1167" })]
            public void Successfully_sets_Completed_Lot_with_Undetermined_to_Accepted()
            {
                //Arrange
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(
                    c => c.Lot.ProductionStatus = LotProductionStatus.Produced,
                    c => c.Lot.QualityStatus = LotQualityStatus.Pending,
                    c => c.ChileProduct.ProductAttributeRanges = new List<ChileProductAttributeRange>
                        {
                            TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 0.0, 1.0).AttributeName = null)
                        },
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Ash, 2.0).Computed = true)
                        }));

                //Act
                var parameters = new SetLotStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        QualityStatus = LotQualityStatus.Released
                    };
                var result = Service.SetLotQualityStatus(parameters);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(parameters.QualityStatus, RVCUnitOfWork.LotRepository.FindByKey(lotKey).QualityStatus);
            }

            [Test, Issue("Actionable defects were previously being resolved when Accepted. We've decided to change it so that defects/resolutions are" +
                         "more for reference and have no consequences in logic associated to them, and as a reference it is ilogical that a" +
                         "an attribute that's out of range could be considered resolved so long as it remains out of range. - RI 2016-06-22",
                         References = new[] { "RVCADMIN-1166" })]
            public void Does_not_create_resolutions_for_unresolved_actionable_defects_if_status_is_successfuly_set_to_Accepted()
            {
                //Arrange
                const int expectedDefects = 3;
                var lotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot().QualityStatus = LotQualityStatus.Pending).ToLotKey();
                var defectKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(lotKey, DefectTypeEnum.ActionableDefect).Resolution = null).ToLotDefectKey();
                var defectKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(lotKey, DefectTypeEnum.ActionableDefect).Resolution = null).ToLotDefectKey();
                var resolvedDefect = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>(d => d.SetValues(lotKey, DefectTypeEnum.ActionableDefect));
                var resolvedDefectKey = resolvedDefect.ToLotDefectKey();

                //Act
                var result = Service.SetLotQualityStatus(new SetLotStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        QualityStatus = LotQualityStatus.Released
                    });

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.LotDefects.Select(d => d.Resolution));
                Assert.AreEqual(LotQualityStatus.Released, lot.QualityStatus);
                Assert.AreEqual(expectedDefects, lot.LotDefects.Count);

                Assert.AreEqual(resolvedDefect.Resolution.TimeStamp, lot.LotDefects.Single(d => resolvedDefectKey.Equals(d)).Resolution.TimeStamp);

                Assert.IsNull(lot.LotDefects.Single(d => defectKey0.Equals(d)).Resolution);
                Assert.IsNull(lot.LotDefects.Single(d => defectKey1.Equals(d)).Resolution);
            }

            [Test]
            public void Successfully_sets_incomplete_Rejected_Lot_with_no_defects_to_Undetermined()
            {
                //Arrange
                var lotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot().SetProductSpec(false, false).QualityStatus = LotQualityStatus.Rejected).ToLotKey();

                //Act
                var result = Service.SetLotQualityStatus(new SetLotStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        QualityStatus = LotQualityStatus.Pending
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(LotQualityStatus.Pending, RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey, c => c.Lot).Lot.QualityStatus);
            }

            [Test, Issue(@"Client has specified need to enter attribute data for AdditiveLots in order to have AdditiveLots affect produced Lot computed attribute values.
                So I figured we should verify QualityStatus can be set too. -RI 2016-12-6",
                References = new[] { "RVCADMIN-1412" })]
            public void Successfully_sets_AdditiveLot_to_desired_QualityStatus()
            {
                //Arrange
                var lotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>(a => a.Lot.EmptyLot().SetProductSpec(false, false).QualityStatus = LotQualityStatus.Rejected).ToLotKey();

                //Act
                var result = Service.SetLotQualityStatus(new SetLotStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        QualityStatus = LotQualityStatus.Pending
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(LotQualityStatus.Pending, RVCUnitOfWork.LotRepository.FindByKey(lotKey).QualityStatus);
            }
        }

        [TestFixture]
        public class GetLabReport : LotServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_specified_Lot_cannot_be_found()
            {
                //Arrange
                var lotKey = new LotKey();

                //Act
                var result = TimedExecution(() => Service.GetLabReport(lotKey));

                //Assert
                result.AssertNotSuccess(UserMessages.LabReportChileLotsNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_no_Lots_can_be_found_for_supplied_test_date_range_with_non_Computed_attributes()
            {
                //Arrange
                var testDate = new DateTime(2014, 3, 29);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Attributes = new List<LotAttribute>
                    {
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.Lot = null, a => a.AttributeDate = testDate, a => a.Computed = true)
                    });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.Attributes = new List<LotAttribute>
                    {
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.Lot = null, a => a.AttributeDate = testDate.AddDays(2))
                    });

                //Act
                var result = TimedExecution(() => Service.GetLabReport(testDate.AddDays(-1), testDate.AddDays(1)));

                //Assert
                result.AssertNotSuccess(UserMessages.LabReportChileLotsNotFound);
            }

            [Test]
            public void Returns_report_for_Lot_specified_by_key()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.LotTypeId = (int)LotTypeEnum.FinishedGood);
                var lotKey = new LotKey(chileLot);
                var dehydrated0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceivedItem>();
                var dehydrated1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceivedItem>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(chileLot.Production, dehydrated0).ToteKey = dehydrated0.ToteKey);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(chileLot.Production, dehydrated0).ToteKey = dehydrated0.ToteKey);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(chileLot.Production, dehydrated1).ToteKey = dehydrated1.ToteKey);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(chileLot.Production, dehydrated1).ToteKey = dehydrated1.ToteKey);

                //Act
                var result = TimedExecution(() => Service.GetLabReport(lotKey));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(lotKey.KeyValue, result.ResultingObject.ChileLots.Single().LotKey);
            }

            [Test]
            public void Returns_report_for_Lots_specified_by_test_date_range()
            {
                var testDate = new DateTime(2014, 3, 29);
                var lotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.LotTypeId = (int)LotTypeEnum.FinishedGood, c => c.Lot.Attributes = null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey0, null, null, false).AttributeDate = testDate.AddDays(-3));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey0, null, null, false).AttributeDate = testDate);

                var lotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.LotTypeId = (int)LotTypeEnum.FinishedGood, c => c.Lot.Attributes = null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey1, null, null, true).AttributeDate = testDate);

                var lotKey2 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.LotTypeId = (int)LotTypeEnum.FinishedGood, c => c.Lot.Attributes = null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(lotKey2, null, null, false).AttributeDate = testDate);

                //Act
                var result = TimedExecution(() => Service.GetLabReport(testDate.AddDays(-1), testDate.AddDays(1)));

                //Assert
                result.AssertSuccess();

                Assert.AreEqual(2, result.ResultingObject.ChileLots.Count());
                Assert.IsNotNull(result.ResultingObject.ChileLots.FirstOrDefault(c => c.LotKey == lotKey0));
                Assert.IsNull(result.ResultingObject.ChileLots.FirstOrDefault(c => c.LotKey == lotKey1));
                Assert.IsNotNull(result.ResultingObject.ChileLots.FirstOrDefault(c => c.LotKey == lotKey2));
            }

            [Test]
            public void Returns_ChileLot_with_expected_PackSchedule_information_if_Lot_has_no_associated_PackSchedule()
            {
                //Arrange
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.LotTypeId = (int)LotTypeEnum.FinishedGood, c => c.NullProduction()));

                //Act
                var result = TimedExecution(() => Service.GetLabReport(lotKey));

                //Assert
                result.AssertSuccess();
                var chileLot = result.ResultingObject.ChileLots.Single(c => c.LotKey == lotKey.KeyValue);
                Assert.IsNull(chileLot.PackScheduleKey);
                Assert.IsNull(chileLot.PSNum);
                Assert.IsNull(chileLot.WorkType);
                Assert.IsNull(chileLot.ProductionLineDescription);
                chileLot.TargetParameters.AssertAsExpected(new ProductionBatchTargetParameters());
            }

            [Test]
            public void Returns_ChileLot_Notes_as_expected()
            {
                //Arrange
                const string note0 = "Note0";
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.LotTypeId = (int)LotTypeEnum.WIP, c => c.Lot.Notes = note0);

                //Act
                var result = TimedExecution(() => Service.GetLabReport(new LotKey(chileLot)));

                //Assert
                result.AssertSuccess();
                var labLot = result.ResultingObject.ChileLots.Single();
                Assert.AreEqual(note0, labLot.Notes);
            }

            [Test]
            public void Returns_ChileLot_with_weighted_attributes_as_expected()
            {
                //Arrange
                var valueSeed = 1d;
                var pickedAttributes0 = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a => valueSeed += 1.5d);
                valueSeed = 2d;
                var pickedAttributes1 = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a => valueSeed += 2.2d);
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.LotTypeId = (int)LotTypeEnum.FinishedGood, c => c.Production.PickedInventory.Items = null, c => c.Lot.Attributes = StaticAttributeNames.AttributeNames.Select(a =>
                    TestHelper.CreateObjectGraph<LotAttribute>(l => l.SetValues(null, a))).ToList());

                var pickedItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(chileLot.Production).SetPicked(2, 100).Lot.Attributes = pickedAttributes0.Select(e =>
                    TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, e.Key, e.Value))).ToList());
                var pickedWeight0 = pickedItem0.Quantity * pickedItem0.PackagingProduct.Weight;

                var pickedItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(chileLot.Production).SetPicked(3, 50).Lot.Attributes = pickedAttributes1.Select(e =>
                    TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, e.Key, e.Value))).ToList());
                var pickedWeight1 = pickedItem1.Quantity * pickedItem1.PackagingProduct.Weight;

                var totalWeight = pickedWeight0 + pickedWeight1;
                var expectedResults = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a => ((pickedAttributes0[a] * pickedWeight0) + (pickedAttributes1[a] * pickedWeight1)) / totalWeight);

                //Act
                var result = TimedExecution(() => Service.GetLabReport(new LotKey(chileLot)));

                //Assert
                result.AssertSuccess();

                var labLot = result.ResultingObject.ChileLots.Single(c => c.LotKey == new LotKey(chileLot).KeyValue);
                foreach(var expected in expectedResults)
                {
                    Assert.AreEqual(expected.Value, labLot.Attributes.Single(a => a.Key == expected.Key.ShortName).Value.WeightedAverage, 0.01);
                }
            }

            [Test]
            public void Returns_LabReport_as_expected_for_MillAndWetdown_ChileLot()
            {
                //Arrange
                var dehydrated = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.Items = new List<ChileMaterialsReceivedItem>
                    {
                        TestHelper.CreateObjectGraph<ChileMaterialsReceivedItem>(i => i.ChileMaterialsReceived = null)
                    });
                var dehydratedItem = dehydrated.Items.First();
                var millAndWetdown = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(c => c.LotTypeId = (int) LotTypeEnum.WIP,
                    m => m.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.ConstrainByKeys(null, dehydratedItem, dehydratedItem, null, null, null, dehydratedItem.ToteKey))
                        });

                //Act
                var result = TimedExecution(() => Service.GetLabReport(new LotKey(millAndWetdown)));

                //Assert
                result.AssertSuccess();
                var chileLot = result.ResultingObject.ChileLots.Single();

                Assert.IsNull(chileLot.PackScheduleKey);
                Assert.IsNull(chileLot.PSNum);
                Assert.IsNull(chileLot.WorkType);
                Assert.IsNotNull(chileLot.ProductionLineDescription);
                Assert.IsNotNull(chileLot.TargetParameters);
                Assert.IsNotNull(chileLot.ProductionShiftKey);
                Assert.IsNull(chileLot.CustomerName);
                Assert.IsNotEmpty(chileLot.DehydratedInputs);
            }

            [Test]
            public void Returns_ValidToPick_property_as_expected()
            {
                //Arrange
                var attributeDate = new DateTime(2014, 3, 15);
                var productionBatch0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase(LabReportProductionBatch(b => b.Production.ResultingChileLot.Lot.Attributes = new List<LotAttribute>
                    {
                        TestHelper.CreateObjectGraph(LabReportAttribute(attributeDate))
                    }, b => b.PackSchedule.SetCustomerKey(null)));

                var customerSpec = TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.Active = true, r => r.RangeMin = 10, r => r.RangeMax = 20);
                var productionBatch1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase(LabReportProductionBatch(b => b.Production.ResultingChileLot.Lot.Attributes = new List<LotAttribute>
                    {
                        TestHelper.CreateObjectGraph(LabReportAttribute(attributeDate, a => a.SetValues(null, customerSpec, 15)))
                    }, b => b.PackSchedule.SetCustomerKey(customerSpec).SetChileProduct(customerSpec),
                    b => b.Production.ResultingChileLot.SetProduct(customerSpec)));

                var productionBatch2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase(LabReportProductionBatch(b => b.Production.ResultingChileLot.Lot.Attributes = new List<LotAttribute>
                    {
                        TestHelper.CreateObjectGraph(LabReportAttribute(attributeDate, a => a.SetValues(null, customerSpec, 30)))
                    }, b => b.PackSchedule.SetCustomerKey(customerSpec).SetChileProduct(customerSpec),
                    b => b.Production.ResultingChileLot.SetProduct(customerSpec)));

                //Act
                var result = TimedExecution(() => Service.GetLabReport(attributeDate.AddDays(-2), attributeDate.AddDays(2)));

                //Assert
                result.AssertSuccess();

                Assert.IsTrue(result.ResultingObject.ChileLots.Single(l => l.LotKey == productionBatch0.ToLotKey().KeyValue).ValidToPick);
                Assert.IsTrue(result.ResultingObject.ChileLots.Single(l => l.LotKey == productionBatch1.ToLotKey().KeyValue).ValidToPick);
                Assert.IsFalse(result.ResultingObject.ChileLots.Single(l => l.LotKey == productionBatch2.ToLotKey().KeyValue).ValidToPick);
            }
        }

        [TestFixture]
        public class SetLotPackagingReceived : LotServiceTests
        {
            [Test]
            public void Modifies_Lot_ReceivedPackaging_as_expected_on_success()
            {
                //Arrange
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>());
                var packagingKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());

                //Act
                var result = Service.SetLotPackagingReceived(new SetLotPackagingReceivedParameters
                    {
                        LotKey = lotKey,
                        ReceivedPackagingProductKey = packagingKey
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(packagingKey.KeyValue, new PackagingProductKey(RVCUnitOfWork.LotRepository.FindByKey(lotKey)).KeyValue);
            }
        }

        [TestFixture]
        public class AddLotAllowance : LotServiceTests
        {
            [Test]
            public void Creates_LotAllowances_as_expected()
            {
                //Arrange
                var lot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                //Act
                var result = Service.AddLotAllowance(new LotAllowanceParameters
                    {
                        LotKey = lot.ToLotKey(),
                        CustomerKey = customer.ToCustomerKey(),
                        ContractKey = contract.ToContractKey(),
                        CustomerOrderKey = customerOrder.ToSalesOrderKey()
                    });

                //Assert
                result.AssertSuccess();
                var lotResult = RVCUnitOfWork.LotRepository.FindByKey(lot.ToLotKey(),
                    l => l.CustomerAllowances.Select(c => c.Customer),
                    l => l.ContractAllowances.Select(c => c.Contract),
                    l => l.SalesOrderAllowances.Select(c => c.SalesOrder));
                Assert.True(customer.ToCustomerKey().Equals(lotResult.CustomerAllowances.Single()));
                Assert.True(contract.ToContractKey().Equals(lotResult.ContractAllowances.Single()));
                Assert.True(customerOrder.ToSalesOrderKey().Equals(lotResult.SalesOrderAllowances.Single()));
            }
        }

        [TestFixture]
        public class RemoveLotAllowance : LotServiceTests
        {
            [Test]
            public void Removes_LotAllowances_as_expected()
            {
                //Arrange
                var lot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(
                    l => l.CustomerAllowances = TestHelper.List<LotCustomerAllowance>(1),
                    l => l.ContractAllowances = TestHelper.List<LotContractAllowance>(1),
                    l => l.SalesOrderAllowances = TestHelper.List<LotSalesOrderAllowance>(1));

                //Act
                var result = Service.RemoveLotAllowance(new LotAllowanceParameters
                    {
                        LotKey = lot.ToLotKey(),
                        CustomerKey = lot.CustomerAllowances.Single().ToCustomerKey(),
                        ContractKey = lot.ContractAllowances.Single().ToContractKey(),
                        CustomerOrderKey = lot.SalesOrderAllowances.Single().ToSalesOrderKey()
                    });

                //Assert
                result.AssertSuccess();
                var lotResult = RVCUnitOfWork.LotRepository.FindByKey(lot.ToLotKey(),
                    l => l.CustomerAllowances.Select(c => c.Customer),
                    l => l.ContractAllowances.Select(c => c.Contract),
                    l => l.SalesOrderAllowances.Select(c => c.SalesOrder));
                Assert.False(lotResult.CustomerAllowances.Any());
                Assert.False(lotResult.ContractAllowances.Any());
                Assert.False(lotResult.SalesOrderAllowances.Any());
            }
        }

        [TestFixture]
        public class GetLotHistory : LotServiceTests
        {
            [Test]
            public void Returns_data_as_expected()
            {
                //Arrange
                var serializableHistory = TestHelper.List<SerializedLotHistory>(3);
                var lot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.History = serializableHistory.Select((s, i) =>
                    TestHelper.CreateObjectGraph<LotHistory>
                        (
                            n => n.TimeStamp = new DateTime(2016, 1, 1).AddDays(i),
                            n => n.Serialized = JsonConvert.SerializeObject(s)
                        )).ToList());

                //Act
                var result = Service.GetLotHistory(lot.ToLotKey());

                //Assert
                result.AssertSuccess();
                serializableHistory.AssertEquivalent(result.ResultingObject.History, (e, r) =>
                    {
                        Assert.AreEqual(e.QualityStatus, r.QualityStatus);
                        Assert.AreEqual(e.ProductionStatus, r.ProductionStatus);
                        Assert.AreEqual(e.Hold, r.HoldType);
                        Assert.AreEqual(e.HoldDescription, r.HoldDescription);
                        Assert.AreEqual(e.LoBac, r.LoBac);
                    });
            }
        }

        [TestFixture]
        public class Create_LotHistory : LotServiceTests
        {
            [Test]
            public void SetLotAttributes()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                var expected = GetExpected(chileLot.Lot);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>()
                    });

                //Assert
                result.AssertSuccess();
                AssertExpected(expected);
            }

            [Test]
            public void AddLotAttributes()
            {
                //Arrange
                var chileLots = new List<ChileLot>();
                for(var i = 0; i < 3; ++i)
                {
                    chileLots.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>());
                }
                var expected = GetExpected(chileLots.Select(c => c.Lot).ToArray());

                //Act
                var result = Service.AddLotAttributes(new AddLotAttributesParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKeys = chileLots.Select(c => c.ToLotKey().KeyValue).ToArray(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>()
                    });

                //Assert
                result.AssertSuccess();
                AssertExpected(expected);
            }

            [Test]
            public void SetLotHoldStatus()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                var expected = GetExpected(chileLot.Lot);

                //Act
                var result = Service.SetLotHoldStatus(new SetLotHoldStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        Hold = new LotHold
                            {
                                HoldType = LotHoldType.HoldForCustomer,
                                Description = "yup"
                            },
                        LotKey = chileLot.ToLotKey()
                    });

                //Assert
                result.AssertSuccess();
                AssertExpected(expected);
            }

            [Test]
            public void CreateLotDefect()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                var expected = GetExpected(chileLot.Lot);

                //Act
                var result = Service.CreateLotDefect(new CreateLotDefectParameters
                    {
                        UserToken = TestUser.UserName,
                        DefectType = DefectTypeEnum.InHouseContamination,
                        Description = "uhuh",
                        LotKey = chileLot.ToLotKey()
                    });

                //Assert
                result.AssertSuccess();
                AssertExpected(expected);
            }

            [Test]
            public void RemoveLotDefectResolution()
            {
                //Arrange
                var lotDefect = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotDefect>();
                var expected = GetExpected(lotDefect.Lot);

                //Act
                var result = Service.RemoveLotDefectResolution(new RemoveLotDefectResolutionParameters
                    {
                        UserToken = TestUser.UserName,
                        LotDefectKey = lotDefect.ToLotDefectKey()
                    });

                //Assert
                result.AssertSuccess();
                AssertExpected(expected);
            }

            [Test]
            public void SetLotQualityStatus()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.QualityStatus = LotQualityStatus.Released);
                var expected = GetExpected(chileLot.Lot);

                //Act
                var result = Service.SetLotQualityStatus(new SetLotStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        QualityStatus = LotQualityStatus.Contaminated
                    });

                //Assert
                result.AssertSuccess();
                AssertExpected(expected);
            }

            private static Dictionary<ILotKey, int> GetExpected(params Lot[] lots)
            {
                return lots.ToDictionary(l => (ILotKey)l, l => (l.History == null ? 0 : l.History.Count) + 1);
            }

            private void AssertExpected(IEnumerable<KeyValuePair<ILotKey, int>> expected)
            {
                foreach(var lot in expected)
                {
                    Assert.AreEqual(lot.Value, RVCUnitOfWork.LotHistoryRepository.CountOf(LotPredicates.ConstructPredicate<LotHistory>(lot.Key)));
                }
            }
        }

        [TestFixture]
        public class GetInputTrace : LotServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Lot_does_not_exist()
            {
                //Act
                var result = Service.GetInputTrace(new LotKey());

                //Assert
                result.AssertNotSuccess(UserMessages.LotNotFound);
            }

            [Test]
            public void Returns_data_as_expected_on_success()
            {
                //Arrange
                var multipleTreatedlot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();
                var pickedProduced = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(p => p.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(3));
                var production = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(p => p.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(3, l =>
                    {
                        l[0].ConstrainByKeys(null, multipleTreatedlot);
                        l[1].ConstrainByKeys(null, multipleTreatedlot);
                        l[2].ConstrainByKeys(null, pickedProduced);
                    }));

                var expected = new[]
                    {
                        new Expected
                            {
                                LotPath = new string[] { production.ToLotKey(), multipleTreatedlot.ToLotKey() },
                                Treatment = production.PickedInventory.Items.ElementAt(0).Treatment.ShortName
                            },
                        new Expected
                            {
                                LotPath = new string[] { production.ToLotKey(), multipleTreatedlot.ToLotKey() },
                                Treatment = production.PickedInventory.Items.ElementAt(1).Treatment.ShortName
                            },
                        new Expected
                            {
                                LotPath = new string[] { production.ToLotKey(), pickedProduced.ToLotKey() },
                                Treatment = production.PickedInventory.Items.ElementAt(2).Treatment.ShortName
                            },
                        new Expected
                            {
                                LotPath = new string[] { production.ToLotKey(), pickedProduced.ToLotKey(), pickedProduced.PickedInventory.Items.ElementAt(0).ToLotKey() },
                                Treatment = pickedProduced.PickedInventory.Items.ElementAt(0).Treatment.ShortName
                            },
                        new Expected
                            {
                                LotPath = new string[] { production.ToLotKey(), pickedProduced.ToLotKey(), pickedProduced.PickedInventory.Items.ElementAt(1).ToLotKey() },
                                Treatment = pickedProduced.PickedInventory.Items.ElementAt(1).Treatment.ShortName
                            },
                        new Expected
                            {
                                LotPath = new string[] { production.ToLotKey(), pickedProduced.ToLotKey(), pickedProduced.PickedInventory.Items.ElementAt(2).ToLotKey() },
                                Treatment = pickedProduced.PickedInventory.Items.ElementAt(2).Treatment.ShortName
                            }
                    };

                //Act
                var result = Service.GetInputTrace(production.ToLotKey());

                //Assert
                result.AssertSuccess();
                expected.AssertEquivalent(result.ResultingObject, TraceString, TraceString);
            }

            private class Expected : ILotInputTraceReturn
            {
                public IEnumerable<string> LotPath { get; internal set; }
                public string Treatment { get; internal set; }
            }

            private static string TraceString(ILotInputTraceReturn trace)
            {
                return string.Format("{0} [{1}]", string.Join(" <- ", trace.LotPath), trace.Treatment);
            }
        }

        [TestFixture]
        public class GetOutputTrace : LotServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Lot_does_not_exist()
            {
                //Act
                var result = Service.GetOutputTrace(new LotKey());

                //Assert
                result.AssertNotSuccess(UserMessages.LotNotFound);
            }

            [Test]
            public void Returns_data_as_expected_on_success()
            {
                //Arrange
                var lot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();
                var producedLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(c => c.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(3, (p, n) =>
                    {
                        if(n < 2)
                        {
                            p.ConstrainByKeys(null, lot);
                        }
                    }));
                var producedLot2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(c => c.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(1, p =>
                    p.ForEach(i => i.ConstrainByKeys(null, producedLot))));
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.SalesOrderPickedItems = TestHelper.List<SalesOrderPickedItem>(2, p => p.ForEach(i => i.PickedInventoryItem.ConstrainByKeys(null, lot))));

                var expected = new[]
                    {
                        new Expected
                            {
                                LotPath = new string[] { lot.ToLotKey() },
                                Inputs = new ILotOutputTraceInputReturn[0],
                                Orders = order.SalesOrderPickedItems.Select(i => new LotOutputTraceOrdersReturn
                                    {
                                        Treatment = i.PickedInventoryItem.Treatment.ShortName,
                                        OrderNumber = i.SalesOrder.InventoryShipmentOrder.MoveNum,
                                        ShipmentDate = i.SalesOrder.InventoryShipmentOrder.ShipmentInformation.ShipmentDate,
                                        CustomerName = i.SalesOrder.Customer.Company.Name
                                    }).ToList()
                            },
                        new Expected
                            {
                                LotPath = new string[] { lot.ToLotKey(), producedLot.ToLotKey() },
                                Inputs = producedLot.PickedInventory.Items.Where(i => i.ToLotKey().Equals(lot))
                                    .OrderBy(i => i.Treatment.ShortName)
                                    .Select(i => new LotOutputTraceInputReturn
                                        {
                                            LotKey = lot.ToLotKey(),
                                            Treatment = i.Treatment.ShortName
                                        }),
                                Orders = new ILotOutputTraceOrdersReturn[0]
                            },
                        new Expected
                            {
                                LotPath = new string[] { lot.ToLotKey(), producedLot.ToLotKey(), producedLot2.ToLotKey() },
                                Inputs = producedLot2.PickedInventory.Items.Where(i => i.ToLotKey().Equals(producedLot))
                                    .OrderBy(i => i.Treatment.ShortName)
                                    .Select(i => new LotOutputTraceInputReturn
                                        {
                                            LotKey = producedLot.ToLotKey(),
                                            Treatment = i.Treatment.ShortName
                                        }),
                                Orders = new ILotOutputTraceOrdersReturn[0]
                            }
                    };

                //Act
                var result = Service.GetOutputTrace(lot.ToLotKey());

                //Assert
                result.AssertSuccess();
                expected.AssertEquivalent(result.ResultingObject, TraceString, TraceString, (e, r) =>
                    {
                        e.Inputs.AssertEquivalent(r.Inputs, eo => eo.Treatment, er => er.Treatment);
                        e.Orders.AssertEquivalent(r.Orders, eo => eo.Treatment, er => er.Treatment);
                    });
            }

            private class Expected : ILotOutputTraceReturn
            {
                public IEnumerable<string> LotPath { get; internal set; }
                public IEnumerable<ILotOutputTraceInputReturn> Inputs { get; internal set; }
                public IEnumerable<ILotOutputTraceOrdersReturn> Orders { get; internal set; }
            }

            private static string TraceString(ILotOutputTraceReturn trace)
            {
                return string.Join(" -> ", trace.LotPath);
            }
        }

        protected static Action<ProductionBatch>[] LabReportProductionBatch(params Action<ProductionBatch>[] init)
        {
            var i = new List<Action<ProductionBatch>>
                    {
                        b => b.LotTypeEnum = LotTypeEnum.WIP,
                        b => b.Production.ResultingChileLot.SetDerivedLot()
                    };
            i.AddRange(init);
            return i.ToArray();
        }

        protected static Action<LotAttribute>[] LabReportAttribute(DateTime attributeDate, params Action<LotAttribute>[] init)
        {
            var i = new List<Action<LotAttribute>>
                    {
                        a => a.Computed = false,
                        a => a.AttributeDate = attributeDate
                    };
            i.AddRange(init);
            return i.ToArray();
        }
    }
}