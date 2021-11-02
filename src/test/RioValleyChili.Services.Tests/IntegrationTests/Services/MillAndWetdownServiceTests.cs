using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class MillAndWetdownServiceTests : ServiceIntegrationTestBase<MillAndWetdownService>
    {
        [TestFixture]
        public class CreateMillAndWetdown : MillAndWetdownServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Lot_already_exists_with_expected_LotKey()
            {
                //Arrange
                var productionDate = new DateTime(2012, 3, 29);
                const int lotSequence = 23;

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.SetLotKey(LotTypeEnum.WIP, productionDate, lotSequence));

                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 100));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var parameters = new CreateMillAndWetdownParameters
                    {
                        ProductionDate = productionDate,
                        LotSequence = lotSequence,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 100200101,
                                        PackagingProductKey = packagingKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                    };
                ((IUserIdentifiable)parameters).UserToken = TestUser.UserName;
                var result = Service.CreateMillAndWetdown(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.LotExistsWithKey);
            }

            [Test]
            public void Returns_non_successful_result_if_ResultItems_is_null_or_empty()
            {
                //Act
                var resultNull = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                                {
                                    new MillAndWetdownPickedItemParameters
                                        {
                                            InventoryKey = new InventoryKey(InventoryKey.Null).KeyValue,
                                            Quantity = 10,
                                        }
                                }
                    });

                var resultEmpty = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        ResultItems = new List<MillAndWetdownResultItemParameters>(),
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                                {
                                    new MillAndWetdownPickedItemParameters
                                        {
                                            InventoryKey = new InventoryKey(InventoryKey.Null).KeyValue,
                                            Quantity = 10,
                                        }
                                }
                    });

                //Assert
                resultNull.AssertNotSuccess(UserMessages.MillAndWetdownResultsRequired);
                resultEmpty.AssertNotSuccess(UserMessages.MillAndWetdownResultsRequired);
            }

            [Test]
            public void Returns_non_sucessful_result_if_PickedItems_is_null_or_empty()
            {
                //Act
                var resultNull = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 12,
                                        PackagingProductKey = new PackagingProductKey(PackagingProductKey.Null).KeyValue,
                                        LocationKey = new LocationKey(LocationKey.Null).KeyValue
                                    }
                            }
                    });

                var resultEmpty = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 12,
                                        PackagingProductKey = new PackagingProductKey(PackagingProductKey.Null).KeyValue,
                                        LocationKey = new LocationKey(LocationKey.Null).KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>()
                    });

                //Assert
                resultNull.AssertNotSuccess(UserMessages.MillAndWetdownPickedRequired);
                resultEmpty.AssertNotSuccess(UserMessages.MillAndWetdownPickedRequired);
            }

            [Test]
            public void Creates_new_MillAndWetdownEntry_record_and_returns_expected_key_on_success()
            {
                //Arrange
                var expectedDate = new DateTime(2010, 3, 29);
                const int expectedSequence = 23;

                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 100, i => i.Lot.SetChileLot().ChileLot.NullProduction()));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var parameters = new CreateMillAndWetdownParameters
                    {
                        ProductionDate = expectedDate,
                        LotSequence = 23,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 100200101,
                                        PackagingProductKey = packagingKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                    };
                ((IUserIdentifiable) parameters).UserToken = TestUser.UserName;
                var result = Service.CreateMillAndWetdown(parameters);

                //Assert
                result.AssertSuccess();
                var millAndWetdown = RVCUnitOfWork.ChileLotProductionRepository.All().Single();
                Assert.AreEqual(expectedDate, millAndWetdown.LotDateCreated);
                Assert.AreEqual(expectedSequence, millAndWetdown.LotDateSequence);
                Assert.AreEqual(new LotKey(millAndWetdown).KeyValue, result.ResultingObject);
            }

            [Test]
            public void Creates_new_ChileLot_record_with_expected_key_on_success()
            {
                //Arrange
                var expectedLotDate = new DateTime(2013, 3, 29);
                const int expectedLotSequence = 23;
                const int expectedLotTypeId = (int) LotTypeEnum.WIP;

                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 100));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                Assert.IsNull(RVCUnitOfWork.ChileLotRepository.All().FirstOrDefault(c => c.LotDateCreated == expectedLotDate && c.LotDateSequence == expectedLotSequence && c.LotTypeId == expectedLotTypeId));
                Assert.IsNull(RVCUnitOfWork.LotRepository.All().FirstOrDefault(l => l.LotDateCreated == expectedLotDate && l.LotDateSequence == expectedLotSequence && l.LotTypeId == expectedLotTypeId));

                //Act
                var parameters = new CreateMillAndWetdownParameters
                    {
                        ProductionDate = expectedLotDate,
                        LotSequence = expectedLotSequence,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 100200101,
                                        PackagingProductKey = packagingKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                    };
                ((IUserIdentifiable)parameters).UserToken = TestUser.UserName;

                var result = Service.CreateMillAndWetdown(parameters);

                //Assert
                result.AssertSuccess();
                Assert.IsNotNull(RVCUnitOfWork.ChileLotRepository.All().FirstOrDefault(c => c.LotDateCreated == expectedLotDate && c.LotDateSequence == expectedLotSequence && c.LotTypeId == expectedLotTypeId));
            }

            [Test]
            public void Creates_Lot_record_with_LotProductionStatus_set_to_Produced()
            {
                //Arrange
                const int expectedLotTypeId = (int)LotTypeEnum.WIP;

                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 100));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                Assert.IsNull(RVCUnitOfWork.ChileLotRepository.All().FirstOrDefault(c => c.LotDateCreated == new DateTime(2013, 3, 29) && c.LotDateSequence == 23 && c.LotTypeId == expectedLotTypeId));
                Assert.IsNull(RVCUnitOfWork.LotRepository.All().FirstOrDefault(l => l.LotDateCreated == new DateTime(2013, 3, 29) && l.LotDateSequence == 23 && l.LotTypeId == expectedLotTypeId));

                //Act
                var parameters = new CreateMillAndWetdownParameters
                    {
                        ProductionDate = new DateTime(2013, 3, 29),
                        LotSequence = 23,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 100200101,
                                        PackagingProductKey = packagingKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                    };
                ((IUserIdentifiable)parameters).UserToken = TestUser.UserName;

                var result = Service.CreateMillAndWetdown(parameters);

                //Assert
                result.AssertSuccess();
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c => true, c => c.Lot).FirstOrDefault(c => c.LotDateCreated == new DateTime(2013, 3, 29) && c.LotDateSequence == 23 && c.LotTypeId == expectedLotTypeId);
                Assert.AreEqual(LotProductionStatus.Produced, chileLot.Lot.ProductionStatus);
            }

            [Test]
            public void Creates_new_PickedInventoryItems_as_expected_though_picking_from_the_same_Inventory_item_more_than_once_on_success()
            {
                //Arrange
                const int expectedItemCount = 3;
                const int quantity0 = 10;
                const int quantity1 = 20;
                const int quantity2 = 30;

                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 100, i => i.Lot.SetChileLot().ChileLot.NullProduction()));
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 200, i => i.Lot.SetChileLot().ChileLot.NullProduction()));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var parameters = new CreateMillAndWetdownParameters
                    {
                        ProductionDate = new DateTime(2012, 3, 29),
                        LotSequence = 23,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 100200101,
                                        PackagingProductKey = packagingKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey0.KeyValue,
                                        Quantity = 10,
                                    },
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey0.KeyValue,
                                        Quantity = 20,
                                    },
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey1.KeyValue,
                                        Quantity = 30,
                                    },
                            }
                    };
                ((IUserIdentifiable)parameters).UserToken = TestUser.UserName;
                var result = Service.CreateMillAndWetdown(parameters);

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.ChileLotProductionRepository.Filter(m => true, m => m.PickedInventory.Items).First().PickedInventory.Items.ToList();
                Assert.AreEqual(expectedItemCount, items.Count);
                Assert.IsNotNull(items.SingleOrDefault(i => new InventoryKey(i, i, i, i, i.ToteKey).KeyValue == inventoryKey0.KeyValue && i.Quantity == quantity0));
                Assert.IsNotNull(items.SingleOrDefault(i => new InventoryKey(i, i, i, i, i.ToteKey).KeyValue == inventoryKey0.KeyValue && i.Quantity == quantity1));
                Assert.IsNotNull(items.SingleOrDefault(i => new InventoryKey(i, i, i, i, i.ToteKey).KeyValue == inventoryKey1.KeyValue && i.Quantity == quantity2));
            }

            [Test]
            public void Adjusts_Inventory_records_as_expected_on_success()
            {
                //Arrange
                const int quantity0 = 100;
                const int quantity1 = 200;
                const int pickedQuantity0_0 = 10;
                const int pickedQuantity0_1 = 20;
                const int expectedQuantity0 = quantity0 - (pickedQuantity0_0 + pickedQuantity0_1);
                const int pickedQuantity1 = quantity1;

                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = quantity0));
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = quantity1));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var result = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = new DateTime(2012, 3, 29),
                        LotSequence = 23,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 100200101,
                                        PackagingProductKey = packagingKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey0.KeyValue,
                                        Quantity = pickedQuantity0_0
                                    },
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey0.KeyValue,
                                        Quantity = pickedQuantity0_1
                                    },
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey1.KeyValue,
                                        Quantity = pickedQuantity1
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedQuantity0, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey0).Quantity);
                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey1));
            }

            [Test]
            public void Creates_new_MillAndWetdownResultItems_as_expected_on_success()
            {
                //Arrange
                const int expectedResults = 3;
                const int expectedQuantity0 = 10;
                const int expectedQuantity1 = 20;
                const int expectedQuantity2 = 30;

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(c => c.LotDateCreated = new DateTime(2012, 3, 29));
                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 100));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var packagingKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var warehouseLocationKey1 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var result = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = new DateTime(2012, 3, 29, 6, 23, 17),
                        LotSequence = 23,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = expectedQuantity0,
                                        PackagingProductKey = packagingKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue
                                    },
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = expectedQuantity1,
                                        PackagingProductKey = packagingKey1.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue
                                    },
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = expectedQuantity2,
                                        PackagingProductKey = packagingKey0.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        Quantity = 4
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var resultItems = RVCUnitOfWork.LotProductionResultItemsRepository.All().ToList();
                Assert.AreEqual(expectedResults, resultItems.Count);
                Assert.IsNotNull(resultItems.SingleOrDefault(i => i.Quantity == expectedQuantity0 && packagingKey0.Equals(i) && warehouseLocationKey0.Equals(i)));
                Assert.IsNotNull(resultItems.SingleOrDefault(i => i.Quantity == expectedQuantity1 && packagingKey1.Equals(i) && warehouseLocationKey1.Equals(i)));
                Assert.IsNotNull(resultItems.SingleOrDefault(i => i.Quantity == expectedQuantity2 && packagingKey0.Equals(i) && warehouseLocationKey1.Equals(i)));
            }

            [Test]
            public void Creates_new_Inventory_records_based_on_ResultItems_as_expected_on_success()
            {
                //Arrange
                var expectedLotDate = new DateTime(2013, 3, 29);
                const int expectedLotSequence = 23;
                const int expectedLotTypeId = (int)LotTypeEnum.WIP;

                const int expectedItems = 3;
                const int expectedQuantity0 = 10;
                const int expectedQuantity1 = 20;
                const int expectedQuantity2 = 30;

                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 100));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var packagingKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var warehouseLocationKey1 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var result = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = expectedLotDate,
                        LotSequence = expectedLotSequence,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = expectedQuantity0,
                                        PackagingProductKey = packagingKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue
                                    },
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = expectedQuantity1,
                                        PackagingProductKey = packagingKey1.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue
                                    },
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = expectedQuantity2,
                                        PackagingProductKey = packagingKey0.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        Quantity = 4
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var inventory = RVCUnitOfWork.InventoryRepository.Filter(i => i.LotDateCreated == expectedLotDate && i.LotDateSequence == expectedLotSequence && i.LotTypeId == expectedLotTypeId).ToList();
                Assert.AreEqual(expectedItems, inventory.Count);
                Assert.IsNotNull(inventory.SingleOrDefault(i => packagingKey0.Equals(i) && warehouseLocationKey0.Equals(i) && i.Quantity == expectedQuantity0));
                Assert.IsNotNull(inventory.SingleOrDefault(i => packagingKey1.Equals(i) && warehouseLocationKey1.Equals(i) && i.Quantity == expectedQuantity1));
                Assert.IsNotNull(inventory.SingleOrDefault(i => packagingKey0.Equals(i) && warehouseLocationKey1.Equals(i) && i.Quantity == expectedQuantity2));
            }

            [Test]
            public void Creates_InventoryTransaction_records_as_expected()
            {
                //Arrange
                var inventoryPickedKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 100));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var packagingKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var warehouseLocationKey1 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var parameters = new CreateMillAndWetdownParameters
                    {
                        ProductionDate = new DateTime(2013, 3, 29),
                        LotSequence = 23,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 10,
                                        PackagingProductKey = packagingKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue
                                    },
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 20,
                                        PackagingProductKey = packagingKey1.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue
                                    },
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 30,
                                        PackagingProductKey = packagingKey0.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryPickedKey.KeyValue,
                                        Quantity = 4
                                    }
                            }
                    };
                ((IUserIdentifiable)parameters).UserToken = TestUser.UserName;
                var result = Service.CreateMillAndWetdown(parameters);

                //Assert
                result.AssertSuccess();

                var resultLotKey = new LotKey(new LotKey().Parse(result.ResultingObject));
                var transactions = RVCUnitOfWork.InventoryTransactionsRepository.Filter(InventoryTransactionPredicates.BySourceLot(resultLotKey), i => i.DestinationLot).ToList();
                transactions.ForEach(t => Assert.IsNull(t.DestinationLot));
                
                transactions = RVCUnitOfWork.InventoryTransactionsRepository.Filter(InventoryTransactionPredicates.BySourceLot(new LotKey(inventoryPickedKey)), i => i.DestinationLot).ToList();
                transactions.ForEach(t => Assert.AreEqual(resultLotKey.KeyValue, new LotKey(t.DestinationLot).KeyValue));
            }

            [Test, Issue("Mill and Wetdown typically picks items from locked locations, and at some point I added a rule to prevent this because it" +
                         "seemed a good idea at the time. -RI 2016-10-18", References = new[] { "RVCADMIN-1345" })]
            public void Can_pick_items_from_locked_locations()
            {
                //Arrange
                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(
                    i => i.SetValidToPick(RinconFacility).Quantity = 100,
                    i => i.Location.Locked = true,
                    i => i.Lot.SetChileLot().ChileLot.NullProduction()));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var result = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = new DateTime(2010, 3, 29),
                        LotSequence = 1,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 100200101,
                                        PackagingProductKey = packagingKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
            }

            [Test, Issue("User entered an invalid lot number in the UI which ended up creating an unexpected lot number." +
                         "Updated service to fail if sequence number is less than 1, this technically doesn't resolve the issue" +
                         "but it would have caught it in this case. -RI 2016-11-22",
                         References = new[] { "RVCADMIN-1397", "RVCADMIN-1396" })]
            public void Sequence_number_must_be_greater_than_zero()
            {
                //Arrange
                var expectedDate = new DateTime(2010, 3, 29);
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var productionLineKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var parameters = new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = expectedDate,
                        LotSequence = 0,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow
                    };
                var result = Service.CreateMillAndWetdown(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.LotDateSequenceLessThanOne);
            }
        }

        [TestFixture]
        public class UpdateMillAndWetdown : MillAndWetdownServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ChileLotProduction_record_does_not_exist()
            {
                //Act
                var result = Service.UpdateMillAndWetdown(new UpdateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = new ChileProductKey(),
                        LotKey = new LotKey(),
                        ProductionLineKey = new LocationKey(),
                        ResultItems = new List<IMillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        LocationKey = new LocationKey(),
                                        PackagingProductKey = new PackagingProductKey(),
                                        Quantity = 1
                                    }
                            },
                        PickedItems = new List<IMillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = new InventoryKey(),
                                        Quantity = 1
                                    }
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.MillAndWetdownEntryNotFound);
            }

            [Test]
            public void Updates_PickedInventoryItems_as_expected()
            {
                //Arrange
                var production = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(c =>
                    c.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(3, l => l.ForEach(i => i.SetCurrentLocationToSource())));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));
                var location = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packaging = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var parameters = new UpdateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = production.ToLotKey(),
                        ChileProductKey = production.ResultingChileLot.ToChileProductKey(),
                        ProductionBegin = production.Results.ProductionBegin,
                        ProductionEnd = production.Results.ProductionEnd,
                        ProductionLineKey = production.Results.ToLocationKey(),
                        PickedItems = new List<IMillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        Quantity = inventory.Quantity
                                    }
                            },
                        ResultItems = new List<IMillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        LocationKey = location.ToLocationKey(),
                                        PackagingProductKey = packaging.ToPackagingProductKey(),
                                        Quantity = 123
                                    }
                            }
                    };

                //Act
                var result = Service.UpdateMillAndWetdown(parameters);

                //Assert
                result.AssertSuccess();
                ResetUnitOfWork();
                var resultItems = RVCUnitOfWork.ChileLotProductionRepository.FindByKey(production.ToLotKey(), p => p.PickedInventory.Items).PickedInventory.Items.ToList();
                parameters.PickedItems.AssertEquivalent(resultItems, e => e.InventoryKey, r => r.ToInventoryKey(), (e, r) => Assert.AreEqual(e.Quantity, r.Quantity));
            }

            [Test]
            public void Updates_ResultsItems_as_expected()
            {
                //Arrange
                var production = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(c =>
                    c.Results.ResultItems = TestHelper.List<LotProductionResultItem>(3));
                production.Results.ResultItems.ForEach(r => TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(r, r, r, r, null, "").Quantity = r.Quantity));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));
                var location = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packaging = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var parameters = new UpdateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = production.ToLotKey(),
                        ChileProductKey = production.ResultingChileLot.ToChileProductKey(),
                        ProductionBegin = production.Results.ProductionBegin,
                        ProductionEnd = production.Results.ProductionEnd,
                        ProductionLineKey = production.Results.ToLocationKey(),
                        PickedItems = new List<IMillAndWetdownPickedItemParameters>
                                {
                                    new MillAndWetdownPickedItemParameters
                                        {
                                            InventoryKey = inventory.ToInventoryKey(),
                                            Quantity = inventory.Quantity
                                        }
                                },
                        ResultItems = new List<IMillAndWetdownResultItemParameters>
                                {
                                    new MillAndWetdownResultItemParameters
                                        {
                                            LocationKey = location.ToLocationKey(),
                                            PackagingProductKey = packaging.ToPackagingProductKey(),
                                            Quantity = 123
                                        }
                                }
                    };

                //Act
                var result = Service.UpdateMillAndWetdown(parameters);

                //Assert
                result.AssertSuccess();
                ResetUnitOfWork();
                var resultItems = RVCUnitOfWork.ChileLotProductionRepository.FindByKey(production.ToLotKey(), p => p.Results.ResultItems).Results.ResultItems.ToList();
                parameters.ResultItems.AssertEquivalent(resultItems, e => new InventoryKey(production, packaging, location, GlobalKeyHelpers.NoTreatmentKey, ""), r => new InventoryKey(r),
                    (e, r) => Assert.AreEqual(e.Quantity, r.Quantity));
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_change_ChileProduct_would_results_in_different_LotType()
            {
                //Arrange
                var production = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ChileState = production.ResultingChileLot.ChileProduct.ChileState != ChileStateEnum.WIP ? ChileStateEnum.WIP : ChileStateEnum.FinishedGoods);
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));
                var location = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packaging = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();

                var parameters = new UpdateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = production.ToLotKey(),
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        ProductionBegin = production.Results.ProductionBegin,
                        ProductionEnd = production.Results.ProductionEnd,
                        ProductionLineKey = production.Results.ToLocationKey(),
                        PickedItems = new List<IMillAndWetdownPickedItemParameters>
                                {
                                    new MillAndWetdownPickedItemParameters
                                        {
                                            InventoryKey = inventory.ToInventoryKey(),
                                            Quantity = inventory.Quantity
                                        }
                                },
                        ResultItems = new List<IMillAndWetdownResultItemParameters>
                                {
                                    new MillAndWetdownResultItemParameters
                                        {
                                            LocationKey = location.ToLocationKey(),
                                            PackagingProductKey = packaging.ToPackagingProductKey(),
                                            Quantity = 123
                                        }
                                }
                    };

                //Act
                var result = Service.UpdateMillAndWetdown(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.ChileProductDifferentLotType);
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_change_ChileProduct_when_resulting_lot_has_been_picked()
            {
                //Arrange
                var production = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(null, production));
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ChileState = production.ResultingChileLot.ChileProduct.ChileState);
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));
                var location = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packaging = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();

                var parameters = new UpdateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = production.ToLotKey(),
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        ProductionBegin = production.Results.ProductionBegin,
                        ProductionEnd = production.Results.ProductionEnd,
                        ProductionLineKey = production.Results.ToLocationKey(),
                        PickedItems = new List<IMillAndWetdownPickedItemParameters>
                                {
                                    new MillAndWetdownPickedItemParameters
                                        {
                                            InventoryKey = inventory.ToInventoryKey(),
                                            Quantity = inventory.Quantity
                                        }
                                },
                        ResultItems = new List<IMillAndWetdownResultItemParameters>
                                {
                                    new MillAndWetdownResultItemParameters
                                        {
                                            LocationKey = location.ToLocationKey(),
                                            PackagingProductKey = packaging.ToPackagingProductKey(),
                                            Quantity = 123
                                        }
                                }
                    };

                //Act
                var result = Service.UpdateMillAndWetdown(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.LotHasExistingPickedInventory);
            }

            [Test]
            public void Updates_existing_ChileLot_product_on_success()
            {
                //Arrange
                var production = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ChileState = production.ResultingChileLot.ChileProduct.ChileState);
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));
                var location = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var packaging = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();

                var parameters = new UpdateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = production.ToLotKey(),
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        ProductionBegin = production.Results.ProductionBegin,
                        ProductionEnd = production.Results.ProductionEnd,
                        ProductionLineKey = production.Results.ToLocationKey(),
                        PickedItems = new List<IMillAndWetdownPickedItemParameters>
                                {
                                    new MillAndWetdownPickedItemParameters
                                        {
                                            InventoryKey = inventory.ToInventoryKey(),
                                            Quantity = inventory.Quantity
                                        }
                                },
                        ResultItems = new List<IMillAndWetdownResultItemParameters>
                                {
                                    new MillAndWetdownResultItemParameters
                                        {
                                            LocationKey = location.ToLocationKey(),
                                            PackagingProductKey = packaging.ToPackagingProductKey(),
                                            Quantity = 123
                                        }
                                }
                    };

                //Act
                var result = Service.UpdateMillAndWetdown(parameters);

                //Assert
                result.AssertSuccess();
                Assert.IsTrue(chileProduct.ToChileProductKey().Equals(RVCUnitOfWork.ChileLotRepository.FindByKey(production.ToLotKey())));
            }

            [Test, Issue("User needs to undo the effects of having entered an invalid lot number, but we currently have no delete function." +
                         "Instead, I've modified the update method to allow user to set empty result/picked items which will at least put" +
                         "inventory in the state it was before the invalid data had been entered. -RI 2016-11-22",
                         References = new[] { "RVCADMIN-1396" })]
            public void Remove_existing_result_and_picked_items()
            {
                //Arrange
                var chileProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>().ToChileProductKey();
                var picked = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility));
                var locationKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>().ToLocationKey();
                var productionLineKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine).ToLocationKey();

                var createResult = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,

                        ChileProductKey = chileProductKey,
                        ProductionLineKey = productionLineKey,
                        LotSequence = 1,

                        ProductionDate = new DateTime(2016, 1, 1),
                        ProductionBegin = new DateTime(2016, 1, 1),
                        ProductionEnd = new DateTime(2016, 1, 1),

                        PickedItems = new List<IMillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = picked.ToInventoryKey(),
                                        Quantity = picked.Quantity
                                    }
                            },
                        ResultItems = new List<IMillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        LocationKey = locationKey,
                                        PackagingProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey(),
                                        Quantity = 123
                                    }
                            }
                    });
                createResult.AssertSuccess();
                var predicate = locationKey.FindByPredicate;
                var inventory = RVCUnitOfWork.InventoryRepository.SourceQuery.AsExpandable().FirstOrDefault(i => predicate.Invoke(i.Location));
                Assert.AreEqual(123, inventory.Quantity);

                //Act
                var result = Service.UpdateMillAndWetdown(new UpdateMillAndWetdownParameters
                    {
                        LotKey = createResult.ResultingObject,
                        ChileProductKey = chileProductKey,
                        ProductionLineKey = productionLineKey,
                        UserToken = TestUser.UserName,
                        ProductionBegin = new DateTime(2016, 1, 1),
                        ProductionEnd = new DateTime(2016, 1, 1),
                    });
                
                //Assert
                result.AssertSuccess();

                Assert.AreEqual(picked.Quantity, RVCUnitOfWork.InventoryRepository.FindByKey(picked.ToInventoryKey()).Quantity);
                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(inventory.ToInventoryKey()));
            }
        }

        [TestFixture]
        public class DeleteMillAndWetdown : MillAndWetdownServiceTests
        {
            [Test]
            public void Deletes_ChileLotProduction_and_associated_records()
            {
                //Arrange
                var inventoryKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(RinconFacility).Quantity = 100).ToInventoryKey();
                var chileProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>().ToChileProductKey();
                var packagingKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey();
                var packagingKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey();
                var warehouseLocationKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>().ToLocationKey();
                var warehouseLocationKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>().ToLocationKey();
                var productionLineKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine).ToLocationKey();
                
                var createResult = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = new DateTime(2013, 3, 29),
                        LotSequence = 23,
                        ShiftKey = "SHIFTY",
                        ChileProductKey = chileProductKey.KeyValue,
                        ProductionLineKey = productionLineKey.KeyValue,
                        ProductionBegin = DateTime.UtcNow,
                        ProductionEnd = DateTime.UtcNow,

                        ResultItems = new List<MillAndWetdownResultItemParameters>
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 10,
                                        PackagingProductKey = packagingKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue
                                    },
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 20,
                                        PackagingProductKey = packagingKey1.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue
                                    },
                                new MillAndWetdownResultItemParameters
                                    {
                                        Quantity = 30,
                                        PackagingProductKey = packagingKey0.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue
                                    }
                            },
                        PickedItems = new List<MillAndWetdownPickedItemParameters>
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        Quantity = 4
                                    }
                            }
                    });
                createResult.AssertSuccess();

                var resultKey = KeyParserHelper.ParseResult<ILotKey>(createResult.ResultingObject).ResultingObject.ToLotKey();
                Assert.IsNotNull(RVCUnitOfWork.ChileLotProductionRepository.FindByKey(resultKey));

                //Act
                var deleteResult = Service.DeleteMillAndWetdown(createResult.ResultingObject);

                //Assert
                deleteResult.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.ChileLotProductionRepository.FindByKey(resultKey));
            }
        }

        [TestFixture]
        public class GetMillAndWetdownDetail : MillAndWetdownServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_MillAndWetdownEntry_could_not_be_found()
            {
                //Act
                var result = Service.GetMillAndWetdownDetail(new LotKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.MillAndWetdownEntryNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_ChileLotProduction_is_not_of_MillAndWetdown_type()
            {
                //Arrange
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>());

                //Act
                var result = Service.GetMillAndWetdownDetail(lotKey.KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.MillAndWetdownEntryNotFound);
            }

            [Test]
            public void Returns_MillAndWetdownDetail_with_expected_key()
            {
                //Arrange
                var entryKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot()));

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(entryKey.KeyValue, result.ResultingObject.MillAndWetdownKey);
            }

            [Test]
            public void Returns_TotalProductionTime_property_as_expected()
            {
                //Arrange
                const int expectedProductionTime = 133;
                var begin = new DateTime(2012, 3, 29);
                var end = begin.AddMinutes(expectedProductionTime);

                var entryKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot(),
                    m => m.Results.ProductionBegin = begin, m => m.Results.ProductionEnd = end));

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedProductionTime, result.ResultingObject.TotalProductionTimeMinutes);
            }

            [Test]
            public void Returns_TotalWeightProduced_of_zero_if_no_MillAndWetdownResultItem_records_exist()
            {
                //Arrange
                var entryKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot(), m => m.Results.ResultItems = null));

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(0, result.ResultingObject.TotalWeightProduced);
            }

            [Test]
            public void Returns_TotalWeightProduced_as_expected()
            {
                //Arrange
                const int quantity0 = 20;
                const int quantity1 = 30;
                const int weight0 = 100;
                const int weight1 = 200;
                const int totalWeightExpected = (quantity0 * weight0) + (quantity1 * weight1);

                var packaging0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Weight = weight0);
                var packaging1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Weight = weight1);

                var entryKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot(), m => m.Results.ResultItems = null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.ConstrainByKeys(entryKey, packaging0).Quantity = quantity0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.ConstrainByKeys(entryKey, packaging1).Quantity = quantity1);

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(totalWeightExpected, result.ResultingObject.TotalWeightProduced);
            }

            [Test]
            public void Returns_TotalWeightPicked_of_zero_if_no_PickedInventoryItem_records_exist()
            {
                //Arrange
                var entryKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.PickedInventory.Items = null));

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(0, result.ResultingObject.TotalWeightPicked);
            }
            
            [Test]
            public void Returns_TotalWeightPicked_as_expected()
            {
                //Arrange
                const int quantity0 = 20;
                const int quantity1 = 30;
                const int weight0 = 100;
                const int weight1 = 200;
                const int totalWeightExpected = (quantity0 * weight0) + (quantity1 * weight1);

                var packaging0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Weight = weight0);
                var packaging1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Weight = weight1);

                var entry = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot(), m => m.PickedInventory.Items = null);
                var entryKey = new LotKey(entry);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(entry, null, packaging0).Quantity = quantity0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(entry, null, packaging1).Quantity = quantity1);

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(totalWeightExpected, result.ResultingObject.TotalWeightPicked);
            }

            [Test]
            public void Returns_MillAndWetdownResultItems_with_expected_keys()
            {
                //Arrange
                const int expectedItems = 3;
                var entry = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot(), m => m.Results.ResultItems = null);
                var entryKey = new LotKey(entry);
                var itemKey0 = new LotProductionResultItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.ConstrainByKeys(entry)));
                var itemKey1 = new LotProductionResultItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.ConstrainByKeys(entry)));
                var itemKey2 = new LotProductionResultItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotProductionResultItem>(i => i.ConstrainByKeys(entry)));

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                var items = result.ResultingObject.ResultItems.ToList();
                Assert.AreEqual(expectedItems, items.Count);
                Assert.IsNotNull(items.Single(i => i.MillAndWetdownResultItemKey == itemKey0.KeyValue));
                Assert.IsNotNull(items.Single(i => i.MillAndWetdownResultItemKey == itemKey1.KeyValue));
                Assert.IsNotNull(items.Single(i => i.MillAndWetdownResultItemKey == itemKey2.KeyValue));
            }

            [Test]
            public void Returns_PickedInventoryItems_with_expected_keys()
            {
                //Arrange
                const int expectedItems = 3;
                var entry = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot(), m => m.PickedInventory.Items = null);
                var entryKey = new LotKey(entry);
                var itemKey0 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(entry)));
                var itemKey1 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(entry)));
                var itemKey2 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(entry)));

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                var items = result.ResultingObject.PickedItems.ToList();
                Assert.AreEqual(expectedItems, items.Count);
                Assert.IsNotNull(items.Single(i => i.PickedInventoryItemKey == itemKey0.KeyValue));
                Assert.IsNotNull(items.Single(i => i.PickedInventoryItemKey == itemKey1.KeyValue));
                Assert.IsNotNull(items.Single(i => i.PickedInventoryItemKey == itemKey2.KeyValue));
            }

            [Test]
            public void Returns_PickedInventoryItems_with_expected_Tote_numbers()
            {
                //Arrange
                const int expectedItems = 3;
                const string expectedTote0 = "123";
                const string expectedTote1 = "312";
                string expectedTote2 = null;

                var entry = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot(), m => m.PickedInventory.Items = null);
                var entryKey = new LotKey(entry);
                var itemKey0 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(entry).ToteKey = expectedTote0));
                var itemKey1 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(entry).ToteKey = expectedTote1));
                var itemKey2 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(entry).ToteKey = expectedTote2));

                //Act
                var result = Service.GetMillAndWetdownDetail(entryKey.KeyValue);

                //Assert
                result.AssertSuccess();
                var items = result.ResultingObject.PickedItems.ToList();
                Assert.AreEqual(expectedItems, items.Count);
                Assert.AreEqual(expectedTote0, items.Single(i => i.PickedInventoryItemKey == itemKey0.KeyValue).ToteKey);
                Assert.AreEqual(expectedTote1, items.Single(i => i.PickedInventoryItemKey == itemKey1.KeyValue).ToteKey);
                Assert.AreEqual(expectedTote2, items.Single(i => i.PickedInventoryItemKey == itemKey2.KeyValue).ToteKey);
            }
        }

        [TestFixture]
        public class GetMillAndWetdownSummaries : MillAndWetdownServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_MillAndWetdownEntry_records_exist()
            {
                //Act
                var result = Service.GetMillAndWetdownSummaries();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_expected_MillAndWetdownEntryKeys_on_success()
            {
                //Arrange
                const int expectedResults = 3;
                var key0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot()));
                var key1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot()));
                var key2 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(m => m.ResultingChileLot.Lot.EmptyLot()));

                //Act
                var result = Service.GetMillAndWetdownSummaries();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.MillAndWetdownKey == key0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.MillAndWetdownKey == key1.KeyValue));
                Assert.IsNotNull(results.Single(r => r.MillAndWetdownKey == key2.KeyValue));
            }

            [Test]
            public void Filtering_results_on_TotalProductionTimeInMinutes_property_returns_expected_keys()
            {
                //Arrange
                const int expectedResults = 2;
                const int minProductionTime = 120;

                var expectedKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(
                    m => m.ResultingChileLot.Lot.EmptyLot(), m => m.Results.ProductionEnd = m.Results.ProductionBegin.AddMinutes(minProductionTime)));
                var expectedKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(
                    m => m.ResultingChileLot.Lot.EmptyLot(), m => m.Results.ProductionEnd = m.Results.ProductionBegin.AddMinutes(minProductionTime + 400)));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(
                    m => m.ResultingChileLot.Lot.EmptyLot(), m => m.Results.ProductionEnd = m.Results.ProductionBegin.AddMinutes(1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLotProduction>(
                    m => m.ResultingChileLot.Lot.EmptyLot(), m => m.Results.ProductionEnd = m.Results.ProductionBegin);

                //Act
                var result = Service.GetMillAndWetdownSummaries();
                var filtered = result.ResultingObject.Where(m => m.TotalProductionTimeMinutes >= minProductionTime);

                //Assert
                result.AssertSuccess();
                var results = filtered.ToList();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(m => m.MillAndWetdownKey == expectedKey0.KeyValue));
                Assert.IsNotNull(results.Single(m => m.MillAndWetdownKey == expectedKey1.KeyValue));
            }
        }
    }
}