using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class SampleOrderServiceTests
    {
        [TestFixture]
        public class SyncSampleOrderUnitTests : SynchronizeOldContextUnitTestsBase<IResult<SyncSampleOrderParameters>>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SampleOrder; } }
        }

        [TestFixture]
        public class SyncSampleOrderIntegrationTests : SynchronizeOldContextIntegrationTestsBase<SampleOrderService>
        {
            [Test]
            public void Create()
            {
                //Arrange
                var parameters = new SetSampleOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        PrintNotes = "Print... me... please...",
                        DateReceived = new DateTime(2016, 1, 1),
                        DateDue = new DateTime(2016, 1, 2),
                        Items = new List<ISampleOrderItemParameters>
                            {
                                new SampleOrderItemParameters
                                    {
                                        Quantity = 123,
                                        Description = "I dunno, some thing I guess"
                                    }
                            }
                    };

                //Act
                var result = Service.SetSampleOrder(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblSample);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var sampleId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblSample = oldContext.tblSamples
                        .Include(s => s.tblSampleDetails)
                        .FirstOrDefault(s => s.SampleID == sampleId);
                    Assert.AreEqual(parameters.PrintNotes, tblSample.Notes2Print);

                    var expectedItem = parameters.Items.Single();
                    var resultItem = tblSample.tblSampleDetails.Single();
                    Assert.AreEqual(expectedItem.Quantity, resultItem.Qty);
                    Assert.AreEqual(expectedItem.Description, resultItem.Desc);
                }
            }

            [Test]
            public void Update()
            {
                //Arrange
                var sampleOrder = RVCUnitOfWork
                    .SampleOrderRepository.Filter(s => s.SampleID != null && s.Items.Count(i => i.SampleDetailID != null) > 1, i => i.Items)
                    .OrderByDescending(s => s.Year)
                    .FirstOrDefault();
                if(sampleOrder == null)
                {
                    Assert.Inconclusive("No suitable SampleOrder to test.");
                }

                var chileProduct = RVCUnitOfWork.ChileProductRepository.Filter(c => true, c => c.Product).FirstOrDefault();
                if(chileProduct == null)
                {
                    Assert.Inconclusive("No ChileProdut to test.");
                }

                var chileLot = RVCUnitOfWork.ChileLotRepository.Filter(c => true).FirstOrDefault();
                if(chileLot == null)
                {
                    Assert.Inconclusive("No ChileLot to test.");
                }

                var parameters = new SetSampleOrderParameters
                    {
                        SampleOrderKey = sampleOrder.ToSampleOrderKey(),
                        UserToken = TestUser.UserName,
                        PrintNotes = ": D",
                        DateReceived = new DateTime(2016, 1, 1),
                        DateDue = new DateTime(2016, 1, 2),
                        Items = new List<ISampleOrderItemParameters>
                                {
                                    new SampleOrderItemParameters
                                        {
                                            SampleOrderItemKey = sampleOrder.Items.First().ToSampleOrderItemKey(),
                                            ProductKey = chileProduct.ToChileProductKey(),
                                            LotKey = chileLot.ToLotKey(),
                                            Quantity = 321,
                                            Description = "updated this item"
                                        }
                                }
                    };

                //Act
                var result = Service.SetSampleOrder(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblSample);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var sampleId = int.Parse(resultString);
                Assert.AreEqual(sampleOrder.SampleID, sampleId);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblSample = oldContext.tblSamples
                        .Include(s => s.tblSampleDetails)
                        .FirstOrDefault(s => s.SampleID == sampleId);
                    Assert.AreEqual(parameters.PrintNotes, tblSample.Notes2Print);

                    var expectedItem = parameters.Items.Single();
                    var resultItem = tblSample.tblSampleDetails.Single();
                    Assert.AreEqual(expectedItem.Quantity, resultItem.Qty);
                    Assert.AreEqual(expectedItem.Description, resultItem.Desc);
                    Assert.AreEqual(chileProduct.Product.ProductCode, resultItem.ProdID.ToString());
                    Assert.AreEqual(LotNumberParser.BuildLotNumber(chileLot), resultItem.Lot);
                }
            }

            [Test]
            public void Delete()
            {
                //Arrange
                var sampleOrder = RVCUnitOfWork.SampleOrderRepository.Filter(o => o.SampleID != null)
                    .OrderByDescending(s => s.Year)
                    .FirstOrDefault();
                if(sampleOrder == null)
                {
                    Assert.Inconclusive("No SampleOrder to test.");
                }

                //Act
                var result = Service.DeleteSampleOrder(sampleOrder.ToSampleOrderKey());
                result.AssertSuccess();

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    Assert.IsNull(oldContext.tblSamples.FirstOrDefault(s => s.SampleID == sampleOrder.SampleID));
                }
            }

            [Test]
            public void SetSpec()
            {
                //Arrange
                var sampleOrderItem = RVCUnitOfWork.SampleOrderItemRepository
                    .Filter(i => i.SampleDetailID != null && i.Spec == null)
                    .OrderByDescending(i => i.SampleOrderYear)
                    .FirstOrDefault();
                if(sampleOrderItem == null)
                {
                    Assert.Inconclusive("No SampleOrderItme to test.");
                }

                var parameters = new SetSampleSpecsParameters
                    {
                        SampleOrderItemKey = sampleOrderItem.ToSampleOrderItemKey(),
                        Notes = "integrated testing"
                    };

                //Act
                var result = Service.SetSampleSpecs(parameters);
                result.AssertSuccess();

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    var specs = oldContext.tblSampleCustSpecs.FirstOrDefault(s => s.SampleDetailID == sampleOrderItem.SampleDetailID);
                    Assert.AreEqual(parameters.Notes, specs.Notes);
                }
            }

            [Test]
            public void SetMatch()
            {
                //Arrange
                var sampleOrderItem = RVCUnitOfWork.SampleOrderItemRepository
                    .Filter(i => i.SampleDetailID != null && i.Match == null)
                    .OrderByDescending(i => i.SampleOrderYear)
                    .FirstOrDefault();
                if(sampleOrderItem == null)
                {
                    Assert.Inconclusive("No SampleOrderItme to test.");
                }

                var parameters = new SetSampleMatchParameters
                    {
                        SampleOrderItemKey = sampleOrderItem.ToSampleOrderItemKey(),
                        Notes = "integrated testing"
                    };

                //Act
                var result = Service.SetSampleMatch(parameters);
                result.AssertSuccess();

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    var match = oldContext.tblSampleRVCMatches.FirstOrDefault(s => s.SampleDetailID == sampleOrderItem.SampleDetailID);
                    Assert.AreEqual(parameters.Notes, match.Notes);
                }
            }

            [Test]
            public void SetJournalEntry()
            {
                //Arrange
                var sampleOrder = RVCUnitOfWork.SampleOrderRepository
                    .Filter(i => i.SampleID != null)
                    .OrderByDescending(i => i.Year)
                    .FirstOrDefault();
                if(sampleOrder == null)
                {
                    Assert.Inconclusive("No SampleOrderItme to test.");
                }

                var parameters = new SetSampleOrderJournalEntryParameters
                    {
                        UserToken = TestUser.UserName,
                        SampleOrderKey = sampleOrder.ToSampleOrderKey(),
                        Date = DateTime.Now,
                        Text = "Today is the first day of the rest of my life."
                    };

                //Act
                var result = Service.SetJournalEntry(parameters);
                result.AssertSuccess();

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    var note = oldContext.tblSampleNotes
                        .Where(n => n.SampleID == sampleOrder.SampleID)
                        .OrderByDescending(n => n.SamNoteID)
                        .FirstOrDefault();
                    Assert.AreEqual(parameters.Text, note.SampleNote);
                }
            }

            [Test]
            public void DeleteJournalEntry()
            {
                //Arrange
                var entry = RVCUnitOfWork.SampleOrderJournalEntryRepository.Filter(j => j.SamNoteID != null).FirstOrDefault();

                //Act
                var result = Service.DeleteJournalEntry(entry.ToSampleOrderJournalEntryKey());
                result.AssertSuccess();

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    Assert.IsNull(oldContext.tblSampleNotes.FirstOrDefault(n => n.SamNoteID == entry.SamNoteID));
                }
            }
        }
    }
}