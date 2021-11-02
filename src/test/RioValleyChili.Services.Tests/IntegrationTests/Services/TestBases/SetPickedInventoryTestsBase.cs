using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Data;
using Solutionhead.Services;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases
{
    [TestFixture]
    public abstract class SetPickedInventoryTestsBase<TService, TOrder, TOrderKey> : ServiceIntegrationTestBase<TService>
        where TService : class
        where TOrder : class, IPickedInventoryOrder
        where TOrderKey : EntityKeyBase, IKey<TOrder>, new()
    {
        protected abstract void InitializeOrder(TOrder order);
        protected abstract TOrderKey CreateKeyFromOrder(TOrder order);
        protected abstract IResult GetResult(string key, SetPickedInventoryParameters parameters);

        protected virtual Inventory SetupInventoryToPick(Inventory item)
        {
            return item.SetValidToPick(RinconFacility);
        }

        protected virtual void InitializeValidPickedInventoryItem(PickedInventoryItem item, TOrder order, int? quantity = null)
        {
            item.Lot.SetValidToPick();
            item.FromLocation.ConstrainByKeys(RinconFacility);
            item.ConstrainByKeys(order.PickedInventory).SetCurrentLocationToSource();
            item.Quantity = quantity ?? item.Quantity;
        }

        protected virtual LocationKey GetDestinationWarehouseLocationKey(TOrder order)
        {
            return null;
        }

        private static void SetInventoryToPickedInventory(Inventory inventory, PickedInventoryItem pickedInventory)
        {
            inventory.Lot = null;
            inventory.PackagingProduct = null;
            inventory.Location = null;
            inventory.Treatment = null;

            inventory.LotDateCreated = pickedInventory.LotDateCreated;
            inventory.LotDateSequence = pickedInventory.LotDateSequence;
            inventory.LotTypeId = pickedInventory.LotTypeId;
            inventory.PackagingProductId = pickedInventory.PackagingProductId;
            inventory.LocationId = pickedInventory.FromLocationId;
            inventory.TreatmentId = pickedInventory.TreatmentId;
            inventory.ToteKey = pickedInventory.ToteKey;
        }

        [Test]
        public void Returns_non_successful_result_if_any_PickInventoryItem_exists_with_a_current_Location_not_equal_to_its_source_Location()
        {
            //Arrange
            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder);
            var otherLocation = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
            TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order), i => i.SetCurrentLocation(otherLocation));
            var orderKey = CreateKeyFromOrder(order);

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new List<IPickedInventoryItemParameters>()
                });

            //Assert
            result.AssertNotSuccess(UserMessages.PickedInventoryItemNotInOriginalLocation);
        }

        [Test]
        public void Successfuly_picking_an_Inventory_item_creates_a_PickedInventoryItem_with_quantity_as_expected()
        {
            //Arrange
            const int inventoryQuantity = 10;
            const int pickedQuantity = 7;
            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.PickedInventory.Items = null);
            var orderKey = CreateKeyFromOrder(order);
            var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetValidToPick(), i => i.Quantity = inventoryQuantity, i => SetupInventoryToPick(i).Location.Locked = false);

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new List<IPickedInventoryItemParameters>
                        {
                            new SetPickedInventoryItemParameters
                                {
                                    InventoryKey = new InventoryKey(inventory).KeyValue,
                                    Quantity = pickedQuantity
                                }
                        }
                });

            //Assert
            result.AssertSuccess();
            Assert.AreEqual(pickedQuantity, RVCUnitOfWork.PickedInventoryRepository.FindByKey(new PickedInventoryKey(order.PickedInventory), i => i.Items).Items.Single().Quantity);
        }

        [Test]
        public void Successfuly_picking_an_Inventory_item_adjusts_Inventory_record_quantity_as_expected()
        {
            //Arrange
            const int inventoryQuantity = 10;
            const int pickedQuantity = 7;
            const int expectedQuantity = inventoryQuantity - pickedQuantity;
            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.PickedInventory.Items = null);
            var orderKey = CreateKeyFromOrder(order);
            var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetValidToPick(), i => i.Quantity = inventoryQuantity, i => SetupInventoryToPick(i).Location.Locked = false);
            var inventoryKey = new InventoryKey(inventory);

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new List<IPickedInventoryItemParameters>
                                {
                                    new SetPickedInventoryItemParameters
                                        {
                                            InventoryKey = inventoryKey.KeyValue,
                                            Quantity = pickedQuantity
                                        }
                                }
                });

            //Assert
            result.AssertSuccess();
            Assert.AreEqual(expectedQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey).Quantity);
        }

        [Test]
        public void Returns_non_successful_result_if_increasing_PickedInventoryItem_quantity_without_enough_Inventory_quantity_to_pick_from()
        {
            //Arrange
            const int pickedQuantity = 10;
            const int inventoryQuantity = 5;
            const int newPickedQuantity = pickedQuantity + inventoryQuantity + 1;
            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.PickedInventory.Items = null);
            var orderKey = CreateKeyFromOrder(order);
            var pickedInventoryItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedQuantity));
            var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => SetInventoryToPickedInventory(i, pickedInventoryItem), i => i.Quantity = inventoryQuantity);

            TestHelper.ResetContext();

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
            {
                UserToken = TestUser.UserName,
                PickedInventoryItems = new List<IPickedInventoryItemParameters>
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = new InventoryKey(inventory),
                                        Quantity = newPickedQuantity
                                    }
                            }
            });

            //Assert
            result.AssertNotSuccess(UserMessages.NegativeInventoryLots);
        }

        [Test]
        public void Increasing_PickedInventoryItem_quantity_will_reduce_existing_Inventory_quantity_as_expected()
        {
            //Arrange
            const int pickedQuantity = 10;
            const int inventoryQuantity = 7;
            const int newPickedQuantity = 12;
            const int expectedInventoryQuantity = 5;

            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder);
            var orderKey = CreateKeyFromOrder(order);
            var pickedInventoryItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedQuantity));
            var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => SetInventoryToPickedInventory(i, pickedInventoryItem), i => i.Quantity = inventoryQuantity);
            
            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
            {
                UserToken = TestUser.UserName,
                PickedInventoryItems = new List<IPickedInventoryItemParameters>
                        {
                            new SetPickedInventoryItemParameters
                                {
                                    InventoryKey = new InventoryKey(inventory).KeyValue,
                                    Quantity = newPickedQuantity
                                }
                        }
            });

            //Assert
            result.AssertSuccess();
            Assert.AreEqual(expectedInventoryQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory)).Quantity);
        }

        [Test]
        public void Reducing_PickedInventoryItem_quantity_will_add_to_Inventory_quantity_if_it_exists()
        {
            //Arrange
            const int pickedQuantity = 10;
            const int inventoryQuantity = 7;
            const int newPickedQuantity = 5;
            const int expectedInventoryQuantity = 12;

            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.PickedInventory.Items = null);
            var orderKey = CreateKeyFromOrder(order);
            var pickedInventoryItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedQuantity));
            var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => SetInventoryToPickedInventory(i, pickedInventoryItem), i => i.Quantity = inventoryQuantity);

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
            {
                UserToken = TestUser.UserName,
                PickedInventoryItems = new List<IPickedInventoryItemParameters>
                        {
                            new SetPickedInventoryItemParameters
                                {
                                    InventoryKey = new InventoryKey(inventory).KeyValue,
                                    Quantity = newPickedQuantity
                                }
                        }
            });

            //Assert
            result.AssertSuccess();
            Assert.AreEqual(expectedInventoryQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory)).Quantity);
        }

        [Test]
        public void Reducing_PickedInventoryItem_quantity_will_create_Inventory_record_with_expected_quantity_if_it_doesnt_exist()
        {
            //Arrange
            const int pickedQuantity = 10;
            const int newPickedQuantity = 6;
            const int expectedInventoryQuantity = 4;

            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.PickedInventory.Items = null);
            var orderKey = CreateKeyFromOrder(order);
            var pickedInventoryItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedQuantity));
            var inventoryKey = new InventoryKey(pickedInventoryItem, pickedInventoryItem, pickedInventoryItem, pickedInventoryItem, pickedInventoryItem.ToteKey);
            
            Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey));

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
            {
                UserToken = TestUser.UserName,
                PickedInventoryItems = new List<IPickedInventoryItemParameters>
                        {
                            new SetPickedInventoryItemParameters
                                {
                                    InventoryKey = inventoryKey,
                                    Quantity = newPickedQuantity
                                }
                        }
            });

            //Assert
            result.AssertSuccess();
            Assert.AreEqual(expectedInventoryQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey).Quantity);
        }

        [Test]
        public void Removing_PickedInventoryItem_quantity_will_update_existing_Inventory_record_quantity_as_expected()
        {
            //Arrange
            const int pickedQuantity = 10;
            const int inventoryQuantity = 7;
            const int expectedInventoryQuantity = 17;

            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.PickedInventory.Items = null);
            var orderKey = CreateKeyFromOrder(order);
            var pickedInventoryItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedQuantity));
            var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => SetInventoryToPickedInventory(i, pickedInventoryItem), i => i.Quantity = inventoryQuantity);

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
            {
                UserToken = TestUser.UserName,
                PickedInventoryItems = new List<IPickedInventoryItemParameters>()
            });

            //Assert
            result.AssertSuccess();
            Assert.AreEqual(expectedInventoryQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory)).Quantity);
        }

        [Test]
        public void Removing_PickedInventoryItem_quantity_will_create_Inventory_record_with_expected_quantity_if_it_doesnt_exist()
        {
            //Arrange
            const int pickedQuantity = 10;
            const int expectedInventoryQuantity = 10;

            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.PickedInventory.Items = null);
            var orderKey = CreateKeyFromOrder(order);
            var pickedInventoryItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedQuantity));
            var inventoryKey = new InventoryKey(pickedInventoryItem);

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new List<IPickedInventoryItemParameters>()
                });

            //Assert
            result.AssertSuccess();
            Assert.AreEqual(expectedInventoryQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey).Quantity);
        }

        [Test]
        public void Will_successfuly_set_PickedInventoryItem_and_Inventory_records_as_expected_in_database()
        {
            //Arrange
            const int pickedItemToLeaveAlone_quantity = 10;

            const int pickedItemToUpdate_quantity = 12;
            const int pickedItemToUpdate_expectedQuantity = 7;
            const int inventoryItemToUpdate_quantity = 1;
            const int inventoryItemToUpdate_expectedQuantity = 6;

            const int pickedItemToCreate_expectedQuantity = 5;
            const int inventoryItemToRemove_quantity = 5;

            const int pickedItemToRemove_quantity = 22;
            const int inventoryItemToCreate_expectedQuantity = 22;

            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.PickedInventory.Items = null);
            var orderKey = CreateKeyFromOrder(order);
            var pickedItemToLeaveAlone = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedItemToLeaveAlone_quantity));
            var pickedItemToUpdate = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedItemToUpdate_quantity), i => i.FromLocation.Locked = false);
            var inventoryItemToUpdate = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => SetInventoryToPickedInventory(i, pickedItemToUpdate), i => i.Quantity = inventoryItemToUpdate_quantity);
            var inventoryItemToRemove = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetValidToPick(), i => i.Quantity = inventoryItemToRemove_quantity, i => SetupInventoryToPick(i).Location.Locked = false);
            var pickedItemToRemove = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => InitializeValidPickedInventoryItem(i, order, pickedItemToRemove_quantity));

            //Act
            var result = GetResult(orderKey.KeyValue, new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new List<IPickedInventoryItemParameters>
                        {
                            new SetPickedInventoryItemParameters
                                {
                                    InventoryKey = new InventoryKey(pickedItemToLeaveAlone),
                                    Quantity = pickedItemToLeaveAlone_quantity
                                },
                            new SetPickedInventoryItemParameters
                                {
                                    InventoryKey = new InventoryKey(inventoryItemToUpdate),
                                    Quantity = pickedItemToUpdate_expectedQuantity
                                },
                            new SetPickedInventoryItemParameters
                                {
                                    InventoryKey = new InventoryKey(inventoryItemToRemove),
                                    Quantity = pickedItemToCreate_expectedQuantity
                                }
                        }
                });

            //Assert
            result.AssertSuccess();
            Assert.AreEqual(pickedItemToLeaveAlone_quantity, RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(new PickedInventoryItemKey(pickedItemToLeaveAlone)).Quantity);
            Assert.AreEqual(pickedItemToUpdate_expectedQuantity, RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(new PickedInventoryItemKey(pickedItemToUpdate)).Quantity);
            Assert.AreEqual(inventoryItemToUpdate_expectedQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventoryItemToUpdate)).Quantity);
            var inventoryToRemoveKey = new InventoryKey(inventoryItemToRemove);
            Assert.AreEqual(pickedItemToCreate_expectedQuantity, RVCUnitOfWork.PickedInventoryItemRepository.All().ToList().Single(p => inventoryToRemoveKey.Equals(new InventoryKey(p))).Quantity);
            Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToRemoveKey));
            Assert.IsNull(RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(new PickedInventoryItemKey(pickedItemToRemove)));
            Assert.AreEqual(inventoryItemToCreate_expectedQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(pickedItemToRemove)).Quantity);
        }
    }
}