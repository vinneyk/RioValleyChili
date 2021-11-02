using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.OldContextSynchronization.Utilities;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class ProductionResultsServiceTests
    {
        [TestFixture]
        public class CreateProductionBatchResultsUnitTests : SynchronizeOldContextUnitTestsBase<IResult<ILotKey>, LotKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncProductionBatchResults; } }
        }

        [TestFixture]
        public class UpdateProductionBatchResultsUnitTests : SynchronizeOldContextUnitTestsBase<IResult<ILotKey>, LotKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncProductionBatchResults; } }
        }

        [TestFixture]
        public class CreateProductionBatchResultsIntegrationTests : SynchronizeOldContextIntegrationTestsBase<ProductionResultsService>
        {
            [Test]
            public void Creates_new_tblIncoming_records_and_KillSwitch_will_not_have_been_engaged_on_success()
            {
                //Arrange
                var unfinishedBatch = GetUnfinishedBatch();

                var productionLine = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.ProductionLine).First();
                var packaging = RVCUnitOfWork.PackagingProductRepository.All().First(p => p.Weight > 0);
                var destinations = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.Warehouse).Take(2).ToList();
                var treatment = RVCUnitOfWork.InventoryTreatmentRepository.All().First();

                const int quantity0 = 100;
                const int quantity1 = 101;
                var now = DateTime.Now;
                var parameters = new CreateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = new LotKey(unfinishedBatch),
                        ProductionLineKey = new LocationKey(productionLine),
                        ProductionShiftKey = "SHIFTTEST",
                        ProductionStartTimestamp = new DateTime(2014, 1, 1, now.Hour, now.Minute, now.Second, now.Millisecond),
                        ProductionEndTimestamp = new DateTime(2014, 1, 2, now.Hour, now.Minute, now.Second, now.Millisecond),
                        InventoryItems = new[]
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingProduct = packaging,
                                        Location = destinations[0],
                                        InventoryTreatment = treatment,
                                        Quantity = quantity0
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingProduct = packaging,
                                        Location = destinations[1],
                                        InventoryTreatment = treatment,
                                        Quantity = quantity1
                                    }
                            }
                    };

                //Act
                var result = Service.CreateProductionBatchResults(parameters);
                var lotString = GetKeyFromConsoleString(ConsoleOutput.SyncProductionResults);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                using(var context = new RioAccessSQLEntities())
                {
                    var tblLot = context.tblLots.Where(a => a.Lot == newLot).Select(l => new
                        {
                            lot = l,
                            incomings = l.tblIncomings
                        }).FirstOrDefault();
                    Assert.IsNotNull(tblLot);
                    Assert.IsNotNull(tblLot.incomings.FirstOrDefault(i => i.Quantity == quantity0));
                    Assert.IsNotNull(tblLot.incomings.FirstOrDefault(i => i.Quantity == quantity1));
                }
            }

            [Test]
            public void Creates_new_tblOutgoing_records_on_success()
            {
                //Arrange
                var unfinishedBatch = GetUnfinishedBatch();

                var productionLine = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.ProductionLine).First();
                var packaging = RVCUnitOfWork.PackagingProductRepository.All().First(p => p.Weight > 0);
                var destinations = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.Warehouse).Take(2).ToList();
                var treatment = RVCUnitOfWork.InventoryTreatmentRepository.All().First();

                const int quantity0 = 100;
                const int quantity1 = 101;
                var now = DateTime.Now;
                var parameters = new CreateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = new LotKey(unfinishedBatch),
                        ProductionLineKey = new LocationKey(productionLine),
                        ProductionShiftKey = "SHIFTTEST",
                        ProductionStartTimestamp = new DateTime(2014, 1, 1, now.Hour, now.Minute, now.Second, now.Millisecond),
                        ProductionEndTimestamp = new DateTime(2014, 1, 2, now.Hour, now.Minute, now.Second, now.Millisecond),
                        InventoryItems = new[]
                            {
                                new BatchResultItemParameters
                                    {
                                        PackagingProduct = packaging,
                                        Location = destinations[0],
                                        InventoryTreatment = treatment,
                                        Quantity = quantity0
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingProduct = packaging,
                                        Location = destinations[1],
                                        InventoryTreatment = treatment,
                                        Quantity = quantity1
                                    }
                            }
                    };

                if(destinations[0] == null || destinations[0] == destinations[1])
                {
                    Assert.Inconclusive("The InventoryItems must specify different WarehouseLocations in order to be valid for this test.");
                }

                //Act
                var result = Service.CreateProductionBatchResults(parameters);
                var lotString = GetKeyFromConsoleString(ConsoleOutput.SyncProductionResults);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                using(var context = new RioAccessSQLEntities())
                {
                    var outgoingRecords = context.tblOutgoings.Where(o => o.NewLot == newLot).ToList();
                    var x = outgoingRecords.Join(parameters.InventoryItems.Cast<BatchResultItemParameters>(),
                                                    o => o.LocID,
                                                    p => p.Location.LocID,
                                                    (o, p) => new
                                                        {
                                                            ParamQuantity = p.Quantity,
                                                            OutgoingQuantity = o.Quantity,
                                                        });
                    Assert.True(x.All(i => ((int) i.OutgoingQuantity).Equals(i.ParamQuantity)));
                }
            }

            [Test]
            public void Sets_tblLot_Produced_field_as_expected_on_success()
            {
                //Arrange
                var unfinishedBatch = GetUnfinishedBatch();

                var productionLine = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.ProductionLine).First();
                var packaging = RVCUnitOfWork.PackagingProductRepository.All().First(p => p.Weight > 0);
                var destinations = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.Warehouse).Take(2).ToList();
                var treatment = RVCUnitOfWork.InventoryTreatmentRepository.All().First();

                const int quantity0 = 100;
                const int quantity1 = 101;
                var now = DateTime.Now;
                var parameters = new CreateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = new LotKey(unfinishedBatch),
                        ProductionLineKey = new LocationKey(productionLine),
                        ProductionShiftKey = "SHIFTTEST",
                        ProductionStartTimestamp = new DateTime(2014, 1, 1, now.Hour, now.Minute, now.Second, now.Millisecond),
                        ProductionEndTimestamp = new DateTime(2014, 1, 2, now.Hour, now.Minute, now.Second, now.Millisecond),
                        InventoryItems = new[]
                            {
                                new BatchResultItemParameters 
                                    {
                                        PackagingProduct = packaging,
                                        Location = destinations[0],
                                        InventoryTreatment = treatment,
                                        Quantity = quantity0
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingProduct = packaging,
                                        Location = destinations[1],
                                        InventoryTreatment = treatment,
                                        Quantity = quantity1
                                    }
                            }
                    };

                if(destinations[0] == null || destinations[0] == destinations[1])
                {
                    Assert.Inconclusive("The InventoryItems must specify different WarehouseLocations in order to be valid for this test.");
                }

                //Act
                var result = Service.CreateProductionBatchResults(parameters);
                var lotString = GetKeyFromConsoleString(ConsoleOutput.SyncProductionResults);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                using(var context = new RioAccessSQLEntities())
                {
                    var lot = context.tblLots.First(o => o.Lot == newLot);
                    Assert.AreEqual(parameters.ProductionStartTimestamp.Date, lot.Produced);
                }
            }
            
            [Test]
            public void Sets_tblLot_LotStat_field_as_expected_on_success()
            {
                //Arrange
                var unfinishedBatch = GetUnfinishedBatch();

                var productionLine = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.ProductionLine).First();
                var packaging = RVCUnitOfWork.PackagingProductRepository.All().First(p => p.Weight > 0);
                var destinations = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.Warehouse).Take(2).ToList();
                var treatment = RVCUnitOfWork.InventoryTreatmentRepository.All().First();

                const int quantity0 = 100;
                const int quantity1 = 101;
                var now = DateTime.Now;
                var parameters = new CreateProductionBatchResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionBatchKey = new LotKey(unfinishedBatch),
                        ProductionLineKey = new LocationKey(productionLine),
                        ProductionShiftKey = "SHIFTTEST",
                        ProductionStartTimestamp = new DateTime(2014, 1, 1, now.Hour, now.Minute, now.Second, now.Millisecond),
                        ProductionEndTimestamp = new DateTime(2014, 1, 2, now.Hour, now.Minute, now.Second, now.Millisecond),
                        InventoryItems = new[]
                            {
                                new BatchResultItemParameters 
                                    {
                                        PackagingProduct = packaging,
                                        Location = destinations[0],
                                        InventoryTreatment = treatment,
                                        Quantity = quantity0
                                    },
                                new BatchResultItemParameters
                                    {
                                        PackagingProduct = packaging,
                                        Location = destinations[1],
                                        InventoryTreatment = treatment,
                                        Quantity = quantity1
                                    }
                            }
                    };

                if(destinations[0] == null || destinations[0] == destinations[1])
                {
                    Assert.Inconclusive("The InventoryItems must specify different WarehouseLocations in order to be valid for this test.");
                }

                //Act
                var result = Service.CreateProductionBatchResults(parameters);
                var lotString = GetKeyFromConsoleString(ConsoleOutput.SyncProductionResults);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                using(var context = new RioAccessSQLEntities())
                {
                    var lot = context.tblLots.First(o => o.Lot == newLot);
                    Assert.AreEqual(0, lot.LotStat);
                }
            }

            private ProductionBatch GetUnfinishedBatch()
            {
                ProductionBatch unfinishedBatch;
                using(var oldContext = new RioAccessSQLEntities())
                {
                    unfinishedBatch = RVCUnitOfWork.ProductionBatchRepository
                        .Filter(b => b.Production.Results == null)
                        .ToList()
                        .FirstOrDefault(b =>
                        {
                            var lot = LotNumberBuilder.BuildLotNumber(b);
                            return !oldContext.tblLots.Any(l => l.Lot == lot);
                        });
                }
                if(unfinishedBatch == null)
                {
                    Assert.Inconclusive("No ProductionBatch with null Results found.");
                }
                return unfinishedBatch;
            }
        }

        [TestFixture]
        public class UpdateProductionResultsIntegrationTests : SynchronizeOldContextIntegrationTestsBase<ProductionResultsService>
        {
            [Test]
            public void Updates_tblLot_production_result_data_and_KillSwitch_will_not_have_been_engaged_on_success()
            {
                //Arrange
                var productionResult = RVCUnitOfWork.LotProductionResultsRepository.Filter(r => r.Production.ProductionType == ProductionType.ProductionBatch &&
                    r.Production.ResultingChileLot.Lot.Inventory.Any(i => i.Quantity > 0), r => r.ResultItems).First();
                var productionLine = RVCUnitOfWork.LocationRepository.Filter(l => l.LocationType == LocationType.ProductionLine && l != productionResult.ProductionLineLocation).First();

                var startTimestamp = new DateTime(2014, 1, 1, 2, 3, 45, 670);
                var parameters = new UpdateProductionResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = new LotKey(productionResult),
                        ProductionShiftKey = "TestShiftKey",
                        ProductionLine = productionLine,
                        ProductionStartTimestamp = startTimestamp,
                        ProductionEndTimestamp = startTimestamp.AddDays(1),
                        InventoryItems = productionResult.ResultItems.Select(i => new BatchResultItemParameters
                            {
                                PackagingKey = new PackagingProductKey(i),
                                LocationKey = new LocationKey(i),
                                InventoryTreatmentKey = new InventoryTreatmentKey(i),
                                Quantity = i.Quantity + 1
                            }).ToList()
                    };

                //Act
                var result = Service.UpdateProductionBatchResults(parameters);
                result.AssertSuccess();
                var lotString = GetKeyFromConsoleString(ConsoleOutput.SyncProductionResults);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                using(var context = new RioAccessSQLEntities())
                {
                    var tblLot = context.tblLots.First(l => l.Lot == newLot);
                    Assert.AreEqual(parameters.ProductionShiftKey, tblLot.Shift);
                    Assert.AreEqual(ProductionLineParser.GetProductionLineNumber(parameters.ProductionLine), tblLot.ProductionLine);
                    parameters.ProductionStartTimestamp.AssertUTCSameAsMST(tblLot.BatchBegTime.Value);
                    parameters.ProductionEndTimestamp.AssertUTCSameAsMST(tblLot.BatchEndTime.Value);
                }
            }

            [Test]
            public void Updates_tblBatchItems_and_KillSwitch_will_not_have_been_engaged_on_success()
            {
                //Arrange
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository
                    .Filter(b =>
                        b.Production.ResultingChileLot.Lot.Inventory.Any(i => i.Quantity > 0) && b.Production.PickedInventory.Items.Any() &&
                        b.Production.PickedInventory.Items.All(i => i.Quantity > 2) &&
                        b.Production.PickedInventory.Items.Distinct().Count() == b.Production.PickedInventory.Items.Count(),
                        b => b.Production.PickedInventory.Items,
                        b => b.Production.Results.ResultItems)
                    .FirstOrDefault();
                if(productionBatch == null)
                {
                    Assert.Inconclusive("Could not find suitable ProductionBatch for testing.");
                }
                
                var pickedModifications = productionBatch.Production.PickedInventory.Items.Select(i => new 
                    {
                        ExpectedQuantity = i.Quantity - 1,
                        Parameter = new SetPickedInventoryItemParameters
                            {
                                InventoryKey = new InventoryKey(i),
                                Quantity = -1
                            }
                    }).ToList();
                var productionLine = RVCUnitOfWork.LocationRepository
                    .Filter(l => l.LocationType == LocationType.ProductionLine && l != productionBatch.Production.Results.ProductionLineLocation).First();

                var startTimestamp = new DateTime(2014, 1, 1, 2, 3, 45, 670);
                var parameters = new UpdateProductionResultsParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductionResultKey = new LotKey(productionBatch),
                        ProductionShiftKey = "TestShiftKey",
                        ProductionLine = productionLine,
                        ProductionStartTimestamp = startTimestamp,
                        ProductionEndTimestamp = startTimestamp.AddDays(1),
                        InventoryItems = productionBatch.Production.Results.ResultItems.Select(i => new BatchResultItemParameters
                            {
                                PackagingKey = new PackagingProductKey(i),
                                LocationKey = new LocationKey(i),
                                InventoryTreatmentKey = new InventoryTreatmentKey(i),
                                Quantity = i.Quantity + 1
                            }).ToList(),
                        PickedInventoryItemChanges = pickedModifications.Select(m => m.Parameter).ToList()
                    };

                //Act
                var result = Service.UpdateProductionBatchResults(parameters);
                result.AssertSuccess();
                var lotString = GetKeyFromConsoleString(ConsoleOutput.SyncProductionResults);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                using(var context = new RioAccessSQLEntities())
                {
                    var lotSelect = context.tblLots.Where(l => l.Lot == newLot)
                        .Select(l => new
                            {
                                l,
                                l.inputBatchItems
                            }).First();
                    var tblLot = lotSelect.l;

                    Assert.AreEqual(parameters.ProductionShiftKey, tblLot.Shift);
                    Assert.AreEqual(ProductionLineParser.GetProductionLineNumber(parameters.ProductionLine), tblLot.ProductionLine);
                    parameters.ProductionStartTimestamp.AssertUTCSameAsMST(tblLot.BatchBegTime.Value);
                    parameters.ProductionEndTimestamp.AssertUTCSameAsMST(tblLot.BatchEndTime.Value);

                    var batchItems = tblLot.inputBatchItems.ToList();
                    Assert.AreEqual(pickedModifications.Count, batchItems.Count);
                    foreach(var item in pickedModifications)
                    {
                        var batchItem = batchItems.Single(i => i.Lot == LotNumberBuilder.BuildLotNumber(new InventoryKey().Parse(item.Parameter.InventoryKey)));
                        Assert.AreEqual(item.ExpectedQuantity, batchItem.Quantity);
                    }
                }
            }
        }
    }
}