using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.LinqPredicates;
using CreateProductionBatchResultsParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.CreateProductionBatchResultsParameters;
using SetPickedInventoryItemParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SetPickedInventoryItemParameters;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class ProductionResultsServiceTests : ServiceIntegrationTestBase<ProductionResultsService>
    {
        [TestFixture]
        public class CreateProductionBatchResults : ProductionResultsServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ProductionBatchKey_could_not_be_parsed()
            {
                //Arrange
                const string badProductionBatchKey = "No bueno... no bueno...";

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        ProductionLineKey = new LocationKey(),
                        ProductionBatchKey = badProductionBatchKey,
                        UserToken = TestUser.UserName,
                        InventoryItems = new List<BatchResultItemParameters>()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.InvalidLotKey);
            }

            [Test]
            public void Returns_non_successful_result_if_any_PackagingProductKey_could_not_be_parsed()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                const string badPackagingProductKey = "Send this bastard key packing.";
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var inventoryTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        ProductionBatchKey = productionBatchKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        UserToken = TestUser.UserName,
                        ProductionStartTimestamp = startDate,
                        ProductionEndTimestamp = endDate,
                        InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = badPackagingProductKey,
                                        LocationKey = warehouseLocationKey.KeyValue,
                                        InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.InvalidPackagingProductKey);
            }

            [Test]
            public void Returns_non_successful_result_if_any_LocationKey_could_not_be_parsed()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                const string badLocationKey = "How can you have any pudding... if you don't eat your meat?";
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var inventoryTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        ProductionBatchKey = productionBatchKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        UserToken = TestUser.UserName,
                        ProductionStartTimestamp = startDate,
                        ProductionEndTimestamp = endDate,
                        InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = packagingProductKey.KeyValue,
                                        LocationKey = badLocationKey,
                                        InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.InvalidLocationKey);
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_add_a_production_result_with_quantity_of_0()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var inventoryTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                {
                    ProductionBatchKey = productionBatchKey.KeyValue,
                    ProductionLineKey = productionLineKey.KeyValue,
                    UserToken = TestUser.UserName,
                    ProductionStartTimestamp = startDate,
                    ProductionEndTimestamp = endDate,
                    InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = packagingProductKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue,
                                        InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                        Quantity = 0
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.QuantityNotGreaterThanZero);
            }

            [Test]
            public void Returns_non_successful_result_if_adding_results_would_result_in_duplicate_Inventory_records()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var inventoryTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                {
                    ProductionBatchKey = productionBatchKey.KeyValue,
                    ProductionLineKey = productionLineKey.KeyValue,
                    UserToken = TestUser.UserName,
                    ProductionStartTimestamp = startDate,
                    ProductionEndTimestamp = endDate,
                    InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = packagingProductKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue,
                                        InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                        Quantity = 10
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = packagingProductKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue,
                                        InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                        Quantity = 20
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionResultAlreadyPendingAddition);
            }

            [Test]
            public void If_method_fails_then_no_new_Inventory_item_records_will_have_been_created()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot());
                var warehouseLocation0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var warehouseLocation1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packagingProduct0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var packagingProduct1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var inventoryTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();

                var inventoryKey0 = new InventoryKey(productionBatch, packagingProduct0, warehouseLocation0, inventoryTreatment, "");
                var inventoryKey1 = new InventoryKey(productionBatch, packagingProduct1, warehouseLocation0, inventoryTreatment, "");
                var inventoryKey2 = new InventoryKey(productionBatch, packagingProduct0, warehouseLocation1, inventoryTreatment, "");
                var inventoryKey3 = new InventoryKey(productionBatch, packagingProduct1, warehouseLocation1, inventoryTreatment, "");

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                {
                    ProductionBatchKey = new LotKey(productionBatch).KeyValue,
                    ProductionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation).KeyValue,
                    UserToken = TestUser.UserName,
                    ProductionStartTimestamp = startDate,
                    ProductionEndTimestamp = endDate,
                    InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey0).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey0).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey0).KeyValue,
                                        Quantity = 10
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey1).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey1).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey1).KeyValue,
                                        Quantity = 10
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey2).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey2).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey2).KeyValue,
                                        Quantity = 10
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey3).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey3).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey3).KeyValue,
                                        Quantity = 0
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(false);
                Assert.IsFalse(RVCUnitOfWork.InventoryRepository.Contains(inventoryKey0.FindByPredicate));
                Assert.IsFalse(RVCUnitOfWork.InventoryRepository.Contains(inventoryKey1.FindByPredicate));
                Assert.IsFalse(RVCUnitOfWork.InventoryRepository.Contains(inventoryKey2.FindByPredicate));
                Assert.IsFalse(RVCUnitOfWork.InventoryRepository.Contains(inventoryKey3.FindByPredicate));
            }

            [Test]
            public void If_method_succeeds_then_new_Inventory_item_records_will_have_been_created_with_expected_quantities()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                const int expectedQuantity0 = 10;
                const int expectedQuantity1 = 11;
                const int expectedQuantity2 = 22;
                const int expectedQuantity3 = 33;

                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var warehouseLocation0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var warehouseLocation1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packagingProduct0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var packagingProduct1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var inventoryTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();

                var inventoryKey0 = new InventoryKey(productionBatch, packagingProduct0, warehouseLocation0, inventoryTreatment, "");
                var inventoryKey1 = new InventoryKey(productionBatch, packagingProduct1, warehouseLocation0, inventoryTreatment, "");
                var inventoryKey2 = new InventoryKey(productionBatch, packagingProduct0, warehouseLocation1, inventoryTreatment, "");
                var inventoryKey3 = new InventoryKey(productionBatch, packagingProduct1, warehouseLocation1, inventoryTreatment, "");

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                {
                    ProductionBatchKey = new LotKey(productionBatch).KeyValue,
                    ProductionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation).KeyValue,
                    UserToken = TestUser.UserName,
                    ProductionStartTimestamp = startDate,
                    ProductionEndTimestamp = endDate,
                    InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey0).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey0).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey0).KeyValue,
                                        Quantity = expectedQuantity0
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey1).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey1).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey1).KeyValue,
                                        Quantity = expectedQuantity1
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey2).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey2).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey2).KeyValue,
                                        Quantity = expectedQuantity2
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey3).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey3).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey3).KeyValue,
                                        Quantity = expectedQuantity3
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedQuantity0, RVCUnitOfWork.InventoryRepository.All().FirstOrDefault(inventoryKey0.FindByPredicate).Quantity);
                Assert.AreEqual(expectedQuantity1, RVCUnitOfWork.InventoryRepository.All().FirstOrDefault(inventoryKey1.FindByPredicate).Quantity);
                Assert.AreEqual(expectedQuantity2, RVCUnitOfWork.InventoryRepository.All().FirstOrDefault(inventoryKey2.FindByPredicate).Quantity);
                Assert.AreEqual(expectedQuantity3, RVCUnitOfWork.InventoryRepository.All().FirstOrDefault(inventoryKey3.FindByPredicate).Quantity);
            }

            [Test]
            public void If_method_succeeds_then_a_new_ProductionResultRecord_will_have_been_created_as_expected()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var warehouseLocationKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packagingProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var inventoryTreatmentKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = productionBatchKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionStartTimestamp = startDate,
                        ProductionEndTimestamp = endDate,
                        InventoryItems = new List<BatchResultItemParameters>
                                {
                                    new BatchResultItemParameters
                                        {
                                            PackagingKey = packagingProduct.ToPackagingProductKey(),
                                            LocationKey = warehouseLocationKey.ToLocationKey(),
                                            InventoryTreatmentKey = inventoryTreatmentKey.ToInventoryTreatmentKey(),
                                            Quantity = 10
                                        }
                                }
                    });

                //Assert
                result.AssertSuccess();
                var productionResult = RVCUnitOfWork.LotProductionResultsRepository.FindByKey(productionBatchKey);
                Assert.AreEqual(startDate, productionResult.ProductionBegin);
                Assert.AreEqual(endDate, productionResult.ProductionEnd);
                Assert.AreEqual(productionLineKey, new LocationKey(productionResult));
            }

            [Test, Issue("Failure to set PickedInventory.Archived to true was causing lot defect resolution (of a lot picked for production) by treatment to fail" +
                         "as if there were still untreated inventory of that lot in the system. -RI 2016-08-23",
                         References = new [] { "RVCADMIN-1250" })]
            public void If_method_succeeds_then_PickedInventory_will_have_been_flagged_as_archived()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.PickedInventory.Archived = false, b => b.Production.Results = null);
                var productionBatchKey = productionBatch.ToLotKey();
                var productionLineKey = productionBatch.PackSchedule.ProductionLineLocation.ToLocationKey();
                var warehouseLocationKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packagingProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var inventoryTreatmentKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = productionBatchKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionStartTimestamp = startDate,
                        ProductionEndTimestamp = endDate,
                        InventoryItems = new List<BatchResultItemParameters>
                                {
                                    new BatchResultItemParameters
                                        {
                                            PackagingKey = packagingProduct.ToPackagingProductKey(),
                                            LocationKey = warehouseLocationKey.ToLocationKey(),
                                            InventoryTreatmentKey = inventoryTreatmentKey.ToInventoryTreatmentKey(),
                                            Quantity = 10
                                        }
                                }
                    });

                //Assert
                result.AssertSuccess();
                var productionResult = RVCUnitOfWork.LotProductionResultsRepository.FindByKey(productionBatchKey, r => r.Production.PickedInventory);
                Assert.AreEqual(startDate, productionResult.ProductionBegin);
                Assert.AreEqual(endDate, productionResult.ProductionEnd);
                Assert.AreEqual(productionLineKey, productionResult.ToLocationKey());
                Assert.IsTrue(productionResult.Production.PickedInventory.Archived);
            }

            [Test]
            public void If_method_succeeds_then_new_ProductionResultItem_records_will_have_been_created_as_expected()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                const int quantity0 = 10;
                const int quantity1 = 11;
                const int quantity2 = 22;
                const int quantity3 = 33;

                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var warehouseLocation0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var warehouseLocation1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packagingProduct0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var packagingProduct1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var inventoryTreatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();

                var inventoryKey0 = new InventoryKey(productionBatch, packagingProduct0, warehouseLocation0, inventoryTreatment, "");
                var inventoryKey1 = new InventoryKey(productionBatch, packagingProduct1, warehouseLocation0, inventoryTreatment, "");
                var inventoryKey2 = new InventoryKey(productionBatch, packagingProduct0, warehouseLocation1, inventoryTreatment, "");
                var inventoryKey3 = new InventoryKey(productionBatch, packagingProduct1, warehouseLocation1, inventoryTreatment, "");

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                {
                    ProductionBatchKey = new LotKey(productionBatch).KeyValue,
                    ProductionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation).KeyValue,
                    UserToken = TestUser.UserName,
                    ProductionStartTimestamp = startDate,
                    ProductionEndTimestamp = endDate,
                    InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey0).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey0).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey0).KeyValue,
                                        Quantity = quantity0
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey1).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey1).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey1).KeyValue,
                                        Quantity = quantity1
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey2).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey2).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey2).KeyValue,
                                        Quantity = quantity2
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = new PackagingProductKey(inventoryKey3).KeyValue,
                                        LocationKey = new LocationKey(inventoryKey3).KeyValue,
                                        InventoryTreatmentKey = new InventoryTreatmentKey(inventoryKey3).KeyValue,
                                        Quantity = quantity3
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(quantity0, RVCUnitOfWork.LotProductionResultItemsRepository.Filter(ProductionResultItemPredicates.FilterByInventoryKey(inventoryKey0)).Single().Quantity);
                Assert.AreEqual(quantity1, RVCUnitOfWork.LotProductionResultItemsRepository.Filter(ProductionResultItemPredicates.FilterByInventoryKey(inventoryKey1)).Single().Quantity);
                Assert.AreEqual(quantity2, RVCUnitOfWork.LotProductionResultItemsRepository.Filter(ProductionResultItemPredicates.FilterByInventoryKey(inventoryKey2)).Single().Quantity);
                Assert.AreEqual(quantity3, RVCUnitOfWork.LotProductionResultItemsRepository.Filter(ProductionResultItemPredicates.FilterByInventoryKey(inventoryKey3)).Single().Quantity);
            }

            [Test]
            public void Returns_non_successful_result_if_ProductionBatch_has_already_been_completed()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(), b => b.ProductionHasBeenCompleted = true);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var inventoryTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                {
                    ProductionBatchKey = productionBatchKey.KeyValue,
                    ProductionLineKey = productionLineKey.KeyValue,
                    UserToken = TestUser.UserName,
                    ProductionStartTimestamp = startDate,
                    ProductionEndTimestamp = endDate,
                    InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = packagingProductKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue,
                                        InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchAlreadyComplete);
            }

            [Test]
            public void Returns_non_successful_result_if_ProductionBatch_has_already_has_ProductionResults()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(), b => b.ProductionHasBeenCompleted = false);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var inventoryTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        ProductionBatchKey = productionBatchKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        UserToken = TestUser.UserName,
                        ProductionStartTimestamp = startDate,
                        ProductionEndTimestamp = endDate,
                        InventoryItems = new List<BatchResultItemParameters>
                                {
                                    new BatchResultItemParameters
                                        {
                                            PackagingKey = packagingProductKey.KeyValue,
                                            LocationKey = warehouseLocationKey.KeyValue,
                                            InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                            Quantity = 10
                                        }
                                }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchHasResult);
            }

            [Test]
            public void If_method_succeeds_then_ProductionBatch_completed_property_will_be_set_to_true()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot(),
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var inventoryTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                {
                    ProductionBatchKey = productionBatchKey.KeyValue,
                    ProductionLineKey = productionLineKey.KeyValue,
                    UserToken = TestUser.UserName,
                    ProductionStartTimestamp = startDate,
                    ProductionEndTimestamp = endDate,
                    InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = packagingProductKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue,
                                        InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                productionBatch = RVCUnitOfWork.ProductionBatchRepository.FindByKey(productionBatchKey);
                Assert.IsTrue(productionBatch.ProductionHasBeenCompleted);
            }

            [Test]
            public void If_method_succeeds_then_the_OutputLot_of_the_ProductionBatch_will_have_its_LotProductionStatus_set_to_Produced()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot().ProductionStatus = LotProductionStatus.Batched,
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = new LotKey(productionBatch);
                var productionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation);
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var inventoryTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                {
                    ProductionBatchKey = productionBatchKey.KeyValue,
                    ProductionLineKey = productionLineKey.KeyValue,
                    UserToken = TestUser.UserName,
                    ProductionStartTimestamp = startDate,
                    ProductionEndTimestamp = endDate,
                    InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = packagingProductKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue,
                                        InventoryTreatmentKey = inventoryTreatmentKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(LotProductionStatus.Produced, RVCUnitOfWork.ChileLotRepository.FindByKey(productionBatchKey, c => c.Lot).Lot.ProductionStatus);
            }

            [Test, Issue("InventoryTransaction logs need to be created when entering results (instead of when picking for a batch) " +
                   "in order to keep step with the way Access does creates tblIncoming/tblOutgoing. - RI 2016-06-14",
                   Todo = "Once free from Access, entering results for the first time should only create Transaction logs for " +
                          "the resulting items, not the items picked for a batch, unless modifications are made to the picked items" +
                          "when entering results. - RI 2016-06-14",
                   References = new [] { "RVCADMIN-1153"},
                   Flags = IssueFlags.TodoWhenAccessFreedom)]
            public void Will_create_InventoryTransaction_records_as_expected()
            {
                //Arrange
                var startDate = new DateTime(2012, 3, 29);
                var endDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot().ProductionStatus = LotProductionStatus.Batched,
                    b => b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>()
                        },
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);

                var productionBatchKey = productionBatch.ToLotKey();
                var productionLineKey = productionBatch.PackSchedule.ProductionLineLocation.ToLocationKey();
                var warehouseLocationKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>().ToLocationKey();
                var packagingProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey();
                var inventoryTreatmentKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>().ToInventoryTreatmentKey();

                Assert.AreEqual(0, RVCUnitOfWork.InventoryTransactionsRepository.All().Count());

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        ProductionBatchKey = productionBatchKey,
                        ProductionLineKey = productionLineKey,
                        UserToken = TestUser.UserName,
                        ProductionStartTimestamp = startDate,
                        ProductionEndTimestamp = endDate,
                        InventoryItems = new List<BatchResultItemParameters>
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingKey = packagingProductKey,
                                        LocationKey = warehouseLocationKey,
                                        InventoryTreatmentKey = inventoryTreatmentKey,
                                        Quantity = 10
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                
                var transaction = RVCUnitOfWork.InventoryTransactionsRepository.Filter(InventoryTransactionPredicates.BySourceLot(productionBatchKey), i => i.DestinationLot).SingleOrDefault();
                Assert.AreEqual(InventoryTransactionType.ProductionResults, transaction.TransactionType);
                Assert.AreEqual(10, transaction.Quantity);
                Assert.IsNull(transaction.DestinationLot);

                Assert.AreEqual(3, productionBatch.Production.PickedInventory.Items.Count);
                foreach(var picked in productionBatch.Production.PickedInventory.Items)
                {
                    transaction = RVCUnitOfWork.InventoryTransactionsRepository.Filter(InventoryTransactionPredicates.BySourceLot(picked.ToLotKey()), i => i.DestinationLot).SingleOrDefault();
                    Assert.AreEqual(InventoryTransactionType.ProductionResults, transaction.TransactionType);
                    Assert.AreEqual(-picked.Quantity, transaction.Quantity);
                    Assert.AreEqual(productionBatchKey, transaction.DestinationLot.ToLotKey());
                }
            }

            [Test]
            public void Will_update_PickedInventoryItems_as_expected()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot().ProductionStatus = LotProductionStatus.Batched,
                    b => b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.SetSourceWarehouse(RinconFacility).SetCurrentLocationToSource().Lot.SetValidToPick()),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.SetSourceWarehouse(RinconFacility).SetCurrentLocationToSource().Lot.SetValidToPick()),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.SetSourceWarehouse(RinconFacility).SetCurrentLocationToSource().Lot.SetValidToPick())
                        },
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = new LotKey(productionBatch);
                
                var chileInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.SetChileLot());

                var pickedItemChanges = productionBatch.Production.PickedInventory.Items.Select(
                        i => new SetPickedInventoryItemParameters
                            {
                                CustomerProductCode = i.CustomerProductCode,
                                CustomerLotCode = i.CustomerLotCode,
                                InventoryKey = new InventoryKey(i),
                                Quantity = i.Quantity
                            }).ToList();
                pickedItemChanges.RemoveAt(0);
                pickedItemChanges[0].Quantity *= -1;
                pickedItemChanges[1].Quantity = -2;
                pickedItemChanges.Add(new SetPickedInventoryItemParameters
                    {
                        InventoryKey = new InventoryKey(chileInventory),
                        Quantity = chileInventory.Quantity
                    });

                var expectedResults = productionBatch.Production.PickedInventory.Items.ToDictionary(i => new InventoryKey(i).KeyValue,
                    i => i.Quantity + pickedItemChanges.Where(c => c.InventoryKey == new InventoryKey(i).KeyValue).Select(c => c.Quantity).DefaultIfEmpty(0).Sum());
                expectedResults.Add(new InventoryKey(chileInventory), chileInventory.Quantity);
                foreach(var expected in expectedResults.Where(r => r.Value == 0).Select(r => r.Key).ToList())
                {
                    expectedResults.Remove(expected);
                }

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = productionBatchKey,
                        ProductionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation),
                        ProductionStartTimestamp = new DateTime(2016, 1, 1),
                        ProductionEndTimestamp = new DateTime(2016, 1, 1),

                        PickedInventoryItemChanges = pickedItemChanges,
                        InventoryItems = new List<IBatchResultItemParameters>()
                    });

                //Assert
                result.AssertSuccess();
                ResetUnitOfWork();
                productionBatch = RVCUnitOfWork.ProductionBatchRepository.FindByKey(productionBatchKey, b => b.Production.PickedInventory.Items);
                var resultItems = productionBatch.Production.PickedInventory.Items.ToList();
                Assert.AreEqual(expectedResults.Count, resultItems.Count);
                foreach(var item in resultItems)
                {
                    Assert.AreEqual(expectedResults[new InventoryKey(item)], item.Quantity);
                }
            }

            [Test, Issue("Will not set computed values for attributes flagged as ActualValueRequired.",
                References = new [] { "RVCADMIN-1170" })]
            public void Will_update_LotAttributes_as_expected()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot().ProductionStatus = LotProductionStatus.Batched,
                    b => b.ProductionHasBeenCompleted = false, b => b.Production.Results = null);
                var productionBatchKey = productionBatch.ToLotKey();

                var attribute = StaticAttributeNames.AttributeNames.First(a => !a.ActualValueRequired);
                var chileInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.SetChileLot().Attributes = new List<LotAttribute>
                    {
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, attribute, 123))
                    });

                var pickedItemChanges = new List<IPickedInventoryItemParameters>
                    {
                        new SetPickedInventoryItemParameters
                            {
                                InventoryKey = chileInventory.ToInventoryKey(),
                                Quantity = chileInventory.Quantity
                            }
                    };

                //Act
                var result = Service.CreateProductionBatchResults(new CreateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = productionBatchKey,
                        ProductionLineKey = productionBatch.PackSchedule.ProductionLineLocation.ToLocationKey(),
                        ProductionStartTimestamp = new DateTime(2016, 1, 1),
                        ProductionEndTimestamp = new DateTime(2016, 1, 1),

                        PickedInventoryItemChanges = pickedItemChanges,
                        InventoryItems = new List<IBatchResultItemParameters>()
                    });

                //Assert
                result.AssertSuccess();
                ResetUnitOfWork();
                productionBatch = RVCUnitOfWork.ProductionBatchRepository.FindByKey(productionBatchKey, b => b.Production.ResultingChileLot.Lot.Attributes);
                var lotAttribute = productionBatch.Production.ResultingChileLot.Lot.Attributes.First(a => a.AttributeShortName == attribute.ShortName);
                Assert.IsTrue(lotAttribute.Computed);
                Assert.AreEqual(123, lotAttribute.AttributeValue);
                Assert.Greater(1, (DateTime.UtcNow.Date - lotAttribute.AttributeDate).Hours);
            }
        }

        [TestFixture]
        public class UpdateProductionBatchResults : ProductionResultsServiceTests
        {
            [Test]
            public void Returns_non_sucessful_result_if_ProductionBatch_could_not_be_found()
            {
                //Arrange
                var productionLine = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine);

                //Act
                var result = Service.UpdateProductionBatchResults(new UpdateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = new LotKey(LotKey.Null).KeyValue,
                        ProductionLineKey = new LocationKey(productionLine).KeyValue,
                        ProductionStartTimestamp = new DateTime(2012, 3, 29),
                        ProductionEndTimestamp = new DateTime(2014, 4, 1),
                        InventoryItems = new List<BatchResultItemParameters>()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionBatchNotFound);
            }

            [Test]
            public void Updates_existing_ProductionResult_record_on_success_as_expected()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>();
                var expectedStartDate = new DateTime(2012, 3, 29);
                var expectedEndDate = new DateTime(2014, 4, 1);
                var productionResultKey = new LotKey(productionBatch);
                var productionLine = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine);

                //Act
                var result = Service.UpdateProductionBatchResults(new UpdateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = productionResultKey.KeyValue,
                        ProductionLineKey = new LocationKey(productionLine).KeyValue,
                        ProductionStartTimestamp = expectedStartDate,
                        ProductionEndTimestamp = expectedEndDate,
                        InventoryItems = new List<BatchResultItemParameters>()
                    });

                //Assert
                result.AssertSuccess();
                var productionResult = RVCUnitOfWork.LotProductionResultsRepository.FindByKey(productionResultKey);
                Assert.AreEqual(productionLine.Id, productionResult.ProductionLineLocationId);
                Assert.AreEqual(expectedStartDate, productionResult.ProductionBegin);
                Assert.AreEqual(expectedEndDate, productionResult.ProductionEnd);
            }

            [Test]
            public void Does_not_create_additional_inventory_transactions_for_items_picked()
            {
                //Arrange
                var expectedStartDate = new DateTime(2012, 3, 29);
                var expectedEndDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                    {
                        TestHelper.CreateObjectGraph<PickedInventoryItem>(),
                        TestHelper.CreateObjectGraph<PickedInventoryItem>(),
                        TestHelper.CreateObjectGraph<PickedInventoryItem>()
                    });
                var productionResultKey = new LotKey(productionBatch);
                var productionLine = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine);
                var parameters = new UpdateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = productionResultKey.KeyValue,
                        ProductionLineKey = new LocationKey(productionLine).KeyValue,
                        ProductionStartTimestamp = expectedStartDate,
                        ProductionEndTimestamp = expectedEndDate,
                        InventoryItems = new List<BatchResultItemParameters>()
                    };
                var transactionCount = productionBatch.Production.PickedInventory.Items.Select(i =>
                        RVCUnitOfWork.InventoryTransactionsRepository.CountOf(InventoryTransactionPredicates.ByInventoryKey(i))
                    ).ToList();

                //Act
                Service.UpdateProductionBatchResults(parameters).AssertSuccess();
                Service.UpdateProductionBatchResults(parameters).AssertSuccess();

                //Assert
                foreach(var i in productionBatch.Production.PickedInventory.Items.Stitched(transactionCount))
                {
                    var transactions = RVCUnitOfWork.InventoryTransactionsRepository.Filter(InventoryTransactionPredicates.ByInventoryKey(i.Item1)).ToList();
                    Assert.AreEqual(i.Item2, transactions.Count);
                }
            }

            [Test]
            public void Does_not_create_duplicate_result_item_transactions_if_results_are_updated_more_than_once()
            {
                //Arrange
                var expectedStartDate = new DateTime(2012, 3, 29);
                var expectedEndDate = new DateTime(2014, 4, 1);
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>();
                var resultInventory = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(productionBatch).ToteKey = ""));
                var productionResultKey = new LotKey(productionBatch);
                var productionLine = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine);
                var parameters = new UpdateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = productionResultKey.KeyValue,
                        ProductionLineKey = new LocationKey(productionLine).KeyValue,
                        ProductionStartTimestamp = expectedStartDate,
                        ProductionEndTimestamp = expectedEndDate,
                        InventoryItems = new List<BatchResultItemParameters>
                            {
                                CreateItem(1, resultInventory)
                            }
                    };
                var resultPredicate = InventoryTransactionPredicates.ByInventoryKey(resultInventory);
                var expectedTransactions = RVCUnitOfWork.InventoryTransactionsRepository.Filter(resultPredicate).Count() + 1;

                //Act/Assert
                Service.UpdateProductionBatchResults(parameters).AssertSuccess();
                Assert.AreEqual(expectedTransactions, RVCUnitOfWork.InventoryTransactionsRepository.Filter(resultPredicate).Count());

                Service.UpdateProductionBatchResults(parameters).AssertSuccess();
                Assert.AreEqual(expectedTransactions, RVCUnitOfWork.InventoryTransactionsRepository.Filter(resultPredicate).Count());
            }

            [Test]
            public void Returns_non_successful_result_if_updating_result_item_quantity_would_result_in_negative_inventory()
            {
                //Arrange
                LotProductionResultItem item = null;
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.Results.ResultItems = new List<LotProductionResultItem>
                    {
                        (item = TestHelper.CreateObjectGraph<LotProductionResultItem>(i => i.Quantity = 10))
                    });

                //Act
                var result = Service.UpdateProductionBatchResults(new UpdateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = new LotKey(item),
                        ProductionLineKey = new LocationKey(item.ProductionResults),
                        ProductionStartTimestamp = item.ProductionResults.ProductionBegin,
                        ProductionEndTimestamp = item.ProductionResults.ProductionEnd,
                        InventoryItems = new List<BatchResultItemParameters>
                            {
                                CreateItem(item, 1)
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.NegativeInventoryLots);
            }

            [Test]
            public void Will_modify_existing_inventory_as_expected_on_success()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>();
                var inventoryToDelete = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(productionBatch, null, null, null, null, ""));
                var inventoryToRemove = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(productionBatch, null, null, null, null, "").Quantity = 10);
                var inventoryToAdd = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(productionBatch, null, null, null, null, ""));
                var inventoryToKeep = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(productionBatch, null, null, null, null, ""));
                var inventoryToCreate = new InventoryKey(productionBatch, TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(), TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(), TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>(), "");

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.SetToInventory(inventoryToDelete).Quantity = inventoryToDelete.Quantity);
                var itemToRemove = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.SetToInventory(inventoryToRemove).Quantity = inventoryToRemove.Quantity);
                var itemToAdd = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.SetToInventory(inventoryToAdd).Quantity = inventoryToAdd.Quantity);
                var itemToKeep = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.SetToInventory(inventoryToKeep).Quantity = inventoryToKeep.Quantity);

                var parameters = new UpdateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = new LotKey(productionBatch),
                        ProductionLineKey = new LocationKey(productionBatch.Production.Results),
                        ProductionStartTimestamp = productionBatch.Production.Results.ProductionBegin,
                        ProductionEndTimestamp = productionBatch.Production.Results.ProductionEnd,
                        InventoryItems = new List<BatchResultItemParameters>
                            {
                                CreateItem(itemToRemove, itemToRemove.Quantity - 5),
                                CreateItem(itemToAdd, itemToAdd.Quantity + 7),
                                CreateItem(itemToKeep),
                                CreateItem(10, inventoryToCreate)
                            }
                    };

                //Act/Assert
                for(var i = 0; i < 2; ++i)
                {
                    Service.UpdateProductionBatchResults(parameters).AssertSuccess();
                    Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventoryToDelete)));
                    Assert.AreEqual(inventoryToRemove.Quantity - 5, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventoryToRemove)).Quantity);
                    Assert.AreEqual(inventoryToAdd.Quantity + 7, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventoryToAdd)).Quantity);
                    Assert.AreEqual(inventoryToKeep.Quantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventoryToKeep)).Quantity);
                    Assert.AreEqual(10, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventoryToCreate)).Quantity);
                }
            }

            [Test]
            public void Will_modify_batch_picked_items_as_expected_on_success()
            {
                //Arrange
                PickedInventoryItem unpickCreateInventory = null;
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                    {
                        (unpickCreateInventory = TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.NullCustomerCodes().SetCurrentLocationToSource().Quantity = 12))
                    });
                var unpickCreateKey = new InventoryKey(unpickCreateInventory);
                var resultItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                var additiveInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetValidToPick().SetAdditiveLot(), i => i.Quantity = 10,
                    i => i.ConstrainByKeys(null, null, null, null, RinconFacility));
                var additiveKey = new InventoryKey(additiveInventory);

                //Act
                var result = Service.UpdateProductionBatchResults(new UpdateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = new LotKey(productionBatch),
                        ProductionLineKey = new LocationKey(productionBatch.Production.Results),
                        ProductionStartTimestamp = productionBatch.Production.Results.ProductionBegin,
                        ProductionEndTimestamp = productionBatch.Production.Results.ProductionEnd,
                        InventoryItems = new List<BatchResultItemParameters>
                            {
                                CreateItem(123, resultItem)
                            },
                        PickedInventoryItemChanges = new List<SetPickedInventoryItemParameters>
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = additiveKey,
                                        Quantity = 8
                                    },
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = unpickCreateKey,
                                        Quantity = -7
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(2, RVCUnitOfWork.InventoryRepository.FindByKey(additiveKey).Quantity);
                Assert.AreEqual(7, RVCUnitOfWork.InventoryRepository.FindByKey(unpickCreateKey).Quantity);

                var batch = RVCUnitOfWork.ProductionBatchRepository.FindByKey(new LotKey(productionBatch), b => b.Production.PickedInventory.Items);
                var pickedItems = batch.Production.PickedInventory.Items;
                Assert.AreEqual(8, pickedItems.Single(additiveKey.Equals).Quantity);
                Assert.AreEqual(5, pickedItems.Single(unpickCreateKey.Equals).Quantity);
                Assert.AreEqual(1, RVCUnitOfWork.InventoryTransactionsRepository.Filter(InventoryTransactionPredicates.ByInventoryKey(additiveKey)).Count());
                Assert.AreEqual(1, RVCUnitOfWork.InventoryTransactionsRepository.Filter(InventoryTransactionPredicates.ByInventoryKey(unpickCreateKey)).Count());
            }

            [Test, Issue("InventoryTransaction logs need to be created when entering results (instead of when picking for a batch) " +
                   "in order to keep step with the way Access does creates tblIncoming/tblOutgoing. - RI 2016-06-14",
                   Todo = "Once free from Access, entering results for the first time should only create Transaction logs for " +
                          "the resulting items, not the items picked for a batch, unless modifications are made to the picked items" +
                          "when entering results. - RI 2016-06-14",
                   References = new[] { "RVCADMIN-1153" },
                   Flags = IssueFlags.TodoWhenAccessFreedom)]
            public void Creates_InventoryTransaction_records_as_expected()
            {
                //Arrange
                PickedInventoryItem pickedItem = null;
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.ProductionStatus = LotProductionStatus.Produced,
                    b => b.Production.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(2, (i, n) =>
                        {
                            i.SetCurrentLocationToSource();
                            if(n == 0)
                            {
                                i.Quantity = 10;
                                pickedItem = i;
                            }
                        }));

                //Act
                var result = Service.UpdateProductionBatchResults(new UpdateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = productionBatch.ToLotKey(),
                        ProductionLineKey = productionBatch.PackSchedule.ProductionLineLocation.ToLocationKey(),
                        ProductionStartTimestamp = productionBatch.Production.Results.ProductionBegin,
                        ProductionEndTimestamp = productionBatch.Production.Results.ProductionEnd,

                        PickedInventoryItemChanges = new IPickedInventoryItemParameters[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = pickedItem.ToInventoryKey(),
                                        CustomerLotCode = pickedItem.CustomerLotCode,
                                        CustomerProductCode = pickedItem.CustomerProductCode,
                                        Quantity = -8
                                    }
                            },
                        InventoryItems = new IBatchResultItemParameters[0]
                    });

                //Assert
                result.AssertSuccess();

                var inventoryTransaction = RVCUnitOfWork.InventoryTransactionsRepository.All().SingleOrDefault();
                Assert.AreEqual(pickedItem.ToInventoryKey(), inventoryTransaction.ToInventoryKey());
                Assert.AreEqual(8, inventoryTransaction.Quantity);
            }
            
            [Test]
            public void Will_update_PickedInventoryItems_as_expected()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot().ProductionStatus = LotProductionStatus.Produced,
                    b => b.Production.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.SetSourceWarehouse(RinconFacility).SetCurrentLocationToSource().Lot.SetValidToPick()),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.SetSourceWarehouse(RinconFacility).SetCurrentLocationToSource().Lot.SetValidToPick()),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.SetSourceWarehouse(RinconFacility).SetCurrentLocationToSource().Lot.SetValidToPick())
                        },
                    b => b.ProductionHasBeenCompleted = true);
                var productionBatchKey = new LotKey(productionBatch);

                var chileInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.SetChileLot());

                var pickedItemChanges = productionBatch.Production.PickedInventory.Items.Select(
                        i => new SetPickedInventoryItemParameters
                        {
                            CustomerProductCode = i.CustomerProductCode,
                            CustomerLotCode = i.CustomerLotCode,
                            InventoryKey = new InventoryKey(i),
                            Quantity = i.Quantity
                        }).ToList();
                pickedItemChanges.RemoveAt(0);
                pickedItemChanges[0].Quantity *= -1;
                pickedItemChanges[1].Quantity = -2;
                pickedItemChanges.Add(new SetPickedInventoryItemParameters
                    {
                        InventoryKey = new InventoryKey(chileInventory),
                        Quantity = chileInventory.Quantity
                    });

                var expectedResults = productionBatch.Production.PickedInventory.Items.ToDictionary(i => new InventoryKey(i).KeyValue,
                    i => i.Quantity + pickedItemChanges.Where(c => c.InventoryKey == new InventoryKey(i).KeyValue).Select(c => c.Quantity).DefaultIfEmpty(0).Sum());
                expectedResults.Add(new InventoryKey(chileInventory), chileInventory.Quantity);
                foreach(var expected in expectedResults.Where(r => r.Value == 0).Select(r => r.Key).ToList())
                {
                    expectedResults.Remove(expected);
                }

                //Act
                var result = Service.UpdateProductionBatchResults(new UpdateProductionResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = productionBatchKey,
                        ProductionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation),
                        ProductionStartTimestamp = new DateTime(2016, 1, 1),
                        ProductionEndTimestamp = new DateTime(2016, 1, 1),

                        PickedInventoryItemChanges = pickedItemChanges,
                        InventoryItems = new List<IBatchResultItemParameters>()
                    });

                //Assert
                result.AssertSuccess();
                ResetUnitOfWork();
                productionBatch = RVCUnitOfWork.ProductionBatchRepository.FindByKey(productionBatchKey, b => b.Production.PickedInventory.Items);
                var resultItems = productionBatch.Production.PickedInventory.Items.ToList();
                Assert.AreEqual(expectedResults.Count, resultItems.Count);
                foreach(var item in resultItems)
                {
                    Assert.AreEqual(expectedResults[new InventoryKey(item)], item.Quantity);
                }
            }

            [Test, Issue("Will not set computed values for attributes flagged as ActualValueRequired.",
                References = new[] { "RVCADMIN-1170" })]
            public void Will_update_LotAttributes_as_expected()
            {
                //Arrange
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.ResultingChileLot.Lot.EmptyLot().ProductionStatus = LotProductionStatus.Produced,
                    b => b.ProductionHasBeenCompleted = true);
                var productionBatchKey = productionBatch.ToLotKey();

                var attribute = StaticAttributeNames.Mold;
                var chileInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Lot.SetChileLot().Attributes = new List<LotAttribute>
                    {
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, attribute, 123))
                    });

                var pickedItemChanges = new List<IPickedInventoryItemParameters>
                    {
                        new SetPickedInventoryItemParameters
                            {
                                InventoryKey = new InventoryKey(chileInventory),
                                Quantity = chileInventory.Quantity
                            }
                    };

                //Act
                var result = Service.UpdateProductionBatchResults(new UpdateProductionResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = productionBatchKey,
                        ProductionLineKey = new LocationKey(productionBatch.PackSchedule.ProductionLineLocation),
                        ProductionStartTimestamp = new DateTime(2016, 1, 1),
                        ProductionEndTimestamp = new DateTime(2016, 1, 1),

                        PickedInventoryItemChanges = pickedItemChanges,
                        InventoryItems = new List<IBatchResultItemParameters>()
                    });

                //Assert
                result.AssertSuccess();
                ResetUnitOfWork();
                productionBatch = RVCUnitOfWork.ProductionBatchRepository.FindByKey(productionBatchKey, b => b.Production.ResultingChileLot.Lot.Attributes);
                var lotAttribute = productionBatch.Production.ResultingChileLot.Lot.Attributes.First(a => a.AttributeShortName == attribute.ShortName);
                Assert.IsTrue(lotAttribute.Computed);
                Assert.AreEqual(123, lotAttribute.AttributeValue);
                Assert.Greater(1, (DateTime.UtcNow.Date - lotAttribute.AttributeDate).Hours);
            }

            private static BatchResultItemParameters CreateItem(LotProductionResultItem item, int? quantity = null)
            {
                return CreateItem(quantity ?? item.Quantity, item);
            }

            private static BatchResultItemParameters CreateItem(int quantity, IInventoryKey item)
            {
                return new BatchResultItemParameters
                    {
                        InventoryTreatmentKey = new InventoryTreatmentKey(item),
                        LocationKey = new LocationKey(item),
                        PackagingKey = new PackagingProductKey(item),
                        Quantity = quantity
                    };
            }
        }

        [TestFixture]
        public class GetProductionBatchResultSummaries : ProductionResultsServiceTests
        {
            [Test]
            public void Returns_empty_collection_if_no_ProductionResults_exist_in_database()
            {
                //Act
                var result = TimedExecution(() => Service.GetProductionResultSummaries());

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_ProductionResultSummaries_as_expected()
            {
                //Arrange
                const int expectedResults = 3;
                var productionResults = new List<ProductionBatch>();
                for(var i = 0; i < expectedResults; i++)
                {
                    productionResults.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>());
                }

                //Act
                StartStopwatch();
                var result = Service.GetProductionResultSummaries();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);

                productionResults.ForEach(p =>
                    {
                        var productionResultKey = new LotKey(p).KeyValue;
                        p.AssertEqual(results.Single(r => r.ProductionResultKey == productionResultKey));
                    });
            }
        }

        [TestFixture]
        public class GetProductionResultDetail : ProductionResultsServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ProductionResult_cannot_be_found()
            {
                //Act
                var result = TimedExecution(() => Service.GetProductionResultDetail(new LotKey()));

                //Assert
                result.AssertInvalid(UserMessages.ProductionResultNotFound);
            }

            [Test]
            public void Returns_ProductionResultDetail_as_expected()
            {
                //Arrange
                const int expectedNumberOfItems = 3;
                var productionResult = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(r => r.Production.Results.EmptyItems());
                var lotKey = new LotKey(productionResult);
                var productionResultItems = new List<LotProductionResultItem>();
                for(var i = 0; i < expectedNumberOfItems; i++)
                {
                    var resultItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(ri => ri.ConstrainByKeys(productionResult));
                    productionResultItems.Add(resultItem);
                }

                //Act
                var result = TimedExecution(() => Service.GetProductionResultDetail(lotKey));

                //Assert
                result.AssertSuccess();
                var items = result.ResultingObject.ResultItems.ToList();

                Assert.AreEqual(expectedNumberOfItems, items.Count);

                productionResult.AssertEqual(result.ResultingObject);
                productionResultItems.ForEach(i =>
                    {
                        var itemKeyValue = new LotProductionResultItemKey(i).KeyValue;
                        i.AssertEqual(items.Single(r => r.ProductionResultItemKey == itemKeyValue));
                    });
            }

            [Test]
            public void Returns_non_successful_result_if_production_results_do_not_exist()
            {
                //Arrange
                var batch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.Results = null);

                //Act
                var result = TimedExecution(() => Service.GetProductionResultDetail(new LotKey(batch)));

                //Assert
                result.AssertNotSuccess(UserMessages.ProductionResultNotFound);
            }

            [Test]
            public void Returns_LotProductionResults_as_expected_if_ProductionBatch_does_not_exist()
            {
                //Arrange
                var results = TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResults>(r => r.Production.ProductionType = ProductionType.ProductionBatch);

                //Act
                var result = TimedExecution(() => Service.GetProductionResultDetail(new LotKey(results)));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(0.0, result.ResultingObject.TargetBatchWeight);
                Assert.IsNull(result.ResultingObject.WorkType);
                Assert.IsNull(result.ResultingObject.BatchStatus);
            }
        }
    }
}