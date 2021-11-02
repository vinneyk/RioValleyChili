using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class ChileMaterialsReceivedServiceTests
    {
        [TestFixture]
        public class SyncDehydratedMaterialsReceivedUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, LotKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncChileMaterialsReceived; } }
        }

        [TestFixture]
        public class CreatedDehydratedMaterialsReceivedIntegrationTests : SynchronizeOldContextIntegrationTestsBase<MaterialsReceivedService>
        {
            [Test]
            public void Creates_new_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var dehydrator = RVCUnitOfWork.CompanyRepository.Filter(c => c.CompanyTypes.Any(t => t.CompanyType == (int)CompanyType.Dehydrator)).First();
                var chileProduct = RVCUnitOfWork.ChileProductRepository.Filter(c => c.ChileState == ChileStateEnum.Dehydrated).First();
                var warehouseLocation = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).First();
                var packagingProduct = RVCUnitOfWork.PackScheduleRepository.All().First();

                var parameters = new ChileMaterialsReceivedParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                        DateReceived = new DateTime(2020, 3, 29),
                        SupplierKey = new CompanyKey(dehydrator),
                        TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                        LoadNumber = "42",
                        ChileProductKey = new ChileProductKey(chileProduct),
                        PurchaseOrder = "Yes",
                        ShipperNumber = "No",
                        Items = new List<ChileMaterialsReceivedItemParameters>
                                {
                                    new ChileMaterialsReceivedItemParameters
                                        {
                                            ToteKey = "TOTE1",
                                            Quantity = 23,
                                            PackagingProductKey = new PackagingProductKey(packagingProduct),
                                            Variety = "Variety",
                                            GrowerCode = "CODE",
                                            LocationKey = new LocationKey(warehouseLocation)
                                        },
                                    new ChileMaterialsReceivedItemParameters
                                        {
                                            ToteKey = "TOTE2",
                                            Quantity = 45,
                                            PackagingProductKey = new PackagingProductKey(packagingProduct),
                                            Variety = "Variety",
                                            GrowerCode = "CODE",
                                            LocationKey = new LocationKey(warehouseLocation)
                                        },
                                }
                    };

                TestHelper.ResetContext();

                //Act
                var result = Service.CreateChileMaterialsReceived(parameters);
                result.AssertSuccess();
                var lotString = GetKeyFromConsoleString(ConsoleOutput.SyncDehydratedReceived);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                Assert.IsNotNull(new RioAccessSQLEntities().tblLots.FirstOrDefault(t => t.Lot == newLot));
            }
        }

        [TestFixture]
        public class UpdateDehydratedMaterialsReceivedIntegrationTests : SynchronizeOldContextIntegrationTestsBase<MaterialsReceivedService>
        {
            [Test]
            public void Modifies_existing_tblLot_and_tblIncoming_records_as_expected()
            {
                //Arrange
                const string expectedPurchaseOrder = "testing";
                var inventory = RVCUnitOfWork.InventoryRepository.All();
                var received = RVCUnitOfWork.ChileMaterialsReceivedRepository
                    .Filter(d => d.Items.Count > 1 && d.ChileLot.Lot.PurchaseOrderNumber != expectedPurchaseOrder
                        && d.Items.All(i => inventory.Any(n => n.LotDateCreated == i.LotDateCreated && n.LotDateSequence == i.LotDateSequence && n.LotTypeId == i.LotTypeId &&
                        n.Quantity >= i.Quantity && n.PackagingProductId == i.PackagingProductId && n.LocationId == i.LocationId && n.ToteKey == i.ToteKey && n.TreatmentId == StaticInventoryTreatments.NoTreatment.Id)),
                        r => r.ChileLot.Lot,
                        r => r.Employee, d => d.Items)
                    .FirstOrDefault();
                if(received == null)
                {
                    Assert.Inconclusive("Could not find suitable ChileMaterialsReceived record for testing.");
                }

                var parameters = new ChileMaterialsReceivedParameters(received) { PurchaseOrder = expectedPurchaseOrder };
                var quantity = 1;
                parameters.Items.ForEach(i => i.Quantity = quantity++);
                parameters.Items.RemoveAt(1);

                //Act
                var result = Service.UpdateChileMaterialsReceived(parameters);
                result.AssertSuccess();
                var lotString = GetKeyFromConsoleString(ConsoleOutput.SyncDehydratedReceived);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var lotNumber = int.Parse(lotString);
                var lot = new RioAccessSQLEntities().tblLots
                    .Select(l => new
                        {
                            lot = l,
                            incoming = l.tblIncomings
                        })
                    .FirstOrDefault(t => t.lot.Lot == lotNumber);
                Assert.AreEqual(expectedPurchaseOrder, lot.lot.PurchOrder);
                var resultItems = lot.incoming.ToList();

                parameters.Items.AssertEquivalent(resultItems,
                    e => new { e.Quantity },
                    r => new { Quantity = (int) r.Quantity },
                    (e, r) => Assert.AreEqual(parameters.TreatmentKey, r.TrtmtID.ToString()));
            }
        }
    }
}