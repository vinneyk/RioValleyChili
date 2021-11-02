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

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class WarehouseOrderServiceTests
    {
        [TestFixture]
        public class SynchronizeInventoryShipmentOrderUnitTests : SynchronizeOldContextUnitTestsBase<IResult<SyncInventoryShipmentOrderParameters>>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncInventoryShipmentOrder; } }
        }

        [TestFixture]
        public class CreateWarehouseOrder : SynchronizeOldContextIntegrationTestsBase<WarehouseOrderService>
        {
            [Test]
            public void Creates_new_tblMove_record_as_expected()
            {
                //Arrange
                var facilities = RVCUnitOfWork.FacilityRepository.Filter(f => f.FacilityType == FacilityType.Internal).Take(2).ToList();
                var parameters = new CreateTreatmentOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        SourceFacilityKey = new FacilityKey(facilities[0]),
                        DestinationFacilityKey = new FacilityKey(facilities[1]),
                        SetShipmentInformation = new SetInventoryShipmentInformationParameters
                            {
                                ShippingInstructions = new SetShippingInstructionsParameters
                                {
                                    ExternalNotes = "When will my ship come in..."
                                }
                            }
                    };

                //Act
                var result = Service.CreateWarehouseOrder(parameters);
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
        public class UpdateInterWarehouseOrder : SynchronizeOldContextIntegrationTestsBase<WarehouseOrderService>
        {
            [Test]
            public void Updates_existing_tblMove_record_as_expected()
            {
                //Arrange
                const string expectedComments = "pew-pew-pew";
                var order = RVCUnitOfWork.InventoryShipmentOrderRepository.FindBy(o => o.OrderType == InventoryShipmentOrderTypeEnum.InterWarehouseOrder && o.ShipmentInformation.ExternalNotes != expectedComments);
                var parameters = new UpdateInterWarehouseOrderParameters
                    {
                        InventoryShipmentOrderKey = new InventoryShipmentOrderKey(order),
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
                var result = Service.UpdateInterWarehouseOrder(parameters);
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SyncTblMove);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var moveNum = int.Parse(resultString);
                var tblMove = new RioAccessSQLEntities().tblMoves.FirstOrDefault(m => m.MoveNum == moveNum);
                Assert.AreEqual(order.MoveNum, tblMove.MoveNum);
                Assert.AreEqual(tblMove.ExternalNotes, parameters.SetShipmentInformation.ShippingInstructions.ExternalNotes);
            }
        }
    }
}