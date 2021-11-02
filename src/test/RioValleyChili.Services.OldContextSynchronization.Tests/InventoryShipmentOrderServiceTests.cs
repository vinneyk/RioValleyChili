using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class InventoryShipmentOrderServiceTests
    {
        [TestFixture]
        public class SetShipmentInformationUnitTests : SynchronizeOldContextUnitTestsBase<IResult<InventoryShipmentOrderKey>>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SetShipmentInformation; } }
        }

        [TestFixture]
        public class PostUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, InventoryShipmentOrderKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.Post; } }
        }

        [TestFixture]
        public class SetShipmentInformationIntegrationTests : SynchronizeOldContextIntegrationTestsBase<InventoryShipmentOrderService>
        {
            [Test]
            public void Sets_InterWarehouseOrderShipmentInformation()
            {
                //Arrange
                const string comments0 = "ShipmentComments! Awesome!";
                const string comments1 = "Awesome comments 2.0!";
                var order = RVCUnitOfWork.InventoryShipmentOrderRepository.Filter(o => o.OrderType == InventoryShipmentOrderTypeEnum.InterWarehouseOrder, o => o.ShipmentInformation).FirstOrDefault();
                if(order == null)
                {
                    Assert.Inconclusive("No suitable InterWarehouseOrder record to test found.");
                }
                var expectedComments = order.ShipmentInformation.ExternalNotes == comments0 ? comments1 : comments0;

                //Act
                var result = Service.SetShipmentInformation(new SetInventoryShipmentInformationParameters
                    {
                        InventoryShipmentOrderKey = new InventoryShipmentOrderKey(order),
                        ShippingInstructions = new SetShippingInstructionsParameters
                            {
                                ExternalNotes = expectedComments
                            }
                    });
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SetShipmentTblMove);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var moveNum = int.Parse(resultString);
                var tblMove = new RioAccessSQLEntities().tblMoves.FirstOrDefault(o => o.MoveNum == moveNum);
                Assert.AreEqual(expectedComments, tblMove.ExternalNotes);
            }
        }

        [TestFixture]
        public class PostIntegrationTests : SynchronizeOldContextIntegrationTestsBase<InventoryShipmentOrderService>
        {
            [Test]
            public void Posts_InterWarehouseOrder()
            {
                //Arrange
                var order = RVCUnitOfWork.InventoryShipmentOrderRepository
                    .Filter(o =>
                        o.OrderType == InventoryShipmentOrderTypeEnum.InterWarehouseOrder &&
                        o.ShipmentInformation.Status != ShipmentStatus.Shipped &&
                        o.PickedInventory.Items.Any(),
                        o => o.DestinationFacility.Locations,
                        o => o.PickedInventory.Items)
                    .FirstOrDefault();
                if(order == null)
                {
                    Assert.Inconclusive("Could not find valid InterWarehouseOrder to post.");
                }
                var destinationLocation = new LocationKey(order.DestinationFacility.Locations.First(l => l.LocID != null));


                //Act
                var result = Service.Post(new PostParameters
                    {
                        UserToken = TestUser.UserName,
                        OrderKey = order.ToInventoryShipmentOrderKey(),
                        PickedItemDestinations = order.PickedInventory.Items.Select(i => new PostItemParameters
                            {
                                PickedInventoryItemKey = new PickedInventoryItemKey(i),
                                DestinationLocationKey = destinationLocation
                            })
                    });
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.PostedOrder);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var moveNum = int.Parse(resultString);
                var outgoings = new RioAccessSQLEntities().tblOutgoings.Where(o => o.MoveNum == moveNum).ToList();
                foreach(var item in order.PickedInventory.Items)
                {
                    Assert.IsNotNull(outgoings.Single(o => o.MDetail == item.DetailID.Value && o.Quantity == item.Quantity));
                    Assert.IsNotNull(outgoings.Single(o => o.MDetail == item.DetailID.Value && o.Quantity == -item.Quantity));
                }
            }

            [Test]
            public void Posts_CustomerOrder()
            {
                //Arrange
                var order = RVCUnitOfWork.SalesOrderRepository
                    .Filter(o =>
                        o.InventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Shipped &&
                        o.SalesOrderPickedItems.Any(),
                        o => o.SalesOrderPickedItems.Select(i => i.PickedInventoryItem))
                    .FirstOrDefault();
                if(order == null)
                {
                    Assert.Inconclusive("Could not find valid CustomerOrder to post.");
                }

                //Act
                var result = Service.Post(new PostParameters
                    {
                        UserToken = TestUser.UserName,
                        OrderKey = order.ToSalesOrderKey()
                    });
                var resultString = GetKeyFromConsoleString(ConsoleOutput.PostedOrder);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var orderNum = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var outgoings = oldContext.tblOutgoings.Where(o => o.OrderNum == orderNum).ToList();
                    foreach(var item in order.SalesOrderPickedItems)
                    {
                        Assert.IsNotNull(outgoings.Single(o => o.ODetail == item.PickedInventoryItem.DetailID && o.Quantity == item.PickedInventoryItem.Quantity));
                    }
                }
            }
        }
    }
}