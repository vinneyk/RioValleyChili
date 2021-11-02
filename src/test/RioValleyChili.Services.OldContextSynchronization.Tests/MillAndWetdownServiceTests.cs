using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.OldContextSynchronization.Utilities;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class MillAndWetdownServiceTests
    {
        [TestFixture]
        public class SyncMillAndWetdownUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, LotKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncMillAndWetdown; } }
        }

        [TestFixture]
        public class CreateMillAndWetdownIntegrationTests : SynchronizeOldContextIntegrationTestsBase<MillAndWetdownService>
        {
            [Test]
            public void Creates_new_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var productionDate = new DateTime(2020, 3, 29);
                int? newSequence = null;
                while(newSequence == null)
                {
                    var existingLots = RVCUnitOfWork.ChileLotRepository.Filter(l => l.LotDateCreated == productionDate && l.LotTypeId == (int) LotTypeEnum.WIP);
                    var sequence = existingLots.Any() ? existingLots.Max(l => l.LotDateSequence) : 0;
                    if(sequence < 99)
                    {
                        newSequence = sequence + 1;
                    }
                    else
                    {
                        productionDate = productionDate.AddDays(1);
                    }
                }

                const int pickedQuantity = 23;
                var chileProduct = RVCUnitOfWork.ChileProductRepository.Filter(c => c.ChileState == ChileStateEnum.WIP).First();
                var productionLine = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.ProductionLine).First();
                var pickedInventory = RVCUnitOfWork.InventoryRepository.Filter(i => i.Lot.Hold == null && i.Lot.QualityStatus == LotQualityStatus.Released &&  i.Quantity > pickedQuantity && i.Location.Facility.Name.Contains("rincon")).First();
                var warehouseLocation = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).First();
                var packagingProduct = RVCUnitOfWork.PackScheduleRepository.All().First();

                var parameters = new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,

                        ProductionDate = productionDate,
                        LotSequence = newSequence.Value,
                        ChileProductKey = chileProduct.ToChileProductKey(),

                        ShiftKey = "SHIFTY",
                        ProductionLineKey = productionLine.ToLocationKey(),
                        ProductionBegin = productionDate.AddDays(-1),
                        ProductionEnd = productionDate.AddHours(12),
                        PickedItems = new[]
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = pickedInventory.ToInventoryKey(),
                                        Quantity = pickedQuantity
                                    }
                            },
                        ResultItems = new[]
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        PackagingProductKey = packagingProduct.ToPackagingProductKey(),
                                        LocationKey = warehouseLocation.ToLocationKey(),
                                        Quantity = 10
                                    }
                            }
                    };

                TestHelper.ResetContext();

                //Act
                var result = Service.CreateMillAndWetdown(parameters);
                result.AssertSuccess();
                var lotString = GetKeyFromConsoleString(ConsoleOutput.AddedLot);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                Assert.IsNotNull(new RioAccessSQLEntities().tblLots.FirstOrDefault(t => t.Lot == newLot));
            }
        }

        [TestFixture]
        public class UpdateMillAndWetdownIntegrationTests : SynchronizeOldContextIntegrationTestsBase<MillAndWetdownService>
        {
            [Test]
            public void Updates_existing_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var production = RVCUnitOfWork.ChileLotProductionRepository.FindBy(c => c.ProductionType == ProductionType.MillAndWetdown && c.Results.ShiftKey != "UpdateTest" &&
                    c.Results.ResultItems.All(i => i.ProductionResults.Production.ResultingChileLot.Lot.Inventory.Any(n => n.PackagingProductId == i.PackagingProductId && n.LocationId == i.LocationId && n.Quantity >= i.Quantity)),
                    c => c.ResultingChileLot);
                if(production == null)
                {
                    Assert.Inconclusive("No suitable Mill and Wetdown record found for testing.");
                }

                var productionDate = DateTime.Now.Date;
                var productionLine = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.ProductionLine).First();
                var pickedInventory = RVCUnitOfWork.InventoryRepository.Filter(i => i.Lot.Hold == null && i.Lot.QualityStatus == LotQualityStatus.Released && i.Location.Facility.Name.Contains("rincon")).First();
                var warehouseLocation = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).First();
                var packagingProduct = RVCUnitOfWork.PackScheduleRepository.All().First();

                //Act
                var result = Service.UpdateMillAndWetdown(new UpdateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = production.ToLotKey(),
                        ChileProductKey = production.ResultingChileLot.ToChileProductKey(),
                        ShiftKey = "UpdateTest",
                        ProductionLineKey = productionLine.ToLocationKey(),
                        ProductionBegin = productionDate.AddDays(-1),
                        ProductionEnd = productionDate.AddHours(12),
                        PickedItems = new[]
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = pickedInventory.ToInventoryKey(),
                                        Quantity = pickedInventory.Quantity
                                    }
                            },
                        ResultItems = new[]
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        PackagingProductKey = packagingProduct.ToPackagingProductKey(),
                                        LocationKey = warehouseLocation.ToLocationKey(),
                                        Quantity = 10
                                    }
                            }
                    });
                result.AssertSuccess();
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                using(var context = new RioAccessSQLEntities())
                {
                    var lot = int.Parse(lotString);
                    Assert.IsNotNull(context.tblLots.FirstOrDefault(t => t.Lot == lot));
                }
            }

            [Test]
            public void Updates_tblLot_product_reference()
            {
                //Arrange
                var pickedInventoryItems = RVCUnitOfWork.PickedInventoryItemRepository.All();
                var production = RVCUnitOfWork.ChileLotProductionRepository.Filter(p => p.ProductionType == ProductionType.MillAndWetdown &&
                    p.PickedInventory.Items.Any() && p.Results.ResultItems.Any() &&
                    !pickedInventoryItems.Any(i => i.LotDateCreated == p.LotDateCreated && i.LotDateSequence == p.LotDateSequence && i.LotTypeId == p.LotTypeId),
                    c => c.ResultingChileLot.ChileProduct,
                    c => c.PickedInventory.Items,
                    c => c.Results.ResultItems)
                    .FirstOrDefault();
                if(production == null)
                {
                    Assert.Inconclusive("No suitable Mill and Wetdown record found for testing.");
                }

                var chileProduct = RVCUnitOfWork.ChileProductRepository
                    .Filter(p => p.ChileState == production.ResultingChileLot.ChileProduct.ChileState && p.Id != production.ResultingChileLot.ChileProductId,
                        c => c.Product)
                    .FirstOrDefault();
                if(chileProduct == null)
                {
                    Assert.Inconclusive("No suitable Chile Product record found for testing.");
                }
                //Act
                var result = Service.UpdateMillAndWetdown(new UpdateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = production.ToLotKey(),
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        ShiftKey = "UpdateTest",
                        ProductionLineKey = production.Results.ToLocationKey(),
                        ProductionBegin = production.Results.ProductionBegin,
                        ProductionEnd = production.Results.ProductionEnd,
                        PickedItems = production.PickedInventory.Items.Select(i => new MillAndWetdownPickedItemParameters
                            {
                                InventoryKey = i.ToInventoryKey(),
                                Quantity = i.Quantity
                            }).ToArray(),
                        ResultItems = production.Results.ResultItems.Select(i => new MillAndWetdownResultItemParameters
                            {
                                PackagingProductKey = i.ToPackagingProductKey(),
                                LocationKey = i.ToLocationKey(),
                                Quantity = i.Quantity
                            }).ToArray()
                    });
                result.AssertSuccess();
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                using(var context = new RioAccessSQLEntities())
                {
                    var product = new OldContextHelper(context).GetProduct(chileProduct);
                    var lot = int.Parse(lotString);
                    Assert.AreEqual(product.ProdID, context.tblLots.FirstOrDefault(t => t.Lot == lot).ProdID);
                }
            }
        }

        [TestFixture]
        public class DeleteMillAndwetdownIntegrationTests : SynchronizeOldContextIntegrationTestsBase<MillAndWetdownService>
        {
            [Test]
            public void Deletes_tblLot_and_associated_records()
            {
                //Arrange
                var productionDate = new DateTime(2020, 3, 29);
                int? newSequence = null;
                while(newSequence == null)
                {
                    var existingLots = RVCUnitOfWork.ChileLotRepository.Filter(l => l.LotDateCreated == productionDate && l.LotTypeId == (int) LotTypeEnum.WIP);
                    var sequence = existingLots.Select(l => l.LotDateSequence).DefaultIfEmpty(0).Max();
                    if(sequence < 99)
                    {
                        newSequence = sequence + 1;
                    }
                    else
                    {
                        productionDate = productionDate.AddDays(1);
                    }
                }

                const int pickedQuantity = 10;
                var chileProduct = RVCUnitOfWork.ChileProductRepository.Filter(c => c.ChileState == ChileStateEnum.WIP).First();
                var productionLine = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.ProductionLine).First();
                var pickedInventory = RVCUnitOfWork.InventoryRepository.Filter(i => i.Lot.Hold == null && i.Lot.QualityStatus == LotQualityStatus.Released &&  i.Quantity > pickedQuantity && i.Location.Facility.Name.Contains("rincon")).First();
                var warehouseLocation = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).First();
                var packagingProduct = RVCUnitOfWork.PackScheduleRepository.All().First();

                var createResult = Service.CreateMillAndWetdown(new CreateMillAndWetdownParameters
                    {
                        UserToken = TestUser.UserName,

                        ProductionDate = productionDate,
                        LotSequence = newSequence.Value,
                        ChileProductKey = chileProduct.ToChileProductKey(),

                        ShiftKey = "SHIFTY",
                        ProductionLineKey = productionLine.ToLocationKey(),
                        ProductionBegin = productionDate.AddDays(-1),
                        ProductionEnd = productionDate.AddHours(12),
                        PickedItems = new[]
                            {
                                new MillAndWetdownPickedItemParameters
                                    {
                                        InventoryKey = pickedInventory.ToInventoryKey(),
                                        Quantity = pickedQuantity
                                    }
                            },
                        ResultItems = new[]
                            {
                                new MillAndWetdownResultItemParameters
                                    {
                                        PackagingProductKey = packagingProduct.ToPackagingProductKey(),
                                        LocationKey = warehouseLocation.ToLocationKey(),
                                        Quantity = 10
                                    }
                            }
                    });
                createResult.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = LotNumberBuilder.BuildLotNumber(KeyParserHelper.ParseResult<ILotKey>(createResult.ResultingObject).ResultingObject).LotNumber;
                
                Assert.IsNotNull(new RioAccessSQLEntities().tblLots.FirstOrDefault(t => t.Lot == newLot));

                //Act
                var deleteResult = Service.DeleteMillAndWetdown(createResult.ResultingObject);

                //Assert
                deleteResult.AssertSuccess();
                var lotString = GetKeyFromConsoleString(ConsoleOutput.DeletedTblLot);

                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                newLot = int.Parse(lotString);
                Assert.IsNull(new RioAccessSQLEntities().tblLots.FirstOrDefault(t => t.Lot == newLot));
            }
        }
    }
}