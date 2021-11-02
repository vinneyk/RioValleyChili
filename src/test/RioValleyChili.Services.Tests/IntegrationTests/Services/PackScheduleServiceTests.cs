using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.Helpers.ParameterExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using Solutionhead.Services;
using SetPickedInventoryParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SetPickedInventoryParameters;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class PackScheduleServiceTests : ServiceIntegrationTestBase<ProductionService>
    {
        [TestFixture]
        public class CreatePackSchedule : PackScheduleServiceTests
        {
            [Test]
            public void Creates_PackSchedule_record_with_null_OrderNumber_and_Customer_reference_as_expected_on_success()
            {
                //Arrange
                const int expectedSequence = 2;
                var expectedDate = TimeStamper.CurrentTimeStamp.Date;
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.DateCreated = expectedDate, p => p.SequentialNumber = expectedSequence - 1);
                
                var parameters = new CreatePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        ScheduledProductionDate = new DateTime(2013, 3, 29),
                        WorkTypeKey = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>()),
                        ChileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()),
                        PackagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()),
                        ProductionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine)),
                        SummaryOfWork = "BadgerBadgerBadgerBadger",
                        BatchTargetAsta = 1,
                        BatchTargetScan = 22,
                        BatchTargetScoville = 333,
                        BatchTargetWeight = 500
                    };

                //Act
                var result = Service.CreatePackSchedule(parameters);

                //Assert
                result.AssertSuccess();
                var packSchedule = RVCUnitOfWork.PackScheduleRepository.Filter(p => p.DateCreated == expectedDate && p.SequentialNumber == expectedSequence, p => p.Employee).Single();
                parameters.AssertAsExpected(packSchedule);
            }

            [Test]
            public void Creates_PackSchedule_record_with_Customer_reference_and_OrderNumber_as_expected_on_success()
            {
                //Arrange
                const int expectedSequence = 2;
                var expectedDate = TimeStamper.CurrentTimeStamp.Date;
                const string expectedOrderNumber = "NUMBILICIOUS!";
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.DateCreated = expectedDate, p => p.SequentialNumber = expectedSequence - 1);
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Company.SetCompanyTypes(CompanyType.Customer).EmptyItems());

                var parameters = new CreatePackScheduleParameters
                {
                    UserToken = TestUser.UserName,
                    ScheduledProductionDate = new DateTime(2013, 3, 29),
                    WorkTypeKey = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>()),
                    ChileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()),
                    PackagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()),
                    ProductionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine)),
                    SummaryOfWork = "BadgerBadgerBadgerBadger",
                    BatchTargetAsta = 1,
                    BatchTargetScan = 22,
                    BatchTargetScoville = 333,
                    BatchTargetWeight = 500,
                    CustomerKey = new CompanyKey(customer),
                    OrderNumber = expectedOrderNumber
                };

                //Act
                var result = Service.CreatePackSchedule(parameters);

                //Assert
                result.AssertSuccess();
                var packSchedule = RVCUnitOfWork.PackScheduleRepository.Filter(p => p.DateCreated == expectedDate && p.SequentialNumber == expectedSequence, p => p.Employee, p => p.Customer).Single();
                parameters.AssertAsExpected(packSchedule);
            }

            [Test]
            public void Returns_non_successful_result_if_Company_is_not_of_Customer_type()
            {
                //Arrange
                const int expectedSequence = 2;
                var expectedDate = TimeStamper.CurrentTimeStamp.Date;
                const string expectedOrderNumber = "NUMBILICIOUS!";
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.DateCreated = expectedDate, p => p.SequentialNumber = expectedSequence - 1);
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Company.SetCompanyTypes(CompanyType.Broker).EmptyItems());

                var parameters = new CreatePackScheduleParameters
                {
                    UserToken = TestUser.UserName,
                    ScheduledProductionDate = new DateTime(2013, 3, 29),
                    WorkTypeKey = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>()),
                    ChileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()),
                    PackagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()),
                    ProductionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine)),
                    SummaryOfWork = "BadgerBadgerBadgerBadger",
                    BatchTargetAsta = 1,
                    BatchTargetScan = 22,
                    BatchTargetScoville = 333,
                    BatchTargetWeight = 500,
                    CustomerKey = new CompanyKey(customer),
                    OrderNumber = expectedOrderNumber
                };

                //Act
                var result = Service.CreatePackSchedule(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.CompanyNotOfType);
            }
        }

        [TestFixture]
        public class UpdatePackSchedule : PackScheduleServiceTests
        {
            [Test]
            public void Returns_non_succesful_result_if_PackSchedule_could_not_be_found()
            {
                //Arrange
                var parameters = new UpdatePackScheduleParameters
                    {
                        PackScheduleKey = new PackScheduleKey(PackScheduleKey.Null),
                        WorkTypeKey = new WorkTypeKey(WorkTypeKey.Null),
                        ChileProductKey = new ChileProductKey(ChileProductKey.Null),
                        PackagingProductKey = new PackagingProductKey(PackagingProductKey.Null)
                    };

                //Act
                var result = Service.UpdatePackSchedule(parameters);

                //Assert
                result.AssertNotSuccess();
            }

            [Test]
            public void Updates_PackSchedule_setting_OrderNumber_and_Customer_reference_to_null_on_success()
            {
                //Arrange
                var packScheduleKey = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>());
                var parameters = new UpdatePackScheduleParameters
                    {
                        PackScheduleKey = packScheduleKey,
                        UserToken = TestUser.UserName,
                        WorkTypeKey = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>()),
                        ChileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()),
                        PackagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()),
                        ScheduledProductionDate = new DateTime(2012, 3, 29),
                        ProductionDeadline = new DateTime(2013, 4, 1),
                        ProductionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine)),
                        SummaryOfWork = "This pretty much sums it up.",
                        BatchTargetWeight = 44,
                        BatchTargetAsta = 11,
                        BatchTargetScan = 22,
                        BatchTargetScoville = 33
                    };

                //Act
                var result = Service.UpdatePackSchedule(parameters);

                //Assert
                result.AssertSuccess();
                parameters.AssertAsExpected(RVCUnitOfWork.PackScheduleRepository.FindByKey(packScheduleKey, p => p.Employee));
            }

            [Test]
            public void Updates_PackSchedule_setting_OrderNumber_and_Customer_reference_as_expected_on_sucess()
            {
                //Arrange
                const string expectedOrderNumber = "Order this!";
                var expectedCustomer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Company.SetCompanyTypes(CompanyType.Customer));
                var packScheduleKey = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.SetCustomerKey(null).OrderNumber = null));
                var parameters = new UpdatePackScheduleParameters
                {
                    PackScheduleKey = packScheduleKey,
                    UserToken = TestUser.UserName,
                    WorkTypeKey = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>()),
                    ChileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()),
                    PackagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()),
                    ScheduledProductionDate = new DateTime(2012, 3, 29),
                    ProductionDeadline = new DateTime(2013, 4, 1),
                    ProductionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine)),
                    SummaryOfWork = "This pretty much sums it up.",
                    BatchTargetWeight = 44,
                    BatchTargetAsta = 11,
                    BatchTargetScan = 22,
                    BatchTargetScoville = 33,
                    OrderNumber = expectedOrderNumber,
                    CustomerKey = new CompanyKey(expectedCustomer)
                };

                

                //Act
                var result = Service.UpdatePackSchedule(parameters);

                //Assert
                result.AssertSuccess();
                parameters.AssertAsExpected(RVCUnitOfWork.PackScheduleRepository.FindByKey(packScheduleKey, p => p.Employee));
            }

            [Test]
            public void Returns_non_successful_result_if_Company_is_not_of_Customer_type()
            {
                //Arrange
                const string expectedOrderNumber = "Order this!";
                var expectedCustomer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Company.SetCompanyTypes(CompanyType.Broker));
                var packScheduleKey = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.SetCustomerKey(null).OrderNumber = null));
                var parameters = new UpdatePackScheduleParameters
                {
                    PackScheduleKey = packScheduleKey,
                    UserToken = TestUser.UserName,
                    WorkTypeKey = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>()),
                    ChileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()),
                    PackagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()),
                    ScheduledProductionDate = new DateTime(2012, 3, 29),
                    ProductionDeadline = new DateTime(2013, 4, 1),
                    ProductionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine)),
                    SummaryOfWork = "This pretty much sums it up.",
                    BatchTargetWeight = 44,
                    BatchTargetAsta = 11,
                    BatchTargetScan = 22,
                    BatchTargetScoville = 33,
                    OrderNumber = expectedOrderNumber,
                    CustomerKey = new CompanyKey(expectedCustomer)
                };

                //Act
                var result = Service.UpdatePackSchedule(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.CompanyNotOfType);
            }

            [Test]
            public void Returns_non_successful_result_if_changing_ChileProduct_and_a_batch_has_been_completed()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = new List<ProductionBatch>
                    {
                        TestHelper.CreateObjectGraph<ProductionBatch>(b => b.SetToNotCompleted()),
                        TestHelper.CreateObjectGraph<ProductionBatch>(b => b.SetToNotCompleted()),
                        TestHelper.CreateObjectGraph<ProductionBatch>(b => b.ProductionHasBeenCompleted = true),
                    });

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ChileState = packSchedule.ChileProduct.ChileState);

                //Act
                var result = Service.UpdatePackSchedule(new UpdatePackScheduleParameters(packSchedule)
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = new ChileProductKey(chileProduct)
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchAlreadyComplete);
            }

            [Test]
            public void Returns_non_successful_result_if_new_ChileProduct_results_in_different_LotType_and_batches_exist()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ChileProduct.ChileState = ChileStateEnum.FinishedGoods,
                    p => p.ProductionBatches = new List<ProductionBatch>
                        {
                            TestHelper.CreateObjectGraph<ProductionBatch>(b => b.SetToNotCompleted()),
                            TestHelper.CreateObjectGraph<ProductionBatch>(b => b.SetToNotCompleted())
                        });

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ChileState = ChileStateEnum.WIP);

                //Act
                var result = Service.UpdatePackSchedule(new UpdatePackScheduleParameters(packSchedule)
                {
                    UserToken = TestUser.UserName,
                    ChileProductKey = new ChileProductKey(chileProduct)
                });

                //Assert
                result.AssertNotSuccess(UserMessages.ChileProductDifferentLotType);
            }

            [Test]
            public void Updates_PackSchedule_and_associated_lot_Chile_and_Packaging_products()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ChileProduct.ChileState = ChileStateEnum.FinishedGoods,
                    p => p.ProductionBatches = new List<ProductionBatch>
                        {
                            TestHelper.CreateObjectGraph<ProductionBatch>(b => b.SetToNotCompleted()),
                            TestHelper.CreateObjectGraph<ProductionBatch>(b => b.SetToNotCompleted())
                        });

                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ChileState = ChileStateEnum.FinishedGoods));
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());

                //Act
                var result = Service.UpdatePackSchedule(new UpdatePackScheduleParameters(packSchedule)
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = chileProductKey,
                        PackagingProductKey = packagingProductKey
                    });

                //Assert
                result.AssertSuccess();
                ResetUnitOfWork();
                packSchedule = RVCUnitOfWork.PackScheduleRepository.FindByKey(new PackScheduleKey(packSchedule), p => p.ProductionBatches.Select(b => b.Production.ResultingChileLot.Lot));
                Assert.AreEqual(chileProductKey, packSchedule);
                Assert.AreEqual(packagingProductKey, packSchedule);
                foreach(var chileLot in packSchedule.ProductionBatches.Select(b => b.Production.ResultingChileLot))
                {
                    Assert.AreEqual(chileProductKey, chileLot);
                    Assert.AreEqual(packagingProductKey, chileLot.Lot);
                }
            }
        }

        [TestFixture]
        public class RemovePackSchedule : PackScheduleServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_PackSchedule_could_not_be_found()
            {
                //Arrange
                var moqKey = new Mock<IPackScheduleKey>();
                moqKey.Setup(m => m.PackScheduleKey_DateCreated).Returns(new DateTime(2012, 3, 29));
                moqKey.Setup(m => m.PackScheduleKey_DateSequence).Returns(1);
                var packScheduleKey = new PackScheduleKey(moqKey.Object);

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = packScheduleKey
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.PackScheduleNotFound);
            }

            [Test]
            public void Returns_non_sucessful_result_if_PackSchedule_contains_ProductionBatches_that_have_been_completed()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = new List<ProductionBatch>
                    {
                        TestHelper.CreateObjectGraph<ProductionBatch>(b => b.ProductionHasBeenCompleted = true)
                    });
                var packScheduleKey = new PackScheduleKey(packSchedule);

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                {
                    UserToken = TestUser.UserName,
                    PackScheduleKey = packScheduleKey
                });

                //Assert
                result.AssertNotSuccess(UserMessages.PackScheduleRemoveFail_LotCompleted);
            }

            [Test]
            public void Returns_non_sucessful_result_if_PackSchedule_contains_ProductionBatches_with_ProductionResults()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = new List<ProductionBatch>
                    {
                        TestHelper.CreateObjectGraph<ProductionBatch>(b => b.PackSchedule = null, b => b.ProductionHasBeenCompleted = false)
                    });
                var packScheduleKey = new PackScheduleKey(packSchedule);

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                {
                    UserToken = TestUser.UserName,
                    PackScheduleKey = packScheduleKey
                });

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchHasResult);
            }

            [Test]
            public void Returns_non_successful_result_if_PackSchedule_contains_ProductionBatches_with_Output_Lots_that_contain_Inventory()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = new List<ProductionBatch>
                    {
                        TestHelper.CreateObjectGraph<ProductionBatch>(b => b.PackSchedule = null, b => b.Production.Results = null, b => b.ProductionHasBeenCompleted = false,
                        b => b.Production.ResultingChileLot.Lot.Inventory = new List<Inventory>
                            {
                                TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null)
                            })
                    });
                var packScheduleKey = new PackScheduleKey(packSchedule);

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                {
                    UserToken = TestUser.UserName,
                    PackScheduleKey = packScheduleKey
                });

                //Assert
                result.AssertNotSuccess(UserMessages.LotHasExistingInventory);
            }

            [Test]
            public void Returns_non_successful_result_if_PackSchedule_contains_ProductionBatches_with_PickedInventoryItems_where_the_current_Location_is_not_equal_to_their_source_Location()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = new List<ProductionBatch>
                    {
                        TestHelper.CreateObjectGraph<ProductionBatch>(b =>
                            {
                                b.ProductionHasBeenCompleted = false;
                                b.Production.Results = null;
                                b.Production.ResultingChileLot.Lot.Inventory = null;
                                b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                                    {
                                        TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null)
                                    };
                            })
                    });
                var packScheduleKey = new PackScheduleKey(packSchedule);

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                {
                    UserToken = TestUser.UserName,
                    PackScheduleKey = packScheduleKey
                });

                //Assert
                result.AssertNotSuccess(UserMessages.PickedInventoryItemNotInOriginalLocation);
            }

            [Test, Issue("Have been given direction to remove instances where the PackSchedule has been scheduled for production. -RI 2016-12-05",
                References = new [] { "RVCADMIN-1414" })]
            public void Returns_successful_result_if_PackSchedule_has_been_scheduled_for_production()
            {
                //Arrange
                var scheduled = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionScheduleItem>();
                var packScheduleKey = scheduled.ToPackScheduleKey();

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = packScheduleKey
                    });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.ProductionScheduleItemRepository.FindByKey(scheduled.ToProductionScheduleItemKey()));
            }

            [Test]
            public void Removes_PackSchedule_and_associated_records_from_database_on_success()
            {
                //Arrange
                Func<ProductionBatch> createProductionBatch = () => TestHelper.CreateObjectGraph<ProductionBatch>(
                    b => b.PackSchedule = null,
                    b => b.Production.Results = null,
                    b => b.ProductionHasBeenCompleted = false,
                    b => b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocationToSource()),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocationToSource()),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocationToSource())
                        },
                    b => b.InstructionNotebook.Notes = new List<Note>
                        {
                            TestHelper.CreateObjectGraph<Note>(n => n.Notebook = null),
                            TestHelper.CreateObjectGraph<Note>(n => n.Notebook = null),
                            TestHelper.CreateObjectGraph<Note>(n => n.Notebook = null)
                        });

                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(
                    p => p.ProductionBatches = new List<ProductionBatch>
                        {
                            createProductionBatch(),
                            createProductionBatch(),
                            createProductionBatch()
                        });

                var packScheduleKey = new PackScheduleKey(packSchedule);
                var productionBatchKeys = packSchedule.ProductionBatches.Select(b => new LotKey(b));
                var pickedInventoryKeys = packSchedule.ProductionBatches.Select(b => new PickedInventoryKey(b.Production));
                var pickedInventoryItemKeys = packSchedule.ProductionBatches.SelectMany(b => b.Production.PickedInventory.Items.Select(i => new PickedInventoryItemKey(i)));
                var productionBatchInstructionReferenceKeys = packSchedule.ProductionBatches.SelectMany(b => b.InstructionNotebook.Notes.Select(n => new NoteKey(n)));

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = packScheduleKey
                    });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.PackScheduleRepository.FindByKey(packScheduleKey));
                productionBatchKeys.ForEach(k => Assert.IsNull(RVCUnitOfWork.ProductionBatchRepository.FindByKey(k)));
                productionBatchInstructionReferenceKeys.ForEach(k => Assert.IsNull(RVCUnitOfWork.NoteRepository.FindByKey(k)));
                pickedInventoryKeys.ForEach(k => Assert.IsNull(RVCUnitOfWork.PickedInventoryRepository.FindByKey(k)));
                pickedInventoryItemKeys.ForEach(k => Assert.IsNull(RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(k)));
            }

            [Test]
            public void Removes_PackSchedule_and_restores_Inventory_picked_multiple_times_for_different_ProductionBatches_as_expected_on_sucess()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = null);
                var packScheduleKey = new PackScheduleKey(packSchedule);
                var inventory0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();

                var productionBatch0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.EmptyItems().SetToNotCompleted().ConstrainByKeys(packSchedule));
                var pickedInventory0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(productionBatch0.Production).SetToInventory(inventory0));

                var productionBatch1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.EmptyItems().SetToNotCompleted().ConstrainByKeys(packSchedule));
                var pickedInventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(productionBatch1.Production).SetToInventory(inventory0));
                
                var expectedInventory0Quantity = inventory0.Quantity + pickedInventory0.Quantity + pickedInventory1.Quantity;

                

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = packScheduleKey
                    });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.PackScheduleRepository.FindByKey(packScheduleKey));
                Assert.AreEqual(expectedInventory0Quantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory0)).Quantity);
            }
        }

        [TestFixture]
        public class GetPackSchedules : PackScheduleServiceTests
        {
            [Test]
            public void Returns_empty_collection_if_no_PackSchedules_exist()
            {
                //Act
                StartStopwatch();
                var result = Service.GetPackSchedules();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(0, results.Count());
            }

            [Test]
            public void Returns_PackSchedules_with_DateCreated_as_the_Date_portion_of_the_PackScheduleKey_DateCreated_property()
            {
                //Arrange
                var dateCreated0 = new DateTime(2012, 03, 29, 1, 23, 45);
                var dateCreated1 = new DateTime(2012, 4, 1, 2, 22, 32);
                var dateCreated2 = new DateTime(1880, 1, 1, 1, 12, 30, 00);
                var packScheduleKey0 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.DateCreated = dateCreated0));
                var packScheduleKey1 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.DateCreated = dateCreated1));
                var packScheduleKey2 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.DateCreated = dateCreated2));

                //Act
                StartStopwatch();
                var result = Service.GetPackSchedules();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(dateCreated0.Date, results.Single(r => r.PackScheduleKey == packScheduleKey0.KeyValue).DateCreated);
                Assert.AreEqual(dateCreated1.Date, results.Single(r => r.PackScheduleKey == packScheduleKey1.KeyValue).DateCreated);
                Assert.AreEqual(dateCreated2.Date, results.Single(r => r.PackScheduleKey == packScheduleKey2.KeyValue).DateCreated);
            }

            [Test]
            public void Returns_PackSchedules_with_ChileProductNames_as_expected()
            {
                //Arrange
                const string chileName0 = "Chili is what you mean.";
                const string chileName1 = "Chile is the actual pepper.";
                const string chileName2 = "It is grown, not manufactured.";
                var packScheduleKey0 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ChileProduct.Product.Name = chileName0));
                var packScheduleKey1 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ChileProduct.Product.Name = chileName1));
                var packScheduleKey2 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ChileProduct.Product.Name = chileName2));

                //Act
                StartStopwatch();
                var result = Service.GetPackSchedules();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(chileName0, results.Single(r => r.PackScheduleKey == packScheduleKey0.KeyValue).ChileProductName);
                Assert.AreEqual(chileName1, results.Single(r => r.PackScheduleKey == packScheduleKey1.KeyValue).ChileProductName);
                Assert.AreEqual(chileName2, results.Single(r => r.PackScheduleKey == packScheduleKey2.KeyValue).ChileProductName);
            }

            [Test]
            public void Returns_PackSchedules_with_ProductionDeadLines_as_expected()
            {
                //Arrange
                DateTime? deadline0 = null;
                DateTime? deadline1 = new DateTime(2012, 3, 29);
                DateTime? deadline2 = DateTime.UtcNow.Date;

                var packScheduleKey0 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionDeadline = deadline0));
                var packScheduleKey1 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionDeadline = deadline1));
                var packScheduleKey2 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionDeadline = deadline2));

                //Act
                StartStopwatch();
                var result = Service.GetPackSchedules();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(deadline0, results.Single(r => r.PackScheduleKey == packScheduleKey0.KeyValue).ProductionDeadline);
                Assert.AreEqual(deadline1, results.Single(r => r.PackScheduleKey == packScheduleKey1.KeyValue).ProductionDeadline);
                Assert.AreEqual(deadline2, results.Single(r => r.PackScheduleKey == packScheduleKey2.KeyValue).ProductionDeadline);
            }

            [Test]
            public void Returns_PackSchedules_with_Customer_information_as_expected_on_success()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems());
                var packScheduleKey0 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>());
                var packScheduleKey1 = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.SetCustomerKey(customer)));

                //Act
                StartStopwatch();
                var result = Service.GetPackSchedules();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.IsNull(results.Single(r => r.PackScheduleKey == packScheduleKey0).Customer);
                Assert.AreEqual(customer.Company.Name, results.Single(r => r.PackScheduleKey == packScheduleKey1).Customer.Name);
            }

            [Test]
            public void Returns_PackSchedules_with_ProductionLine_information_as_expected()
            {
                //Arrange
                const string line0 = "Line 0";
                const string line1 = "Line1";
                const string line2 = "OH THE HORROR!!!";

                var packSchedule0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionLineLocation.Description = line0);
                var packScheduleKey0 = new PackScheduleKey(packSchedule0);
                var productionLineKey0 = new LocationKey(packSchedule0);
                var packSchedule1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionLineLocation.Description = line1);
                var packScheduleKey1 = new PackScheduleKey(packSchedule1);
                var productionLineKey1 = new LocationKey(packSchedule1);
                var packSchedule2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionLineLocation.Description = line2);
                var packScheduleKey2 = new PackScheduleKey(packSchedule2);
                var productionLineKey2 = new LocationKey(packSchedule2);

                //Act
                StartStopwatch();
                var result = Service.GetPackSchedules();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                var result0 = results.Single(p => p.PackScheduleKey == packScheduleKey0.KeyValue);
                Assert.AreEqual(productionLineKey0.KeyValue, result0.ProductionLineKey);
                Assert.AreEqual(line0, result0.ProductionLineDescription);

                var result1 = results.Single(p => p.PackScheduleKey == packScheduleKey1.KeyValue);
                Assert.AreEqual(productionLineKey1.KeyValue, result1.ProductionLineKey);
                Assert.AreEqual(line1, result1.ProductionLineDescription);

                var result2 = results.Single(p => p.PackScheduleKey == packScheduleKey2.KeyValue);
                Assert.AreEqual(productionLineKey2.KeyValue, result2.ProductionLineKey);
                Assert.AreEqual(line2, result2.ProductionLineDescription);
            }
        }

        [TestFixture]
        public class GetPackSchedule : PackScheduleServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_PackSchedule_could_not_be_found()
            {
                //Arrange
                var mockPackScheduleKey = new Mock<IPackScheduleKey>();
                mockPackScheduleKey.Setup(m => m.PackScheduleKey_DateCreated).Returns(new DateTime(2012, 3, 29));
                mockPackScheduleKey.Setup(m => m.PackScheduleKey_DateSequence).Returns(1);
                var packScheduleKey = new PackScheduleKey(mockPackScheduleKey.Object);

                //Act
                var result = TimedExecution(() => Service.GetPackSchedule(packScheduleKey.KeyValue), "Act");

                //Assert
                result.AssertNotSuccess(UserMessages.PackScheduleNotFound);
            }

            [Test]
            public void Returns_PackagingProduct_Key_Name_and_weight_as_expected()
            {
                //Arrange
                const float expectedPackagingWeight = 25.43f;
                const string expectedPackagingName = "I am a package. Hear me roar.";
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.PackagingProduct.Weight = expectedPackagingWeight, p => p.PackagingProduct.Product.Name = expectedPackagingName);
                var packacgingProductKey = new PackagingProductKey(packSchedule);
                var packScheduleKey = new PackScheduleKey(packSchedule);

                //Act
                var result = TimedExecution(() => Service.GetPackSchedule(packScheduleKey.KeyValue), "Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(packacgingProductKey.KeyValue, result.ResultingObject.PackagingProductKey);
                Assert.AreEqual(expectedPackagingName, result.ResultingObject.PackagingProductName);
                Assert.AreEqual(expectedPackagingWeight, result.ResultingObject.PackagingWeight);
            }

            [Test]
            public void Returns_PackSchedule_with_expected_information_on_success()
            {
                //Arrange
                const int targetWeight = 100;
                const int packagingWeight = 20;
                const int numberOfPackagingUnits = 15;
                const int expectedBatchTargetWeight = packagingWeight * numberOfPackagingUnits;
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(
                    p => p.DefaultBatchTargetParameters.BatchTargetWeight = targetWeight,
                    p => p.PackagingProduct.Weight = packagingWeight,
                    p => p.ProductionBatches = new List<ProductionBatch>
                        {
                            TestHelper.CreateObjectGraph<ProductionBatch>(b => b.PackSchedule = null, b => b.TargetParameters.BatchTargetWeight = expectedBatchTargetWeight),
                            TestHelper.CreateObjectGraph<ProductionBatch>(b => b.PackSchedule = null, b => b.TargetParameters.BatchTargetWeight = expectedBatchTargetWeight),
                            TestHelper.CreateObjectGraph<ProductionBatch>(b => b.PackSchedule = null, b => b.TargetParameters.BatchTargetWeight = expectedBatchTargetWeight)
                        });
                var packScheduleKey = new PackScheduleKey(packSchedule);
                var productionBatchKeys = packSchedule.ProductionBatches.ToList();

                //Act
                var result = TimedExecution(() => Service.GetPackSchedule(packScheduleKey.KeyValue), "Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(packScheduleKey.KeyValue, result.ResultingObject.PackScheduleKey);
                packSchedule.DefaultBatchTargetParameters.AssertAsExpected(result.ResultingObject.TargetParameters);
                productionBatchKeys.ForEach(b => b.TargetParameters.AssertAsExpected(result.ResultingObject.ProductionBatches.Single(r => r.ProductionBatchKey == new LotKey(b))));
            }
        }

        [TestFixture]
        public class CreateProductionBatch : PackScheduleServiceTests
        {
            [Test]
            public void Returns_expected_ProductionBatch_and_InstructionNotebook_on_success()
            {
                //Arrange
                var packScheduleKey = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = null, p => p.ChileProduct.ChileState = ChileStateEnum.FinishedGoods));
                var instructions = new[]
                    {
                        "Instruction0",
                        "Instruction1," +
                        "Hi mom!"
                    };
                
                var parameters = new CreateProductionBatchParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = packScheduleKey,
                        Instructions = instructions
                    };

                //Act
                var result = Service.CreateProductionBatch(parameters);

                //Assert
                result.AssertSuccess();
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.Filter(b => true, b => b.InstructionNotebook.Notes).Single();
                Assert.AreEqual(new LotKey(productionBatch).KeyValue, result.ResultingObject.ProductionBatchKey);
                Assert.AreEqual(new NotebookKey(productionBatch.InstructionNotebook).KeyValue, result.ResultingObject.InstructionNotebookKey);
                instructions.ForEach(i => Assert.AreEqual(1, productionBatch.InstructionNotebook.Notes.Count(n => n.Text == i)));
            }

            [Test]
            public void Creates_ProductionBatch_and_associated_records_in_database_as_expected_on_success()
            {
                //Arrange
                const int expectedPickedSequence = 1;
                const int expectedChileLotSequence = 1;
                const int expectedChileLotTypeId = (int)LotTypeEnum.FinishedGood;
                const string expectedNotes = "These are my notes.";

                var date = TimeStamper.CurrentTimeStamp.Date;
                var packScheduleKey = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = null, p => p.ChileProduct.ChileState = ChileStateEnum.FinishedGoods));
                var parameters = new CreateProductionBatchParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = packScheduleKey,
                        Notes = expectedNotes
                    };

                //Act
                var result = Service.CreateProductionBatch(parameters);

                //Assert
                result.AssertSuccess();
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository
                    .Filter(b => b.PackScheduleDateCreated == packScheduleKey.PackScheduleKey_DateCreated && b.PackScheduleSequence == packScheduleKey.PackScheduleKey_DateSequence, b => b.Production.ResultingChileLot.Lot)
                    .FirstOrDefault();
                Assert.IsNotNull(productionBatch);
                Assert.AreEqual(expectedNotes, productionBatch.Production.ResultingChileLot.Lot.Notes);
                Assert.IsNotNull(RVCUnitOfWork.ChileLotRepository.All().Single(c => c.LotDateCreated == date && c.LotDateSequence == expectedChileLotSequence && c.LotTypeId == expectedChileLotTypeId));
                Assert.IsNotNull(RVCUnitOfWork.PickedInventoryRepository.All().Single(p => p.DateCreated == date && p.Sequence == expectedPickedSequence && p.PickedReason == PickedReason.Production));
            }

            [Test]
            public void Creates_OutputChileLot_for_ProductionBatch_as_expected()
            {
                //Arrange
                var packScheduleKey = new PackScheduleKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = null,
                    p => p.ChileProduct.ChileState = ChileStateEnum.FinishedGoods));
                var parameters = new CreateProductionBatchParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = packScheduleKey
                    };

                //Act
                var result = Service.CreateProductionBatch(parameters);

                //Assert
                result.AssertSuccess();
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.All().Single(b => b.PackScheduleDateCreated == packScheduleKey.PackScheduleKey_DateCreated && b.PackScheduleSequence == packScheduleKey.PackScheduleKey_DateSequence);
                var lotKey = new LotKey(productionBatch);
                var chileLot = RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey, c => c.Lot);

                Assert.AreEqual(LotQualityStatus.Pending, chileLot.Lot.QualityStatus);
                Assert.AreEqual(LotProductionStatus.Batched, chileLot.Lot.ProductionStatus);
                Assert.AreEqual(false, chileLot.AllAttributesAreLoBac);
            }
        }

        [TestFixture]
        public class UpdateProductionBatch : PackScheduleServiceTests
        {
            [Test]
            public void Returns_non_succesful_result_if_ProductionBatch_could_not_be_found()
            {
                //Arrange
                var parameters = new UpdateProductionBatchParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = new LotKey(LotKey.Null)
                    };

                //Act
                var result = Service.UpdateProductionBatch(parameters);

                //Assert
                result.AssertNotSuccess();
            }

            [Test]
            public void Updates_ProductionBatch_record_in_database_as_expected()
            {
                //Arrange
                var expectedUser = TestUser;
                var productionBatchKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>());
                const string expectedNotes = "Notey-note-notes.";
                var parameters = new UpdateProductionBatchParameters
                    {
                        UserToken = expectedUser.UserName,
                        ProductionBatchKey = productionBatchKey,
                        BatchTargetWeight = 1234,
                        BatchTargetAsta = 0.4321,
                        BatchTargetScan = 1.234,
                        BatchTargetScoville = 5.312,
                        Notes = expectedNotes
                    };

                //Act
                var result = Service.UpdateProductionBatch(parameters);

                //Assert
                result.AssertSuccess();
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.FindByKey(productionBatchKey, b => b.Production.ResultingChileLot.Lot);
                Assert.IsNotNull(productionBatch);
                Assert.AreEqual(expectedUser.EmployeeId, productionBatch.EmployeeId);
                Assert.AreEqual(expectedNotes, productionBatch.Production.ResultingChileLot.Lot.Notes);
                parameters.AssertAsExpected(productionBatch.TargetParameters);
            }
        }

        [TestFixture]
        public class RemoveProductionBatch : PackScheduleServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ProductionBatch_could_not_be_found()
            {
                //Arrange
                var moqKey = new Mock<ILotKey>();
                moqKey.Setup(m => m.LotKey_DateCreated).Returns(new DateTime(2012, 3, 29));
                moqKey.Setup(m => m.LotKey_DateSequence).Returns(1);
                moqKey.Setup(m => m.LotKey_LotTypeId).Returns(2);
                var productionBatchKey = new LotKey(moqKey.Object);

                //Act
                var result = Service.RemoveProductionBatch(productionBatchKey.KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_ProductionBatch_has_been_completed()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ProductionHasBeenCompleted = true);
                var productionBatchKey = new LotKey(productionBatch);

                //Act
                var result = Service.RemoveProductionBatch(productionBatchKey.KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchAlreadyComplete);
            }

            [Test]
            public void Returns_non_successful_result_if_ProductionBatch_has_ProductionResults()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ProductionHasBeenCompleted = false);
                var productionBatchKey = new LotKey(productionBatch);

                //Act
                var result = Service.RemoveProductionBatch(productionBatchKey.KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchHasResult);
            }

            [Test]
            public void Returns_non_successful_result_if_any_PickedInventoryItem_for_the_ProductionBatch_is_not_in_its_original_location()
            {
                //Arrange
                var currentLocation = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null,
                    b => b.Production.ResultingChileLot.Lot.Inventory = null,
                    b => b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocation(currentLocation)),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocation(currentLocation)),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocation(currentLocation))
                        });
                var productionBatchKey = new LotKey(productionBatch);

                //Act
                var result = Service.RemoveProductionBatch(productionBatchKey.KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.PickedInventoryItemNotInOriginalLocation);
            }

            [Test]
            public void Returns_non_successful_result_if_the_OutputLot_has_existing_Inventory_items()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null,
                    b => b.Production.ResultingChileLot.Lot.Inventory = new List<Inventory>
                        {
                            TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null)
                        });
                var productionBatchKey = new LotKey(productionBatch);

                //Act
                var result = Service.RemoveProductionBatch(productionBatchKey.KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.LotHasExistingInventory);
            }

            [Test]
            public void Removes_ProductionBatch_and_associated_records_from_database_on_success()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(
                    b => b.Production.ResultingChileLot.Lot.Inventory = null,
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null,
                    b => b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocationToSource()),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocationToSource()),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null, i => i.SetCurrentLocationToSource())
                        },
                    b => b.InstructionNotebook.Notes = new List<Note>
                        {
                            TestHelper.CreateObjectGraph<Note>(n => n.Notebook = null),
                            TestHelper.CreateObjectGraph<Note>(n => n.Notebook = null),
                            TestHelper.CreateObjectGraph<Note>(n => n.Notebook = null)
                        });
                var lotKey = new LotKey(productionBatch);
                var instructionReferenceKeys = productionBatch.InstructionNotebook.Notes.Select(n => new NoteKey(n));

                var pickedInventoryKey = new PickedInventoryKey(productionBatch.Production);
                var pickedInventoryItemKeys = productionBatch.Production.PickedInventory.Items.Select(i => new PickedInventoryItemKey(i));

                //Act
                var result = Service.RemoveProductionBatch(lotKey);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.ProductionBatchRepository.FindByKey(lotKey));
                instructionReferenceKeys.ForEach(k => Assert.IsNull(RVCUnitOfWork.NoteRepository.FindByKey(k)));
                Assert.IsNull(RVCUnitOfWork.PickedInventoryRepository.FindByKey(pickedInventoryKey));
                pickedInventoryItemKeys.ForEach(k => Assert.IsNull(RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(k)));
                Assert.IsNull(RVCUnitOfWork.LotRepository.FindByKey(lotKey));
                Assert.IsNull(RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey));
                Assert.IsNull(RVCUnitOfWork.ChileLotProductionRepository.FindByKey(lotKey));
            }

            [Test]
            public void Places_PickedInventoryItems_for_ProductionBatch_back_into_Inventory()
            {
                //Arrange
                const int pickedQuantity0 = 20;
                const int pickedQuantity1 = 33;
                const int existingInventoryQuantity = 10;
                const int expectedInventoryQuantity0 = existingInventoryQuantity + pickedQuantity0;
                const int expectedInventoryQuantity1 = pickedQuantity1;

                var exisitingInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = existingInventoryQuantity);

                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.EmptyItems().SetToNotCompleted());
                var productionBatchKey = new LotKey(productionBatch);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(productionBatch.Production).SetToInventory(exisitingInventory),
                    i => i.Quantity = pickedQuantity0);
                var pickedItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(productionBatch.Production),
                    i =>
                    {
                        i.CurrentLocation = null;
                        i.CurrentLocationId = i.FromLocationId;
                    }, i => i.Quantity = pickedQuantity1);
                var expectedInventoryKey0 = new InventoryKey(exisitingInventory);
                var expectedInventoryKey1 = new InventoryKey(pickedItem1);

                

                //Act
                var result = Service.RemoveProductionBatch(productionBatchKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedInventoryQuantity0, RVCUnitOfWork.InventoryRepository.FindByKey(expectedInventoryKey0).Quantity);
                Assert.AreEqual(expectedInventoryQuantity1, RVCUnitOfWork.InventoryRepository.FindByKey(expectedInventoryKey1).Quantity);
            }
        }

        [TestFixture]
        public class SetPickedInventoryForProductionBatch : SetPickedInventoryTestsBase<ProductionService, ProductionBatch, LotKey>
        {
            protected override bool SetupStaticRecords { get { return true; } }

            protected override void InitializeOrder(ProductionBatch order)
            {
                order.ProductionHasBeenCompleted = false;
            }

            protected override LotKey CreateKeyFromOrder(ProductionBatch order)
            {
                return new LotKey(order);
            }

            protected override IResult GetResult(string key, SetPickedInventoryParameters parameters)
            {
                return Service.SetPickedInventoryForProductionBatch(key, parameters);
            }

            [Test]
            public void Returns_non_successful_result_if_ProductionBatch_has_been_completed()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ProductionHasBeenCompleted = true);

                //Act
                var result = Service.SetPickedInventoryForProductionBatch(productionBatch.ToLotKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new List<IPickedInventoryItemParameters>
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = new InventoryKey(InventoryKey.Null).KeyValue,
                                        Quantity = 10
                                    }
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchAlreadyComplete);
            }

            [Test]
            public void Does_not_create_InventoryTransaction_records()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ProductionHasBeenCompleted = false);
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));

                //Act
                var result = Service.SetPickedInventoryForProductionBatch(productionBatch.ToLotKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new List<IPickedInventoryItemParameters>
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        Quantity = 1
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(0, RVCUnitOfWork.InventoryTransactionsRepository.All().Count());
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_pick_from_a_Lot_with_its_Contaminated_flag_set_to_true()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ProductionHasBeenCompleted = false, b => b.Production.PickedInventory.Items = null);
                var productionBatchKey = new LotKey(productionBatch);

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.QualityStatus = LotQualityStatus.Contaminated, i => i.Quantity = 10);

                //Act
                var result = Service.SetPickedInventoryForProductionBatch(productionBatchKey.KeyValue, new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new List<IPickedInventoryItemParameters>
                        {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = new InventoryKey(inventory).KeyValue,
                                        Quantity = 1
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickLotQualityState);
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_pick_from_a_Lot_that_has_been_set_on_Hold()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ProductionHasBeenCompleted = false, b => b.Production.PickedInventory.Items = null);
                var productionBatchKey = new LotKey(productionBatch);

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.Hold = LotHoldType.HoldForAdditionalTesting, i => i.Quantity = 10);

                //Act
                var result = Service.SetPickedInventoryForProductionBatch(productionBatchKey.KeyValue, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new List<IPickedInventoryItemParameters>
                            {
                                    new SetPickedInventoryItemParameters
                                        {
                                            InventoryKey = new InventoryKey(inventory).KeyValue,
                                            Quantity = 1
                                        }
                                }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickLotOnHold);
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_pick_from_a_Lot_that_has_not_been_produced()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ProductionHasBeenCompleted = false, b => b.Production.PickedInventory.Items = null);
                var productionBatchKey = new LotKey(productionBatch);

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.ProductionStatus = LotProductionStatus.Batched, i => i.Quantity = 10);

                //Act
                var result = Service.SetPickedInventoryForProductionBatch(productionBatchKey.KeyValue, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new List<IPickedInventoryItemParameters>
                            {
                                    new SetPickedInventoryItemParameters
                                        {
                                            InventoryKey = new InventoryKey(inventory).KeyValue,
                                            Quantity = 1
                                        }
                                }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickLotNotProduced);
            }

            [Test, Issue("Will not set computed values for attributes flagged as ActualValueRequired.",
                References = new [] { "RVCADMIN-1170" })]
            public void Updates_lot_weighted_attributes_as_expected_on_success()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.EmptyItems().SetToNotCompleted().Production.PickedInventory.Items = new List<PickedInventoryItem>
                    {
                        TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.SetCurrentLocationToSource().SetSourceWarehouse(RinconFacility).Lot.Attributes = new List<LotAttribute>
                            {
                                TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Asta, 100)),
                                TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Scan, 50))
                            }),
                        TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.SetCurrentLocationToSource().SetSourceWarehouse(RinconFacility).Lot.Attributes = new List<LotAttribute>
                            {
                                TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Asta, 100)),
                                TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Scan, 80))
                            })
                    },
                    b => b.Production.ResultingChileLot.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Granularity, 123, true))
                        });
                var previousPicked = productionBatch.Production.PickedInventory.Items.First();
                var productionBatchKey = productionBatch.ToLotKey();

                var newPicked = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility),
                    i => i.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Asta, 50)),
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Scan, 75))
                        });

                var expected = CalculateAttributeWeightedAveragesCommand.Execute(new List<PickedInventoryItem>
                    {
                        previousPicked,
                        new PickedInventoryItem
                            {
                                Lot = newPicked.Lot,
                                PackagingProduct = newPicked.PackagingProduct,
                                Quantity = newPicked.Quantity
                            }
                    }).ResultingObject
                    .Where(a => StaticAttributeNames.AttributeNames.All(n => !n.ToAttributeNameKey().Equals(a.Key.AttributeNameKey) || !n.ActualValueRequired))
                    .ToList();

                //Act
                var result = Service.SetPickedInventoryForProductionBatch(productionBatchKey, new Utilities.Models.SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new IPickedInventoryItemParameters[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = previousPicked.ToInventoryKey(),
                                        Quantity = previousPicked.Quantity
                                    }, 
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = newPicked.ToInventoryKey(),
                                        Quantity = newPicked.Quantity
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var lotAttributes = RVCUnitOfWork.ChileLotRepository.FindByKey(productionBatchKey, c => c.Lot.Attributes).Lot.Attributes.ToList();
                Assert.AreEqual(expected.Count, lotAttributes.Count);
                foreach(var attribute in expected)
                {
                    var lotAttribute = lotAttributes.Single(l => l.AttributeShortName == attribute.Key.AttributeNameKey.AttributeNameKey_ShortName);
                    Assert.IsTrue(lotAttribute.Computed);
                    Assert.Less(Math.Abs(lotAttribute.AttributeValue - lotAttribute.AttributeValue), 0.01);
                    Assert.Greater(1, (DateTime.UtcNow.Date - lotAttribute.AttributeDate).Hours);
                }
            }
        }

        [TestFixture]
        public class GetProductionBatch : PackScheduleServiceTests
        {
            [Test]
            public void Returns_non_succesful_result_if_ProductionBatch_could_not_be_found()
            {
                //Arrange
                var moqKey = new Mock<ILotKey>();
                moqKey.Setup(m => m.LotKey_DateCreated).Returns(new DateTime(2012, 3, 29));
                moqKey.Setup(m => m.LotKey_DateSequence).Returns(1);
                moqKey.Setup(m => m.LotKey_LotTypeId).Returns(2);

                //Act
                var result = TimedExecution(() => Service.GetProductionBatch(new LotKey(moqKey.Object).KeyValue), "Act");

                //Assert
                result.AssertNotSuccess();
            }

            [Test]
            public void Returns_expected_result_on_success()
            {
                //Arrange
                string productionBatchKey = null, packScheduleKey = null, outputLotKey = null;
                TimedExecution(() =>
                    {
                        var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>();
                        productionBatchKey = new LotKey(productionBatch);
                        packScheduleKey = new PackScheduleKey(productionBatch);
                        outputLotKey = new LotKey(productionBatch);
                    }, "Arrange");

                //Act
                var result = TimedExecution(() => Service.GetProductionBatch(productionBatchKey), "Act");

                //Assert
                result.AssertSuccess();
                var productionBatchDetail = result.ResultingObject;
                Assert.AreEqual(productionBatchKey, productionBatchDetail.ProductionBatchKey);
                Assert.AreEqual(packScheduleKey, productionBatchDetail.PackScheduleKey);
                Assert.AreEqual(outputLotKey, productionBatchDetail.OutputLotKey);
            }

            [Test]
            public void Returns_successful_result_if_ProductionBatch_has_empty_collections()
            {
                //Arrange
                var productionBatchKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(
                    b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.EmptyItems(),
                    b => b.PackSchedule.ChileProduct.Ingredients = null,
                    b => b.PackSchedule.ChileProduct.ProductAttributeRanges = null));

                //Act
                var result = TimedExecution(() => Service.GetProductionBatch(productionBatchKey.KeyValue), "Act");

                //Assert
                result.AssertSuccess();
            }

            [Test]
            public void Returns_Instruction_notes_as_expected_on_success()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(
                    b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.EmptyItems(),
                    b => b.InstructionNotebook.Notes = new List<Note>
                        {
                            TestHelper.CreateObjectGraph<Note>(),
                            TestHelper.CreateObjectGraph<Note>(),
                            TestHelper.CreateObjectGraph<Note>()
                        }
                    );
                var notes = productionBatch.InstructionNotebook.Notes.ToList();
                Assert.Greater(notes.Count, 0);
                var productionBatchKey = new LotKey(productionBatch);

                //Act
                var result = TimedExecution(() => Service.GetProductionBatch(productionBatchKey.KeyValue), "Act");

                //Assert
                result.AssertSuccess();
                notes.ForEach(n => Assert.AreEqual(1, result.ResultingObject.InstructionsNotebook.Notes.Count(i => i.Text == n.Text)));
            }

            [Test]
            public void Returns_PickedInventoryItems_on_success()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(productionBatch.Production));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(productionBatch.Production));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(productionBatch.Production));

                //Act
                var result = TimedExecution(() => Service.GetProductionBatch(new LotKey(productionBatch)), "Act");

                //Assert
                result.AssertSuccess();
                var resultItems = result.ResultingObject.PickedInventoryItems.ToList();
                Assert.AreEqual(3, resultItems.Count);
                productionBatch.Production.PickedInventory.Items.ForEach(i =>
                    {
                        var key = new PickedInventoryItemKey(i).KeyValue;
                        var resultItem = resultItems.Single(r => r.PickedInventoryItemKey == key);
                        Assert.AreEqual(new LotKey(i.Lot).KeyValue, resultItem.LotKey);
                        Assert.AreEqual(i.PackagingProduct.Weight, resultItem.PackagingProduct.Weight);
                    });
            }
        }

        [TestFixture]
        public class GetInventoryItemsToPickBatch : PackScheduleServiceTests
        {
            [Test]
            public void Returns_only_Inventory_of_Rincon_Warehouse()
            {
                //Arrange
                const int expectedResults = 3;
                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();

                Assert.Greater(TestHelper.Context.Set<Inventory>().Count(), expectedResults);

                //Act
                StartStopwatch();
                var result = Service.GetInventoryItemsToPickBatch();
                var results = result.ResultingObject != null ? result.ResultingObject.Items.ToList() : null;
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                var rinconWarehouseKey = new FacilityKey(RinconFacility);
                Assert.AreEqual(expectedResults, results.Count);
                Assert.AreEqual(rinconWarehouseKey.KeyValue, results.Single(r => r.InventoryKey == expectedInventoryKey0.KeyValue).Location.FacilityKey);
                Assert.AreEqual(rinconWarehouseKey.KeyValue, results.Single(r => r.InventoryKey == expectedInventoryKey1.KeyValue).Location.FacilityKey);
                Assert.AreEqual(rinconWarehouseKey.KeyValue, results.Single(r => r.InventoryKey == expectedInventoryKey2.KeyValue).Location.FacilityKey);
            }

            [Test]
            public void Returns_Inventory_with_ValidForPicking_as_expected_by_LotQualityStatus()
            {
                //Arrange
                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));

                var unexpectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.QualityStatus = LotQualityStatus.Contaminated));
                var unexpectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.QualityStatus = LotQualityStatus.Rejected));
                var unexpectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.QualityStatus = LotQualityStatus.Pending));

                var expected = new Dictionary<string, bool>
                    {
                        { expectedInventoryKey0, true },
                        { expectedInventoryKey1, true },
                        { expectedInventoryKey2, true },
                          
                        { unexpectedInventoryKey0, false },
                        { unexpectedInventoryKey1, false },
                        { unexpectedInventoryKey2, false },
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryItemsToPickBatch();
                var results = result.ResultingObject != null ? result.ResultingObject.Items.ToList() : null;
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                expected.Where(e => e.Value).AssertEquivalent(results, e => e.Key, r => r.InventoryKey,
                    (e, r) => Assert.AreEqual(e.Value, r.ValidForPicking));
            }

            [Test]
            public void Returns_Inventory_with_ValidForPicking_as_expected_by_LotProductionStatus()
            {
                //Arrange
                var expected0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));
                var expected1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));
                var expected2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));

                var unexpected0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.ProductionStatus = LotProductionStatus.Batched);
                var unexpected1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.ProductionStatus = LotProductionStatus.Batched);
                var unexpected2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.ProductionStatus = LotProductionStatus.Batched);
                var unexpected3 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.ProductionStatus = LotProductionStatus.Batched);

                var expected = new Dictionary<string, bool>
                    {
                        { expected0.ToInventoryKey(), true },
                        { expected1.ToInventoryKey(), true },
                        { expected2.ToInventoryKey(), true },
                          
                        { unexpected0.ToInventoryKey(), false },
                        { unexpected1.ToInventoryKey(), false },
                        { unexpected2.ToInventoryKey(), false },
                        { unexpected3.ToInventoryKey(), false }
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryItemsToPickBatch();
                var results = result.ResultingObject != null ? result.ResultingObject.Items.ToList() : null;
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                expected.Where(e => e.Value).AssertEquivalent(results, e => e.Key, r => r.InventoryKey,
                    (e, r) => Assert.AreEqual(e.Value, r.ValidForPicking));
            }

            [Test]
            public void Returns_Inventory_OnHold_with_ValidForPicking_as_expected()
            {
                //Arrange
                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));

                var unexpectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.Hold = LotHoldType.HoldForAdditionalTesting));
                var unexpectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.Hold = LotHoldType.HoldForCustomer));
                var unexpectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.Hold = LotHoldType.HoldForTreatment));

                var expected = new Dictionary<string, bool>
                    {
                        { expectedInventoryKey0, true },
                        { expectedInventoryKey1, true },
                        { expectedInventoryKey2, true }
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryItemsToPickBatch();
                var results = result.ResultingObject != null ? result.ResultingObject.Items.ToList() : null;
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                expected.AssertEquivalent(results, e => e.Key, r => r.InventoryKey,
                    (e, r) => Assert.AreEqual(e.Value, r.ValidForPicking));
            }

            [Test]
            public void Returns_Inventory_filtered_by_IngredientKey_as_expected()
            {
                //Arrange
                var additiveType = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>();
                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)
                    .Lot.SetAdditiveLot().AdditiveLot.AdditiveProduct.SetAdditiveType(additiveType)));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)
                    .Lot.SetAdditiveLot().AdditiveLot.AdditiveProduct.SetAdditiveType(additiveType)));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.SetChileLot());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.SetPackagingLot());

                //Act
                StartStopwatch();
                var result = Service.GetInventoryItemsToPickBatch(new FilterInventoryForBatchParameters
                    {
                        IngredientKey = new AdditiveTypeKey(additiveType)
                    });
                var results = result.ResultingObject != null ? result.ResultingObject.Items.ToList() : null;
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(2, results.Count);
                Assert.IsNotNull(results.Single(r => r.InventoryKey == expectedInventoryKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.InventoryKey == expectedInventoryKey1.KeyValue));
            }

            [Test]
            public void Returns_inventory_with_received_packaging_name_as_expected()
            {
                //Arrange
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));

                //Act
                StartStopwatch();
                var result = Service.GetInventoryItemsToPickBatch();
                var results = result.ResultingObject != null ? result.ResultingObject.Items.ToList() : null;
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new PackagingProductKey(inventory.Lot.ReceivedPackaging).KeyValue, results.Single().PackagingReceived.ProductKey);
            }

            [Test]
            public void Returns_Customer_information_if_association_exists()
            {
                //Arrange
                const int expectedResults = 3;
                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility)));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                
                var customer1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.SetOutputLot(expectedInventoryKey1).Production = null,
                    b => b.PackSchedule.SetCustomerKey(customer1));

                Assert.Greater(TestHelper.Context.Set<Inventory>().Count(), expectedResults);

                //Act
                StartStopwatch();
                var result = Service.GetInventoryItemsToPickBatch();
                var results = result.ResultingObject != null ? result.ResultingObject.Items.ToList() : null;
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);

                Assert.IsNull(results.Single(r => r.InventoryKey == expectedInventoryKey0.KeyValue).Customer);

                Assert.AreEqual(new CustomerKey((ICustomerKey) customer1).KeyValue, results.Single(r => r.InventoryKey == expectedInventoryKey1.KeyValue).Customer.CompanyKey);
                Assert.AreEqual(customer1.Company.Name, results.Single(r => r.InventoryKey == expectedInventoryKey1.KeyValue).Customer.Name);

                Assert.IsNull(results.Single(r => r.InventoryKey == expectedInventoryKey2.KeyValue).Customer);
            }

            [Test]
            public void Returns_Inventory_filtered_by_Treatment_as_expected()
            {
                //Arrange
                var treatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();
                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, treatment).SetValidToPick(RinconFacility)
                    .Lot.SetChileLot()));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, treatment).SetValidToPick(RinconFacility)
                    .Lot.SetChileLot()));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.SetChileLot());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.SetPackagingLot());

                //Act
                StartStopwatch();
                var result = Service.GetInventoryItemsToPickBatch(new FilterInventoryForBatchParameters
                    {
                        TreatmentKey = treatment.ToInventoryTreatmentKey()
                    });
                var results = result.ResultingObject != null ? result.ResultingObject.Items.ToList() : null;
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(2, results.Count);
                Assert.IsNotNull(results.Single(r => r.InventoryKey == expectedInventoryKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.InventoryKey == expectedInventoryKey1.KeyValue));
            }
        }

        [TestFixture]
        public class GetProductionBatchInstructions : PackScheduleServiceTests
        {
            [Test]
            public void Returns_empty_collection_if_no_Instruction_records_exist()
            {
                //Act
                var result = Service.GetProductionBatchInstructions();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(0, result.ResultingObject.Count());
            }

            [Test]
            public void Returns_Instructions_of_ProductionBatchInstruction_type_as_expected_on_success()
            {
                //Arrange
                var expectedInstructions = new List<string>
                    {
                        null,
                        "   ",
                        "asdfas",
                        "pack this and dat",
                        "pack this and dat",
                        "some other instruction"
                    };

                expectedInstructions.ForEach(i => TestHelper.CreateObjectGraphAndInsertIntoDatabase<Instruction>(t => t.InstructionText = i, t => t.InstructionType = InstructionType.ProductionBatchInstruction));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Instruction>(i => i.InstructionText = "Shouldn't be seeing this", i => i.InstructionType = InstructionType.ProductionScheduleInstruction);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Instruction>(i => i.InstructionText = "HI MOM!", i => i.InstructionType = InstructionType.ProductionScheduleInstruction);

                TestHelper.ResetContext();

                //Act
                var result = Service.GetProductionBatchInstructions();

                //Assert
                result.AssertSuccess();
                expectedInstructions = expectedInstructions.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().OrderBy(t => t).ToList();
                var resultInstructions = result.ResultingObject.ToList();
                for(var i = 0; i < resultInstructions.Count; ++i)
                {
                    Assert.AreEqual(expectedInstructions[i], resultInstructions[i]);
                }
            }
        }

        [TestFixture]
        public class GetProductionPacketForPackSchedule : PackScheduleServiceTests
        {
            [Test]
            public void Returns_production_packet_as_expected()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = null);
                var batchKeys = new List<LotKey>
                    {
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ConstrainByKeys(packSchedule))),
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ConstrainByKeys(packSchedule))),
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ConstrainByKeys(packSchedule)))
                    };

                //Act
                StartStopwatch();
                var result = Service.GetProductionPacketForPackSchedule(new PackScheduleKey(packSchedule));
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new ChileProductKey(packSchedule).KeyValue, result.ResultingObject.ChileProduct.ProductKey);
                Assert.AreEqual(new PackagingProductKey(packSchedule).KeyValue, result.ResultingObject.PackagingProduct.ProductKey);

                var batches = result.ResultingObject.Batches.ToList();
                Assert.AreEqual(batchKeys.Count, batches.Count);
                batchKeys.ForEach(k => Assert.AreEqual(1, result.ResultingObject.Batches.Count(b => b.LotKey == k.KeyValue)));
            }
        }

        [TestFixture]
        public class GetProductionPacketForBatch : PackScheduleServiceTests
        {
            protected override bool SetupStaticRecords { get { return true; } }

            [Test]
            public void Returns_production_packet_for_specific_ProductionBatch()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>(p => p.ProductionBatches = null);
                var batchKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ConstrainByKeys(packSchedule)));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ConstrainByKeys(packSchedule));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ConstrainByKeys(packSchedule));

                //Act
                StartStopwatch();
                var result = Service.GetProductionPacketForBatch(batchKey);
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var batches = result.ResultingObject.Batches.ToList();
                Assert.AreEqual(1, batches.Count);
                Assert.AreEqual(1, result.ResultingObject.Batches.Count(b => b.LotKey == batchKey.KeyValue));
            }

            [Test]
            public void Returns_CalculatedTargets_as_expected()
            {
                //Arrange
                var batch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>();
                
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(batch.Production.PickedInventory)
                    .Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Asta, 1.0))
                        },
                    i => i.PackagingProduct.Weight = 10.0f, i => i.Quantity = 1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(batch.Production.PickedInventory)
                    .Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, StaticAttributeNames.Asta, 2.0))
                        },
                    i => i.PackagingProduct.Weight = 10.0f, i => i.Quantity = 1);

                //Act
                StartStopwatch();
                var result = Service.GetProductionPacketForBatch(new LotKey(batch));
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var targets = result.ResultingObject.Batches.Single().CalculatedParameters;
                Assert.AreEqual(20.0, targets.BatchTargetWeight);
                Assert.AreEqual(1.5, targets.BatchTargetAsta);
            }
        }
        
        [TestFixture]
        public class GetPackSchedulePickSheet : PackScheduleServiceTests
        {
            [Test]
            public void Returns_PackSchedulePickSheet_with_expected_PickedInventoryItemKeys()
            {
                //Arrange
                var packSchedule = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackSchedule>();
                var expectedKeys = new List<PickedInventoryItemKey>();
                for(var i = 0; i < 3; ++i)
                {
                    expectedKeys.AddRange(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ConstrainByKeys(packSchedule).Production.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>()
                        }).Production.PickedInventory.Items.Select(n => new PickedInventoryItemKey(n)));
                }

                //Act
                StartStopwatch();
                var result = Service.GetPackSchedulePickSheet(new PackScheduleKey(packSchedule));
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new PackScheduleKey(packSchedule).KeyValue, result.ResultingObject.PackScheduleKey);

                var items = result.ResultingObject.PickedItems.ToList();
                Assert.AreEqual(expectedKeys.Count, items.Count);
                expectedKeys.ForEach(k => Assert.AreEqual(1, items.Count(i => i.PickedInventoryItemKey == k.KeyValue)));
            }
        }
    }
}