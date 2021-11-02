using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class InventoryShipmentOrderServiceTests : ServiceIntegrationTestBase<InventoryShipmentOrderService>
    {
        [TestFixture]
        public class GetShipments : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_records_exist()
            {
                StartStopwatch();
                var results = Service.GetShipments().ResultingObject.ToList();
                StopWatchAndWriteTime();

                Assert.IsEmpty(results);
            }

            [Test]
            public void Returns_records_as_expected()
            {
                var treatmentOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>();
                var interwarehouseOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                StartStopwatch();
                var results = Service.GetShipments();
                var orders = results.ResultingObject != null ? results.ResultingObject.ToList() : new List<IShipmentOrderSummaryReturn>();
                StopWatchAndWriteTime();

                Assert.AreEqual(3, orders.Count);
                Assert.IsNotNull(orders.Single(o => o.InventoryShipmentOrderKey == new InventoryShipmentOrderKey(treatmentOrder).KeyValue));
                Assert.IsNotNull(orders.Single(o => o.InventoryShipmentOrderKey == new InventoryShipmentOrderKey(interwarehouseOrder).KeyValue));
                Assert.IsNotNull(orders.Single(o => o.InventoryShipmentOrderKey == new InventoryShipmentOrderKey(customerOrder).KeyValue));
            }
        }

        [TestFixture]
        public class SetShipmentInformation : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Sets_ShipmentInformation_as_expected()
            {
                //Arrange
                var orderKey = new InventoryShipmentOrderKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>());
                var parameters = new SetInventoryShipmentInformationParameters
                    {
                        InventoryShipmentOrderKey = orderKey,
                        ShippingInstructions = new SetShippingInstructionsParameters
                            {
                                ExternalNotes = "In Russia, comments write *you*."
                            }
                    };

                //Act
                var result = Service.SetShipmentInformation(parameters);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(parameters.ShippingInstructions.ExternalNotes, RVCUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey, o => o.ShipmentInformation).ShipmentInformation.ExternalNotes);
            }
        }

        [TestFixture]
        public class Post : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Will_return_non_successful_result_if_destination_location_keys_are_not_provided_for_all_items()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(
                    o => o.InventoryShipmentOrder.OrderType = InventoryShipmentOrderTypeEnum.TreatmentOrder,
                    o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled,
                    o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Scheduled);

                var destinationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(order.InventoryShipmentOrder.DestinationFacility)));
                var items = new List<PostItemParameters>
                    {
                        new PostItemParameters
                            {
                                PickedInventoryItemKey = new PickedInventoryItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order.InventoryShipmentOrder))),
                                DestinationLocationKey = destinationKey,
                            }
                    };
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order.InventoryShipmentOrder));

                //Act
                var result = Service.Post(new PostParameters
                {
                    UserToken = TestUser.UserName,
                    OrderKey = new InventoryShipmentOrderKey(order),
                    PickedItemDestinations = items
                });

                //Assert
                result.AssertNotSuccess(UserMessages.DestinationLocationRequiredForPicked);
            }

            [Test]
            public void Will_modify_PickedInventory_as_expected_when_posting_TreatmentOrder()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(
                    o => o.InventoryShipmentOrder.OrderType = InventoryShipmentOrderTypeEnum.TreatmentOrder,
                    o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled,
                    o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Scheduled);

                var destinationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(order.InventoryShipmentOrder.DestinationFacility)));
                var items = new List<PickedInventoryItem>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order.InventoryShipmentOrder)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order.InventoryShipmentOrder))
                    };

                //Act
                var result = Service.Post(new PostParameters
                    {
                        UserToken = TestUser.UserName,
                        OrderKey = new InventoryShipmentOrderKey(order),
                        PickedItemDestinations = items.Select(i => new PostItemParameters
                            {
                                PickedInventoryItemKey = new PickedInventoryItemKey(i),
                                DestinationLocationKey = destinationKey
                            })
                    });

                //Assert
                result.AssertSuccess();
                foreach(var item in items)
                {
                    Assert.AreEqual(destinationKey, RVCUnitOfWork.PickedInventoryItemRepository.FindByKey(new PickedInventoryItemKey(item), i => i.CurrentLocation).CurrentLocation);
                }
            }

            [Test]
            public void Will_create_Inventory_as_expected_when_posting_InterWarehouseOrder()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(
                    o => o.OrderType = InventoryShipmentOrderTypeEnum.InterWarehouseOrder,
                    o => o.OrderStatus = OrderStatus.Scheduled,
                    o => o.ShipmentInformation.Status = ShipmentStatus.Scheduled);
                var items = new List<PickedInventoryItem>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order))
                    };
                var destinationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(order.DestinationFacility)));
                var expected = items.ToDictionary(i => i, i => new InventoryKey(i, i, destinationKey, i, i.ToteKey));

                //Act
                var result = Service.Post(new PostParameters
                    {
                        UserToken = TestUser.UserName,
                        OrderKey = new InventoryShipmentOrderKey(order),
                        PickedItemDestinations = items.Select(i => new PostItemParameters
                            {
                                PickedInventoryItemKey = new PickedInventoryItemKey(i),
                                DestinationLocationKey = destinationKey
                            })
                    });

                //Assert
                result.AssertSuccess();
                foreach(var item in expected)
                {
                    Assert.AreEqual(item.Key.Quantity, RVCUnitOfWork.InventoryRepository.FindByKey(item.Value).Quantity);
                }
            }

            [Test]
            public void Will_create_Inventory_as_expected_when_posting_TreatmentOrder()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<TreatmentOrder>(
                    o => o.InventoryShipmentOrder.OrderType = InventoryShipmentOrderTypeEnum.TreatmentOrder,
                    o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Scheduled,
                    o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Scheduled);
                var items = new List<PickedInventoryItem>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order.InventoryShipmentOrder)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order.InventoryShipmentOrder))
                    };
                var destinationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.ConstrainByKeys(order.InventoryShipmentOrder.DestinationFacility)));
                var expected = items.ToDictionary(i => i, i => new InventoryKey(i, i, destinationKey, i, i.ToteKey));

                //Act
                var result = Service.Post(new PostParameters
                    {
                        UserToken = TestUser.UserName,
                        OrderKey = new InventoryShipmentOrderKey(order),
                        PickedItemDestinations = items.Select(i => new PostItemParameters
                            {
                                PickedInventoryItemKey = new PickedInventoryItemKey(i),
                                DestinationLocationKey = destinationKey
                            })
                    });

                //Assert
                result.AssertSuccess();
                foreach(var item in expected)
                {
                    Assert.AreEqual(item.Key.Quantity, RVCUnitOfWork.InventoryRepository.FindByKey(item.Value).Quantity);
                }
            }
        }

        [TestFixture]
        public class GetInventoryShipmentOrderAcknowledgement : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_order_cannot_be_found()
            {
                //Act
                var result = Service.GetInhouseShipmentOrderAcknowledgement(new InventoryShipmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.InventoryShipmentOrderNotFound);
            }

            [Test]
            public void Returns_customer_notes_as_expected()
            {
                //Arrange
                var customer0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Notes = new List<CustomerNote>
                    {
                        TestHelper.CreateObjectGraph<CustomerNote>(),
                        TestHelper.CreateObjectGraph<CustomerNote>()
                    });
                var customer1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Notes = new List<CustomerNote>
                    {
                        TestHelper.CreateObjectGraph<CustomerNote>()
                    });
                var customer2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Notes = new List<CustomerNote>());

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(i => i.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(null)),
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(customer0)),
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(customer0)),
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(customer1)),
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(customer2))
                    });

                var expectedNotes = order.InventoryPickOrder.Items.Where(i => i.Customer != null && i.Customer.Notes != null)
                    .SelectMany(i => i.Customer.Notes)
                    .Distinct()
                    .Select(n => n.ToCustomerNoteKey())
                    .ToList();

                //Act
                StartStopwatch();
                var result = Service.GetInhouseShipmentOrderAcknowledgement(order.ToInventoryShipmentOrderKey());
                StopWatchAndWriteTime();
                result.AssertSuccess();

                //Assert
                var resultNotes = result.ResultingObject.CustomerNotes.SelectMany(c => c.CustomerNotes).ToList();
                Assert.AreEqual(expectedNotes.Count, resultNotes.Count);
                expectedNotes.ForEach(e => Assert.IsNotNull(resultNotes.FirstOrDefault(n => n.CustomerNoteKey == e.KeyValue)));
            }
        }

        [TestFixture]
        public class GetInventoryShipmentOrderPackingList : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_order_cannot_be_found()
            {
                //Act
                var result = Service.GetInventoryShipmentOrderPackingList(new InventoryShipmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.InventoryShipmentOrderNotFound);
            }

            [Test]
            public void Returns_items_as_expected()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(i => i.PickedInventory.Items = new List<PickedInventoryItem>
                    {
                        TestHelper.CreateObjectGraph<PickedInventoryItem>(),
                        TestHelper.CreateObjectGraph<PickedInventoryItem>(),
                        TestHelper.CreateObjectGraph<PickedInventoryItem>()
                    });

                var expectedItems = new List<LotKey>
                    {
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)).Lot),
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)).Lot),
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)).Lot)
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryShipmentOrderPackingList(new InventoryShipmentOrderKey(order));
                StopWatchAndWriteTime();
                result.AssertSuccess();

                //Assert
                var resultItems = result.ResultingObject.Items.ToList();
                Assert.AreEqual(expectedItems.Count, resultItems.Count);
                expectedItems.ForEach(e => Assert.IsNotNull(resultItems.FirstOrDefault(n => n.LotKey == e.KeyValue)));
            }
        }

        [TestFixture]
        public class GetInventoryShipmentOrderBillOfLading : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_order_cannot_be_found()
            {
                //Act
                var result = Service.GetInventoryShipmentOrderBillOfLading(new InventoryShipmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.InventoryShipmentOrderNotFound);
            }

            [Test]
            public void Returns_items_as_expected()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();

                var expectedItems = new List<LotKey>
                    {
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)).Lot),
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)).Lot),
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)).Lot)
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryShipmentOrderBillOfLading(new InventoryShipmentOrderKey(order));
                StopWatchAndWriteTime();
                result.AssertSuccess();

                //Assert
                var resultItems = result.ResultingObject.Items.ToList();
                Assert.AreEqual(expectedItems.Count, resultItems.Count);
                expectedItems.ForEach(e => Assert.IsNotNull(resultItems.FirstOrDefault(n => n.LotKey == e.KeyValue)));
            }
        }

        [TestFixture]
        public class GetInventoryShipmentOrderPickSheet : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_order_cannot_be_found()
            {
                //Act
                var result = Service.GetInventoryShipmentOrderPickSheet(new InventoryShipmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.InventoryShipmentOrderNotFound);
            }

            [Test]
            public void Returns_CustomerNotes_as_expected()
            {
                //Arrange
                var customer0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Notes = new List<CustomerNote>
                    {
                        TestHelper.CreateObjectGraph<CustomerNote>(),
                        TestHelper.CreateObjectGraph<CustomerNote>()
                    });
                var customer1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Notes = new List<CustomerNote>
                    {
                        TestHelper.CreateObjectGraph<CustomerNote>()
                    });
                var customer2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Notes = new List<CustomerNote>());

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(i => i.InventoryPickOrder.Items = new List<InventoryPickOrderItem>
                    {
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(null)),
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(customer0)),
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(customer0)),
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(customer1)),
                        TestHelper.CreateObjectGraph<InventoryPickOrderItem>(o => o.SetCustomer(customer2))
                    });

                var expectedNotes = order.InventoryPickOrder.Items.Where(i => i.Customer != null && i.Customer.Notes != null)
                    .SelectMany(i => i.Customer.Notes)
                    .Distinct()
                    .Select(n => new CustomerNoteKey(n))
                    .ToList();

                //Act
                StartStopwatch();
                var result = Service.GetInventoryShipmentOrderPickSheet(new InventoryShipmentOrderKey(order));
                StopWatchAndWriteTime();
                result.AssertSuccess();

                //Assert
                var resultNotes = result.ResultingObject.CustomerNotes.SelectMany(c => c.CustomerNotes).ToList();
                Assert.AreEqual(expectedNotes.Count, resultNotes.Count);
                expectedNotes.ForEach(e => Assert.IsNotNull(resultNotes.FirstOrDefault(n => n.CustomerNoteKey == e.KeyValue)));
            }

            [Test]
            public void Returns_items_as_expected()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                var location = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var expectedItems = new List<LotKey>
                    {
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order).SetFromLocation(location)).Lot),
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order).SetFromLocation(location)).Lot),
                        new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order)).Lot)
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryShipmentOrderPickSheet(new InventoryShipmentOrderKey(order));
                StopWatchAndWriteTime();
                result.AssertSuccess();

                //Assert
                var resultItems = result.ResultingObject.Items.ToList();
                Assert.AreEqual(expectedItems.Count, resultItems.Count);
                expectedItems.ForEach(e => Assert.IsNotNull(resultItems.FirstOrDefault(n => n.LotKey == e.KeyValue)));
            }
        }

        [TestFixture]
        public class GetInventoryShipmentOrderCertificateOfAnalysis : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_order_cannot_be_found()
            {
                //Act
                var result = Service.GetInventoryShipmentOrderCertificateOfAnalysis(new InventoryShipmentOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.InventoryShipmentOrderNotFound);
            }

            [Test]
            public void Returns_items_as_expected()
            {
                //Arrange
                var additiveLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>(l => l.Lot.SetAdditiveLot());
                var chileLot0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.Lot.SetChileLot());
                var chileLot1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.Lot.SetChileLot());
                var treatment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order, additiveLot));
                var expectedItems = new List<PickedInventoryItem>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order, chileLot0, null, treatment)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order, chileLot0)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order, chileLot1, null, treatment)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(order, chileLot1, null, treatment))
                    };
                var expectedKeys = expectedItems.Select(i => new
                    {
                        LotKey = new LotKey(i.Lot).KeyValue,
                        TreatmentKey = new InventoryTreatmentKey(i.Treatment)
                    }).Distinct().ToList();

                //Act
                StartStopwatch();
                var result = Service.GetInventoryShipmentOrderCertificateOfAnalysis(new InventoryShipmentOrderKey(order));
                StartStopwatch();
                result.AssertSuccess();

                //Assert
                var resultItems = result.ResultingObject.Items.ToList();
                Assert.AreEqual(expectedKeys.Count, resultItems.Count);
                expectedKeys.ForEach(e => Assert.IsNotNull(resultItems.FirstOrDefault(n => n.LotKey == e.LotKey && n.TreatmentReturn.TreatmentKey == e.TreatmentKey)));
            }
        }

        [TestFixture]
        public class GetShipmentMethods : InventoryShipmentOrderServiceTests
        {
            [Test]
            public void Returns_distinct_shipment_methods_as_expected()
            {
                //Arrange
                var expectedResults = new[]
                    {
                        "Shipment",
                        "Method"
                    };

                foreach(var method in expectedResults)
                {
                    TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.ShipmentMethod = method);
                    TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.ShipmentMethod = method);
                }

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.ShipmentMethod = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.ShipmentMethod = "");
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryShipmentOrder>(o => o.ShipmentInformation.ShipmentMethod = "  ");

                //Act
                var result = Service.GetShipmentMethods();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(expectedResults.Count(), results.Count);
                Assert.IsTrue(expectedResults.All(results.Contains));
            }
        }
    }
}