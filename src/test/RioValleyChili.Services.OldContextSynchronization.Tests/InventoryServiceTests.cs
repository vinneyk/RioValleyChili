using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.OldContextSynchronization.Utilities;
using RioValleyChili.Services.Tests;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class InventoryServiceTests
    {
        [TestFixture]
        public class ReceiveInventoryUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, LotKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.ReceiveInventory; } }
        }

        [TestFixture]
        public class ReceiveInventoryIntegrationTests : SynchronizeOldContextIntegrationTestsBase<InventoryService>
        {
            [Test]
            public void Creates_new_tblLot_and_tblIncoming_records_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var packaging = RVCUnitOfWork.PackagingProductRepository.All().First();
                var locations = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).Take(2).ToList();

                var parameters = new ReceiveInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        LotType = LotTypeEnum.Additive,
                        ProductKey = new ProductKey((IProductKey)RVCUnitOfWork.AdditiveProductRepository.All().First()),
                        PackagingReceivedKey = new PackagingProductKey(packaging),
                        Items = new List<ReceiveInventoryItemParameters>
                            {
                                new ReceiveInventoryItemParameters
                                    {
                                        Quantity = 1,
                                        PackagingProductKey = new PackagingProductKey(packaging),
                                        WarehouseLocationKey = new LocationKey(locations[0]),
                                        TreatmentKey = new InventoryTreatmentKey(RVCUnitOfWork.InventoryTreatmentRepository.All().First()),
                                        ToteKey = "TOTE"
                                    },
                                new ReceiveInventoryItemParameters
                                    {
                                        Quantity = 2,
                                        PackagingProductKey = new PackagingProductKey(packaging),
                                        WarehouseLocationKey = new LocationKey(locations[1]),
                                        TreatmentKey = new InventoryTreatmentKey(RVCUnitOfWork.InventoryTreatmentRepository.All().First())
                                    }
                            }
                    };

                //Act
                var result = Service.ReceiveInventory(parameters);
                var lotString = GetKeyFromConsoleString(ConsoleOutput.ReceivedInventory);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var oldContext = new RioAccessSQLEntities();

                var tblLot = oldContext.tblLots.FirstOrDefault(a => a.Lot == newLot);
                Assert.IsNotNull(tblLot);

                var tblIncoming = oldContext.tblIncomings.Where(i => i.Lot == newLot).ToList();
                Assert.AreEqual(parameters.Items.Count(), tblIncoming.Count);
                foreach(var item in parameters.Items)
                {
                    Assert.AreEqual(1, tblIncoming.Count(i => i.Quantity == item.Quantity));
                }
            }

            [Test]
            public void Creates_new_tblProduct_record_when_receiving_packaging_if_record_does_not_exist()
            {
                //Arrange
                var oldContext = new RioAccessSQLEntities();
                var helper = new OldContextHelper(oldContext);
                var packagings = TestHelper.Context.PackagingProducts.Select(p => new
                    {
                        Packaging = p,
                        p.Product
                    }).ToList();
                tblPackaging tblPackaging;
                var packaging = packagings.FirstOrDefault(p => helper.GetProductFromPackagingId(p.Product.ProductCode, out tblPackaging) == null);
                if(packaging == null)
                {
                    Assert.Inconclusive("No suitable Packaging product for testing.");
                }

                var location = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).First();
                var parameters = new ReceiveInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        LotType = LotTypeEnum.Additive,
                        ProductKey = new ProductKey((IProductKey)RVCUnitOfWork.AdditiveProductRepository.All().First()),
                        PackagingReceivedKey = new PackagingProductKey(packaging.Packaging),
                        Items = new List<ReceiveInventoryItemParameters>
                                {
                                    new ReceiveInventoryItemParameters
                                        {
                                            Quantity = 1,
                                            PackagingProductKey = new PackagingProductKey(packaging.Packaging),
                                            WarehouseLocationKey = new LocationKey(location),
                                            TreatmentKey = new InventoryTreatmentKey(RVCUnitOfWork.InventoryTreatmentRepository.All().First()),
                                            ToteKey = ""
                                        },
                                }
                    };

                //Act
                var result = Service.ReceiveInventory(parameters);
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                Assert.IsNotNull(oldContext.tblPackagings.Single(p => p.Packaging == packaging.Product.Name));
            }

            [Test]
            public void Creates_LotKey_as_expected()
            {
                //Arrange
                var oldContext = new RioAccessSQLEntities();
                var helper = new OldContextHelper(oldContext);
                var packagings = TestHelper.Context.PackagingProducts.Select(p => new
                    {
                        Packaging = p,
                        p.Product
                    }).ToList();
                tblPackaging tblPackaging;
                var packaging = packagings.FirstOrDefault(p => helper.GetProductFromPackagingId(p.Product.ProductCode, out tblPackaging) == null);
                if(packaging == null)
                {
                    Assert.Inconclusive("No suitable Packaging product for testing.");
                }

                var location = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).First();
                var lotDate = DateTime.Now.Date;
                var lotSequence = 42;

                var parameters = new ReceiveInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        LotType = LotTypeEnum.Additive,
                        LotDate = lotDate,
                        LotSequence = lotSequence,
                        ProductKey = new ProductKey((IProductKey)RVCUnitOfWork.AdditiveProductRepository.All().First()),
                        PackagingReceivedKey = new PackagingProductKey(packaging.Packaging),
                        Items = new List<ReceiveInventoryItemParameters>
                                {
                                    new ReceiveInventoryItemParameters
                                        {
                                            Quantity = 1,
                                            PackagingProductKey = new PackagingProductKey(packaging.Packaging),
                                            WarehouseLocationKey = new LocationKey(location),
                                            TreatmentKey = new InventoryTreatmentKey(RVCUnitOfWork.InventoryTreatmentRepository.All().First()),
                                            ToteKey = ""
                                        },
                                }
                    };

                //Act
                var result = Service.ReceiveInventory(parameters);
                result.AssertSuccess();

                var lotString = GetKeyFromConsoleString(ConsoleOutput.ReceivedInventory);
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var lotKey = LotNumberParser.ParseLotNumber(newLot);
                Assert.AreEqual((int)LotTypeEnum.Additive, lotKey.LotKey_LotTypeId);
                Assert.AreEqual(lotDate, lotKey.LotKey_DateCreated);
                Assert.AreEqual(lotSequence, lotKey.LotKey_DateSequence);

                var tblLot = oldContext.tblLots.FirstOrDefault(a => a.Lot == newLot);
                Assert.IsNotNull(tblLot);

                Assert.IsNotNull(oldContext.tblPackagings.Single(p => p.Packaging == packaging.Product.Name));
            }
        }
    }
}