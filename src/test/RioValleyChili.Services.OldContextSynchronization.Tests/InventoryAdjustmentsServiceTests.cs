using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class InventoryAdjustmentsServiceTests
    {
        [TestFixture]
        public class CreateInventoryAdjustmentUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, InventoryAdjustmentKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.CreateInventoryAdjustment; } }
        }

        [TestFixture]
        public class CreateInventoryAdjustmentIntegrationTests : SynchronizeOldContextIntegrationTestsBase<InventoryAdjustmentsService>
        {
            [Test]
            public void Creates_new_tblAdjust_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var chileLot = RVCUnitOfWork.ChileLotRepository.All().First();
                var packaging = RVCUnitOfWork.PackagingProductRepository.All().First();
                var treatment = RVCUnitOfWork.TreatmentOrderRepository.All().First();
                var warehouseLocation = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).First();
                
                var parameters = new CreateInventoryAdjustmentParameters
                    {
                        UserToken = TestUser.UserName,
                        Comment = "Test inventory adjustment",
                        InventoryAdjustments = new[]
                            {
                                new InventoryAdjustmentParameters
                                    {
                                        LotKey = new LotKey(chileLot),
                                        PackagingProductKey = new PackagingProductKey(packaging),
                                        ToteKey = "TEST",
                                        TreatmentKey = new InventoryTreatmentKey(treatment),
                                        WarehouseLocationKey = new LocationKey(warehouseLocation),
                                        Adjustment = 100
                                    }
                            }
                    };

                TestHelper.ResetContext();

                //Act
                var result = Service.CreateInventoryAdjustment(parameters);
                var adjustString = GetKeyFromConsoleString(ConsoleOutput.AddedAdjust);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newAdjustID = DateTime.ParseExact(adjustString, DateTimeExtensions.SQLDateTimeFormat, null);
                Assert.IsNotNull(new RioAccessSQLEntities().tblAdjusts.FirstOrDefault(a => a.AdjustID == newAdjustID));
            }
        }
    }
}