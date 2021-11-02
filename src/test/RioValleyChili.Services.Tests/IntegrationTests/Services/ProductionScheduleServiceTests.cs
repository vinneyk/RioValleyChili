using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Helpers;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class ProductionScheduleServiceTests : ServiceIntegrationTestBase<ProductionService>
    {
        [TestFixture]
        public class CreateProductionSchedule : ProductionScheduleServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ProductionSchedule_already_exists()
            {
                //Arrange
                var productionSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionSchedule>(p => p.ProductionLineLocation.LocationType = LocationType.ProductionLine);

                //Act
                var result = Service.CreateProductionSchedule(new CreateProductionScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = productionSchedule.ProductionDate,
                        ProductionLineLocationKey = productionSchedule.ToLocationKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionScheduleAlreadyExists);
            }

            [Test]
            public void Only_orders_PackSchedules_that_have_at_least_one_unproduced_batch()
            {
                //Arrange
                var productionLine = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine);
                var expected = new List<PackSchedule>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.SetProductionLine(productionLine),
                            p => p.ProductionBatches = TestHelper.List<ProductionBatch>(1, (b, n) => b.ProductionHasBeenCompleted = false)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.SetProductionLine(productionLine),
                            p => p.ProductionBatches = TestHelper.List<ProductionBatch>(2, (b, n) => b.ProductionHasBeenCompleted = n == 0)),
                    };

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.SetProductionLine(productionLine));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.SetProductionLine(productionLine),
                    p => p.ProductionBatches = TestHelper.List<ProductionBatch>(1, (b, n) => b.ProductionHasBeenCompleted = true));

                //Act
                var result = Service.CreateProductionSchedule(new CreateProductionScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = new DateTime(2016, 1, 1),
                        ProductionLineLocationKey = productionLine.ToLocationKey()
                    });

                //Assert
                result.AssertSuccess();

                var productionScheduleKey = KeyParserHelper.ParseResult<IProductionScheduleKey>(result.ResultingObject).ResultingObject.ToProductionScheduleKey();
                var results = RVCUnitOfWork.ProductionScheduleRepository.FindByKey(productionScheduleKey, p => p.ScheduledItems).ScheduledItems.ToList();
                expected.AssertEquivalent(results, e => e.ToPackScheduleKey().KeyValue, r => r.ToPackScheduleKey().KeyValue);
            }

            [Test]
            public void Schedules_previously_scheduled_PackSchedules_as_expected()
            {
                //Arrange
                var productionLine = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine);
                var expected = new List<ProductionScheduleItem>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionScheduleItem>(p => p.PackSchedule.SetProductionLine(productionLine),
                            p => p.PackSchedule.ProductionBatches = TestHelper.List<ProductionBatch>(1, (b, n) => b.ProductionHasBeenCompleted = false)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionScheduleItem>(p => p.PackSchedule.SetProductionLine(productionLine),
                            p => p.PackSchedule.ProductionBatches = TestHelper.List<ProductionBatch>(1, (b, n) => b.ProductionHasBeenCompleted = false))
                    };

                //Act
                var result = Service.CreateProductionSchedule(new CreateProductionScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = new DateTime(2016, 1, 1),
                        ProductionLineLocationKey = productionLine.ToLocationKey()
                    });

                //Assert
                result.AssertSuccess();
                var productionScheduleKey = KeyParserHelper.ParseResult<IProductionScheduleKey>(result.ResultingObject).ResultingObject.ToProductionScheduleKey();
                var results = RVCUnitOfWork.ProductionScheduleRepository.FindByKey(productionScheduleKey, p => p.ScheduledItems).ScheduledItems.ToList();
                expected.AssertEquivalent(results, e => e.ToPackScheduleKey().KeyValue, r => r.ToPackScheduleKey().KeyValue,
                    (e, r) =>
                        {
                            Assert.AreEqual(e.FlushBefore, r.FlushBefore);
                            Assert.AreEqual(e.FlushBeforeInstructions, r.FlushBeforeInstructions);
                            Assert.AreEqual(e.FlushAfter, r.FlushAfter);
                            Assert.AreEqual(e.FlushAfterInstructions, r.FlushAfterInstructions);
                        });
            }
        }

        [TestFixture]
        public class UpdateProductionSchedule : ProductionScheduleServiceTests
        {
            [Test]
            public void Returns_on_successful_result_if_ProductionSchedule_does_not_exist()
            {
                //Act
                var result = Service.UpdateProductionSchedule(new UpdateProductionScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionScheduleKey = new ProductionScheduleKey(),
                        ScheduledItems = new List<ISetProductionScheduleItemParameters>()
                    });
                
                //Assert
                result.AssertNotSuccess(UserMessages.ProductionScheduleNotFound);
            }

            [Test]
            public void Updates_ProductionSchedule_as_expected()
            {
                //Arrange
                var productionSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionSchedule>(p => p.ScheduledItems = TestHelper.List<ProductionScheduleItem>(3));
                var parameters = new UpdateProductionScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionScheduleKey = productionSchedule.ToProductionScheduleKey(),
                        ScheduledItems = new List<ISetProductionScheduleItemParameters>
                            {
                                new SetProductionScheduleItemParameters
                                    {
                                        Index = 0,
                                        FlushBefore = true,
                                        FlushBeforeInstructions = "hi",
                                        PackScheduleKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>().ToPackScheduleKey()
                                    },
                                new SetProductionScheduleItemParameters
                                    {
                                        Index = 1,
                                        FlushAfter = true,
                                        FlushBeforeInstructions = "bye",
                                        PackScheduleKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>().ToPackScheduleKey()
                                    }
                            }
                    };

                //Act
                var result = Service.UpdateProductionSchedule(parameters);
                productionSchedule = RVCUnitOfWork.ProductionScheduleRepository.FindByKey(productionSchedule.ToProductionScheduleKey(), p => p.ScheduledItems);

                //Assert
                result.AssertSuccess();
                parameters.ScheduledItems.OrderBy(i => i.Index)
                    .AssertEquivalent(productionSchedule.ScheduledItems.OrderBy(i => i.Index),
                    (e, r) =>
                        {
                            Assert.AreEqual(e.Index, r.Index);
                            Assert.AreEqual(e.PackScheduleKey, r.ToPackScheduleKey().KeyValue);
                            Assert.AreEqual(e.FlushAfter, r.FlushAfter);
                            Assert.AreEqual(e.FlushAfterInstructions, r.FlushAfterInstructions);
                            Assert.AreEqual(e.FlushBefore, r.FlushBefore);
                            Assert.AreEqual(e.FlushBeforeInstructions, r.FlushBeforeInstructions);
                        });
            }
        }

        [TestFixture]
        public class DeleteProductionSchedule : ProductionScheduleServiceTests
        {
            [Test]
            public void Returns_successful_result_if_ProductionSchedule_does_not_exist()
            {
                //Act
                var result = Service.DeleteProductionSchedule(new ProductionScheduleKey());

                //Assert
                result.AssertSuccess();
            }

            [Test]
            public void Deletes_ProductionSchedule_items_and_record()
            {
                //Arrange
                var productionSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionSchedule>(p => p.ScheduledItems = TestHelper.List<ProductionScheduleItem>(3));

                //Act
                var result = Service.DeleteProductionSchedule(productionSchedule.ToProductionScheduleKey());

                //Assert
                result.AssertSuccess();

                foreach(var item in productionSchedule.ScheduledItems)
                {
                    Assert.IsNull(RVCUnitOfWork.ProductionScheduleItemRepository.FindByKey(item.ToProductionScheduleItemKey()));
                }

                Assert.IsNull(RVCUnitOfWork.ProductionScheduleRepository.FindByKey(productionSchedule.ToProductionScheduleKey()));
            }
        }

        [TestFixture]
        public class GetProductionSchedule : ProductionScheduleServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ProductionSchedule_cannot_be_found()
            {
                //Act
                var result = Service.GetProductionSchedule(new ProductionScheduleKey());

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionScheduleNotFound);
            }

            [Test]
            public void Returns_ProductionSchedule_as_expected()
            {
                //Arrange
                var productionSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionSchedule>(p => p.ScheduledItems = TestHelper.List<ProductionScheduleItem>(3,
                    (r, n) => r.PackSchedule.ProductionBatches = TestHelper.List<ProductionBatch>(3,
                        (b, n2) => b.Production.ResultingChileLot.Lot.Attributes = new List<LotAttribute>
                            {
                                TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Granularity)),
                                TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.AB)),
                                TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Scoville)),
                                TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Scan))
                            })));

                //Act
                var result = Service.GetProductionSchedule(productionSchedule.ToProductionScheduleKey());

                //Assert
                productionSchedule.AssertEquivalent(result.ResultingObject);
            }
        }

        [TestFixture]
        public class GetProductionSchedules : ProductionScheduleServiceTests
        {
            [Test]
            public void Returns_results_as_expected()
            {
                //Arrange
                var expected = new List<ProductionSchedule>();
                for(var i = 0; i < 3; ++i)
                {
                    expected.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionSchedule>(p => p.ScheduledItems = TestHelper.List<ProductionScheduleItem>(3)));
                }

                //Act
                var result = Service.GetProductionSchedules();

                //Assert
                expected.AssertEquivalent(result.ResultingObject.ToList(), e => e.ToProductionScheduleKey().KeyValue, r => r.ProductionScheduleKey,
                    (e, r) => e.AssertEquivalent(r));
            }

            [Test]
            public void Filters_results_by_ProductionDate()
            {
                //Arrange
                var productionDate = new DateTime(2016, 1, 1);
                var expected = new List<ProductionSchedule>();
                for(var i = 0; i < 9; ++i)
                {
                    var productionSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionSchedule>(
                        p => p.ProductionDate = productionDate.AddDays(i % 3),
                        p => p.ScheduledItems = TestHelper.List<ProductionScheduleItem>(3));
                    if(productionSchedule.ProductionDate == productionDate)
                    {
                        expected.Add(productionSchedule);
                    }
                }

                //Act
                var result = Service.GetProductionSchedules(new FilterProductionScheduleParameters
                    {
                        ProductionDate = productionDate
                    });

                //Assert
                expected.AssertEquivalent(result.ResultingObject.ToList(), e => e.ToProductionScheduleKey().KeyValue, r => r.ProductionScheduleKey,
                    (e, r) => e.AssertEquivalent(r));
            }

            [Test]
            public void Filters_results_by_ProductionLine()
            {
                //Arrange
                var productionLine = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine);
                var expected = new List<ProductionSchedule>();
                for(var i = 0; i < 9; ++i)
                {
                    TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionSchedule>(p =>
                        {
                            p.ScheduledItems = TestHelper.List<ProductionScheduleItem>(3);
                            if(i % 3 == 0)
                            {
                                p.SetProductionLine(productionLine);
                                expected.Add(p);
                            }
                        });
                }

                //Act
                var result = Service.GetProductionSchedules(new FilterProductionScheduleParameters
                    {
                        ProductionLineLocationKey = productionLine.ToLocationKey()
                    });

                //Assert
                expected.AssertEquivalent(result.ResultingObject.ToList(), e => e.ToProductionScheduleKey().KeyValue, r => r.ProductionScheduleKey,
                    (e, r) => e.AssertEquivalent(r));
            }
        }
    }
}