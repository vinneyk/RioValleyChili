using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;
using CreateIntraWarehouseOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.CreateIntraWarehouseOrderParameters;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class IntraWarehouseOrderServiceTests : ServiceIntegrationTestBase<IntraWarehouseOrderService>
    {
        [TestFixture]
        public class CreateIntraWarehouseOrder : IntraWarehouseOrderServiceTests
        {
            [Test]
            public void Returns_expected_IntraWarehouseOrderKey_on_success()
            {
                //Arrange
                var expectedUser = TestUser.UserName;
                const decimal expectedSheetNumber = 123.1m;
                const string expectedOperatorName = "Little Jimmy";
                var expectedMovementDate = new DateTime(2012, 3, 29);

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 100);
                var inventoryKey = new InventoryKey(inventory);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                {
                    UserToken = expectedUser,
                    TrackingSheetNumber = expectedSheetNumber,
                    OperatorName = expectedOperatorName,
                    MovementDate = expectedMovementDate,
                    PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                            {
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        DestinationLocationKey = destinationLocationKey,
                                        Quantity = 10
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.IntraWarehouseOrderRepository.All().Single();
                Assert.AreEqual(new IntraWarehouseOrderKey(order).KeyValue, result.ResultingObject);
            }

            [Test]
            public void Creates_IntraWarehouseOrder_record_as_expected()
            {
                //Arrange
                const int expectedSequence = 1;
                var expectedUser = TestUser.UserName;
                const decimal expectedSheetNumber = 123.1m;
                const string expectedOperatorName = "Little Jimmy";
                var expectedMovementDate = new DateTime(2012, 3, 29);

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 100);
                var inventoryKey = new InventoryKey(inventory);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                    {
                        UserToken = expectedUser,
                        TrackingSheetNumber = expectedSheetNumber,
                        OperatorName = expectedOperatorName,
                        MovementDate = expectedMovementDate,
                        PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                            {
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        DestinationLocationKey = destinationLocationKey,
                                        Quantity = 10
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.IntraWarehouseOrderRepository.Filter(o => true, o => o.Employee).Single();
                Assert.AreEqual(expectedSequence, order.Sequence);
                Assert.AreEqual(expectedUser, order.Employee.UserName);
                Assert.AreEqual(expectedSheetNumber, order.TrackingSheetNumber);
                Assert.AreEqual(expectedOperatorName, order.OperatorName);
                Assert.AreEqual(expectedMovementDate, order.MovementDate);
            }

            [Test]
            public void Creates_PickedInventory_record_as_expected()
            {
                //Arrange
                const int expectedSequence = 1;
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetAdditiveLot(), i => i.Quantity = 100);
                var inventoryKey = new InventoryKey(inventory);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                {
                    UserToken = TestUser.UserName,
                    TrackingSheetNumber = 123.1m,
                    OperatorName = "Manny",
                    MovementDate = new DateTime(2020, 1, 1),
                    PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                            {
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        DestinationLocationKey = destinationLocationKey,
                                        Quantity = 10
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.PickedInventoryRepository.All().Single();
                Assert.AreEqual(expectedSequence, order.Sequence);
            }

            [Test]
            public void Creates_PickedInventory_with_Archived_set_to_true()
            {
                //Arrange
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetAdditiveLot(), i => i.Quantity = 100);
                var inventoryKey = new InventoryKey(inventory);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                {
                    UserToken = TestUser.UserName,
                    TrackingSheetNumber = 123.1m,
                    OperatorName = "Manny",
                    MovementDate = new DateTime(2020, 1, 1),
                    PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                            {
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        DestinationLocationKey = destinationLocationKey,
                                        Quantity = 10
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                var pickedInventory = RVCUnitOfWork.PickedInventoryRepository.All().Single();
                Assert.IsTrue(pickedInventory.Archived);
            }

            [Test]
            public void Creates_PickedInventoryItem_records_as_expected()
            {
                //Arrange
                const int quantityPicked0 = 50;
                const int quantityPicked1 = 75;

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 100);
                var inventoryKey0 = new InventoryKey(inventory);
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.ConstrainByKeys(inventory.Location), i => i.Quantity = 100));
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TrackingSheetNumber = 123.1m,
                        OperatorName = "Oh operator",
                        MovementDate = new DateTime(2012, 3, 29),
                        PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                            {
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey0.KeyValue,
                                        DestinationLocationKey = destinationLocationKey,
                                        Quantity = quantityPicked0
                                    },
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey1.KeyValue,
                                        DestinationLocationKey = destinationLocationKey,
                                        Quantity = quantityPicked1
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var orderKey = new IntraWarehouseOrderKey(KeyParserHelper.ParseResult<IIntraWarehouseOrderKey>(result.ResultingObject).ResultingObject);
                var items = RVCUnitOfWork.IntraWarehouseOrderRepository.FindByKey(orderKey,
                    i => i.PickedInventory.Items.Select(t => t.FromLocation),
                    i => i.PickedInventory.Items.Select(t => t.CurrentLocation))
                    .PickedInventory.Items.ToList();
                Assert.AreEqual(2, items.Count);
                
                var item0 = items.Single(i => inventoryKey0.Equals(new InventoryKey(i, i, i.FromLocation, i, i.ToteKey)));
                Assert.AreEqual(quantityPicked0, item0.Quantity);

                var item1 = items.Single(i => inventoryKey1.Equals(new InventoryKey(i, i, i.FromLocation, i, i.ToteKey)));
                Assert.AreEqual(quantityPicked1, item1.Quantity);
            }

            [Test]
            public void Modifies_existing_Inventory_records_as_expected()
            {
                //Arrange
                const int quantityPicked0 = 50;
                const int quantityPicked1 = 75;
                const int quantity0 = 100;
                const int quantity1 = 200;
                const int expectedQuantity0 = quantity0 - quantityPicked0;
                const int expectedQuantity1 = quantity1 - quantityPicked1;

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = quantity0);
                var inventoryKey0 = new InventoryKey(inventory);
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.ConstrainByKeys(inventory.Location), i => i.Quantity = quantity1));
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));               

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TrackingSheetNumber = 123.1m,
                        OperatorName = "Oh operator",
                        MovementDate = new DateTime(2012, 3, 29),
                        PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                                {
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey0.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked0
                                        },
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey1.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked1
                                        }
                                }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedQuantity0, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey0).Quantity);
                Assert.AreEqual(expectedQuantity1, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey1).Quantity);
            }

            [Test]
            public void Creates_Inventory_records_as_expected()
            {
                //Arrange
                const int quantityPicked0 = 50;
                const int quantityPicked1 = 75;
                const int quantity0 = 100;
                const int quantity1 = 200;
                
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = quantity0);
                var inventoryKey0 = new InventoryKey(inventory);
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.ConstrainByKeys(inventory.Location), i => i.Quantity = quantity1));
                var destinationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location)));
                var destinationKey1 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location)));
                var expectedInventoryKey0 = new InventoryKey(inventoryKey0, inventoryKey0, destinationKey0, inventoryKey0, inventoryKey0.InventoryKey_ToteKey);
                var expectedInventoryKey1 = new InventoryKey(inventoryKey1, inventoryKey1, destinationKey1, inventoryKey1, inventoryKey1.InventoryKey_ToteKey);

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TrackingSheetNumber = 123.1m,
                        OperatorName = "Oh operator",
                        MovementDate = new DateTime(2012, 3, 29),
                        PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                                {
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey0.KeyValue,
                                            DestinationLocationKey = destinationKey0,
                                            Quantity = quantityPicked0
                                        },
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey1.KeyValue,
                                            DestinationLocationKey = destinationKey1,
                                            Quantity = quantityPicked1
                                        }
                                }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(quantityPicked0, RVCUnitOfWork.InventoryRepository.FindByKey(expectedInventoryKey0).Quantity);
                Assert.AreEqual(quantityPicked1, RVCUnitOfWork.InventoryRepository.FindByKey(expectedInventoryKey1).Quantity);
            }

            [Test]
            public void Picking_from_the_same_Inventory_record_will_modify_that_record_by_total_quantity_picked()
            {
                //Arrange
                const int existingQuantity = 100;
                const int quantityPicked0 = 22;
                const int quantityPicked1 = 50;
                const int expectedQuantity = existingQuantity - (quantityPicked0 + quantityPicked1);

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = existingQuantity);
                var inventoryKey = new InventoryKey(inventory);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));
                
                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TrackingSheetNumber = 123.1m,
                        OperatorName = "Oh operator",
                        MovementDate = new DateTime(2012, 3, 29),
                        PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                            {
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        DestinationLocationKey = destinationLocationKey,
                                        Quantity = quantityPicked0
                                    },
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = inventoryKey.KeyValue,
                                        DestinationLocationKey = destinationLocationKey,
                                        Quantity = quantityPicked1
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey).Quantity);
            }

            [Test]
            public void Picking_into_the_same_existing_Inventory_record_will_modify_that_record_by_total_quantity_picked()
            {
                //Arrange
                const int quantityPicked0 = 23;
                const int quantityPicked1 = 54;
                const int existingQuantity = 100;
                const int expectedQuantity = existingQuantity + quantityPicked0 + quantityPicked1;

                var existingInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = existingQuantity);
                var existingInventoryKey = new InventoryKey(existingInventory);
                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 200, i => i.ConstrainByKeys(existingInventoryKey, existingInventoryKey, null, existingInventoryKey, existingInventory.Location, existingInventoryKey.InventoryKey_ToteKey)));
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 200, i => i.ConstrainByKeys(existingInventoryKey, existingInventoryKey, null, existingInventoryKey, existingInventory.Location, existingInventoryKey.InventoryKey_ToteKey)));
                var destinationLocationKey = new LocationKey(existingInventory);

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TrackingSheetNumber = 123.1m,
                        OperatorName = "Oh operator",
                        MovementDate = new DateTime(2012, 3, 29),
                        PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                                {
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey0.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked0
                                        },
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey1.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked1
                                        }
                                }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(existingInventoryKey).Quantity);
            }

            [Test]
            public void Returns_non_successful_result_if_total_picked_quantity_exceeds_existing_quantity_of_an_Inventory_record()
            {
                //Arrange
                const int existingQuantity = 100;
                const int quantityPicked0 = 67;
                const int quantityPicked1 = 34;

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = existingQuantity);
                var inventoryKey = new InventoryKey(inventory);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                {
                    UserToken = TestUser.UserName,
                    TrackingSheetNumber = 123.1m,
                    OperatorName = "Oh operator",
                    MovementDate = new DateTime(2012, 3, 29),
                    PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                                {
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked0
                                        },
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked1
                                        }
                                }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.NegativeInventoryLots);
            }

            [Test]
            public void Returns_non_successful_result_if_moving_Inventory_from_different_Warehouses()
            {
                //Arrange
                const int existingQuantity = 100;
                const int quantityPicked0 = 67;
                const int quantityPicked1 = 34;

                var inventory0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = existingQuantity);
                var inventoryKey0 = new InventoryKey(inventory0);
                var inventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = existingQuantity);
                var inventoryKey1 = new InventoryKey(inventory1);

                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory0.Location.Facility)));

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                {
                    UserToken = TestUser.UserName,
                    TrackingSheetNumber = 123.1m,
                    OperatorName = "Oh operator",
                    MovementDate = new DateTime(2012, 3, 29),
                    PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                                {
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey0.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked0
                                        },
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey1.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked1
                                        }
                                }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.IntraWarehouseOrderDifferentWarehouses);
            }

            [Test]
            public void Returns_non_successful_result_if_moving_Inventory_to_a_different_Warehouse()
            {
                //Arrange
                const int existingQuantity = 100;
                const int quantityPicked0 = 67;
                const int quantityPicked1 = 34;

                var inventory0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = existingQuantity);
                var inventoryKey0 = new InventoryKey(inventory0);
                var inventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.ConstrainByKeys(inventory0.Location), i => i.Quantity = existingQuantity);
                var inventoryKey1 = new InventoryKey(inventory1);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                {
                    UserToken = TestUser.UserName,
                    TrackingSheetNumber = 123.1m,
                    OperatorName = "Oh operator",
                    MovementDate = new DateTime(2012, 3, 29),
                    PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                                {
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey0.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked0
                                        },
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey1.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked1
                                        }
                                }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.IntraWarehouseOrderDifferentWarehouses);
            }

            [Test]
            public void Removes_Inventory_record_if_total_picked_quantity_equals_existing_quantity()
            {
                //Arrange
                const int existingQuantity = 100;
                const int quantityPicked0 = 67;
                const int quantityPicked1 = 33;

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = existingQuantity);
                var inventoryKey = new InventoryKey(inventory);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(inventory.Location.Facility)));

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                {
                    UserToken = TestUser.UserName,
                    TrackingSheetNumber = 123.00010005m,
                    OperatorName = "Oh operator",
                    MovementDate = new DateTime(2012, 3, 29),
                    PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                                {
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked0
                                        },
                                    new IntraWarehouseOrderPickedItemParameters
                                        {
                                            InventoryKey = inventoryKey.KeyValue,
                                            DestinationLocationKey = destinationLocationKey,
                                            Quantity = quantityPicked1
                                        }
                                }
                });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey));
            }
        }

        [TestFixture]
        public class UpdateIntraWarehouseOrder : IntraWarehouseOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_IntraWarehouseOrder_record_could_not_be_found()
            {
                //Act
                var result = Service.UpdateIntraWarehouseOrder(new UpdateIntraWarehouseOrderParameters
                    {
                        IntraWarehouseOrderKey = new IntraWarehouseOrderKey(),
                        UserToken = TestUser.UserName,
                        OperatorName = "Smoooth",
                        MovementDate = new DateTime(2012, 4, 1)
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.IntraWarehouseOrderNotFound);
            }

            [Test]
            public void Updates_IntraWarehouseOrder_record_as_expected_on_success()
            {
                //Arrange
                var expectedUser = TestUser.UserName;
                const string expectedOperatorName = "OPERATOR";
                var expectedMovementDate = new DateTime(2012, 3, 29);

                var orderKey = new IntraWarehouseOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<IntraWarehouseOrder>());

                //Act
                var result = Service.UpdateIntraWarehouseOrder(new UpdateIntraWarehouseOrderParameters
                    {
                        IntraWarehouseOrderKey = orderKey.KeyValue,
                        UserToken = expectedUser,
                        OperatorName = expectedOperatorName,
                        MovementDate = expectedMovementDate
                    });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.IntraWarehouseOrderRepository.FindByKey(orderKey, o => o.Employee);
                Assert.AreEqual(expectedUser, order.Employee.UserName);
                Assert.AreEqual(expectedOperatorName, order.OperatorName);
                Assert.AreEqual(expectedMovementDate, order.MovementDate);
            }
        }

        [TestFixture]
        public class GetIntraWarehouseOrderSummaries : IntraWarehouseOrderServiceTests
        {
            [Test]
            public void Returns_empty_collection_if_there_are_no_IntraWarehouseOrders()
            {
                //Act
                var result = Service.GetIntraWarehouseOrderSummaries();

                //Assert
                result.AssertSuccess();
                Assert.NotNull(result.ResultingObject);
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_IntraWarehouseOrders_without_PickedInventoryDetails()
            {
                //Arrange
                StartStopwatch();

                var orderKey0 = new IntraWarehouseOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<IntraWarehouseOrder>());
                var orderKey1 = new IntraWarehouseOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<IntraWarehouseOrder>());
                var orderKey2 = new IntraWarehouseOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<IntraWarehouseOrder>());

                TestHelper.ResetContext();
                StopWatchAndWriteTime("Arrange");

                //Act
                var result = TimedExecution(() => Service.GetIntraWarehouseOrderSummaries());

                //Assert
                result.AssertSuccess();
                var orders = result.ResultingObject.ToList().Cast<IntraWarehouseOrderReturn>().ToList();
                Assert.IsNull(orders.Single(o => o.MovementKey == orderKey0.KeyValue).PickedInventoryDetail);
                Assert.IsNull(orders.Single(o => o.MovementKey == orderKey1.KeyValue).PickedInventoryDetail);
                Assert.IsNull(orders.Single(o => o.MovementKey == orderKey2.KeyValue).PickedInventoryDetail);
            }
        }

        [TestFixture]
        public class GetIntraWarehouseOrders : GetInventoryOrderTestsBase<IntraWarehouseOrderService, IntraWarehouseOrder, IIntraWarehouseOrderDetailReturn>
        {
            [Test]
            public void Returns_non_empty_collection_if_no_orders_exist()
            {
                //Act
                var result = Service.GetIntraWarehouseOrders();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_IntraWarehouseOrder_with_details_as_expected()
            {
                //Arrange
                StartStopwatch();

                const int expectedNumberOfItems = 3;
                var intraWarehouseOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<IntraWarehouseOrder>(o => o.PickedInventory.Items = null);
                var pickedItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(intraWarehouseOrder));
                var pickedKey0 = new PickedInventoryItemKey(pickedItem0);
                var pickedItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(intraWarehouseOrder));
                var pickedKey1 = new PickedInventoryItemKey(pickedItem1);
                var pickedItem2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(intraWarehouseOrder));
                var pickedKey2 = new PickedInventoryItemKey(pickedItem2);

                StopWatchAndWriteTime("Arrange");

                //Act
                var result = TimedExecution(() => Service.GetIntraWarehouseOrders());

                //Assert
                result.AssertSuccess();
                var warehouseOrder = result.ResultingObject.FirstOrDefault(o => o.TrackingSheetNumber == intraWarehouseOrder.TrackingSheetNumber);
                Assert.AreEqual(new PickedInventoryKey(intraWarehouseOrder).KeyValue, warehouseOrder.PickedInventory.PickedInventoryKey);

                var items = warehouseOrder.PickedInventory.PickedInventoryItems.ToList();
                Assert.AreEqual(expectedNumberOfItems, items.Count);
                Assert.IsNotNull(items.Single(i => pickedKey0.KeyValue == i.PickedInventoryItemKey));
                Assert.IsNotNull(items.Single(i => pickedKey1.KeyValue == i.PickedInventoryItemKey));
                Assert.IsNotNull(items.Single(i => pickedKey2.KeyValue == i.PickedInventoryItemKey));
            }

            #region Protected Parts

            protected override Func<IntraWarehouseOrder, IResult<IIntraWarehouseOrderDetailReturn>> MethodUnderTest { get { return o => new SuccessResult<IIntraWarehouseOrderDetailReturn>(Service.GetIntraWarehouseOrders().ResultingObject.FirstOrDefault(r => r.TrackingSheetNumber == o.TrackingSheetNumber)); } }
            protected override Func<IntraWarehouseOrder> CreateParentRecord { get { return () => TestHelper.CreateObjectGraphAndInsertIntoDatabase<IntraWarehouseOrder>(o => o.PickedInventory.Items = null); } }
            protected override Func<IntraWarehouseOrder, PickedInventory> GetPickedInventoryRecordFromParent { get { return o => o.PickedInventory; } }
            protected override Func<IIntraWarehouseOrderDetailReturn, List<IPickedInventoryItemReturn>> GetPickedInventoryItemsFromResult { get { return r => r.PickedInventory.PickedInventoryItems.ToList(); } }

            #endregion
        }
    }
}