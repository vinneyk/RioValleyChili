using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.Helpers.ParameterExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Helpers;
using SampleOrderItemParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SampleOrderItemParameters;
using SetSampleOrderParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SetSampleOrderParameters;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class SampleOrderServiceTests : ServiceIntegrationTestBase<SampleOrderService>
    {
        [TestFixture]
        public class GetSampleOrder : SampleOrderServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_SampleOrder_does_not_exist()
            {
                //Act
                var result = Service.GetSampleOrder(new SampleOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.SampleOrderNotFound);
            }

            [Test]
            public void Returns_SampleOrder_as_expected()
            {
                //Arrange
                var sampleOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o => o.Items = TestHelper.List<SampleOrderItem>(3,items =>
                    {
                        items.ElementAt(0).Product = null;
                        items.ElementAt(0).ProductId = null;
                    }),
                    o => o.JournalEntries = TestHelper.List<SampleOrderJournalEntry>(3));

                //Act
                var result = Service.GetSampleOrder(sampleOrder.ToSampleOrderKey());

                //Assert
                result.AssertSuccess();
                sampleOrder.AssertEqual(result.ResultingObject);
            }
        }

        [TestFixture]
        public class GetSampleOrders : SampleOrderServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_records_exist()
            {
                //Act
                var result = Service.GetSampleOrders();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.IsEmpty(results);
            }

            [Test]
            public void Returns_SampleOrders_as_expected()
            {
                //Arrange
                var expected = new List<SampleOrder>();
                for(var i = 0; i < 5; ++i)
                {
                    expected.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o =>
                        {
                            switch(i)
                            {
                                case 1: o.SetRequestCustomer(null); break;
                                case 2: o.SetBroker(null); break;
                                case 4: o.SetRequestCustomer(null).SetBroker(null); break;
                            }
                        }));
                }

                //Act
                var result = Service.GetSampleOrders();

                //Assert
                result.AssertSuccess();
                expected.AssertEquivalent(result.ResultingObject.ToList(), e => e.ToSampleOrderKey().KeyValue, r => r.SampleRequestKey,
                    (e, r) => e.AssertEqual(r));
            }

            [Test]
            public void Returns_SampleOrders_filtered_as_expected()
            {
                //Arrange
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var filters = new FilterSampleOrdersParameters
                    {
                        DateReceivedStart = new DateTime(2016, 2, 1),
                        DateReceivedEnd = new DateTime(2016, 2, 10),
                        DateCompletedStart = new DateTime(2016, 3, 1),
                        DateCompletedEnd = new DateTime(2016, 3, 10),
                        Status = SampleOrderStatus.Sent,
                        BrokerKey = broker.ToCompanyKey(),
                        RequestedCompanyKey = customer.ToCustomerKey()
                    };

                var expected = new List<SampleOrder>();
                for(var i = 0; i < 3; ++i)
                {
                    expected.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o =>
                        o.DateReceived = filters.DateReceivedStart.Value.AddDays(1),
                        o => o.DateCompleted = filters.DateCompletedStart.Value.AddDays(1),
                        o => o.Status = filters.Status.Value,
                        o => o.SetBroker(broker).SetRequestCustomer(customer)));
                }

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o =>
                    o.DateReceived = filters.DateReceivedEnd.Value.AddDays(1),
                    o => o.DateCompleted = filters.DateCompletedStart.Value.AddDays(1),
                    o => o.Status = filters.Status.Value,
                    o => o.SetBroker(broker).SetRequestCustomer(customer));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o =>
                    o.DateReceived = filters.DateReceivedStart.Value.AddDays(1),
                    o => o.DateCompleted = filters.DateCompletedEnd.Value.AddDays(1),
                    o => o.Status = filters.Status.Value,
                    o => o.SetBroker(broker).SetRequestCustomer(customer));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o =>
                        o.DateReceived = filters.DateReceivedStart.Value.AddDays(1),
                        o => o.DateCompleted = filters.DateCompletedStart.Value.AddDays(1),
                        o => o.Status = SampleOrderStatus.Approved,
                        o => o.SetBroker(broker).SetRequestCustomer(customer));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o =>
                        o.DateReceived = filters.DateReceivedStart.Value.AddDays(1),
                        o => o.DateCompleted = filters.DateCompletedStart.Value.AddDays(1),
                        o => o.Status = filters.Status.Value,
                        o => o.SetBroker(null).SetRequestCustomer(customer));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o =>
                        o.DateReceived = filters.DateReceivedStart.Value.AddDays(1),
                        o => o.DateCompleted = filters.DateCompletedStart.Value.AddDays(1),
                        o => o.Status = filters.Status.Value,
                        o => o.SetBroker(broker).SetRequestCustomer(null));

                //Act
                var result = Service.GetSampleOrders(filters);
                var results = result.Success ? result.ResultingObject.ToList() : null;

                //Assert
                result.AssertSuccess();
                expected.AssertEquivalent(results, e => e.ToSampleOrderKey().KeyValue, r => r.SampleRequestKey,
                    (e, r) => e.AssertEqual(r));
            }
        }

        [TestFixture]
        public class SetSampleOrder : SampleOrderServiceTests
        {
            [Test]
            public void Creates_new_SampleOrder_and_Items_as_expected()
            {
                //Arrange
                var parameters = new SetSampleOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        DateDue = new DateTime(2016, 2, 1),
                        DateReceived = new DateTime(2016, 1, 1),

                        BrokerKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker)).ToCompanyKey(),
                        RequestedByCompanyKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>().ToCustomerKey(),

                        Items = new List<ISampleOrderItemParameters>
                            {
                                new SampleOrderItemParameters
                                    {
                                        Quantity = 123,
                                        Description = "little plastic baggies",
                                        LotKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>().ToLotKey(),
                                        ProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>().ToChileProductKey()
                                    },
                                new SampleOrderItemParameters
                                    {
                                        Quantity = 321,
                                        Description = "little plastic baggies part 2",
                                        ProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Product>(p => p.ProductType = ProductTypeEnum.NonInventory).ToProductKey()
                                    }
                            }
                    };

                //Act
                var result = Service.SetSampleOrder(parameters);

                //Assert
                result.AssertSuccess();
                var sampleOrderKey = KeyParserHelper.ParseResult<ISampleOrderKey>(result.ResultingObject).ResultingObject.ToSampleOrderKey();
                var sampleOrder = RVCUnitOfWork.SampleOrderRepository.FindByKey(sampleOrderKey,
                    o => o.Broker,
                    o => o.RequestCustomer,
                    o => o.Items.Select(i => i.Product),
                    o => o.Items.Select(i => i.Lot));
                parameters.AssertEqual(sampleOrder);
            }

            [Test]
            public void Sets_existing_SampleOrder_and_Items_as_expected()
            {
                //Arrange
                var sampleOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(o => o.Items = TestHelper.List<SampleOrderItem>(3));
                var parameters = new SetSampleOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        SampleOrderKey = sampleOrder.ToSampleOrderKey(),
                        DateReceived = new DateTime(2016, 1, 2),
                        DateDue = sampleOrder.DateDue,
                        Items = sampleOrder.Items.Take(2)
                            .Select(i => new SampleOrderItemParameters
                                {
                                    SampleOrderItemKey = i.ToSampleOrderItemKey()
                                })
                            .Concat(new List<SampleOrderItemParameters>
                                {
                                    new SampleOrderItemParameters
                                        {
                                            Quantity = 132,
                                            Description = "some kind of thing"
                                        }
                                })
                    };

                //Act
                var result = Service.SetSampleOrder(parameters);

                //Assert
                result.AssertSuccess();
                sampleOrder = RVCUnitOfWork.SampleOrderRepository.FindByKey(sampleOrder.ToSampleOrderKey(),
                    o => o.Broker,
                    o => o.RequestCustomer,
                    o => o.Items.Select(i => i.Product),
                    o => o.Items.Select(i => i.Lot));
                parameters.AssertEqual(sampleOrder);
            }
        }

        [TestFixture]
        public class DeleteSampleOrder : SampleOrderServiceTests
        {
            [Test]
            public void Deletes_SampleOrder_and_related_records_on_success()
            {
                //Arrange
                var sampleOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>(
                    o => o.JournalEntries = TestHelper.List<SampleOrderJournalEntry>(3),
                    o => o.Items = TestHelper.List<SampleOrderItem>(3));

                //Act
                var result = Service.DeleteSampleOrder(sampleOrder.ToSampleOrderKey());

                //Assert
                result.AssertSuccess();
                Assert.IsFalse(RVCUnitOfWork.SampleOrderRepository.All().Any());
                Assert.IsFalse(RVCUnitOfWork.SampleOrderItemRepository.All().Any());
                Assert.IsFalse(RVCUnitOfWork.SampleOrderItemSpecRepository.All().Any());
                Assert.IsFalse(RVCUnitOfWork.SampleOrderItemMatchRepository.All().Any());
                Assert.IsFalse(RVCUnitOfWork.SampleOrderJournalEntryRepository.All().Any());
            }
        }

        [TestFixture]
        public class SetSampleSpecs : SampleOrderServiceTests
        {
            [Test]
            public void Creates_new_SampleOrderItemSpec_record_on_success()
            {
                //Arrange
                var sampleOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrderItem>(i => i.Spec = null);
                Assert.IsNull(sampleOrderItem.Spec);
                var expected = TestHelper.CreateObjectGraph<SetSampleSpecsParameters>(p => p.SampleOrderItemKey = sampleOrderItem.ToSampleOrderItemKey());

                //Act
                var result = Service.SetSampleSpecs(expected);

                //Assert
                result.AssertSuccess();
                sampleOrderItem = RVCUnitOfWork.SampleOrderItemRepository.FindByKey(sampleOrderItem.ToSampleOrderItemKey(), i => i.Spec);
                expected.AssertEqual(sampleOrderItem.Spec);
            }

            [Test]
            public void Updates_existing_SampleOrderItemSpec_record_on_success()
            {
                //Arrange
                var sampleOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrderItem>();
                Assert.IsNotNull(sampleOrderItem.Spec);
                var expected = TestHelper.CreateObjectGraph<SetSampleSpecsParameters>(p => p.SampleOrderItemKey = sampleOrderItem.ToSampleOrderItemKey());

                //Act
                var result = Service.SetSampleSpecs(expected);

                //Assert
                result.AssertSuccess();
                sampleOrderItem = RVCUnitOfWork.SampleOrderItemRepository.FindByKey(sampleOrderItem.ToSampleOrderItemKey(), i => i.Spec);
                expected.AssertEqual(sampleOrderItem.Spec);
            }
        }

        [TestFixture]
        public class SetSampleMatch : SampleOrderServiceTests
        {
            [Test]
            public void Creates_new_SampleOrderItemMatch_record_on_success()
            {
                //Arrange
                var sampleOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrderItem>(i => i.Match = null);
                Assert.IsNull(sampleOrderItem.Match);
                var expected = TestHelper.CreateObjectGraph<SetSampleMatchParameters>(p => p.SampleOrderItemKey = sampleOrderItem.ToSampleOrderItemKey());

                //Act
                var result = Service.SetSampleMatch(expected);

                //Assert
                result.AssertSuccess();
                sampleOrderItem = RVCUnitOfWork.SampleOrderItemRepository.FindByKey(sampleOrderItem.ToSampleOrderItemKey(), i => i.Match);
                expected.AssertEqual(sampleOrderItem.Match);
            }

            [Test]
            public void Updates_existing_SampleOrderItemMatch_record_on_success()
            {
                //Arrange
                var sampleOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrderItem>();
                Assert.IsNotNull(sampleOrderItem.Match);
                var expected = TestHelper.CreateObjectGraph<SetSampleMatchParameters>(p => p.SampleOrderItemKey = sampleOrderItem.ToSampleOrderItemKey());

                //Act
                var result = Service.SetSampleMatch(expected);

                //Assert
                result.AssertSuccess();
                sampleOrderItem = RVCUnitOfWork.SampleOrderItemRepository.FindByKey(sampleOrderItem.ToSampleOrderItemKey(), i => i.Match);
                expected.AssertEqual(sampleOrderItem.Match);
            }
        }

        [TestFixture]
        public class SetJournalEntry : SampleOrderServiceTests
        {
            [Test]
            public void Creates_new_Journal_entry_record_on_success()
            {
                //Arrange
                var sampleOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrder>();
                var parameters = new SetSampleOrderJournalEntryParameters
                    {
                        UserToken = TestUser.UserName,
                        SampleOrderKey = sampleOrder.ToSampleOrderKey(),
                        Date = new DateTime(2016, 10, 10),
                        Text = "AAAAAAAH!"
                    };

                //Act
                var result = Service.SetJournalEntry(parameters);

                //Assert
                result.AssertSuccess();
                var journalEntry = RVCUnitOfWork.SampleOrderRepository.FindByKey(sampleOrder.ToSampleOrderKey(), s => s.JournalEntries.Select(j => j.Employee))
                    .JournalEntries.Single();
                parameters.AssertEqual(journalEntry);
            }

            [Test]
            public void Updates_existing_Journal_entry_record_on_success()
            {
                //Arrange
                var journalEntry = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrderJournalEntry>();
                var parameters = new SetSampleOrderJournalEntryParameters
                    {
                        UserToken = TestUser.UserName,
                        SampleOrderKey = journalEntry.ToSampleOrderKey(),
                        JournalEntryKey = journalEntry.ToSampleOrderJournalEntryKey(),
                        Date = new DateTime(2016, 10, 10),
                        Text = "AAAAAAAH!"
                    };

                //Act
                var result = Service.SetJournalEntry(parameters);

                //Assert
                result.AssertSuccess();
                journalEntry = RVCUnitOfWork.SampleOrderJournalEntryRepository.FindByKey(journalEntry.ToSampleOrderJournalEntryKey(), e => e.Employee);
                parameters.AssertEqual(journalEntry);
            }
        }

        [TestFixture]
        public class DeleteJournalEntry : SampleOrderServiceTests
        {
            [Test]
            public void Deletes_existing_JournalEntry_record_on_success()
            {

                //Arrange
                var entryKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SampleOrderJournalEntry>().ToSampleOrderJournalEntryKey();

                //Act
                var result = Service.DeleteJournalEntry(entryKey);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.SampleOrderJournalEntryRepository.FindByKey(entryKey));
            }
        }
    }
}