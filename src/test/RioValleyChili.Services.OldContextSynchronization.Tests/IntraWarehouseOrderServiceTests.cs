using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;
using CreateIntraWarehouseOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.CreateIntraWarehouseOrderParameters;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class IntraWarehouseOrderServiceTests
    {
        [TestFixture]
        public class UnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, IntraWarehouseOrderKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncIntraWarehouseOrder; } }
        }

        [TestFixture]
        public class CreateIntraWarehouseOrder : SynchronizeOldContextIntegrationTestsBase<IntraWarehouseOrderService>
        {
            [Test]
            public void Creates_tblRincon_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var trackingSheetNumber = RVCUnitOfWork.IntraWarehouseOrderRepository.All().Any() ? RVCUnitOfWork.IntraWarehouseOrderRepository.All().Max(i => i.TrackingSheetNumber) + 1m : 123.1m;
                var inventory = RVCUnitOfWork.InventoryRepository
                    .Filter(i => i.Lot.LotTypeId == (int) LotTypeEnum.FinishedGood && i.Quantity > 2 && i.Location.Facility.Locations.Count(l => l.LocID != null) > 1 && i.Location.LocID != null,

                        i => i.Location.Facility.Locations)
                    .FirstOrDefault();
                if(inventory == null)
                {
                    Assert.Inconclusive("Could not find valid inventory for intrawarehouse order.");
                }

                var destinationLocation = inventory.Location.Facility.Locations.First(l => l.Id != inventory.Location.Id && l.LocID != null);

                //Act
                var result = Service.CreateIntraWarehouseOrder(new CreateIntraWarehouseOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        TrackingSheetNumber = trackingSheetNumber,
                        OperatorName = "Mr. Smooth",
                        MovementDate = new DateTime(2014, 1, 1),
                        PickedItems = new List<IIntraWarehouseOrderPickedItemParameters>
                            {
                                new IntraWarehouseOrderPickedItemParameters
                                    {
                                        InventoryKey = new InventoryKey(inventory),
                                        DestinationLocationKey = new LocationKey(destinationLocation),
                                        Quantity = (int) (inventory.Quantity * 0.5)
                                    }
                            }

                    });
                var movementString = GetKeyFromConsoleString(ConsoleOutput.SynchronizedIntraWarehouserMovement);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var rinconId = DateTime.ParseExact(movementString, SyncIntraWarehouseOrder.DateTimeFormat, CultureInfo.InvariantCulture);
                Assert.AreEqual(trackingSheetNumber, new RioAccessSQLEntities().tblRincons.FirstOrDefault(r => r.RinconID == rinconId).SheetNum);
            }
        }

        [TestFixture]
        public class UpdateIntraWarehouseOrder : SynchronizeOldContextIntegrationTestsBase<IntraWarehouseOrderService>
        {
            [Test]
            public void Updates_tblRincon_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                const string operatorName = "Dr. Update";
                var order = RVCUnitOfWork.IntraWarehouseOrderRepository.Filter(i => i.RinconID != null && i.OperatorName != operatorName).FirstOrDefault();
                if(order == null)
                {
                    Assert.Inconclusive("Could not find valid IntraWarehouseOrder to update.");
                }

                //Act
                var result = Service.UpdateIntraWarehouseOrder(new UpdateIntraWarehouseOrderParameters
                    {
                        IntraWarehouseOrderKey = new IntraWarehouseOrderKey(order),
                        UserToken = TestUser.UserName,
                        OperatorName = operatorName,
                        MovementDate = new DateTime(2014, 1, 1)
                    });
                var movementString = GetKeyFromConsoleString(ConsoleOutput.SynchronizedIntraWarehouserMovement);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var rinconId = DateTime.ParseExact(movementString, SyncIntraWarehouseOrder.DateTimeFormat, CultureInfo.InvariantCulture);
                Assert.AreEqual(operatorName, new RioAccessSQLEntities().tblRincons.FirstOrDefault(r => r.RinconID == rinconId).PrepBy);
            }
        }
    }
}