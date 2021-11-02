using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class LotServiceTests
    {
        [TestFixture]
        public class SynchronizeLotUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, SynchronizeLotParameters>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncLot; } }
        }

        [TestFixture]
        public class SetLotAttributesIntegrationTests : SynchronizeOldContextIntegrationTestsBase<LotService>
        {
            [Test]
            public void Updates_existing_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c => !c.Lot.LotDefects.Any() && c.ChileProduct.ProductAttributeRanges.Count() > 1,
                    c => c.ChileProduct.ProductAttributeRanges.Select(r => r.AttributeName)).FirstOrDefault();
                if(chileLot == null)
                {
                    throw new Exception("Could not find ChileLot with no defects.");
                }

                var attributes = StaticAttributeNames.AttributeNames.Select(n => new
                        {
                            nameKey = n.ToAttributeNameKey(),
                            value = chileLot.ChileProduct.ProductAttributeRanges
                                .Where(r => r.AttributeShortName == n.ShortName)
                                .Select(r => r.AttributeName.DefectType == DefectTypeEnum.BacterialContamination ? r.RangeMin : ((r.RangeMax * 2) + 1))
                                .DefaultIfEmpty(0)
                                .First()
                        }).ToArray();
                var dateTested = new DateTime(2029, 3, 30);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Attributes = attributes.ToDictionary(a => a.nameKey.KeyValue, a => new AttributeValueParameters
                            {
                                AttributeInfo = new AttributeInfoParameters
                                    {
                                        Value = a.value,
                                        Date = dateTested
                                    },
                            } as IAttributeValueParameters)
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                foreach(var attribute in attributes)
                {
                    var getter = tblLot.AttributeGet(attribute.nameKey);
                    if(getter != null)
                    {
                        Assert.AreEqual((decimal?)attribute.value, getter());
                    }
                }
            }

            [Test]
            public void tblLot_record_LotStatus_will_not_have_been_set_to_Completed_if_override_parameter_is_set_to_false_and_Lot_is_not_completed()
            {
                //Arrange
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c => c.ChileProduct.ProductAttributeRanges.Count() > 1,
                    c => c.Lot.Attributes,
                    c => c.ChileProduct.ProductAttributeRanges).FirstOrDefault();
                if(chileLot == null)
                {
                    throw new Exception("Could not find ChileLot with no defects.");
                }

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        OverrideOldContextLotAsCompleted = false,
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(chileLot),
                        Attributes = chileLot.ChileProduct.ProductAttributeRanges.ToDictionary(a => new AttributeNameKey(a).KeyValue, a => new AttributeValueParameters
                            {
                                Resolution = new DefectResolutionParameters
                                    {
                                        ResolutionType = ResolutionTypeEnum.DataEntryCorrection,
                                        Description = "tee-hee"
                                    }
                            } as IAttributeValueParameters)
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                Assert.AreNotEqual((int?) LotStat.Completed, tblLot.LotStat);
            }

            [Test]
            public void tblLot_record_LotStatus_will_have_been_set_to_Completed_if_override_parameter_is_set_to_true_even_though_Lot_is_not_completed()
            {
                //Arrange
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c => c.ChileProduct.ProductAttributeRanges.Count() > 1,
                    c => c.Lot.Attributes,
                    c => c.ChileProduct.ProductAttributeRanges).FirstOrDefault();
                if(chileLot == null)
                {
                    throw new Exception("Could not find ChileLot with no defects.");
                }

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        OverrideOldContextLotAsCompleted = true,
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(chileLot),
                        Attributes = chileLot.ChileProduct.ProductAttributeRanges.ToDictionary(a => new AttributeNameKey(a).KeyValue, a => new AttributeValueParameters
                            {
                                Resolution = new DefectResolutionParameters
                                    {
                                        ResolutionType = ResolutionTypeEnum.DataEntryCorrection,
                                        Description = "tee-hee"
                                    }
                            } as IAttributeValueParameters)
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                Assert.AreEqual((int?) LotStat.Completed, tblLot.LotStat);
            }

            [Test, Issue("Null reference being caused in lot sync procedure when attempting to determine if history record should be created and accessing values" +
                         "that are actually null. Note that test is only valid if resulting lot stat is equal to that of the latest history record in the sync. -RI 2016/8/22",
                         References = new[] { "RVCADMIN-1232" })]
            public void Missing_attribute_values_will_not_cause_sync_to_fail()
            {
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c => !c.Lot.Attributes.Any(),
                    c => c.Lot.Attributes).FirstOrDefault();
                if(chileLot == null)
                {
                    throw new Exception("Could not find ChileLot.");
                }

                var attributes = chileLot.Lot.Attributes.Where((a, n) => n > 0).ToArray();

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Attributes = attributes.ToDictionary(a => a.ToAttributeNameKey().KeyValue, a => new AttributeValueParameters
                            {
                                AttributeInfo = new AttributeInfoParameters
                                    {
                                        Value = a.AttributeValue,
                                        Date = a.AttributeDate
                                    }
                            } as IAttributeValueParameters)
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                foreach(var attribute in attributes)
                {
                    var getter = tblLot.AttributeGet(attribute);
                    if(getter != null)
                    {
                        Assert.AreEqual((decimal?)attribute.AttributeValue, getter());
                    }
                }
            }

            [Test, Issue("Reported that resolved defects are still being taken into account in the synch when they shouldn't be. -RI 2016-08-22",
                References = new[] { "RVCADMIN-1249" })]
            public void Resolved_defect_will_not_be_taken_into_account_for_LotStat_evaluation()
            {
                //Arrange
                tblLot oldLot;
                using(var oldContext = new RioAccessSQLEntities())
                {
                    oldLot = oldContext.tblLots.OrderByDescending(l => l.EntryDate).FirstOrDefault(l => l.LotStat == (int) LotStat.Asta);
                }

                if(oldLot == null)
                {
                    Assert.Inconclusive("Could not find valid tblLot record for testing.");
                }

                var lotKey = LotNumberParser.ParseLotNumber(oldLot.Lot);
                var newLot = RVCUnitOfWork.LotRepository.FindByKey(lotKey, l => l.Attributes);

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = lotKey,
                        Attributes = newLot.Attributes.ToDictionary(a => a.ToAttributeNameKey().KeyValue, a => new AttributeValueParameters
                            {
                                AttributeInfo = new AttributeInfoParameters
                                    {
                                        Value = a.AttributeValue,
                                        Date = a.AttributeDate
                                    },
                                Resolution = new DefectResolutionParameters
                                    {
                                        Description = "Test!",
                                        ResolutionType = ResolutionTypeEnum.AcceptedByUser
                                    }
                            } as IAttributeValueParameters)
                    });
                
                //Assert
                result.AssertSuccess();
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var synchedLot = oldContext.tblLots.FirstOrDefault(l => l.Lot == oldLot.Lot);
                    Assert.AreNotEqual((int)LotStat.Asta, synchedLot.LotStat);
                }
            }

            [Test, Issue("Client has specified need to enter attribute data for AdditiveLots in order to have AdditiveLots affect produced Lot computed attribute values. -RI 2016-12-6",
                References = new[] { "RVCADMIN-1412" })]
            public void Set_LotAttributes_for_AdditiveLot()
            {
                //Arrange
                var additiveLot = RVCUnitOfWork.AdditiveLotRepository.Filter(l => true, l => l.Lot.Attributes)
                    .OrderByDescending(a => a.LotDateCreated).FirstOrDefault();
                if(additiveLot == null)
                {
                    Assert.Inconclusive("AdditiveLot for testing not found.");
                }
                var expectedScan = additiveLot.Lot.Attributes.Where(a => a.AttributeShortName == StaticAttributeNames.Scan.ShortName).Select(a => a.AttributeValue).DefaultIfEmpty(0).Max() + 1;

                //Act
                var result = Service.SetLotAttributes(new SetLotAttributeParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = additiveLot.ToLotKey(),
                        Attributes = new Dictionary<string, IAttributeValueParameters>
                            {
                                {
                                    StaticAttributeNames.Scan.ToAttributeNameKey(), new AttributeValueParameters
                                        {
                                            AttributeInfo = new AttributeInfoParameters
                                                {
                                                    Value = expectedScan,
                                                    Date = additiveLot.LotDateCreated.AddDays(1)
                                                }
                                        }
                                }
                            }
                    });

                //Assert
                result.AssertSuccess();
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var lotNumber = LotNumberBuilder.BuildLotNumber(additiveLot);
                    var lot = oldContext.tblLots.FirstOrDefault(l => l.Lot == lotNumber);
                    Assert.AreEqual(expectedScan, lot.Scan);
                }
            }
        }

        [TestFixture]
        public class AddLotAttributesIntegrationTests : SynchronizeOldContextIntegrationTestsBase<LotService>
        {
            [Test]
            public void Updates_existing_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var chileLots = RVCUnitOfWork.ChileLotRepository.Filter(c => !c.Lot.LotDefects.Any() && c.ChileProduct.ProductAttributeRanges.Count() > 1,
                    c => c.ChileProduct.ProductAttributeRanges)
                    .OrderByDescending(c => c.LotDateCreated)
                    .Take(3)
                    .ToList();
                if(chileLots.Count < 3)
                {
                    throw new Exception("Could not find ChileLots with no defects.");
                }

                var attributes = StaticAttributeNames.AttributeNames.Select(n => new
                    {
                        nameKey = new AttributeNameKey(n),
                        value = chileLots[0].ChileProduct.ProductAttributeRanges
                            .Where(r => r.AttributeShortName == n.ShortName)
                            .Select(r => (r.RangeMax * 2) + 1)
                            .DefaultIfEmpty(0)
                            .First()
                    }).ToArray();
                var dateTested = new DateTime(2029, 3, 30);

                //Act
                var result = Service.AddLotAttributes(new AddLotAttributesParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKeys = chileLots.Select(c => c.ToLotKey().KeyValue).ToArray(),
                        Attributes = attributes.ToDictionary(a => a.nameKey.KeyValue, a => new AttributeValueParameters
                            {
                                AttributeInfo = new AttributeInfoParameters
                                    {
                                        Value = a.value,
                                        Date = dateTested
                                    },
                            } as IAttributeValueParameters)
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                foreach(var lot in lotString.Split(new [] { ", " }, StringSplitOptions.None))
                {
                    var newLot = int.Parse(lot);
                    var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                    foreach(var attribute in attributes)
                    {
                        var getter = tblLot.AttributeGet(attribute.nameKey);
                        if(getter != null)
                        {
                            Assert.AreEqual((decimal?) attribute.value, getter());
                        }
                    }
                }
            }

            [Test]
            public void tblLot_record_LotStatus_will_not_have_been_set_to_Completed_if_override_parameter_is_set_to_false_and_Lot_is_not_completed()
            {
                //Arrange
                var chileLots = RVCUnitOfWork.ChileLotRepository.Filter(c => !c.Lot.LotDefects.Any() && c.ChileProduct.ProductAttributeRanges.Count() > 1,
                    c => c.ChileProduct.ProductAttributeRanges)
                    .OrderByDescending(c => c.LotDateCreated)
                    .Take(3)
                    .ToList();
                if(chileLots.Count < 3)
                {
                    throw new Exception("Could not find ChileLots with no defects.");
                }

                //Act
                var result = Service.AddLotAttributes(new AddLotAttributesParameters
                    {
                        OverrideOldContextLotAsCompleted = false,
                        UserToken = TestUser.UserName,
                        LotKeys = chileLots.Select(l => l.ToLotKey().KeyValue).ToArray(),
                        Attributes = chileLots[0].ChileProduct.ProductAttributeRanges.ToDictionary(a => new AttributeNameKey(a).KeyValue, a => new AttributeValueParameters
                            {
                                Resolution = new DefectResolutionParameters
                                    {
                                        ResolutionType = ResolutionTypeEnum.DataEntryCorrection,
                                        Description = "tee-hee"
                                    }
                            } as IAttributeValueParameters)
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                foreach(var lot in lotString.Split(new[] {", "}, StringSplitOptions.None))
                {
                    var newLot = int.Parse(lot);
                    var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                    Assert.AreNotEqual((int?) LotStat.Completed, tblLot.LotStat);
                }
            }

            [Test]
            public void tblLot_record_LotStatus_will_have_been_set_to_Completed_if_override_parameter_is_set_to_true_even_though_Lot_is_not_completed()
            {
                //Arrange
                var chileLots = RVCUnitOfWork.ChileLotRepository.Filter(c => !c.Lot.LotDefects.Any() && c.ChileProduct.ProductAttributeRanges.Count() > 1,
                    c => c.ChileProduct.ProductAttributeRanges)
                    .OrderByDescending(c => c.LotDateCreated)
                    .Take(3)
                    .ToList();
                if(chileLots.Count < 3)
                {
                    throw new Exception("Could not find ChileLots with no defects.");
                }

                //Act
                var result = Service.AddLotAttributes(new AddLotAttributesParameters
                    {
                        OverrideOldContextLotAsCompleted = true,
                        UserToken = TestUser.UserName,
                        LotKeys = chileLots.Select(l => l.ToLotKey().KeyValue).ToArray(),
                        Attributes = chileLots[0].ChileProduct.ProductAttributeRanges.ToDictionary(a => new AttributeNameKey(a).KeyValue, a => new AttributeValueParameters
                            {
                                Resolution = new DefectResolutionParameters
                                    {
                                        ResolutionType = ResolutionTypeEnum.DataEntryCorrection,
                                        Description = "tee-hee"
                                    }
                            } as IAttributeValueParameters)
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                foreach(var lot in lotString.Split(new[] {", "}, StringSplitOptions.None))
                {
                    var newLot = int.Parse(lot);
                    var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                    Assert.AreEqual((int?) LotStat.Completed, tblLot.LotStat);
                }
            }
        }

        [TestFixture]
        public class CreateLotDefect : SynchronizeOldContextIntegrationTestsBase<LotService>
        {
            [Test]
            public void Updates_existing_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                

                var attributeLotStats = StaticAttributeNames.GetAttributesWithAssociatedLotStats().Select(a => a.AttributeNameKey_ShortName).ToArray();
                var lotAttributeDefects = RVCUnitOfWork.LotAttributeDefectRepository.All();
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c =>
                    c.Lot.Hold == null &&
                    c.Lot.QualityStatus == LotQualityStatus.Released &&
                    c.Lot.ProductSpecOutOfRange &&
                    lotAttributeDefects.Where(d => d.LotDateCreated == c.LotDateCreated && d.LotDateSequence == c.LotDateSequence && d.LotTypeId == c.LotTypeId && d.LotDefect.Resolution == null)
                        .All(d => attributeLotStats.All(a => a != d.AttributeShortName))
                    ).FirstOrDefault();
                if(chileLot == null)
                {
                    throw new Exception("Could not find ChileLot valid for testing.");
                }

                //Act
                var result = Service.CreateLotDefect(new CreateLotDefectParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(chileLot),
                        DefectType = DefectTypeEnum.InHouseContamination,
                        Description = "dark specs"
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                Assert.AreEqual((int?)LotStat.Dark_Specs, tblLot.LotStat);
            }

            [Test]
            public void Will_create_new_tblLotHistory_record_if_new_defect_results_in_a_LotStat_change()
            {
                //Arrange
                var attributeLotStats = StaticAttributeNames.GetAttributesWithAssociatedLotStats().Select(a => a.AttributeNameKey_ShortName).ToArray();
                var lotAttributeDefects = RVCUnitOfWork.LotAttributeDefectRepository.All();
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c =>
                    c.Lot.Hold == null &&
                    c.Lot.QualityStatus == LotQualityStatus.Released &&
                    c.Lot.ProductSpecOutOfRange &&
                    lotAttributeDefects.Where(d => d.LotDateCreated == c.LotDateCreated && d.LotDateSequence == c.LotDateSequence && d.LotTypeId == c.LotTypeId && d.LotDefect.Resolution == null)
                        .All(d => attributeLotStats.All(a => a != d.AttributeShortName))
                    ).FirstOrDefault();
                if(chileLot == null)
                {
                    Assert.Inconclusive("Could not find ChileLot valid for testing.");
                }
                var lotNumber = LotNumberBuilder.BuildLotNumber(chileLot);
                var expectedLotAttributeHistories = new RioAccessSQLEntities().tblLotAttributeHistories.Count(h => h.Lot == lotNumber) + 1;

                //Act
                var result = Service.CreateLotDefect(new CreateLotDefectParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(chileLot),
                        DefectType = DefectTypeEnum.InHouseContamination,
                        Description = "dark specs"
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                Assert.AreEqual(lotNumber, int.Parse(lotString));
                Assert.AreEqual(expectedLotAttributeHistories, new RioAccessSQLEntities().tblLotAttributeHistories.Count(h => h.Lot == lotNumber));
            }

            [Test]
            public void Will_not_create_new_tblLotHistory_record_if_new_defect_does_not_result_in_a_LotStat_change()
            {
                //Arrange
                ChileLot chileLot;
                using(var oldContext = new RioAccessSQLEntities())
                {
                    chileLot = oldContext.tblLots
                        .Where(l => l.LotStat == (int) LotStat.See_Desc)
                        .Select(l => l.Lot)
                        .ToList()
                        .Select(l =>
                            {
                                var lotKey = LotNumberParser.ParseLotNumber(l);
                                var lot = RVCUnitOfWork.ChileLotRepository.FindByKey(lotKey, c => c.Lot.LotDefects.Select(d => d.Resolution));
                                return lot.Lot.QualityStatus == LotQualityStatus.Released ? lot : null;
                            })
                        .FirstOrDefault(c => c != null);
                }
                
                if(chileLot == null)
                {
                    throw new Exception("Could not find test ChileLot.");
                }
                var lotNumber = LotNumberBuilder.BuildLotNumber(chileLot);
                var expectedLotAttributeHistories = new RioAccessSQLEntities().tblLotAttributeHistories.Count(h => h.Lot == lotNumber);

                //Act
                var result = Service.CreateLotDefect(new CreateLotDefectParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        DefectType = DefectTypeEnum.InHouseContamination,
                        Description = "unmapped defect"
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                
                Assert.AreEqual(lotNumber, int.Parse(lotString));
                Assert.AreEqual(expectedLotAttributeHistories, new RioAccessSQLEntities().tblLotAttributeHistories.Count(h => h.Lot == lotNumber));
            }
        }

        [TestFixture]
        public class SetLotHoldStatus : SynchronizeOldContextIntegrationTestsBase<LotService>
        {
            [Test]
            public void Updates_existing_tblLot_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c => c.Lot.Hold == null && c.Lot.LotDefects.All(d => d.Resolution != null),
                    c => c.Lot.LotDefects).FirstOrDefault();
                if(chileLot == null)
                {
                    throw new Exception("Could not find ChileLot with single InHouseContamination defect.");
                }

                //Act
                var result = Service.SetLotHoldStatus(new SetLotHoldStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(chileLot),
                        Hold = new LotHold
                            {
                                HoldType = LotHoldType.HoldForCustomer,
                                Description = "TestTestTest"
                            }
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(a => a.Lot == newLot);
                Assert.AreEqual((int?)LotStat.Completed_Hold, tblLot.LotStat);
            }

            [Test]
            public void Will_create_new_tblLotHistory_record_if_setting_lot_hold_status_results_in_a_LotStat_change()
            {
                //Arrange
                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c => c.Lot.Hold == null && c.Lot.QualityStatus == LotQualityStatus.Released).FirstOrDefault();
                if(chileLot == null)
                {
                    throw new Exception("Could not find Accepted ChileLot without hold.");
                }
                var lotNumber = LotNumberBuilder.BuildLotNumber(chileLot);
                var expectedLotAttributeHistories = new RioAccessSQLEntities().tblLotAttributeHistories.Count(h => h.Lot == lotNumber) + 1;

                //Act
                var result = Service.SetLotHoldStatus(new SetLotHoldStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = chileLot.ToLotKey(),
                        Hold = new LotHold
                            {
                                HoldType = LotHoldType.HoldForCustomer,
                                Description = "HOLDIT!"
                            }
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                Assert.AreEqual(lotNumber, int.Parse(lotString));
                Assert.AreEqual(expectedLotAttributeHistories, new RioAccessSQLEntities().tblLotAttributeHistories.Count(h => h.Lot == lotNumber));
            }
        }

        [TestFixture]
        public class SetLotStatus : SynchronizeOldContextIntegrationTestsBase<LotService>
        {
            [Test]
            public void Updates_existing_tblLot_record_and_KillSwitch_will_not_have_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var lot = RVCUnitOfWork.LotRepository.Filter(l => l.LotTypeId == (decimal) LotTypeEnum.FinishedGood && l.QualityStatus != LotQualityStatus.Rejected).FirstOrDefault();
                if(lot == null)
                {
                    throw new Exception("Could not find Lot that has not been rejected.");
                }
                
                //Act
                var result = Service.SetLotQualityStatus(new SetLotStatusParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(lot),
                        QualityStatus = LotQualityStatus.Rejected
                    });
                var lotString = GetKeyFromConsoleString(ConsoleOutput.UpdatedLot);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var newLot = int.Parse(lotString);
                var tblLot = new RioAccessSQLEntities().tblLots.FirstOrDefault(l => l.Lot == newLot);
                Assert.AreEqual((int?)LotStat.Rejected, tblLot.LotStat);
            }
        }
    }
}