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
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using Solutionhead.Services;
using SetInventoryPickOrderItemParameters = RioValleyChili.Services.Models.Parameters.SetInventoryPickOrderItemParameters;
using SetPickedInventoryItemParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SetPickedInventoryItemParameters;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class WarehouseOrderServiceTests : ServiceIntegrationTestBase<WarehouseOrderService>
    {
        [TestFixture]
        public class CreateWarehouseOrder : WarehouseOrderServiceTests
        {
            [Test]
            public void Returns_expected_InterWarehouseOrderKey_on_success()
            {
                //Arrange
                var sourceFacility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var destinationFacility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var expectedUser = TestUser.UserName;
                var expectedDateCreated = TimeStamper.CurrentTimeStamp.Date;
                const int expectedSequence = 1;
                Assert.IsNull(RVCUnitOfWork.InventoryShipmentOrderRepository.FindBy(w => w.DateCreated == expectedDateCreated && w.Sequence == expectedSequence));

                //Act
                var result = Service.CreateWarehouseOrder(new SetOrderParameters
                    {
                        UserToken = expectedUser,
                        SourceFacilityKey = new FacilityKey(sourceFacility),
                        DestinationFacilityKey = new FacilityKey(destinationFacility),
                    });

                //Assert
                result.AssertSuccess();
                var warehouseOrder = RVCUnitOfWork.InventoryShipmentOrderRepository.All().Single();
                Assert.AreEqual(new InventoryShipmentOrderKey(warehouseOrder).KeyValue, result.ResultingObject);
            }

            [Test]
            public void Creates_InterWarehouseOrder_record_as_expected()
            {
                //Arrange
                var sourceFacility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var destinationFacility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var expectedUser = TestUser.UserName;
                var expectedDateCreated = TimeStamper.CurrentTimeStamp.Date;
                const int expectedSequence = 1;

                Assert.IsNull(RVCUnitOfWork.InventoryShipmentOrderRepository.FindBy(w => w.DateCreated == expectedDateCreated && w.Sequence == expectedSequence));

                //Act
                var result = Service.CreateWarehouseOrder(new SetOrderParameters
                {
                    SourceFacilityKey = new FacilityKey(sourceFacility),
                    DestinationFacilityKey = new FacilityKey(destinationFacility),
                    UserToken = expectedUser
                });

                //Assert
                result.AssertSuccess();
                var warehouseOrder = RVCUnitOfWork.InventoryShipmentOrderRepository
                    .FindBy(w => w.DateCreated == expectedDateCreated && w.Sequence == expectedSequence, i => i.Employee, i => i.DestinationFacility);
                Assert.AreEqual(new InventoryShipmentOrderKey(warehouseOrder).KeyValue, result.ResultingObject);
                Assert.AreEqual(new FacilityKey(destinationFacility).KeyValue, new FacilityKey(warehouseOrder.DestinationFacility).KeyValue);
                Assert.AreEqual(expectedUser, warehouseOrder.Employee.UserName);
                Assert.AreEqual(OrderStatus.Scheduled, warehouseOrder.OrderStatus);
            }

            [Test]
            public void Creates_InterWarehouseOrder_with_no_InventoryPickOrderItems_if_PickOrder_parameter_is_null()
            {
                //Arrange
                var sourceFacility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var destinationFacility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var expectedUser = TestUser.UserName;
                var expectedDateCreated = TimeStamper.CurrentTimeStamp.Date;
                const int expectedSequence = 1;
                Assert.IsNull(RVCUnitOfWork.InventoryShipmentOrderRepository.FindBy(w => w.DateCreated == expectedDateCreated && w.Sequence == expectedSequence));

                //Act
                var result = Service.CreateWarehouseOrder(new SetOrderParameters
                    {
                        UserToken = expectedUser,
                        SourceFacilityKey = new FacilityKey(sourceFacility),
                        DestinationFacilityKey = new FacilityKey(destinationFacility)
                    });

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.InventoryShipmentOrderRepository.All().Select(o => o.InventoryPickOrder.Items).Single();
                Assert.IsEmpty(items);
            }

            [Test]
            public void Creates_InterWarehouseOrder_with_expected_InventoryPickOrderItems_if_PickOrder_parameters_is_not_null()
            {
                //Arrange
                var sourceFacility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var destinationFacility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var expectedUser = TestUser.UserName;
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
                var expectedCustomerKey0 = new CustomerKey((ICustomerKey) TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>());
                const string expectedCustomerCode0 = "customerCode0";
                const string expectedLotCode0 = "lotCode0";
                const string expectedCustomerCode1 = "customerCode1";
                const string expectedLotCode1 = "lotCode1";

                Assert.IsNull(RVCUnitOfWork.InventoryShipmentOrderRepository.FindBy(w => w.DateCreated == expectedDateCreated && w.Sequence == expectedSequence));

                //Act
                var result = Service.CreateWarehouseOrder(new SetOrderParameters
                    {
                        SourceFacilityKey = new FacilityKey(sourceFacility),
                        DestinationFacilityKey = new FacilityKey(destinationFacility),
                        UserToken = expectedUser,
                        InventoryPickOrderItems = new List<SetInventoryPickOrderItemParameters>
                            {
                                new SetInventoryPickOrderItemParameters
                                    {
                                        ProductKey = chileProductKey0.KeyValue,
                                        PackagingKey = packagingProductKey0.KeyValue,
                                        TreatmentKey = treatmentKey0.KeyValue,
                                        Quantity = quantity0,
                                        CustomerKey = expectedCustomerKey0,
                                        CustomerProductCode = expectedCustomerCode0,
                                        CustomerLotCode = expectedLotCode0
                                    },
                                new SetInventoryPickOrderItemParameters
                                    {
                                        ProductKey = chileProductKey1.KeyValue,
                                        PackagingKey = packagingProductKey1.KeyValue,
                                        TreatmentKey = treatmentKey1.KeyValue,
                                        Quantity = quantity1,
                                        CustomerProductCode = expectedCustomerCode1,
                                        CustomerLotCode = expectedLotCode1
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.InventoryShipmentOrderRepository.All().Select(o => o.InventoryPickOrder.Items).Single();
                Assert.IsNotNull(items.SingleOrDefault(i => i.Quantity == quantity0 &&
                    i.ProductId == chileProductKey0.ChileProductKey_ProductId &&
                    i.PackagingProductId == packagingProductKey0.PackagingProductKey_ProductId &&
                    i.TreatmentId == treatmentKey0.InventoryTreatmentKey_Id &&
                    i.CustomerId == expectedCustomerKey0.CustomerKey_Id &&
                    i.CustomerProductCode == expectedCustomerCode0 &&
                    i.CustomerLotCode == expectedLotCode0));

                Assert.IsNotNull(items.SingleOrDefault(i => i.Quantity == quantity1 &&
                    i.ProductId == chileProductKey1.ChileProductKey_ProductId &&
                    i.PackagingProductId == packagingProductKey1.PackagingProductKey_ProductId &&
                    i.TreatmentId == treatmentKey1.InventoryTreatmentKey_Id &&
                    i.CustomerProductCode == expectedCustomerCode1 &&
                    i.CustomerLotCode == expectedLotCode1));
            }
        }

        [TestFixture]
        public class UpdateInterWarehouseOrder : WarehouseOrderServiceTests
        {
            [Test]
            public void Sets_header_as_expected_on_success()
            {
                //Arrange
                var orderKey = new InventoryShipmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>());
                var destinationFacility = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>());
                var sourceFacility = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>());

                //Act
                var result = Service.UpdateInterWarehouseOrder(new TestUpdateInterWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        InventoryShipmentOrderKey = orderKey,
                        DestinationFacilityKey = destinationFacility,
                        SourceFacilityKey = sourceFacility
                    });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey, o => o.DestinationFacility, o => o.SourceFacility);
                Assert.AreEqual(destinationFacility, order.DestinationFacility);
                Assert.AreEqual(sourceFacility, order.SourceFacility);
            }

            [Test]
            public void Sets_shipment_information_on_success()
            {
                //Arrange
                const string expectedComments = "Ship it good.";
                var orderKey = new InventoryShipmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>());

                //Act
                var result = Service.UpdateInterWarehouseOrder(new TestUpdateInterWarehouseOrderParameters
                {
                    UserToken = TestUser.UserName,
                    InventoryShipmentOrderKey = orderKey,
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
                var order = RVCUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey, o => o.ShipmentInformation);
                Assert.AreEqual(expectedComments, order.ShipmentInformation.ExternalNotes);
            }

            [Test]
            public void Sets_PickOrderItems_on_success()
            {
                //Arrange
                var orderKey = new InventoryShipmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>());
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot));

                var parameters = new TestUpdateInterWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        InventoryShipmentOrderKey = orderKey,
                        InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters>
                                {
                                    new Parameters.SetInventoryPickOrderItemParameters
                                        {
                                            ProductKey = new ChileProductKey(chileLot),
                                            PackagingKey = new PackagingProductKey(inventory),
                                            TreatmentKey = new InventoryTreatmentKey(inventory),
                                            Quantity = 10,
                                        }
                                }
                    };

                //Act
                var result = Service.UpdateInterWarehouseOrder(parameters);

                //Assert
                result.AssertSuccess();
                var pickOrderItems = RVCUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey, o => o.InventoryPickOrder.Items).InventoryPickOrder.Items.ToList();
                Assert.AreEqual(parameters.InventoryPickOrderItems.Count(), pickOrderItems.Count);
                foreach(var item in parameters.InventoryPickOrderItems)
                {
                    Assert.IsNotNull(pickOrderItems.First(i => i.Quantity == item.Quantity));
                }
            }

            [Test]
            public void Updates_PickOrderItem_Customer_information_as_expected()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderKey = new InventoryShipmentOrderKey(order);
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order));
                var expectedCustomerKey = new CustomerKey((ICustomerKey) TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>());
                var expectedOrderItem = new Parameters.SetInventoryPickOrderItemParameters
                    {
                        ProductKey = new ProductKey(orderItem.Product),
                        PackagingKey = new PackagingProductKey(orderItem.PackagingProduct),
                        TreatmentKey = new InventoryTreatmentKey(orderItem),
                        Quantity = orderItem.Quantity,
                        CustomerKey = expectedCustomerKey,
                        CustomerProductCode = "ProductCode!",
                        CustomerLotCode = "LotCode!"
                    };
                var parameters = new TestUpdateInterWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        InventoryShipmentOrderKey = orderKey,
                        InventoryPickOrderItems = new List<ISetInventoryPickOrderItemParameters> { expectedOrderItem }
                    };

                //Act
                var result = Service.UpdateInterWarehouseOrder(parameters);

                //Assert
                result.AssertSuccess();
                var resultItem = RVCUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey, o => o.InventoryPickOrder.Items.Select(i => i.Customer)).InventoryPickOrder.Items.ToList().Single();
                Assert.AreEqual(expectedCustomerKey.KeyValue, new CustomerKey((ICustomerKey) resultItem.Customer).KeyValue);
                Assert.AreEqual(expectedOrderItem.CustomerProductCode, resultItem.CustomerProductCode);
                Assert.AreEqual(expectedOrderItem.CustomerLotCode, resultItem.CustomerLotCode);
            }

            [Test]
            public void Sets_customer_product_and_lot_codes_for_PickedInventoryItems_as_expected()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var pickedItemKey0 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)));
                var pickedItemKey1 = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)));
                const string lotCode0 = "LotCode!";
                const string productCode1 = "ProductCode!";
                var parameters = new TestUpdateInterWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        InventoryShipmentOrderKey = new InventoryShipmentOrderKey(order),
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
                var result = Service.UpdateInterWarehouseOrder(parameters);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(lotCode0, RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(pickedItemKey0).CustomerLotCode);
                Assert.AreEqual(productCode1, RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(pickedItemKey1).CustomerProductCode);
            }
        }

        [TestFixture]
        public class SetInventoryPickOrderTests : SetInventoryPickOrderTestsBase<WarehouseOrderService, InventoryShipmentOrder, InventoryShipmentOrderKey>
        {
            protected override void InitializeOrder(InventoryShipmentOrder order)
            {
                order.OrderStatus = OrderStatus.Scheduled;
                order.OrderType = InventoryShipmentOrderTypeEnum.InterWarehouseOrder;
            }

            protected override InventoryShipmentOrderKey CreateKeyFromOrder(InventoryShipmentOrder order)
            {
                return new InventoryShipmentOrderKey(order);
            }

            protected override InventoryPickOrder GetPickOrderFromOrder(InventoryShipmentOrder order)
            {
                return order.InventoryPickOrder;
            }

            protected override IResult GetResult(SetInventoryPickOrderParameters parameters)
            {
                return Service.UpdateInterWarehouseOrder(new TestUpdateInterWarehouseOrderParameters
                    {
                        UserToken = parameters.UserToken,
                        InventoryShipmentOrderKey = parameters.OrderKey,
                        InventoryPickOrderItems = parameters.InventoryPickOrderItems
                    });
            }
        }

        [TestFixture]
        public class GetInventoryWarehouseOrders : WarehouseOrderServiceTests
        {
            [Test]
            public void Returns_empty_result_if_there_are_no_InterWarehouseOrders()
            {
                //Act
                var result = Service.GetWarehouseOrders();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject.ToList());
            }

            [Test]
            public void Returns_InterWarehouseOrders_as_expected()
            {
                //Arrange
                const int orderCount = 3;
                var expectedOrders = new List<InventoryShipmentOrder>();
                for(var i = 0; i < orderCount; i++)
                {
                    expectedOrders.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>());
                }

                //Act
                var result = Service.GetWarehouseOrders();

                //Assert
                result.AssertSuccess();

                var results = result.ResultingObject.ToList();
                Assert.AreEqual(orderCount, results.Count);

                expectedOrders.ForEach(o =>
                    {
                        var key = new InventoryShipmentOrderKey(o);
                        var order = results.FirstOrDefault(r => r.MovementKey == key.KeyValue);
                        Assert.IsNotNull(order);
                        Assert.AreEqual(o.ShipmentInformation.ShipmentDate, order.ShipmentDate);
                    });
            }

            [Test]
            public void Can_query_off_OrderStatus_as_expected()
            {
                //Arrange
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.OrderStatus = OrderStatus.Fulfilled);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.OrderStatus = OrderStatus.Scheduled);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.OrderStatus = OrderStatus.Scheduled);

                //Act
                var result = Service.GetWarehouseOrders();

                //Assert
                result.AssertSuccess();
                Assert.IsNotNull(result.ResultingObject.FirstOrDefault(o => o.OrderStatus == OrderStatus.Fulfilled));
            }

            [Test]
            public void Can_query_off_ShipmentStatus_as_expected()
            {
                //Arrange
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.Status = ShipmentStatus.Shipped);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.Status = ShipmentStatus.Unscheduled);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.Status = ShipmentStatus.Scheduled);

                //Act
                var result = Service.GetWarehouseOrders();

                //Assert
                result.AssertSuccess();
                Assert.IsNotNull(result.ResultingObject.FirstOrDefault(o => o.Shipment.Status == ShipmentStatus.Shipped));
            }

            [Test]
            public void Filters_by_Origin_Facility_as_expected()
            {
                //Arrange
                var expected = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                
                //Act
                var result = Service.GetWarehouseOrders(new FilterInterWarehouseOrderParameters
                    {
                        OriginFacilityKey = new FacilityKey(expected.SourceFacility)
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new InventoryShipmentOrderKey(expected).KeyValue, result.ResultingObject.ToList().Single().MovementKey);
            }

            [Test]
            public void Filters_by_Destination_Facility_as_expected()
            {
                //Arrange
                var expected = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                
                //Act
                var result = Service.GetWarehouseOrders(new FilterInterWarehouseOrderParameters
                    {
                        DestinationFacilityKey = new FacilityKey(expected.DestinationFacility)
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new InventoryShipmentOrderKey(expected).KeyValue, result.ResultingObject.ToList().Single().MovementKey);
            }
        }

        [TestFixture]
        public class GetInventoryWarehouseOrder : GetInventoryOrderTestsBase<WarehouseOrderService, InventoryShipmentOrder, IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>>
        {
            [Test]
            public void Returns_Invalid_result_if_InterWarehouseOrderKey_is_invalid()
            {
                //Act
                var result = TimedExecution(() => Service.GetWarehouseOrder("invalid key"));

                //Assert
                result.AssertInvalid(UserMessages.InvalidInventoryShipmentOrderKey);
            }

            [Test]
            public void Returns_Invalid_result_if_InterWarehouseOrder_could_not_be_found()
            {
                //Act
                var result = TimedExecution(() => Service.GetWarehouseOrder(new InventoryShipmentOrderKey().KeyValue));

                //Assert
                result.AssertInvalid(UserMessages.InterWarehouseOrderNotFound);
            }

            [Test]
            public void Returns_result_with_expected_OrderKey_and_detailed_item_information()
            {
                //Arrange
                StartStopwatch();

                const OrderStatus orderStatus = OrderStatus.Scheduled;
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.OrderStatus = orderStatus,
                    o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                        {
                            TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null),
                            TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null),
                            TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.InventoryPickOrder = null)
                        },
                    o => o.InventoryPickOrder.PickedInventory.Items = new List<PickedInventoryItem>
                        {
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null),
                            TestHelper.CreateObjectGraph<PickedInventoryItem>(i => i.PickedInventory = null)
                        });
                var orderKey = new InventoryShipmentOrderKey(order).KeyValue;
                Assert.GreaterOrEqual(order.InventoryPickOrder.Items.Count, 1);
                Assert.GreaterOrEqual(order.PickedInventory.Items.Count, 1);

                StopWatchAndWriteTime("Arrange");

                //Act
                var result = TimedExecution(() => Service.GetWarehouseOrder(orderKey));

                //Assert
                result.AssertSuccess();
                var warehouseOrderResult = result.ResultingObject;
                Assert.IsNotNull(warehouseOrderResult);
                Assert.AreEqual(orderKey, warehouseOrderResult.MovementKey);
                Assert.AreEqual(orderStatus, warehouseOrderResult.OrderStatus);

                Assert.AreEqual(order.InventoryPickOrder.Items.Count, result.ResultingObject.PickOrder.PickOrderItems.Count());
                order.InventoryPickOrder.Items.ForEach(i =>
                    Assert.AreEqual(1, result.ResultingObject.PickOrder.PickOrderItems.Count(p =>
                        p.ProductKey == new ProductKey((IProductKey) i).KeyValue &&
                        p.PackagingProductKey == new PackagingProductKey(i).KeyValue &&
                        p.TreatmentKey == new InventoryTreatmentKey(i).KeyValue &&
                        p.Quantity == i.Quantity)));

                Assert.AreEqual(order.PickedInventory.Items.Count, result.ResultingObject.PickedInventory.PickedInventoryItems.Count());
                order.PickedInventory.Items.ForEach(i =>
                    Assert.AreEqual(1, result.ResultingObject.PickedInventory.PickedInventoryItems.Count(p =>
                        p.InventoryKey == new InventoryKey(i, i, i.FromLocation, i, i.ToteKey).KeyValue &&
                        p.QuantityPicked == i.Quantity)));
            }

            [Test]
            public void Returns_result_with_expected_ShippingInstructions_Comments_field()
            {
                //Arrange
                const string expectedComments = "In the not too distant future";
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.ExternalNotes = expectedComments);
                var orderKey = new InventoryShipmentOrderKey(order);

                //Act
                var result = TimedExecution(() => Service.GetWarehouseOrder(orderKey.KeyValue));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedComments, result.ResultingObject.Shipment.ShippingInstructions.ExternalNotes);
            }

            [Test]
            public void Returns_result_with_expected_ShipmentDetail_InventoryOrderType()
            {
                //Arrange
                const string expectedComments = "In the not too distant future";
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.ExternalNotes = expectedComments);
                var orderKey = new InventoryShipmentOrderKey(order);

                //Act
                var result = TimedExecution(() => Service.GetWarehouseOrder(orderKey.KeyValue));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(InventoryOrderEnum.TransWarehouseMovements, result.ResultingObject.Shipment.InventoryOrderEnum);
            }

            #region Protected Parts

            protected override Func<InventoryShipmentOrder, IResult<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>>> MethodUnderTest { get { return o => Service.GetWarehouseOrder(new InventoryShipmentOrderKey(o)); } }
            protected override Func<InventoryShipmentOrder> CreateParentRecord { get { return () => TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.PickedInventory.Items = null); } }
            protected override Func<InventoryShipmentOrder, PickedInventory> GetPickedInventoryRecordFromParent { get { return o => o.PickedInventory; } }
            protected override Func<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>, List<IPickedInventoryItemReturn>> GetPickedInventoryItemsFromResult { get { return r => r.PickedInventory.PickedInventoryItems.ToList(); } }

            #endregion
        }

        [TestFixture, Issue("Looks like requirements may have changed or something - but the tests are failing and I'm not sure what to make of it. -RI 2016-11-15",
            References = new[] { "changeset: d8a8f58b1887" })]
        public class GetInventoryToPickForOrderTests : WarehouseOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Order_could_not_be_found()
            {
                //Act
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = new InventoryShipmentOrderKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.InterWarehouseOrderNotFound);
            }

            [Test]
            public void Returns_Inventory_with_ValidForPicking_as_expected_for_an_Order()
            {
                //Arrange
                var expectedInventory = new Dictionary<Inventory, bool>();
                
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                var chileLot0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot0, orderItem, null, orderItem, warehouse)), true);

                var chileLot1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot1, orderItem, null, orderItem, warehouse)), true);

                var chileLot2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot2, orderItem, null, orderItem, warehouse)), true);

                var chileLot3 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot3, orderItem, null, orderItem));

                var chileLot4 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.LotDefects = TestHelper.List<LotDefect>(1, (d, n) => d.Resolution = null));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot4, orderItem, null, orderItem, warehouse)), true);

                var chileLot5 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().ProductionStatus = LotProductionStatus.Batched);
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot5, orderItem, null, orderItem, warehouse)), false);

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                {
                    OrderKey = order.ToInventoryShipmentOrderKey(),
                    OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                expectedInventory.AssertEquivalent(results, e => e.Key.ToInventoryKey(), r => r.InventoryKey,
                    (e, r) => Assert.AreEqual(e.Value, r.ValidForPicking));
            }

            [Test]
            public void Returns_ValidForPicking_as_expected_by_orderItem_product_spec()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = TestHelper.List<ChileProductAttributeRange>(1, (c, n) => c.SetValues(StaticAttributeNames.Asta, 5, 10)));
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(orderItem.Customer, StaticAttributeNames.Asta, 6, 9, chileProduct).Active = true);

                var expected = new Dictionary<string, bool>
                    {
                        {
                            TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse)
                                .Lot.SetChileLot()
                                .Attributes = TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 7.5)))
                            .ToInventoryKey(),
                            true
                        },
                        {
                            TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse)
                                .Lot.SetChileLot()
                                .Attributes = TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 1)))
                            .ToInventoryKey(),
                            false
                        },
                        {
                            TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse)
                                .Lot.SetChileLot())
                            .ToInventoryKey(),
                            false
                        }
                    };

                //Act
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.Items.AsEnumerable().Select(i =>
                    {
                        if(result.ResultingObject.Initializer != null)
                        {
                            result.ResultingObject.Initializer.Initialize(i);
                        }
                        return i;
                    }).ToList();
                expected.AssertEquivalent(results, k => k.Key, k => k.InventoryKey,
                    (e, r) => Assert.AreEqual(e.Value, r.ValidForPicking));
            }

            [Test]
            public void ValidForPicking_will_be_true_if_there_are_no_specs()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                var items = results.Select(i =>
                        {
                            if(result.ResultingObject.Initializer != null)
                            {
                                result.ResultingObject.Initializer.Initialize(i);
                            }
                            return i;
                        })
                    .ToList();
                Assert.IsTrue(items.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_false_if_product_spec_is_out_of_range()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                if(result.ResultingObject.Initializer != null)
                {
                    results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                }
                Assert.IsFalse(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_false_if_product_spec_is_out_of_range_and_has_unresolved_defect()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Asta).LotDefect.Resolution = null);
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                {
                    OrderKey = order.ToInventoryShipmentOrderKey(),
                    OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                if(result.ResultingObject.Initializer != null)
                {
                    results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                }
                Assert.IsFalse(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_product_spec_is_out_of_range_but_has_resolved_defect()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Asta));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                if(result.ResultingObject.Initializer != null)
                {
                    results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                }
                Assert.IsTrue(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_product_spec_is_out_of_range_but_customer_spec_is_in_range()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(orderItem.Customer, StaticAttributeNames.Asta, 1, 3, chileProduct));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                if(result.ResultingObject.Initializer != null)
                {
                    results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                }
                Assert.IsTrue(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_false_if_customer_spec_is_out_of_range()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 3))
                    });
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                        {
                            TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                        });
                var orderItem = order.InventoryPickOrder.Items.Single();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(orderItem.Customer, StaticAttributeNames.Asta, 1, 2, chileProduct));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, order.SourceFacility)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                if(result.ResultingObject.Initializer != null)
                {
                    results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                }
                Assert.False(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_false_if_customer_spec_is_out_of_range_has_resolution_but_product_spec_is_more_permissive()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 0, 3))
                    });

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(orderItem.Customer, StaticAttributeNames.Asta, 1, 3, chileProduct));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 4))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Asta));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                if(result.ResultingObject.Initializer != null)
                {
                    results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                }
                Assert.False(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_customer_spec_is_out_of_range_has_resolution_and_product_spec_is_less_permissive()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(orderItem.Customer, StaticAttributeNames.Asta, 1, 3, chileProduct));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 4))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Asta));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                if(result.ResultingObject.Initializer != null)
                {
                    results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                }
                Assert.True(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_product_spec_is_out_of_range_but_has_customer_allowance()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(i => i.SetProduct(chileProduct))
                    });
                var warehouse = order.SourceFacility;
                var orderItem = order.InventoryPickOrder.Items.Single();

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 4))
                        },
                    c => c.Lot.AddCustomerAllowance(orderItem.Customer));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, orderItem, null, orderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                if(result.ResultingObject.Initializer != null)
                {
                    results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                }
                Assert.True(results.Single().ValidForPicking);
            }
        }

        [TestFixture]
        public class SetPickedInventory : WarehouseOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_order_could_not_be_found()
            {
                //Act
                var result = Service.SetPickedInventory(new InventoryShipmentOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.InventoryShipmentOrderNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_reference_an_OrderItem_that_does_not_exist()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(c => c.ShipmentInformation.Status = ShipmentStatus.Unscheduled);
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, null, order.SourceFacility).Quantity = 100);

                //Act
                var result = Service.SetPickedInventory(order.ToInventoryShipmentOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                {
                                    new SetPickedInventoryItemParameters
                                        {
                                            OrderItemKey = new SalesOrderItemKey(),
                                            InventoryKey = inventory.ToInventoryKey(),
                                            Quantity = 10
                                        }
                                }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickForOrderItem_DoesNotExist);
            }

            [Test]
            public void Returns_non_successful_result_if_Inventory_is_not_in_Order_ShipFromWarehouse()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick().Quantity = 100);

                //Act
                var result = Service.SetPickedInventory(order.ToInventoryShipmentOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                {
                                    new SetPickedInventoryItemParameters
                                        {
                                            OrderItemKey = orderItem.ToInventoryPickOrderItemKey(),
                                            InventoryKey = inventory.ToInventoryKey(),
                                            Quantity = 10
                                        }
                                }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.SourceLocationMustBelongToFacility);
            }

            [Test]
            public void Returns_non_successful_result_if_Inventory_is_of_a_Lot_with_a_ProductionStatus_that_is_not_Complete()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(facility: order.SourceFacility).Lot.EmptyLot().ProductionStatus = LotProductionStatus.Batched, i => i.Quantity = 100);

                //Act
                var result = Service.SetPickedInventory(order.ToInventoryShipmentOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                    {
                                        new SetPickedInventoryItemParameters
                                            {
                                                OrderItemKey = orderItem.ToInventoryPickOrderItemKey(),
                                                InventoryKey = inventory.ToInventoryKey(),
                                                Quantity = 1
                                            }
                                    }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickLotNotProduced);
            }

            [Test]
            public void Returns_successful_result_if_Inventory_is_of_a_Lot_with_an_unresolved_defect()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 100, i => i.SetValidToPick(order.SourceFacility).Lot.LotDefects =
                    TestHelper.List<LotDefect>(1, (d, n) => d.Resolution = null));

                //Act
                var result = Service.SetPickedInventory(order.ToInventoryShipmentOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                        {
                                            new SetPickedInventoryItemParameters
                                                {
                                                    OrderItemKey = orderItem.ToInventoryPickOrderItemKey(),
                                                    InventoryKey = inventory.ToInventoryKey(),
                                                    Quantity = 1
                                                }
                                        }
                    });

                //Assert
                result.AssertSuccess();
            }

            [Test]
            public void Creates_PickedInventoryItem_records_as_expected_on_success()
            {
                //Arrange
                const int expectedItems = 3;
                const int quantity0 = 10;
                const int quantity1 = 22;
                const int quantity2 = 303;

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderKey = order.ToInventoryShipmentOrderKey();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct)).ToInventoryPickOrderItemKey();
                var inventoryKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Quantity = quantity0 + 10).ToInventoryKey();
                var inventoryKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Quantity = quantity1 + 10).ToInventoryKey();
                var inventoryKey2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Quantity = quantity2 + 10).ToInventoryKey();

                //Act
                var result = Service.SetPickedInventory(orderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                {
                                    new SetPickedInventoryItemParameters { OrderItemKey = orderItem, InventoryKey = inventoryKey0.KeyValue, Quantity = quantity0 },
                                    new SetPickedInventoryItemParameters { OrderItemKey = orderItem, InventoryKey = inventoryKey1.KeyValue, Quantity = quantity1 },
                                    new SetPickedInventoryItemParameters { OrderItemKey = orderItem, InventoryKey = inventoryKey2.KeyValue, Quantity = quantity2 }
                                }
                    });

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey, o => o.PickedInventory.Items).PickedInventory.Items.ToList();

                Assert.AreEqual(expectedItems, items.Count);
                Assert.AreEqual(quantity0, items.Single(i => inventoryKey0.Equals(i)).Quantity);
                Assert.AreEqual(quantity1, items.Single(i => inventoryKey1.Equals(i)).Quantity);
                Assert.AreEqual(quantity2, items.Single(i => inventoryKey2.Equals(i)).Quantity);
            }

            [Test]
            public void Modifies_existing_PickedInventoryItem_records_as_expected_on_success()
            {
                //Arrange
                const int expectedItems = 2;

                const int currentQuantity0 = 100;
                const int currentQuantity1 = 200;
                const int currentQuantity2 = 300;

                const int newQuantity0 = 50;
                const int newQuantity1 = 300;

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderKey = order.ToInventoryShipmentOrderKey();
                var orderItemKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct)).ToInventoryPickOrderItemKey();

                var picked0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(
                    i => i.ConstrainByKeys(order).SetSourceWarehouse(order.SourceFacility).SetCurrentLocationToSource().Quantity = currentQuantity0,
                    i => i.Lot.SetValidToPick());

                var inventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Quantity = newQuantity1);
                var picked1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(
                    i => i.ConstrainByKeys(order).SetToInventory(inventory1).SetCurrentLocationToSource().Quantity = currentQuantity1);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order).SetSourceWarehouse(order.SourceFacility).SetCurrentLocationToSource().Quantity = currentQuantity2,
                    i => i.Lot.SetValidToPick());

                //Act
                var result = Service.SetPickedInventory(orderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        OrderItemKey = orderItemKey,
                                        InventoryKey = picked0.ToInventoryKey(),
                                        Quantity = newQuantity0
                                    },
                                new SetPickedInventoryItemParameters
                                    {
                                        OrderItemKey = orderItemKey,
                                        InventoryKey = picked1.ToInventoryKey(),
                                        Quantity = newQuantity1
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();

                var results = RVCUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey, o => o.PickedInventory.Items).PickedInventory.Items.ToList();

                Assert.AreEqual(expectedItems, results.Count);
                Assert.AreEqual(newQuantity0, results.Single(picked0.ToPickedInventoryItemKey().Equals).Quantity);
                Assert.AreEqual(newQuantity1, results.Single(picked1.ToPickedInventoryItemKey().Equals).Quantity);
            }

            [Test]
            public void Modifies_existing_Inventory_records_as_expected_on_success()
            {
                //Arrange
                const int inventoryToSubtractQuantity = 145;
                const int quantityToSubtract0 = 50;
                const int quantityToSubtract1 = 30;
                const int inventoryToSubtractNewQuantity = inventoryToSubtractQuantity - (quantityToSubtract0 + quantityToSubtract1);

                const int inventoryToAddQuantity = 555;
                const int quantityToAdd0 = 66;
                const int quantityToAdd1 = 77;
                const int inventoryToAddNewQuantity = inventoryToAddQuantity + (quantityToAdd0 + quantityToAdd1);

                const int inventoryToCreateQuantity = 524;

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderKey = order.ToInventoryShipmentOrderKey();
                var orderItemKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct)).ToInventoryPickOrderItemKey();

                var inventoryToRemove = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility));
                var inventoryToRemoveKey = inventoryToRemove.ToInventoryKey();

                var inventoryToSubtract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Quantity = inventoryToSubtractQuantity);
                var inventoryToSubtractKey = inventoryToSubtract.ToInventoryKey();

                var inventoryToAdd = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Quantity = inventoryToAddQuantity);
                var inventoryToAddKey = inventoryToAdd.ToInventoryKey();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(order).SetToInventory(inventoryToAdd).SetCurrentLocationToSource().Quantity = quantityToAdd0 * 2);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(order).SetToInventory(inventoryToAdd).SetCurrentLocationToSource().Quantity = quantityToAdd1 * 2);

                var inventoryToCreateKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(p => p.ConstrainByKeys(order).SetSourceWarehouse(order.SourceFacility).SetCurrentLocationToSource().Quantity = inventoryToCreateQuantity).ToInventoryKey();

                //Act
                var result = Service.SetPickedInventory(orderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                {
                                    new SetPickedInventoryItemParameters { OrderItemKey = orderItemKey, InventoryKey = inventoryToRemoveKey.KeyValue, Quantity = inventoryToRemove.Quantity },
                                    new SetPickedInventoryItemParameters { OrderItemKey = orderItemKey, InventoryKey = inventoryToSubtractKey.KeyValue, Quantity = quantityToSubtract0 },
                                    new SetPickedInventoryItemParameters { OrderItemKey = orderItemKey, InventoryKey = inventoryToSubtractKey.KeyValue, Quantity = quantityToSubtract1 },
                                    new SetPickedInventoryItemParameters { OrderItemKey = orderItemKey, InventoryKey = inventoryToAddKey.KeyValue, Quantity = quantityToAdd0 },
                                    new SetPickedInventoryItemParameters { OrderItemKey = orderItemKey, InventoryKey = inventoryToAddKey.KeyValue, Quantity = quantityToAdd1 }
                                }
                    });

                //Assert
                result.AssertSuccess();

                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToRemoveKey));
                Assert.AreEqual(inventoryToSubtractNewQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToSubtractKey).Quantity);
                Assert.AreEqual(inventoryToAddNewQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToAddKey).Quantity);
                Assert.AreEqual(inventoryToCreateQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToCreateKey).Quantity);
            }

            [Test]
            public void Returns_non_successful_result_if_Inventory_is_out_of_ChileProduct_attribute_range()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = TestHelper.List<ChileProductAttributeRange>(1, (r, n) => r.SetValues(StaticAttributeNames.Asta, 5, 10)));
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Lot.SetChileLot(), i => i.Lot.Attributes =
                    TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 1)));

                //Act
                var result = Service.SetPickedInventory(order.ToInventoryShipmentOrderKey(), new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey(),
                                        Quantity = 1
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.LotAttributeOutOfRequiredRange);
            }

            [Test]
            public void Returns_non_successful_result_if_Inventory_is_out_of_CustomerProduct_attribute_range()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.CustomerProductAttributeRanges = TestHelper.List<CustomerProductAttributeRange>(1, (r, n) => r.SetValues(customer, StaticAttributeNames.Asta, 5, 10)));
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct).SetCustomer(customer));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Lot.SetChileLot().Attributes =
                    TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 1)));

                //Act
                var result = Service.SetPickedInventory(order.ToInventoryShipmentOrderKey(), new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey(),
                                        Quantity = 1
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.LotAttributeOutOfRequiredRange);
            }

            [Test]
            public void Allows_picking_of_Inventory_that_is_in_range_of_spec()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(i => i.CustomerProductAttributeRanges =
                    TestHelper.List<CustomerProductAttributeRange>(1, (r, n) => r.SetValues(customer, StaticAttributeNames.Asta, 5, 10)));
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct).SetCustomer(customer));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Lot.SetChileLot()
                    .Attributes = TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 5)));

                //Act
                var result = Service.SetPickedInventory(order.ToInventoryShipmentOrderKey(), new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey(),
                                        Quantity = 1
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                var pickedItem = RVCUnitOfWork.InventoryShipmentOrderRepository
                    .FindByKey(order.ToInventoryShipmentOrderKey(), i => i.PickedInventory.Items)
                    .PickedInventory.Items.Single();
                Assert.AreEqual(1, pickedItem.Quantity);
            }

            [Test]
            public void Allows_picking_of_Inventory_that_is_out_of_range_but_has_a_LotAllowance()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(i => i.CustomerProductAttributeRanges =
                    TestHelper.List<CustomerProductAttributeRange>(1, (r, n) => r.SetValues(customer, StaticAttributeNames.Asta, 5, 10)));
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryPickOrderItem>(i => i.SetOrder(order).SetProduct(chileProduct).SetCustomer(customer));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(order.SourceFacility).Lot.SetChileLot()
                    .AddCustomerAllowance(orderItem.Customer)
                    .Attributes = TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 1)));

                //Act
                var result = Service.SetPickedInventory(order.ToInventoryShipmentOrderKey(), new SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        OrderItemKey = orderItem.ToInventoryPickOrderItemKey(),
                                        Quantity = 1
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();
                var pickedItem = RVCUnitOfWork.InventoryShipmentOrderRepository
                    .FindByKey(order.ToInventoryShipmentOrderKey(), i => i.PickedInventory.Items)
                    .PickedInventory.Items.Single();
                Assert.AreEqual(1, pickedItem.Quantity);
            }
        }
    }
}