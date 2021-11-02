using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.EntityKey;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;
using Solutionhead.Services;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases
{
    [TestFixture]
    public abstract class SetInventoryPickOrderTestsBase<TService, TOrder, TOrderKey> : ServiceIntegrationTestBase<TService>
        where TService : class
        where TOrderKey : EntityKeyBase, new()
        where TOrder : class, IInventoryPickOrder
    {
        protected abstract void InitializeOrder(TOrder order);
        protected abstract TOrderKey CreateKeyFromOrder(TOrder order);
        protected abstract InventoryPickOrder GetPickOrderFromOrder(TOrder order);
        protected abstract IResult GetResult(SetInventoryPickOrderParameters parameters);

        private void ConstrainItemToOrder(InventoryPickOrderItem item, InventoryPickOrder order)
        {
            item.DateCreated = order.DateCreated;
            item.OrderSequence = order.Sequence;
        }
            
        [Test]
        public void Returns_non_successful_result_if_Order_Key_is_not_valid()
        {
            //Act
            var result = GetResult(new SetInventoryPickOrderParameters("totally invalid key oh my god"));

            //Assert
            result.AssertNotSuccess();
        }

        [Test]
        public void Returns_non_successful_result_if_pickOrderItems_is_null()
        {
            //Act
            var result = GetResult(new SetInventoryPickOrderParameters(new TOrderKey().KeyValue) { InventoryPickOrderItems = null });

            //Assert
            result.AssertNotSuccess();
        }

        [Test]
        public void Returns_non_successful_result_if_any_Quantity_is_less_than_or_equal_to_0()
        {
            //Arrange
            const int quantity = 0;
            var orderKey = CreateKeyFromOrder(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                {
                    TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null),
                    TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null),
                    TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null)
                }));
            var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()).KeyValue;
            var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()).KeyValue;
            var treatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>()).KeyValue;

            Assert.IsNotEmpty(RVCUnitOfWork.InventoryPickOrderItemRepository.All());

            //Act
            var result = GetResult(new SetInventoryPickOrderParameters(orderKey.KeyValue)
                {
                    UserToken = TestUser.UserName,
                    InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters>
                        {
                            new SetInventoryPickOrderItemParameters
                                {
                                    ProductKey = chileProductKey,
                                    PackagingKey = packagingProductKey,
                                    TreatmentKey = treatmentKey,
                                    Quantity = quantity
                                }
                        }
                });

            //Assert
            result.AssertNotSuccess();
        }

        [Test]
        public void Will_remove_all_InventoryPickOrderItems_in_database_if_pickOrderItems_is_empty()
        {
            //Arrange
            var orderKey = CreateKeyFromOrder(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                {
                    TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null),
                    TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null),
                    TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null)
                }));

            Assert.IsNotEmpty(RVCUnitOfWork.InventoryPickOrderItemRepository.All());

            //Act
            var result = GetResult(new SetInventoryPickOrderParameters(orderKey.KeyValue)
                {
                    UserToken = TestUser.UserName,
                    InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters>()
                });

            //Assert
            result.AssertSuccess();
            Assert.IsEmpty(RVCUnitOfWork.InventoryPickOrderItemRepository.All());
        }

        [Test]
        public void Will_set_InventoryPickOrderItems_in_database_as_expected()
        {
            //Arrange
            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => GetPickOrderFromOrder(o).Items = null);
            var orderKey = CreateKeyFromOrder(order);
            TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.InventoryPickOrder = null, i => ConstrainItemToOrder(i, GetPickOrderFromOrder(order)));
            var chileProductKey0 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()).KeyValue;
            var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()).KeyValue;
            var treatmentKey0 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>()).KeyValue;
            const int expectedQuantity0 = 10;
            var chileProductKey1 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()).KeyValue;
            var packagingProductKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()).KeyValue;
            var treatmentKey1 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>()).KeyValue;
            const int expectedQuantity1 = 20;

            //Act
            var testParams = new SetInventoryPickOrderParameters(orderKey.KeyValue)
                {
                    UserToken = TestUser.UserName,
                    InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters>
                        {
                            new SetInventoryPickOrderItemParameters
                                {
                                    ProductKey = chileProductKey0,
                                    PackagingKey = packagingProductKey0,
                                    TreatmentKey = treatmentKey0,
                                    Quantity = expectedQuantity0
                                },
                            new SetInventoryPickOrderItemParameters
                                {
                                    ProductKey = chileProductKey1,
                                    PackagingKey = packagingProductKey1,
                                    TreatmentKey = treatmentKey1,
                                    Quantity = expectedQuantity1
                                }
                        }
                };
            var result = GetResult(testParams);

            //Assert
            result.AssertSuccess();
            var items = RVCUnitOfWork.InventoryPickOrderRepository.Filter(i => true, i => i.Items).Single().Items.ToList();
            Assert.AreEqual(testParams.InventoryPickOrderItems.Count(), items.Count);
            testParams.InventoryPickOrderItems.ForEach(i => items.Single(r =>
                                                                         i.ProductKey == new ProductKey((IProductKey) r).KeyValue &&
                                                                         i.PackagingKey == new PackagingProductKey(r).KeyValue &&
                                                                         i.TreatmentKey == new InventoryTreatmentKey(r).KeyValue &&
                                                                         i.Quantity == r.Quantity));
        }

        [Test]
        public void Will_successfully_create_an_InventoryPickOrderItem_with_null_Treatment()
        {
            //Arrange
            var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TOrder>(InitializeOrder, o => GetPickOrderFromOrder(o).Items = null);
            var orderKey = CreateKeyFromOrder(order);
            TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.InventoryPickOrder = null, i => ConstrainItemToOrder(i, GetPickOrderFromOrder(order)));

            var chileProductKey0 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()).KeyValue;
            var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()).KeyValue;
            const int expectedQuantity0 = 10;

            var chileProductKey1 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()).KeyValue;
            var packagingProductKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()).KeyValue;
            const int expectedQuantity1 = 20;

            //Act
            var testParams = new SetInventoryPickOrderParameters(orderKey.KeyValue)
                {
                    UserToken = TestUser.UserName,
                    InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters>
                            {
                                new SetInventoryPickOrderItemParameters
                                    {
                                        ProductKey = chileProductKey0,
                                        PackagingKey = packagingProductKey0,
                                        TreatmentKey = null,
                                        Quantity = expectedQuantity0
                                    },
                                new SetInventoryPickOrderItemParameters
                                    {
                                        ProductKey = chileProductKey1,
                                        PackagingKey = packagingProductKey1,
                                        TreatmentKey = null,
                                        Quantity = expectedQuantity1
                                    }
                            }
                };
            var result = GetResult(testParams);

            //Assert
            result.AssertSuccess();
            var items = RVCUnitOfWork.InventoryPickOrderRepository.Filter(i => true, i => i.Items.Select(m => m.InventoryTreatment)).Single().Items.ToList();
            Assert.AreEqual(testParams.InventoryPickOrderItems.Count(), items.Count);
            testParams.InventoryPickOrderItems.ForEach(i => items.Single(r =>
                                                                         i.ProductKey == new ProductKey((IProductKey) r).KeyValue &&
                                                                         i.PackagingKey == new PackagingProductKey(r).KeyValue &&
                                                                         r.TreatmentId == 0 &&
                                                                         i.Quantity == r.Quantity));
        }
    }
}