using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using Solutionhead.Services;
using CreateTreatmentOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.CreateTreatmentOrderParameters;
using ReceiveTreatmentOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.ReceiveTreatmentOrderParameters;
using UpdateTreatmentOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.UpdateTreatmentOrderParameters;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class TreatmentOrderServiceTests
    {
        [TestFixture]
        public class DeleteTreatmentOrderUnitTests : SynchronizeOldContextUnitTestsBase<IResult, int?>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.DeleteTreatmentOrder; } }
        }

        [TestFixture]
        public class SynchronizeInventoryShipmentOrderUnitTests : SynchronizeOldContextUnitTestsBase<IResult<SyncInventoryShipmentOrderParameters>>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncInventoryShipmentOrder; } }
        }

        [TestFixture]
        public class CreateTreatmentOrder : SynchronizeOldContextIntegrationTestsBase<TreatmentOrderService>
        {
            [Test]
            public void Creates_new_tblMove_record_as_expected()
            {
                //Arrange
                var source = RVCUnitOfWork.FacilityRepository.Filter(f => f.Name.Contains("rincon")).First();
                var destination = RVCUnitOfWork.FacilityRepository.All().First(f => f.FacilityType == FacilityType.Treatment);
                var treatment = RVCUnitOfWork.InventoryTreatmentRepository.All().First();
                var parameters = new CreateTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        SourceFacilityKey = new FacilityKey(source),
                        DestinationFacilityKey = new FacilityKey(destination),
                        TreatmentKey = new InventoryTreatmentKey(treatment),
                        SetShipmentInformation = new SetInventoryShipmentInformationParameters
                            {
                                ShippingInstructions = new SetShippingInstructionsParameters
                                    {
                                        ExternalNotes = "When will my ship come in..."
                                    }
                            }
                    };

                //Act
                var result = Service.CreateInventoryTreatmentOrder(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SyncTblMove);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var moveNum = int.Parse(resultString);
                var tblMove = new RioAccessSQLEntities().tblMoves.FirstOrDefault(m => m.MoveNum == moveNum);
                Assert.AreEqual(tblMove.ExternalNotes, parameters.SetShipmentInformation.ShippingInstructions.ExternalNotes);
            }
        }

        [TestFixture]
        public class UpdateTreatmentOrder : SynchronizeOldContextIntegrationTestsBase<TreatmentOrderService>
        {
            [Test]
            public void Updates_existing_tblMove_record_as_expected()
            {
                //Arrange
                const string expectedComments = "pew-pew-pew";
                var treatmentOrder = RVCUnitOfWork.TreatmentOrderRepository.FindBy(o => o.InventoryShipmentOrder.ShipmentInformation.ExternalNotes != expectedComments, o => o.InventoryShipmentOrder);
                var parameters = new UpdateTreatmentOrderParameters
                    {
                        TreatmentOrderKey = new TreatmentOrderKey(treatmentOrder),
                        UserToken = TestUser.UserName,
                        SetShipmentInformation = new SetInventoryShipmentInformationParameters
                            {
                                ShippingInstructions = new SetShippingInstructionsParameters
                                {
                                    ExternalNotes = expectedComments
                                }
                            }
                    };

                //Act
                var result = Service.UpdateTreatmentOrder(parameters);
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SyncTblMove);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var moveNum = int.Parse(resultString);
                var tblMove = new RioAccessSQLEntities().tblMoves.FirstOrDefault(m => m.MoveNum == moveNum);
                Assert.AreEqual(treatmentOrder.InventoryShipmentOrder.MoveNum, tblMove.MoveNum);
                Assert.AreEqual(tblMove.ExternalNotes, parameters.SetShipmentInformation.ShippingInstructions.ExternalNotes);
            }
        }

        [TestFixture]
        public class ReceiveTreatmentOrder : SynchronizeOldContextIntegrationTestsBase<TreatmentOrderService>
        {
            [Test]
            public void Updates_tblMove_Status_as_expected()
            {
                var inventory = RVCUnitOfWork.InventoryRepository.All().AsQueryable();
                var treatmentOrder = RVCUnitOfWork.TreatmentOrderRepository.FindBy(o => o.InventoryShipmentOrder.OrderStatus == OrderStatus.Scheduled && o.InventoryShipmentOrder.ShipmentInformation.Status == ShipmentStatus.Shipped && o.InventoryShipmentOrder.PickedInventory.Items.Any() &&
                    o.InventoryShipmentOrder.PickedInventory.Items.All(p => inventory.Any(i => i.LotTypeId == p.LotTypeId && i.LotDateCreated == p.LotDateCreated && i.LotDateSequence == p.LotDateSequence && i.LocationId == p.CurrentLocationId && i.PackagingProductId == p.PackagingProductId && i.TreatmentId == p.TreatmentId && i.ToteKey == p.ToteKey && i.Quantity >= p.Quantity)),
                    o => o.InventoryShipmentOrder.DestinationFacility.Locations,
                    o => o.InventoryShipmentOrder.SourceFacility.Locations);
                if(treatmentOrder == null)
                {
                    Assert.Inconclusive("No suitable treatment order for testing.");
                }

                //Act
                var result = Service.ReceiveOrder(new ReceiveTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TreatmentOrderKey = new TreatmentOrderKey(treatmentOrder),
                        DestinationLocationKey = new LocationKey(treatmentOrder.InventoryShipmentOrder.DestinationFacility.Locations.First())
                    });
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.ReceivedTreatmentOrder);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var moveNum = int.Parse(resultString);
                var tblMove = new RioAccessSQLEntities().tblMoves.FirstOrDefault(m => m.MoveNum == moveNum);
                Assert.AreEqual((int)tblOrderStatus.Treated, tblMove.Status);
            }
        }

        [TestFixture]
        public class DeleteTreatmentOrder : SynchronizeOldContextIntegrationTestsBase<TreatmentOrderService>
        {
            [Test]
            public void Removes_tblMove_record_as_expected()
            {
                //Arrange
                var treatmentOrder = RVCUnitOfWork.TreatmentOrderRepository.Filter(o => o.InventoryShipmentOrder.OrderStatus != OrderStatus.Fulfilled &&
                    o.InventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Shipped &&
                    o.InventoryShipmentOrder.PickedInventory.Items.All(i => i.CurrentLocationId == i.FromLocationId)).FirstOrDefault();
                if(treatmentOrder == null)
                {
                    Assert.Inconclusive("No suitable treatment order for testing.");
                }

                //Act
                var result = Service.DeleteTreatmentOrder(new TreatmentOrderKey(treatmentOrder));
                result.AssertSuccess();

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var resultString = GetKeyFromConsoleString(ConsoleOutput.RemovedTblMove);
                var moveNum = int.Parse(resultString);
                using(var context = new RioAccessSQLEntities())
                {
                    Assert.IsNull(context.tblMoves.FirstOrDefault(m => m.MoveNum == moveNum));
                }
            }
        }
    }
}