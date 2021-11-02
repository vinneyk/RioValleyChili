using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.OldContextSynchronization.Utilities;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class ProductionServiceTests
    {
        [TestFixture]
        public class CreatePackScheduleUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, PackScheduleKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.CreatePackSchedule;} }
        }

        [TestFixture]
        public class UpdatePackScheduleUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, PackScheduleKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.UpdatePackSchedule; } }
        }

        [TestFixture]
        public class RemovePackScheduleUnitTests : SynchronizeOldContextUnitTestsBase<DateTime?>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.DeletePackSchedule; } }
        }

        [TestFixture]
        public class CreateProductionBatchUnitTests : SynchronizeOldContextUnitTestsBase<ICreateProductionBatchReturn>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.CreateProductionBatch; } }
        }

        [TestFixture]
        public class UpdateProductionBatchUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, LotKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.UpdateProductionBatch; } }
        }

        [TestFixture]
        public class ProductionScheduleUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, ProductionScheduleKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.ProductionSchedule; } }
        }

        [TestFixture]
        public class CreatePackScheduleIntegrationTests : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Creates_new_tblPackSch_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var workType = RVCUnitOfWork.WorkTypeRepository.All().FirstOrDefault();
                if(workType == null)
                {
                    throw new Exception("No WorkType record found.");
                }

                var chileProduct = RVCUnitOfWork.ChileProductRepository.All().FirstOrDefault();
                if(chileProduct == null)
                {
                    throw new Exception("No ChileProduct record found.");
                }

                var packagingProduct = RVCUnitOfWork.PackagingProductRepository.All().FirstOrDefault(p => p.Weight > 0.0f);
                if(packagingProduct == null)
                {
                    throw new Exception("No PackagingProduct record found.");
                }

                var productionLine = RVCUnitOfWork.LocationRepository.All().FirstOrDefault(l => l.LocationType == LocationType.ProductionLine);
                if(productionLine == null)
                {
                    throw new Exception("No ProductionLocation of Line type found.");
                }
                
                var parameters = new CreatePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        WorkTypeKey = new WorkTypeKey(workType),
                        ChileProductKey = new ChileProductKey(chileProduct),
                        PackagingProductKey = new PackagingProductKey(packagingProduct),
                        ProductionLineKey = new LocationKey(productionLine),
                        SummaryOfWork = "SynchTest",
                        ProductionDeadline = new DateTime(2020, 2, 20),
                        ScheduledProductionDate = DateTime.UtcNow,
                        BatchTargetWeight = 1000,
                        BatchTargetAsta = 1,
                        BatchTargetScan = 2,
                        BatchTargetScoville = 3
                    };

                TestHelper.ResetContext();

                //Act
                var result = Service.CreatePackSchedule(parameters);
                var packSchString = GetKeyFromConsoleString(ConsoleOutput.AddedPackSchedule);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var packSchId = SynchronizePackScheduleHelper.ToPackSchId(packSchString).Value;
                Assert.IsNotNull(new RioAccessSQLEntities().tblPackSches.FirstOrDefault(p => p.PackSchID == packSchId));
            }

            [Test]
            public void Preserves_supplied_PackSchedule_key_and_PSNum()
            {
                //Arrange
                var workType = RVCUnitOfWork.WorkTypeRepository.All().FirstOrDefault();
                if(workType == null)
                {
                    throw new Exception("No WorkType record found.");
                }

                var chileProduct = RVCUnitOfWork.ChileProductRepository.All().FirstOrDefault();
                if(chileProduct == null)
                {
                    throw new Exception("No ChileProduct record found.");
                }

                var packagingProduct = RVCUnitOfWork.PackagingProductRepository.All().FirstOrDefault(p => p.Weight > 0.0f);
                if(packagingProduct == null)
                {
                    throw new Exception("No PackagingProduct record found.");
                }

                var productionLine = RVCUnitOfWork.LocationRepository.All().FirstOrDefault(l => l.LocationType == LocationType.ProductionLine);
                if(productionLine == null)
                {
                    throw new Exception("No ProductionLocation of Line type found.");
                }
                
                var dateCreated = new DateTime(2017, 1, 1);
                var sequence = 123;
                var psNum = 312;

                var parameters = new CreatePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        WorkTypeKey = new WorkTypeKey(workType),
                        ChileProductKey = new ChileProductKey(chileProduct),
                        PackagingProductKey = new PackagingProductKey(packagingProduct),
                        ProductionLineKey = new LocationKey(productionLine),
                        SummaryOfWork = "SynchTest",
                        ProductionDeadline = new DateTime(2020, 2, 20),
                        ScheduledProductionDate = DateTime.UtcNow,
                        BatchTargetWeight = 1000,
                        BatchTargetAsta = 1,
                        BatchTargetScan = 2,
                        BatchTargetScoville = 3,

                        DateCreated = dateCreated,
                        Sequence = sequence,
                        PSNum = psNum
                    };

                TestHelper.ResetContext();

                //Act
                var result = Service.CreatePackSchedule(parameters);

                //Assert
                result.AssertSuccess();
                var packSchedule = TestHelper.Context.PackSchedules.FirstOrDefault(p => p.DateCreated == dateCreated && p.SequentialNumber == sequence);
                Assert.AreEqual(psNum, packSchedule.PSNum);
            }
        }

        [TestFixture]
        public class UpdatePackScheduleIntegrationTests : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Updates_tblPackSch_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_syncrhonization_were_successful()
            {
                //Arrange
                

                const string expectedSummary = "UpdatePackScheduleSynchTest";
                var packSchedule = RVCUnitOfWork.PackScheduleRepository.All().FirstOrDefault(p => p.SummaryOfWork != expectedSummary);
                if(packSchedule == null)
                {
                    throw new Exception("No valid test PackSchedule found.");
                }

                var parameters = new UpdatePackScheduleParameters
                    {
                        PackScheduleKey = new PackScheduleKey(packSchedule),
                        UserToken = TestUser.UserName,
                        ScheduledProductionDate = packSchedule.ScheduledProductionDate,
                        ProductionDeadline = packSchedule.ProductionDeadline,
                        ProductionLineKey = new LocationKey(packSchedule),
                        SummaryOfWork = expectedSummary,
                        ChileProductKey = new ChileProductKey(packSchedule),
                        PackagingProductKey = new PackagingProductKey(packSchedule),
                        WorkTypeKey = new WorkTypeKey(packSchedule),
                    };
                parameters.SetBatchTargetParameters(packSchedule.DefaultBatchTargetParameters);

                TestHelper.ResetContext();

                //Act
                var result = Service.UpdatePackSchedule(parameters);
                var packSchString = GetKeyFromConsoleString(ConsoleOutput.UpdatedPackSchedule);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var packSchId = SynchronizePackScheduleHelper.ToPackSchId(packSchString).Value;
                Assert.AreEqual(expectedSummary, new RioAccessSQLEntities().tblPackSches.FirstOrDefault(p => p.PackSchID == packSchId).PackSchDesc);
            }

            [Test]
            public void Updates_associated_batch_tblLot_records_Company_IA_column_as_expected()
            {
                //Arrange
                var customer = RVCUnitOfWork.CustomerRepository.Filter(c => true, c => c.Company).FirstOrDefault();
                if(customer == null)
                {
                    Assert.Inconclusive("No Customer records loaded.");
                }

                var packSchedule = RVCUnitOfWork.PackScheduleRepository
                    .Filter(p => p.ProductionBatches.Count > 2
                        && p.ProductionBatches.All(b => b.Production.PickedInventory.Items.Any())
                        && p.CustomerId != customer.Id,
                    p => p.ProductionBatches).FirstOrDefault();
                if(packSchedule == null)
                {
                    Assert.Inconclusive("No valid test PackSchedule found.");
                }

                var parameters = new UpdatePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = new PackScheduleKey(packSchedule),

                        WorkTypeKey = new WorkTypeKey(packSchedule),
                        ChileProductKey = new ChileProductKey(packSchedule),
                        PackagingProductKey = new PackagingProductKey(packSchedule),
                        ScheduledProductionDate = packSchedule.ScheduledProductionDate,
                        ProductionDeadline = packSchedule.ProductionDeadline,
                        ProductionLineKey = new LocationKey(packSchedule),
                        SummaryOfWork = packSchedule.SummaryOfWork,
                        CustomerKey = new CompanyKey(customer),
                        OrderNumber = packSchedule.OrderNumber,
                        BatchTargetWeight = packSchedule.DefaultBatchTargetParameters.BatchTargetWeight,
                        BatchTargetAsta = packSchedule.DefaultBatchTargetParameters.BatchTargetAsta,
                        BatchTargetScan = packSchedule.DefaultBatchTargetParameters.BatchTargetScan,
                        BatchTargetScoville = packSchedule.DefaultBatchTargetParameters.BatchTargetScoville,
                    };

                //Act
                var result = Service.UpdatePackSchedule(parameters);

                //Assert
                result.AssertSuccess();

                var packSchId = SynchronizePackScheduleHelper.ToPackSchId(GetKeyFromConsoleString(ConsoleOutput.UpdatedPackSchedule)).Value;
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblLots = oldContext.tblLots.Where(l => l.PackSchID == packSchId).ToList();
                    Assert.Greater(tblLots.Count, 2);
                    foreach(var lot in tblLots)
                    {
                        Assert.AreEqual(customer.Company.Name, lot.Company_IA);
                    }
                }
            }
        }

        [TestFixture]
        public class RemovePackScheduleIntegrationTests : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Removes_tblPackSch_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var packSchedules = RVCUnitOfWork.PackScheduleRepository.Filter(p =>
                    p.ScheduledItems.Any() &&
                    (!p.ProductionBatches.Any() ||
                    p.ProductionBatches.All(b =>
                        !b.ProductionHasBeenCompleted &&
                        b.Production.Results == null &&
                        b.Production.ResultingChileLot.Lot.ProductionStatus == LotProductionStatus.Batched &&
                        !b.Production.ResultingChileLot.Lot.Inventory.Any() &&
                        (
                            !b.Production.PickedInventory.Items.Any() ||
                             b.Production.PickedInventory.Items.All(i => i.CurrentLocationId == i.FromLocationId)
                        ))))
                    .ToList();
                Console.WriteLine("Valid test PackSchedules: {0}", packSchedules.Count);
                if(!packSchedules.Any())
                {
                    throw new Exception("No valid test PackSchedules found.");
                }
                var packSchedule = packSchedules.First();
                var packSchId = packSchedule.PackSchID.ToPackSchIdString();
                Console.WriteLine("Expecting to remove tblPackSch[{0}]", packSchId);

                TestHelper.ResetContext();

                //Act
                var result = Service.RemovePackSchedule(new RemovePackScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        PackScheduleKey = new PackScheduleKey(packSchedule)
                    });
                var packSchString = GetKeyFromConsoleString(ConsoleOutput.RemovedPackSchedule);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var packSchIdKey = SynchronizePackScheduleHelper.ToPackSchId(packSchString).Value;
                using(var oldContext = new RioAccessSQLEntities())
                {
                    Assert.IsNull(oldContext.tblPackSches.FirstOrDefault(p => p.PackSchID == packSchIdKey));
                    Assert.AreEqual(0, oldContext.tblBatchItems.Count(b => b.PackSchID == packSchIdKey));
                }
            }
        }

        [TestFixture]
        public class CreateProductionBatchIntegrationTests : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Creates_new_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var packSchedule = RVCUnitOfWork.PackScheduleRepository.All().Where(p => p.PSNum != null).OrderByDescending(p => p.DateCreated).FirstOrDefault();
                if(packSchedule == null)
                {
                    throw new Exception("Could not find valid test PackSchedule.");
                }

                //Act
                var param = new CreateProductionBatchParameters
                    {
                        PackScheduleKey = new PackScheduleKey(packSchedule),
                        UserToken = TestUser.UserName,
                        Instructions = new[]
                            {
                                "Instruction 0",
                                "Instruction 1",
                                "Instruction 2"
                            }
                    };
                var result = Service.CreateProductionBatch(param);
                var lotString = GetKeyFromConsoleString(ConsoleOutput.AddedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                Assert.IsNotNull(new RioAccessSQLEntities().tblLots.FirstOrDefault(t => t.Lot == newLot));
            }

            [Test]
            public void Assigns_lot_key_as_expected()
            {
                //Arrange
                var packSchedule = RVCUnitOfWork.PackScheduleRepository.All().Where(p => p.PSNum != null).OrderByDescending(p => p.DateCreated).FirstOrDefault();
                if(packSchedule == null)
                {
                    throw new Exception("Could not find valid test PackSchedule.");
                }

                //Act
                var lotType = 3;
                var lotDateCreated = new DateTime(2017, 1, 1);
                var lotSequence = 24;

                var param = new CreateProductionBatchParameters
                    {
                        PackScheduleKey = new PackScheduleKey(packSchedule),
                        UserToken = TestUser.UserName,
                        Instructions = new[]
                            {
                                "Instruction 0",
                                "Instruction 1",
                                "Instruction 2"
                            },

                        LotType = (LotTypeEnum?) lotType,
                        LotDateCreated = lotDateCreated,
                        LotSequence = lotSequence
                    };
                var result = Service.CreateProductionBatch(param);

                //Assert
                result.AssertSuccess();
                var lot = TestHelper.Context.Lots.FirstOrDefault(l => l.LotTypeId == lotType && l.LotDateCreated == lotDateCreated && l.LotDateSequence == lotSequence);
                Assert.IsNotNull(lot);
            }
        }

        [TestFixture]
        public class UpdateProductionBatchIntegrationTests : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Updates_existing_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var parameters = new UpdateProductionBatchParameters
                    {
                        UserToken = TestUser.UserName,
                        BatchTargetWeight = 1.0,
                        BatchTargetAsta = 2.0,
                        BatchTargetScan = 3.0,
                        BatchTargetScoville = 4.0,
                        Notes = "UpdateBatchNotes"
                    };

                
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.All().FirstOrDefault(b =>
                    b.TargetParameters.BatchTargetWeight != parameters.BatchTargetWeight ||
                    b.TargetParameters.BatchTargetAsta != parameters.BatchTargetAsta ||
                    b.TargetParameters.BatchTargetScan != parameters.BatchTargetScan ||
                    b.TargetParameters.BatchTargetScoville != parameters.BatchTargetScoville);
                if(productionBatch == null)
                {
                    throw new Exception("Could not find valid test ProductionBatch.");
                }
                parameters.ProductionBatchKey = new LotKey(productionBatch);

                //Act
                var result = Service.UpdateProductionBatch(parameters);
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                
                var newLot = int.Parse(lotString);
                var lot = new RioAccessSQLEntities().tblLots.FirstOrDefault(t => t.Lot == newLot);
                parameters.AssertAsExpected(lot);
            }
        }

        [TestFixture]
        public class RemoveProductionBatchIntegrationTests : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Removes_existing_tblLot_record_and_KillSwitch_will_not_have_been_enagged_if_service_method_and_syncrhonization_were_successful()
            {
                //Arrange
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.Filter(b =>
                        !b.ProductionHasBeenCompleted &&
                        b.Production.Results == null &&
                        b.Production.ResultingChileLot.Lot.ProductionStatus != LotProductionStatus.Produced &&
                        !b.Production.ResultingChileLot.Lot.Inventory.Any() &&
                        (
                            !b.Production.PickedInventory.Items.Any() ||
                             b.Production.PickedInventory.Items.All(i => i.CurrentLocationId == i.FromLocationId)
                        ))
                    .FirstOrDefault();
                if(productionBatch == null)
                {
                    throw new Exception("No valid test ProductionBatch found.");
                }

                TestHelper.ResetContext();

                //Act
                var result = Service.RemoveProductionBatch(new LotKey(productionBatch));
                var lotString = GetKeyFromConsoleString(ConsoleOutput.RemovedProductionBatch);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var lotNumber = int.Parse(lotString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    Assert.IsNull(oldContext.tblLots.FirstOrDefault(l => l.Lot == lotNumber));
                    Assert.AreEqual(0, oldContext.tblBatchItems.Count(b => b.BatchLot == lotNumber));
                    Assert.AreEqual(0, oldContext.tblBatchInstrs.Count(i => i.Lot == lotNumber));
                }
            }
        }

        [TestFixture]
        public class SetPickedInventoryForProductionBatchTests : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Creates_tblBatch_records_and_KillSwitch_will_not_have_been_engaged_on_success()
            {
                //Arrange
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.Filter(b => !b.ProductionHasBeenCompleted).FirstOrDefault();
                if(productionBatch == null)
                {
                    Assert.Inconclusive("No unproduced production batch found to pick for.");
                }

                const int pickChile = 2;
                var chileInventory = RVCUnitOfWork.InventoryRepository
                    .Filter(i => i.LotTypeId == (int)LotTypeEnum.FinishedGood && i.Quantity >= pickChile &&
                        i.Lot.QualityStatus == LotQualityStatus.Released && i.Lot.Hold == null &&
                        i.Location.Active && !i.Location.Locked && i.Location.Facility.Id == GlobalKeyHelpers.RinconFacilityKey.FacilityKey_Id &&
                        i.Lot.Attributes.Any(a => a.AttributeShortName == Constants.ChileAttributeKeys.Asta))
                    .FirstOrDefault();
                if(chileInventory == null)
                {
                    Assert.Inconclusive("No chile inventory found in an active location.");
                }

                const int pickDextrose = 3;
                var dextrose = RVCUnitOfWork.InventoryRepository.Filter(i =>
                    i.Lot.QualityStatus == LotQualityStatus.Released &&
                    i.Lot.AdditiveLot.AdditiveProduct.AdditiveTypeId == (int) SyncProductionBatchPickedInventoryHelper.Ingredient.Dextrose &&
                    i.Location.Active && !i.Location.Locked && i.Quantity >= pickDextrose)
                    .FirstOrDefault();
                if(dextrose == null)
                {
                    Assert.Inconclusive("No dextrose inventory found in an active location.");
                }

                //Act
                var pickedItems = new List<IPickedInventoryItemParameters>
                    {
                        new SetPickedInventoryItemParameters
                            {
                                InventoryKey = chileInventory.ToInventoryKey(),
                                Quantity = pickChile
                            },
                        new SetPickedInventoryItemParameters
                            {
                                InventoryKey = dextrose.ToInventoryKey(),
                                Quantity = pickDextrose
                            }
                    };
                var result = Service.SetPickedInventoryForProductionBatch(productionBatch.ToLotKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = pickedItems
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var lotNumber = int.Parse(GetKeyFromConsoleString(ConsoleOutput.SetPickedItemsForLot));
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var batchItems = oldContext.tblLots.Where(l => l.Lot == lotNumber).Select(l => l.inputBatchItems).First().ToList();
                    Assert.AreEqual(pickedItems.Count, batchItems.Count);
                    foreach(var picked in pickedItems)
                    {
                        lotNumber = LotNumberBuilder.BuildLotNumber(new InventoryKey().Parse(picked.InventoryKey)).LotNumber;
                        Assert.AreEqual(picked.Quantity, batchItems.First(b => b.Lot == lotNumber).Quantity);
                    }
                }
            }
        }

        [TestFixture]
        public class CreateProductionSchedule : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Creates_tblProductionSchedule_record_as_expected()
            {
                //Arrange
                var productionLineLocation = RVCUnitOfWork.LocationRepository.FindBy(l => l.LocationType == LocationType.ProductionLine);
                if(productionLineLocation == null)
                {
                    Assert.Inconclusive("No ProductionLine location found.");
                }

                var productionDate = RVCUnitOfWork.ProductionScheduleRepository.SourceQuery.Select(p => p.ProductionDate).DefaultIfEmpty(DateTime.Now.Date).Max().AddDays(1);

                //Act
                var result = Service.CreateProductionSchedule(new CreateProductionScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionDate = productionDate,
                        ProductionLineLocationKey = productionLineLocation.ToLocationKey()
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                string street;
                int row;
                LocationDescriptionHelper.GetStreetRow(productionLineLocation.Description, out street, out row);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var productionSchedule = oldContext.tblProductionSchedules.FirstOrDefault(p => p.ProductionDate == productionDate && (int) p.LineNumber == row);
                    Assert.AreEqual(TestUser.EmployeeId, productionSchedule.CreatedBy);
                }
            }
        }

        [TestFixture]
        public class UpdateProductionSchedule : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Updates_existing_tblProductionSchedule_record_as_expected()
            {
                //Arrange
                var productionSchedule = RVCUnitOfWork.ProductionScheduleRepository.Filter(p => p.ScheduledItems.Any(),
                        p => p.ProductionLineLocation,
                        p => p.ScheduledItems.Select(i => i.PackSchedule))
                    .OrderByDescending(p => p.ProductionDate)
                    .FirstOrDefault();
                if(productionSchedule == null)
                {
                    Assert.Inconclusive("No suitable ProductionSchedule to test.");
                }

                //Act
                var result = Service.UpdateProductionSchedule(new UpdateProductionScheduleParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionScheduleKey = productionSchedule.ToProductionScheduleKey(),
                        ScheduledItems = productionSchedule.ScheduledItems.Select((i, n) => new SetProductionScheduleItemParameters
                            {
                                Index = n,
                                FlushBeforeInstructions = "testing old context sync",
                                PackScheduleKey = i.ToPackScheduleKey()
                            }).ToList()
                    });

                //Assert
                result.AssertSuccess();
                using(var oldContext = new RioAccessSQLEntities())
                {
                    string street;
                    int row;
                    LocationDescriptionHelper.GetStreetRow(productionSchedule.ProductionLineLocation.Description, out street, out row);
                    var oldProductionSchedule = oldContext.tblProductionSchedules
                        .Include(p => p.tblProductionScheduleGroups)
                        .FirstOrDefault(p => p.ProductionDate == productionSchedule.ProductionDate && p.LineNumber == row);
                    foreach(var item in productionSchedule.ScheduledItems)
                    {
                        var oldItem = oldProductionSchedule.tblProductionScheduleGroups.FirstOrDefault(g => g.PSNum == item.PackSchedule.PSNum);
                        Assert.AreEqual("testing old context sync", oldItem.FlushBeforeInstructions);
                    }
                }
            }
        }

        [TestFixture]
        public class DeleteProductionSchedule : SynchronizeOldContextIntegrationTestsBase<ProductionService>
        {
            [Test]
            public void Deletes_tblProductionSchedule_and_associated_records()
            {
                //Arrange
                var productionSchedule = RVCUnitOfWork.ProductionScheduleRepository.Filter(p => p.ScheduledItems.Any(),
                        p => p.ProductionLineLocation)
                    .OrderByDescending(p => p.ProductionDate)
                    .FirstOrDefault();
                if(productionSchedule == null)
                {
                    Assert.Inconclusive("No suitable ProductionSchedule to test.");
                }

                //Act
                var result = Service.DeleteProductionSchedule(productionSchedule.ToProductionScheduleKey());

                //Assert
                result.AssertSuccess();
                using(var oldContext = new RioAccessSQLEntities())
                {
                    string street;
                    int row;
                    LocationDescriptionHelper.GetStreetRow(productionSchedule.ProductionLineLocation.Description, out street, out row);
                    Assert.IsEmpty(oldContext.tblProductionScheduleItems.Where(i => i.ProductionDate == productionSchedule.ProductionDate && (int)i.LineNumber == row));
                    Assert.IsEmpty(oldContext.tblProductionScheduleGroups.Where(i => i.ProductionDate == productionSchedule.ProductionDate && (int)i.LineNumber == row));
                    Assert.IsEmpty(oldContext.tblProductionSchedules.Where(i => i.ProductionDate == productionSchedule.ProductionDate && (int)i.LineNumber == row));
                }
            }
        }
    }
}