using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.TreatmentOrderService;
using RioValleyChili.Services.Interfaces.ServiceCompositions;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using Solutionhead.Services;
using CreateTreatmentOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.CreateTreatmentOrderParameters;
using ReceiveTreatmentOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.ReceiveTreatmentOrderParameters;
using SetInventoryPickOrderItemParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SetInventoryPickOrderItemParameters;
using SetPickedInventoryItemParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SetPickedInventoryItemParameters;
using UpdateTreatmentOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.UpdateTreatmentOrderParameters;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class TreatmentOrderServiceTests : ServiceIntegrationTestBase<TreatmentOrderService>
    {
        protected override bool SetupStaticRecords { get { return false; } }

        [TestFixture]
        public class CreateInventoryTreatmentOrder : TreatmentOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_TreatmentFacilityCompany_is_not_of_TreatmentFacility_type()
            {
                //Arrange
                var facility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(c => c.FacilityType = FacilityType.Internal);
                var treatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();

                //Act
                var result = Service.CreateInventoryTreatmentOrder(new CreateTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        SourceFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>()),
                        DestinationFacilityKey = new FacilityKey(facility),
                        TreatmentKey = new InventoryTreatmentKey(treatment)
                    });

                //Assert
                result.AssertNotSuccess(string.Format(UserMessages.FacilityNotOfType, "{0}", FacilityType.Treatment));
            }

            [Test]
            public void Returns_expected_TreatmentOrderKey_on_success()
            {
                //Arrange
                var expectedDateCreated = TimeStamper.CurrentTimeStamp.Date;
                const int expectedSequence = 1;

                var expectedTreatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());
                var expectedFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(c => c.FacilityType = FacilityType.Treatment));

                Assert.IsNull(RVCUnitOfWork.TreatmentOrderRepository.FindBy(t => t.DateCreated == expectedDateCreated && t.Sequence == expectedSequence));

                //Act
                var result = Service.CreateInventoryTreatmentOrder(new CreateTreatmentOrderParameters
                    {
                        SourceFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>()),
                        DestinationFacilityKey = expectedFacilityKey,
                        TreatmentKey = expectedTreatmentKey.KeyValue,
                        UserToken = TestUser.UserName,
                    });

                //Assert
                result.AssertSuccess();
                var treatmentOrder = RVCUnitOfWork.TreatmentOrderRepository.All().Single();
                Assert.AreEqual(new TreatmentOrderKey(treatmentOrder).KeyValue, result.ResultingObject);
                Assert.AreEqual(expectedTreatmentKey, treatmentOrder);
            }

            [Test]
            public void Creates_TreatmentOrder_record_as_expected()
            {
                //Arrange
                const OrderStatus expectedStatus = OrderStatus.Scheduled;
                var expectedUser = TestUser.UserName;
                var expectedDateCreated = TimeStamper.CurrentTimeStamp.Date;
                const int expectedSequence = 1;

                var treatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();
                var expectedTreatmentKey = new InventoryTreatmentKey(treatment);
                var expectedTreatmentFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(c => c.FacilityType = FacilityType.Treatment));

                Assert.IsNull(RVCUnitOfWork.TreatmentOrderRepository.FindBy(t => t.DateCreated == expectedDateCreated && t.Sequence == expectedSequence));

                //Act
                var result = Service.CreateInventoryTreatmentOrder(new CreateTreatmentOrderParameters
                    {
                        SourceFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>()),
                        DestinationFacilityKey = expectedTreatmentFacilityKey,
                        TreatmentKey = expectedTreatmentKey.KeyValue,
                        UserToken = expectedUser
                    });

                //Assert
                result.AssertSuccess();
                var treatmentOrder = RVCUnitOfWork.TreatmentOrderRepository.FindBy(t => t.DateCreated == expectedDateCreated && t.Sequence == expectedSequence, o => o.InventoryShipmentOrder.Employee, o => o.InventoryShipmentOrder.DestinationFacility);
                Assert.AreEqual(expectedUser, treatmentOrder.InventoryShipmentOrder.Employee.UserName);
                Assert.AreEqual(expectedTreatmentKey.KeyValue, new InventoryTreatmentKey(treatmentOrder).KeyValue);
                Assert.AreEqual(expectedStatus, treatmentOrder.InventoryShipmentOrder.OrderStatus);
                Assert.AreEqual(expectedTreatmentFacilityKey, treatmentOrder.InventoryShipmentOrder.DestinationFacility);
            }

            [Test]
            public void Creates_TreatmentOrder_with_no_InventoryPickOrderItems_if_PickOrder_parameters_is_null()
            {
                //Arrange
                var expectedDateCreated = TimeStamper.CurrentTimeStamp.Date;
                const int expectedSequence = 1;

                var treatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();
                var expectedTreatmentKey = new InventoryTreatmentKey(treatment);

                Assert.IsNull(RVCUnitOfWork.TreatmentOrderRepository.FindBy(t => t.DateCreated == expectedDateCreated && t.Sequence == expectedSequence));

                //Act
                var result = Service.CreateInventoryTreatmentOrder(new CreateTreatmentOrderParameters
                    {
                        SourceFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>()),
                        DestinationFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(c => c.FacilityType = FacilityType.Treatment)),
                        TreatmentKey = expectedTreatmentKey.KeyValue,
                        UserToken = TestUser.UserName
                    });

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.TreatmentOrderRepository.All().Select(o => o.InventoryShipmentOrder.InventoryPickOrder.Items).Single();
                Assert.IsEmpty(items);
            }

            [Test]
            public void Creates_TreatmentOrder_with_expected_InventoryPickOrderItems_if_PickOrder_parameters_is_not_null()
            {
                //Arrange
                var expectedDateCreated = TimeStamper.CurrentTimeStamp.Date;
                const int expectedSequence = 1;

                var chileProductKey0 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey0 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());
                const int quantity0 = 10;

                var chileProductKey1 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingProductKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey1 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());
                const int quantity1 = 55;

                var treatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();
                var expectedTreatmentKey = new InventoryTreatmentKey(treatment);
                

                Assert.IsNull(RVCUnitOfWork.TreatmentOrderRepository.FindBy(t => t.DateCreated == expectedDateCreated && t.Sequence == expectedSequence));

                //Act
                var result = Service.CreateInventoryTreatmentOrder(new CreateTreatmentOrderParameters
                    {
                        SourceFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>()),
                        DestinationFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(c => c.FacilityType = FacilityType.Treatment)),
                        TreatmentKey = expectedTreatmentKey,
                        UserToken = TestUser.UserName,
                        InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters>
                                    {
                                        new SetInventoryPickOrderItemParameters
                                            {
                                                ProductKey = chileProductKey0.KeyValue,
                                                PackagingKey = packagingProductKey0.KeyValue,
                                                TreatmentKey = treatmentKey0.KeyValue,
                                                Quantity = quantity0
                                            },
                                        new SetInventoryPickOrderItemParameters
                                            {
                                                ProductKey = chileProductKey1.KeyValue,
                                                PackagingKey = packagingProductKey1.KeyValue,
                                                TreatmentKey = treatmentKey1.KeyValue,
                                                Quantity = quantity1
                                            }
                                    }
                            }
                    );

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.TreatmentOrderRepository.All().Select(o => o.InventoryShipmentOrder.InventoryPickOrder.Items).Single();
                Assert.IsNotNull(items.SingleOrDefault(i => i.Quantity == quantity0 &&
                    i.ProductId == chileProductKey0.ChileProductKey_ProductId &&
                    i.PackagingProductId == packagingProductKey0.PackagingProductKey_ProductId &&
                    i.TreatmentId == treatmentKey0.InventoryTreatmentKey_Id));

                Assert.IsNotNull(items.SingleOrDefault(i => i.Quantity == quantity1 &&
                    i.ProductId == chileProductKey1.ChileProductKey_ProductId &&
                    i.PackagingProductId == packagingProductKey1.PackagingProductKey_ProductId &&
                    i.TreatmentId == treatmentKey1.InventoryTreatmentKey_Id));
            }
        }

        [TestFixture]
        public class UpdateInventoryTreatmentOrder : TreatmentOrderServiceTests
        {
            [Test]
            public void Sets_header_as_expected_on_success()
            {
                //Arrange
                var orderKey = new TreatmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>());
                var treatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());
                var destinationFacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(f => f.FacilityType = FacilityType.Treatment));

                //Act
                var result = Service.UpdateTreatmentOrder(new UpdateTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = orderKey,
                        TreatmentKey = treatmentKey,
                        DestinationFacilityKey = destinationFacilityKey
                    });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.TreatmentOrderRepository.FindByKey(orderKey, o => o.InventoryShipmentOrder.DestinationFacility);
                Assert.AreEqual(treatmentKey, order);
                Assert.AreEqual(destinationFacilityKey, order.InventoryShipmentOrder.DestinationFacility);
            }

            [Test]
            public void Sets_shipment_information_on_success()
            {
                //Arrange
                const string expectedComments = "Ship it good.";
                var orderKey = new TreatmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>());

                //Act
                var result = Service.UpdateTreatmentOrder(new UpdateTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = orderKey,
                        SetShipmentInformation = new SetInventoryShipmentInformationParameters
                            {
                                ShippingInstructions = new SetShippingInstructionsParameters
                                {
                                    ExternalNotes = expectedComments
                                }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.TreatmentOrderRepository.FindByKey(orderKey, o => o.InventoryShipmentOrder.ShipmentInformation);
                Assert.AreEqual(expectedComments, order.InventoryShipmentOrder.ShipmentInformation.ExternalNotes);
            }

            [Test]
            public void Sets_PickOrderItems_on_success()
            {
                //Arrange
                var orderKey = new TreatmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>());
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot));

                var parameters = new UpdateTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = orderKey,
                        InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters>
                            {
                                new SetInventoryPickOrderItemParameters
                                    {
                                        ProductKey = new ChileProductKey(chileLot),
                                        PackagingKey = new PackagingProductKey(inventory),
                                        TreatmentKey = new InventoryTreatmentKey(inventory),
                                        Quantity = 10,
                                    }
                            }
                    };

                //Act
                var result = Service.UpdateTreatmentOrder(parameters);

                //Assert
                result.AssertSuccess();
                var pickOrderItems = RVCUnitOfWork.TreatmentOrderRepository.FindByKey(orderKey, o => o.InventoryShipmentOrder.InventoryPickOrder.Items).InventoryShipmentOrder.InventoryPickOrder.Items.ToList();
                Assert.AreEqual(parameters.InventoryPickOrderItems.Count(), pickOrderItems.Count);
                foreach(var item in parameters.InventoryPickOrderItems)
                {
                    Assert.IsNotNull(pickOrderItems.First(i => i.Quantity == item.Quantity));
                }
            }

            [Test]
            public void Sets_customer_product_and_lot_codes_for_PickedInventoryItems_as_expected()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>();
                var pickedItemKey0 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order.InventoryShipmentOrder)));
                var pickedItemKey1 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order.InventoryShipmentOrder)));
                const string lotCode0 = "LotCode!";
                const string productCode1 = "ProductCode!";
                var parameters = new UpdateTreatmentOrderParameters
                {
                    UserToken = TestUser.UserName,
                    TreatmentOrderKey = new TreatmentOrderKey(order),
                    PickedInventoryItemCodes = new List<ISetPickedInventoryItemCodesParameters>
                            {
                                new SetPickedInventoryItemCodesParameters
                                    {
                                        PickedInventoryItemKey = pickedItemKey0,
                                        CustomerLotCode = lotCode0
                                    },
                                new SetPickedInventoryItemCodesParameters
                                    {
                                        PickedInventoryItemKey = pickedItemKey1,
                                        CustomerProductCode = productCode1
                                    }
                            }
                };

                //Act
                var result = Service.UpdateTreatmentOrder(parameters);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(lotCode0, RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(pickedItemKey0).CustomerLotCode);
                Assert.AreEqual(productCode1, RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(pickedItemKey1).CustomerProductCode);
            }
        }

        [TestFixture]
        public class DeleteInventoryTreatmentOrder : TreatmentOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_TreatmentOrder_does_not_exist()
            {
                //Act
                var result = Service.DeleteTreatmentOrder(new TreatmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.TreatmentOrderNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_order_has_been_fulfilled()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(
                    o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Fulfilled,
                    o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Scheduled);
                
                //Act
                var result = Service.DeleteTreatmentOrder(treatmentOrder.ToTreatmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.CannotDeleteFulfilledShipmentOrder);
            }

            [Test]
            public void Returns_non_successful_result_if_order_has_been_shipped()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(
                    o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled,
                    o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Shipped);
                
                //Act
                var result = Service.DeleteTreatmentOrder(treatmentOrder.ToTreatmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.CannotDeleteShippedShipmentOrder);
            }

            [Test]
            public void Returns_non_successful_result_if_order_has_items_picked_that_are_not_in_their_original_location()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(
                    o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled,
                    o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Scheduled,
                    o => o.InventoryShipmentOrder.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(1),
                    o => o.InventoryShipmentOrder.InventoryPickOrder.PickedInventory.Items = o.InventoryShipmentOrder.PickedInventory.Items);

                //Act
                var result = Service.DeleteTreatmentOrder(treatmentOrder.ToTreatmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.PickedInventoryItemNotInOriginalLocation);
            }

            [Test]
            public void Deletes_TreatmentOrder_record_on_success()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(
                    o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled,
                    o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Scheduled,
                    o => o.InventoryShipmentOrder.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(1, l => l.ForEach(i => i.SetCurrentLocationToSource())),
                    o => o.InventoryShipmentOrder.InventoryPickOrder.PickedInventory.Items = o.InventoryShipmentOrder.PickedInventory.Items);

                //Act
                var result = Service.DeleteTreatmentOrder(treatmentOrder.ToTreatmentOrderKey());

                //Assert
                result.AssertSuccess();
                ResetUnitOfWork();
                Assert.IsNull(RVCUnitOfWork.TreatmentOrderRepository.FindByKey(treatmentOrder.ToTreatmentOrderKey()));
            }
        }

        [TestFixture]
        public class SetInventoryPickOrderTests : SetInventoryPickOrderTestsBase<TreatmentOrderService, TreatmentOrder, TreatmentOrderKey>
        {
            protected override void InitializeOrder(TreatmentOrder order)
            {
                order.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled;
            }

            protected override TreatmentOrderKey CreateKeyFromOrder(TreatmentOrder order)
            {
                return new TreatmentOrderKey(order);
            }

            protected override InventoryPickOrder GetPickOrderFromOrder(TreatmentOrder order)
            {
                return order.InventoryShipmentOrder.InventoryPickOrder;
            }

            protected override IResult GetResult(SetInventoryPickOrderParameters parameters)
            {
                return Service.UpdateTreatmentOrder(new UpdateTreatmentOrderParameters
                    {
                        UserToken = parameters.UserToken,
                        TreatmentOrderKey = parameters.OrderKey,
                        InventoryPickOrderItems = parameters.InventoryPickOrderItems
                    });
            }

            [Test]
            public void Returns_non_successful_result_if_TreatmentOrder_Status_is_fulfilled()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Fulfilled);

                //Act
                var result = GetResult(new SetInventoryPickOrderParameters(treatmentOrder.ToTreatmentOrderKey())
                    {
                        UserToken = TestUser.UserName,
                        InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters>
                            {
                                new SetInventoryPickOrderItemParameters
                                    {
                                        ProductKey = new ProductKey(ProductKey.Null).KeyValue,
                                        PackagingKey = new PackagingProductKey(PackagingProductKey.Null).KeyValue,
                                        TreatmentKey = new InventoryTreatmentKey(InventoryTreatmentKey.Null).KeyValue,
                                        Quantity = 10
                                    }
                            }
                    });

                //Assert
                result.AssertNotSuccess();
            }
        }

        [TestFixture]
        public class SetPickedInventoryTests : SetPickedInventoryTestsBase<TreatmentOrderService, TreatmentOrder, TreatmentOrderKey>
        {
            private TreatmentOrder _order;

            protected override void InitializeOrder(TreatmentOrder order)
            {
                _order = order;
                order.InventoryShipmentOrder.SetSourceFacility(RinconFacility);
                order.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled;
                order.InventoryShipmentOrder.DestinationFacility.Locations = new List<Location> { TestHelper.CreateObjectGraph<Location>() };
            }

            protected override TreatmentOrderKey CreateKeyFromOrder(TreatmentOrder order)
            {
                return new TreatmentOrderKey(order);
            }

            protected override LocationKey GetDestinationWarehouseLocationKey(TreatmentOrder order)
            {
                return new LocationKey(order.InventoryShipmentOrder.DestinationFacility.Locations.First());
            }

            protected override IResult GetResult(string key, SetPickedInventoryParameters parameters)
            {
                return ((IPickInventoryServiceComponent)Service).SetPickedInventory(key, parameters);
            }

            protected override void InitializeValidPickedInventoryItem(PickedInventoryItem item, TreatmentOrder order, int? quantity = null)
            {
                item.SetSourceWarehouse(_order.InventoryShipmentOrder.SourceFacility);
                item.Treatment = null;
                item.TreatmentId = order.InventoryTreatmentId;
                base.InitializeValidPickedInventoryItem(item, order, quantity);
            }

            protected override Inventory SetupInventoryToPick(Inventory item)
            {
                return base.SetupInventoryToPick(item.ConstrainByKeys(null, null, null, Data.Models.StaticRecords.StaticInventoryTreatments.NoTreatment, _order.InventoryShipmentOrder.SourceFacility));
            }

            [Test]
            public void Returns_non_successful_result_if_picking_from_a_Lot_associated_with_a_conflicting_treatment()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(InitializeOrder);
                var invalidInventory0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Released, i => i.Location.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder.SourceFacility),
                    i => i.Lot.Inventory = TestHelper.List<Inventory>(1));

                var invalidInventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Released, i => i.Location.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder.SourceFacility),
                    i => i.Lot.PickedInventory = TestHelper.List<PickedInventoryItem>(1));

                var invalidInventory2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Released, i => i.Location.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder.SourceFacility));
                var otherOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(otherOrder.InventoryShipmentOrder, invalidInventory2, null, Data.Models.StaticRecords.StaticInventoryTreatments.NoTreatment));

                //Act
                var result = GetResult(treatmentOrder.ToTreatmentOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new List<IPickedInventoryItemParameters>
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = invalidInventory0.ToInventoryKey(),
                                        Quantity = 1
                                    },
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = invalidInventory1.ToInventoryKey(),
                                        Quantity = 1
                                    },
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = invalidInventory2.ToInventoryKey(),
                                        Quantity = 1
                                    }
                            }
                    });

                //Assert
                result.AssertNotSuccess(new List<string>
                    {
                        string.Format(UserMessages.LotConflictingInventoryTreatment, invalidInventory0.ToLotKey()),
                        string.Format(UserMessages.LotConflictingInventoryTreatment, invalidInventory1.ToLotKey()),
                        string.Format(UserMessages.LotConflictingInventoryTreatment, invalidInventory2.ToLotKey())
                    }, "\n");
            }
        }

        [TestFixture]
        public class GetTreatmentOrders : TreatmentOrderServiceTests
        {
            [Test]
            public void Returns_empty_result_if_there_are_no_TreatmentOrders()
            {
                //Act
                StartStopwatch();
                var result = Service.GetTreatmentOrders();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject.ToList());
            }

            [Test]
            public void Returns_TreatmentOrders_with_expected_keys()
            {
                //Arrange
                const int treatmentOrderCount = 3;
                var treatmentOrderKeys = new List<string>();
                for(int i = 0; i < treatmentOrderCount; i++)
                {
                    treatmentOrderKeys.Add(new TreatmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>()).KeyValue);
                }

                //Act
                StartStopwatch();
                var result = Service.GetTreatmentOrders();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(treatmentOrderCount, results.Count);
                treatmentOrderKeys.ForEach(k => Assert.AreEqual(1, results.Count(o => o.MovementKey == k)));
            }

            [Test]
            public void Returns_successful_result_if_PickedInventory_has_no_items()
            {
                //Arrange
                var treatmentOrderKey = new TreatmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(o => o.ClearPickedItems()));

                //Act
                StartStopwatch();
                var result = Service.GetTreatmentOrders();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(0, results.Single(r => r.MovementKey == treatmentOrderKey.KeyValue).PickedInventory.TotalQuantityPicked);
            }

            [Test]
            public void Returns_successful_result_if_InventoryPickOrder_has_no_items()
            {
                //Arrange
                var treatmentOrderKey = new TreatmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(o => o.InventoryShipmentOrder.InventoryPickOrder.Items = null));

                //Act
                StartStopwatch();
                var result = Service.GetTreatmentOrders();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(0, results.Single(r => r.MovementKey == treatmentOrderKey.KeyValue).PickOrder.TotalQuantity);
            }
        }

        [TestFixture]
        public class GetTreatmentOrder : GetInventoryOrderTestsBase<TreatmentOrderService, TreatmentOrder, ITreatmentOrderDetailReturn>
        {
            [Test]
            public void Returns_Invalid_result_if_TreatmentOrderKey_is_invalid()
            {
                //Act
                var result = TimedExecution(() => Service.GetTreatmentOrder("invalid key"));

                //Assert
                result.AssertInvalid(UserMessages.InvalidTreatmentOrderKey);
            }

            [Test]
            public void Returns_Failure_result_if_TreatmentOrder_could_not_be_found()
            {
                //Act
                var result = TimedExecution(() => Service.GetTreatmentOrder(new TreatmentOrderKey()));

                //Assert
                result.AssertFailure(UserMessages.TreatmentOrderNotFound);
            }

            [Test]
            public void Returns_result_with_expected_OrderKey_and_detailed_item_information()
            {
                //Arrange
                StartStopwatch();

                const OrderStatus expectedStatus = OrderStatus.Scheduled;
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = expectedStatus,
                    o => o.InventoryShipmentOrder.InventoryPickOrder.Items = new List<InventoryPickOrderItem> { TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null) },
                    o => o.InventoryShipmentOrder.InventoryPickOrder.PickedInventory.Items = new List<PickedInventoryItem> { TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null) });
                var treatmentOrderKey = new TreatmentOrderKey(treatmentOrder).KeyValue;
                Assert.GreaterOrEqual(treatmentOrder.InventoryShipmentOrder.InventoryPickOrder.Items.Count, 1);
                Assert.GreaterOrEqual(treatmentOrder.InventoryShipmentOrder.PickedInventory.Items.Count, 1);
                
                StopWatchAndWriteTime("Arrange");

                //Act
                var result = TimedExecution(() => Service.GetTreatmentOrder(treatmentOrderKey));

                //Assert
                result.AssertSuccess();
                var treatmentOrderResult = result.ResultingObject;
                Assert.IsNotNull(treatmentOrderResult);
                Assert.AreEqual(treatmentOrderKey, treatmentOrderResult.MovementKey);
                Assert.AreEqual(expectedStatus, treatmentOrderResult.OrderStatus);
                Assert.AreEqual(InventoryOrderEnum.Treatments, treatmentOrderResult.InventoryOrderEnum);

                Assert.AreEqual(treatmentOrder.InventoryShipmentOrder.InventoryPickOrder.Items.Count, result.ResultingObject.PickOrder.PickOrderItems.Count());
                treatmentOrder.InventoryShipmentOrder.InventoryPickOrder.Items.ForEach(i =>
                    Assert.AreEqual(1, result.ResultingObject.PickOrder.PickOrderItems.Count(p =>
                        p.ProductKey == new ProductKey((IProductKey) i).KeyValue &&
                        p.PackagingProductKey == new PackagingProductKey(i).KeyValue &&
                        p.TreatmentKey == new InventoryTreatmentKey(i).KeyValue &&
                        p.Quantity == i.Quantity)));

                Assert.AreEqual(treatmentOrder.InventoryShipmentOrder.PickedInventory.Items.Count, result.ResultingObject.PickedInventory.PickedInventoryItems.Count());
                treatmentOrder.InventoryShipmentOrder.PickedInventory.Items.ForEach(p =>
                    Assert.AreEqual(1, result.ResultingObject.PickedInventory.PickedInventoryItems.Count(r =>
                        r.InventoryKey == new InventoryKey(p, p, p.FromLocation, p, p.ToteKey).KeyValue &&
                        r.QuantityPicked == p.Quantity)));
            }

            [Test]
            public void Returns_expected_AttributeNamesByProductType_on_success()
            {
                //Arrange
                var unexpectedAttributeName = new KeyValuePair<string, string>("00", "Bad Attribute");
                var expectedChileAttributeName0 = new KeyValuePair<string, string>("c0", "Chile Attribute Name 0");
                var expectedChileAttributeName1 = new KeyValuePair<string, string>("c1", "Chile Attribute Name 1");
                var expectedAdditiveAttributeName0 = new KeyValuePair<string, string>("a0", "Additive AttributeName 0");
                var expectedAdditiveAttributeName1 = new KeyValuePair<string, string>("a1", "AdditiveAttributeName 1");
                var expectedPackagingAttributeName0 = new KeyValuePair<string, string>("p0", "PackagingAttributeName 0");
                var expectedPackagingAttributeName1 = new KeyValuePair<string, string>("p1", "PackagingAttributeName 1");
                var expectedChileAdditiveAttributeName = new KeyValuePair<string, string>("ad0", "Chile Additive Attribute");

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(unexpectedAttributeName, false));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedChileAttributeName0, true, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedChileAttributeName1, true, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedAdditiveAttributeName0, true, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedAdditiveAttributeName1, true, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedPackagingAttributeName0, true, false, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedPackagingAttributeName1, true, false, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedChileAdditiveAttributeName, true, true, true));

                var treatmentOrderKey = new TreatmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(o => o.InventoryShipmentOrder.PickedInventory.Items = null));

                //Act
                var result = TimedExecution(() => Service.GetTreatmentOrder(treatmentOrderKey.KeyValue));

                //Assert
                result.AssertSuccess();
                Assert.IsTrue(result.ResultingObject.PickedInventory.AttributeNamesByProductType.All(a => a.Value.Count(n => n.Key == unexpectedAttributeName.Key) == 0));
                Assert.AreEqual(1, result.ResultingObject.PickedInventory.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Chile).Value.Count(n => n.Equals(expectedChileAttributeName0)));
                Assert.AreEqual(1, result.ResultingObject.PickedInventory.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Chile).Value.Count(n => n.Equals(expectedChileAttributeName1)));
                Assert.AreEqual(1, result.ResultingObject.PickedInventory.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Additive).Value.Count(n => n.Equals(expectedAdditiveAttributeName0)));
                Assert.AreEqual(1, result.ResultingObject.PickedInventory.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Additive).Value.Count(n => n.Equals(expectedAdditiveAttributeName1)));
                Assert.AreEqual(1, result.ResultingObject.PickedInventory.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Packaging).Value.Count(n => n.Equals(expectedPackagingAttributeName0)));
                Assert.AreEqual(1, result.ResultingObject.PickedInventory.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Packaging).Value.Count(n => n.Equals(expectedPackagingAttributeName1)));
                Assert.AreEqual(1, result.ResultingObject.PickedInventory.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Chile).Value.Count(n => n.Equals(expectedChileAdditiveAttributeName)));
                Assert.AreEqual(1, result.ResultingObject.PickedInventory.AttributeNamesByProductType.Single(a => a.Key == ProductTypeEnum.Additive).Value.Count(n => n.Equals(expectedChileAdditiveAttributeName)));
            }

            [Test]
            public void Returns_TransitInformation_as_expected_on_success()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>();
                var treatmentOrderKey = new TreatmentOrderKey(treatmentOrder);

                //Act
                var result = TimedExecution(() => Service.GetTreatmentOrder(treatmentOrderKey.KeyValue));

                //Assert
                result.AssertSuccess();
                var treatmentOrderResult = result.ResultingObject as ITreatmentOrderDetailReturn;
                Assert.IsNotNull(treatmentOrderResult);
                Assert.AreEqual(treatmentOrder.InventoryShipmentOrder.ShipmentInformation.TrailerLicenseNumber, treatmentOrderResult.Shipment.TransitInformation.TrailerLicenseNumber);
            }

            [Test]
            public void Returns_ShippingInstructions_as_expected_on_success()
            {
                //Arrange
                const string expectedComments = "Random comment.";
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(o => o.InventoryShipmentOrder.ShipmentInformation.ExternalNotes = expectedComments);
                var treatmentOrderKey = new TreatmentOrderKey(treatmentOrder);

                //Act
                var result = TimedExecution(() => Service.GetTreatmentOrder(treatmentOrderKey.KeyValue));

                //Assert
                result.AssertSuccess();
                var treatmentOrderResult = result.ResultingObject;
                Assert.IsNotNull(treatmentOrderResult);
                Assert.AreEqual(treatmentOrder.InventoryShipmentOrder.ShipmentInformation.ShipTo.Address.AddressLine1, treatmentOrderResult.Shipment.ShippingInstructions.ShipToShippingLabel.Address.AddressLine1);
                Assert.AreEqual(expectedComments, treatmentOrderResult.Shipment.ShippingInstructions.ExternalNotes);
            }

            [Test]
            public void Returns_TreatmentFacilitySummary_as_expected_on_success()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>();
                var treatmentOrderKey = new TreatmentOrderKey(treatmentOrder);
                var treatmentFacilityKey = new FacilityKey(treatmentOrder.InventoryShipmentOrder.DestinationFacility);

                //Act
                var result = TimedExecution(() => Service.GetTreatmentOrder(treatmentOrderKey.KeyValue));

                //Assert
                result.AssertSuccess();
                var treatmentOrderResult = result.ResultingObject;
                Assert.IsNotNull(treatmentOrderResult);
                Assert.AreEqual(treatmentOrder.InventoryShipmentOrder.ShipmentInformation.ShipTo.Address.AddressLine1, treatmentOrderResult.Shipment.ShippingInstructions.ShipToShippingLabel.Address.AddressLine1);
                Assert.AreEqual(treatmentFacilityKey.KeyValue, treatmentOrderResult.DestinationFacility.FacilityKey);
            }

            #region Protected Parts

            protected override Func<TreatmentOrder, IResult<ITreatmentOrderDetailReturn>> MethodUnderTest { get { return o => Service.GetTreatmentOrder(new TreatmentOrderKey(o)); } }
            protected override Func<TreatmentOrder> CreateParentRecord { get { return () => TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(o => o.InventoryShipmentOrder.PickedInventory.Items = null); } }
            protected override Func<TreatmentOrder, PickedInventory> GetPickedInventoryRecordFromParent { get { return o => o.InventoryShipmentOrder.PickedInventory; } }
            protected override Func<ITreatmentOrderDetailReturn, List<IPickedInventoryItemReturn>> GetPickedInventoryItemsFromResult { get { return r => r.PickedInventory.PickedInventoryItems.ToList(); } }

            #endregion
        }

        [TestFixture]
        public class SetShipment : SetShipmentTestsBase<TreatmentOrderService, TreatmentOrder, TreatmentOrderKey>
        {
            protected override TreatmentOrder SetupOrder()
            {
                return TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled);
            }

            protected override TreatmentOrderKey CreateKeyFromOrder(TreatmentOrder order)
            {
                return new TreatmentOrderKey(order);
            }

            protected override ShipmentInformationKey GetShipmentInformationKey(TreatmentOrder order)
            {
                return new ShipmentInformationKey(order.InventoryShipmentOrder);
            }

            protected override IResult GetResult(string key, Shipment shipment)
            {
                return Service.UpdateTreatmentOrder(new UpdateTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = key,
                        SetShipmentInformation = shipment != null ? new SetInventoryShipmentInformationParameters(shipment) : null
                    });
            }
        }

        [TestFixture]
        public class ReceiveOrder : TreatmentOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_TreatmentOrder_could_not_be_found()
            {
                //Arrange
                var warehouseLocation = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                        {
                           UserToken = TestUser.UserName,
                           TreatmentOrderKey = new TreatmentOrderKey(),
                           DestinationLocationKey = new LocationKey(warehouseLocation)
                        });

                //Assert
                result.AssertNotSuccess(UserMessages.TreatmentOrderNotFound);
            }

            [Test]
            public void Returns_non_succesful_result_if_TreatmentOrder_is_already_fulfilled()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = OrderStatus.Fulfilled);
                var treatmentOrderKey = new TreatmentOrderKey(treatmentOrder);
                var warehouseLocation = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var warehouseLocationKey = new LocationKey(warehouseLocation);

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = treatmentOrderKey,
                        DestinationLocationKey = warehouseLocationKey
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.TreatmentOrderAlreadyFulfilled);
            }

            [Test]
            public void Returns_non_successful_result_if_current_location_Inventory_does_not_have_sufficient_quantity()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled, t => t.InventoryShipmentOrder.PickedInventory.Items = null);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                var pickedInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetToPickedCurrentLocation(pickedInventory));

                pickedInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder).Quantity = 10);
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetToPickedCurrentLocation(pickedInventory).Quantity = 9));

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = new TreatmentOrderKey(treatmentOrder),
                        DestinationLocationKey = new LocationKey(destinationLocationKey)
                    });

                //Assert
                result.AssertNotSuccess(string.Format(UserMessages.NegativeInventoryLots, new LotKey(inventoryKey1)));
            }

            [Test]
            public void Sets_TreatmentOrder_Status_to_fulfilled()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled);
                var treatmentOrderKey = new TreatmentOrderKey(treatmentOrder);
                var warehouseLocation = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var warehouseLocationKey = new LocationKey(warehouseLocation);

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = treatmentOrderKey,
                        DestinationLocationKey = warehouseLocationKey
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(OrderStatus.Fulfilled, RVCUnitOfWork.TreatmentOrderRepository.FindByKey(treatmentOrderKey, o => o.InventoryShipmentOrder).InventoryShipmentOrder.OrderStatus);
            }

            [Test]
            public void Sets_PickedInventory_Archived_to_true()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled);
                var treatmentOrderKey = new TreatmentOrderKey(treatmentOrder);
                var warehouseLocation = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var warehouseLocationKey = new LocationKey(warehouseLocation);

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = treatmentOrderKey,
                        DestinationLocationKey = warehouseLocationKey
                    });

                //Assert
                result.AssertSuccess();
                Assert.IsTrue(RVCUnitOfWork.TreatmentOrderRepository.FindByKey(treatmentOrderKey, w => w.InventoryShipmentOrder.PickedInventory).InventoryShipmentOrder.PickedInventory.Archived);
            }

            [Test]
            public void Creates_and_modifies_existing_Inventory_records_as_expected_on_success()
            {
                //Arrange
                const int pickedQuantity0 = 10;
                const int pickedQuantity1 = 22;
                const int pickedQuantity2 = 34;
                const int existingQuantity2 = 25;
                const int expectedQuantity2 = pickedQuantity2 + existingQuantity2;

                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled, t => t.InventoryShipmentOrder.PickedInventory.Items = null);
                var destinationLocation = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                
                var lot0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot());
                var pickedInventory0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder, lot0), i => i.Quantity = pickedQuantity0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetToPickedCurrentLocation(pickedInventory0));
                var expectedInventoryKey0 = new InventoryKey(pickedInventory0, pickedInventory0, destinationLocation, treatmentOrder, pickedInventory0.ToteKey);

                var lot1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot());
                var pickedInventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder, lot1), i => i.Quantity = pickedQuantity1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetToPickedCurrentLocation(pickedInventory1));
                var expectedInventoryKey1 = new InventoryKey(pickedInventory1, pickedInventory1, destinationLocation, treatmentOrder, pickedInventory1.ToteKey);

                var lot2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot());
                var currentLocation = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var inventory2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(lot2, null, destinationLocation, treatmentOrder), i => i.Quantity = existingQuantity2);
                var pickedInventory2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder).SetToInventory(inventory2).SetCurrentLocation(currentLocation), i => i.Quantity = pickedQuantity2);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetToPickedCurrentLocation(pickedInventory2));
                var expectedInventoryKey2 = new InventoryKey(pickedInventory2, pickedInventory2, destinationLocation, treatmentOrder, pickedInventory2.ToteKey);

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = new TreatmentOrderKey(treatmentOrder),
                        DestinationLocationKey = new LocationKey(destinationLocation)
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(pickedQuantity0, RVCUnitOfWork.InventoryRepository.FindByKey(expectedInventoryKey0).Quantity);
                Assert.AreEqual(pickedQuantity1, RVCUnitOfWork.InventoryRepository.FindByKey(expectedInventoryKey1).Quantity);
                Assert.AreEqual(expectedQuantity2, RVCUnitOfWork.InventoryRepository.FindByKey(expectedInventoryKey2).Quantity);
            }

            [Test]
            public void Modifies_PickedInventoryItem_CurrentLocations_as_expected_on_success()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled, t => t.InventoryShipmentOrder.PickedInventory.Items = null);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                var pickedItemKeys = new List<PickedInventoryItemKey>();
                for(var p = 0; p < 3; ++p)
                {
                    var pickedInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder));
                    TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetToPickedCurrentLocation(pickedInventory));
                    pickedItemKeys.Add(new PickedInventoryItemKey(pickedInventory));
                }

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = new TreatmentOrderKey(treatmentOrder),
                        DestinationLocationKey = new LocationKey(destinationLocationKey)
                    });

                //Assert
                result.AssertSuccess();
                foreach(var key in pickedItemKeys)
                {
                    Assert.AreEqual(destinationLocationKey.LocationKey_Id, RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(key).CurrentLocationId);
                }
            }

            [Test]
            public void Modifies_current_location_Inventory_as_expected_on_success()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(t => t.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled, t => t.InventoryShipmentOrder.PickedInventory.Items = null);
                var destinationLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                var pickedInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder));
                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetToPickedCurrentLocation(pickedInventory)));

                pickedInventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(treatmentOrder.InventoryShipmentOrder));
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetToPickedCurrentLocation(pickedInventory).Quantity += 123));

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = new TreatmentOrderKey(treatmentOrder),
                        DestinationLocationKey = new LocationKey(destinationLocationKey)
                    });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey0));
                Assert.AreEqual(123, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey1).Quantity);
            }
        }

        [TestFixture]
        public class GetInventoryTreatments : TreatmentOrderServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_InventoryTreatment_records_exist()
            {
                //Act
                var result = Service.GetTreatmentOrders();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_expected_InventoryTreatments_on_success()
            {
                //Arrange
                const int expectedResults = 4;
                var key0 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());
                var key1 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());
                var key2 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());
                var key3 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.GetInventoryTreatments();

                //Assert
                result.AssertSuccess();
                
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.AreEqual(1, results.Count(r => r.TreatmentKey == key0.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.TreatmentKey == key1.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.TreatmentKey == key2.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.TreatmentKey == key3.KeyValue));
            }
        }

        [TestFixture]
        public class GetPickableInventoryForContext : TreatmentOrderServiceTests
        {
            protected override bool SetupStaticRecords { get { return true; } }
            private FacilityKey RinconFacilityKey { get; set; }

            [SetUp]
            public void Setup()
            {
                RinconFacilityKey = new FacilityKey(RinconFacility);
            }

            [TearDown]
            public void TearDown()
            {
                RinconFacilityKey = null;
            }

            [Test]
            public void Returns_Inventory_from_SourceFacility()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>();
                var sourceFacilityKey = treatmentOrder.InventoryShipmentOrder.SourceFacility.ToFacilityKey();

                var expectedInventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey))
                    };

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);

                //Act
                var result = ((IPickInventoryServiceComponent)Service).GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = treatmentOrder.ToTreatmentOrderKey()
                    });

                //Assert
                result.AssertSuccess();
                expectedInventory.AssertEquivalent(result.ResultingObject.Items.ToList(), e => e.ToInventoryKey().KeyValue, r => r.InventoryKey,
                    (e, r) => Assert.AreEqual(sourceFacilityKey.KeyValue, r.Location.FacilityKey));
            }

            [Test]
            public void Returns_Inventory_with_ValidForPicking_as_expected_by_QualityStatus()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>();
                var sourceFacilityKey = new FacilityKey(treatmentOrder.InventoryShipmentOrder.SourceFacility);

                var expectedInventory = new Dictionary<Inventory, bool>
                    {
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Released, i => i.Location.ConstrainByKeys(sourceFacilityKey)), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Contaminated, i => i.Location.ConstrainByKeys(sourceFacilityKey)), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Pending, i => i.Location.ConstrainByKeys(sourceFacilityKey)), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Rejected, i => i.Location.ConstrainByKeys(sourceFacilityKey)), false }
                    };
                
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);

                //Act
                var result = ((IPickInventoryServiceComponent)Service).GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = treatmentOrder.ToTreatmentOrderKey()
                    });

                //Assert
                result.AssertSuccess();
                expectedInventory.AssertEquivalent(result.ResultingObject.Items.ToList(),
                    e => e.Key.ToInventoryKey(), r => r.InventoryKey,
                    (e, r) =>
                    {
                        Assert.AreEqual(sourceFacilityKey.KeyValue, r.Location.FacilityKey);
                        Assert.AreEqual(e.Value, r.ValidForPicking);
                    });
            }

            [Test]
            public void Excludes_Lots_with_invalid_Treatment_type()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>();
                var sourceFacilityKey = new FacilityKey(treatmentOrder.InventoryShipmentOrder.SourceFacility);

                var expectedInventory = new Dictionary<Inventory, bool>
                    {
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Released, i => i.Location.ConstrainByKeys(sourceFacilityKey)), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Contaminated, i => i.Location.ConstrainByKeys(sourceFacilityKey)), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Pending, i => i.Location.ConstrainByKeys(sourceFacilityKey)), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Rejected, i => i.Location.ConstrainByKeys(sourceFacilityKey)), false }
                    };
                
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Released, i => i.Location.ConstrainByKeys(sourceFacilityKey),
                    i => i.Lot.Inventory = TestHelper.List<Inventory>(1));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Released, i => i.Location.ConstrainByKeys(sourceFacilityKey),
                    i => i.Lot.PickedInventory = TestHelper.List<PickedInventoryItem>(1));

                var pickedLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Lot.QualityStatus = LotQualityStatus.Released, i => i.Location.ConstrainByKeys(sourceFacilityKey)).Lot;
                var otherOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(otherOrder.InventoryShipmentOrder, pickedLot, null, Data.Models.StaticRecords.StaticInventoryTreatments.NoTreatment));

                Assert.Greater(TestHelper.Context.Set<Inventory>().Count(), expectedInventory.Count);

                //Act
                var result = ((IPickInventoryServiceComponent)Service).GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = treatmentOrder.ToTreatmentOrderKey()
                    });

                //Assert
                result.AssertSuccess();
                expectedInventory.AssertEquivalent(result.ResultingObject.Items.ToList(),
                    e => e.Key.ToInventoryKey(), r => r.InventoryKey,
                    (e, r) =>
                        {
                            Assert.AreEqual(sourceFacilityKey.KeyValue, r.Location.FacilityKey);
                            Assert.AreEqual(e.Value, r.ValidForPicking);
                        });
            }

            [Test, Issue("Technically this functionality should be tested in all picking contexts, but I didn't feel like going through all of them. This implementation should be commonly wired" +
                         "for all contexts, so if it works in one context it should work in another. The first instance that breaks that hypothesis should be a clue to add explicit testing for each" +
                         "context. -RI 2016-12-27",
                References = new[] { "RVCADMIN-1438" })]
            public void Filters_Inventory_by_LocationGroupName_as_expected()
            {
                //Arrange
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>();
                var sourceFacilityKey = treatmentOrder.InventoryShipmentOrder.SourceFacility.ToFacilityKey();

                var expectedInventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey).Description = "street~1"),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey).Description = "street~2"),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey).Description = "street~3")
                    };

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetNoTreatment().Lot.ProductionStatus = LotProductionStatus.Produced, i => i.Location.ConstrainByKeys(sourceFacilityKey));

                //Act
                var result = ((IPickInventoryServiceComponent)Service).GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = treatmentOrder.ToTreatmentOrderKey(),
                        LocationGroupName = "street"
                    });

                //Assert
                result.AssertSuccess();
                expectedInventory.AssertEquivalent(result.ResultingObject.Items.ToList(), e => e.ToInventoryKey().KeyValue, r => r.InventoryKey,
                    (e, r) => Assert.AreEqual(sourceFacilityKey.KeyValue, r.Location.FacilityKey));
            }
        }
    }
}