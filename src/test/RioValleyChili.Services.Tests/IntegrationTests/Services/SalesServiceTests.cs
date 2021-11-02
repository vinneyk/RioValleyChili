// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.Helpers.ParameterExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class SalesServiceTests : ServiceIntegrationTestBase<SalesService>
    {
        [TestFixture]
        public class GetCustomersForBroker : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Broker_Company_could_not_be_found()
            {
                //Act
                var result = Service.GetCustomersForBroker(new CompanyKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.CompanyNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_Broker_Company_is_not_of_Broker_type()
            {
                //Arrange
                var company = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator));

                //Act
                var result = Service.GetCustomersForBroker(new CompanyKey(company).KeyValue);

                //Assert
                result.AssertNotSuccess(string.Format(UserMessages.CompanyNotOfType, "{0}", CompanyType.Broker));
            }

            [Test]
            public void Returns_all_Customer_Companies_associated_with_the_specified_Broker_on_success()
            {
                //Arrange
                const int expectedResults = 3;
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));

                var customer0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().SetBroker(broker));
                var customer1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().SetBroker(broker));
                var customer2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().SetBroker(broker));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems());

                //Act
                var result = Service.GetCustomersForBroker(new CompanyKey(broker).KeyValue);

                //Assert
                result.AssertSuccess();
                var customers = result.ResultingObject.ToList();

                Assert.AreEqual(expectedResults, customers.Count);
                customer0.Company.AssertEqual(customers.Single(c => c.CompanyKey == new CompanyKey(customer0).KeyValue));
                customer1.Company.AssertEqual(customers.Single(c => c.CompanyKey == new CompanyKey(customer1).KeyValue));
                customer2.Company.AssertEqual(customers.Single(c => c.CompanyKey == new CompanyKey(customer2).KeyValue));
            }
        }

        [TestFixture]
        public class AssignSalesToBroker : SalesServiceTests
        {
            [Test]

            public void Returns_non_successful_result_if_Broker_Company_could_not_be_found()
            {
                //Act
                var result = Service.AssignCustomerToBroker(new CompanyKey().KeyValue, new CustomerKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.CompanyNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_Broker_Company_is_not_of_type_Broker()
            {
                //Arrange
                var company = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Customer));

                //Act
                var result = Service.AssignCustomerToBroker(new CompanyKey(company).KeyValue, new CustomerKey().KeyValue);

                //Assert
                result.AssertNotSuccess(string.Format(UserMessages.CompanyNotOfType, "{0}", CompanyType.Broker));
            }

            [Test]
            public void Returns_non_successful_result_if_Customer_could_not_be_found()
            {
                //Arrange
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));

                //Act
                var result = Service.AssignCustomerToBroker(new CompanyKey(broker).KeyValue, new CustomerKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerNotFound);
            }

            [Test]
            public void Updates_Customer_Broker_as_expected_on_success()
            {
                //Arrange
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));
                var customerKey = new CustomerKey((ICustomerKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems()));

                //Act
                var result = Service.AssignCustomerToBroker(new CompanyKey(broker).KeyValue, customerKey.KeyValue);

                //Assert
                result.AssertSuccess();

                var customer = RVCUnitOfWork.CustomerRepository.FindByKey(customerKey, c => c.Broker);
                Assert.AreEqual(new CompanyKey(broker), new CompanyKey(customer.Broker));
            }
        }

        [TestFixture]
        public class RemoveCustomerFromBroker : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Broker_Company_could_not_be_found()
            {
                //Arrange
                var dummyBrokerKey = new CompanyKey(new Company { Id = 99 });

                //Act
                var result = Service.RemoveCustomerFromBroker(dummyBrokerKey.KeyValue, new CustomerKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.CompanyNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_Broker_Company_is_not_of_type_Broker()
            {
                //Arrange
                var company = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Customer));

                //Act
                var result = Service.RemoveCustomerFromBroker(new CompanyKey(company).KeyValue, new CustomerKey().KeyValue);

                //Assert
                result.AssertNotSuccess(string.Format(UserMessages.CompanyNotOfType, "{0}", CompanyType.Broker));
            }

            [Test]
            public void Returns_non_successful_result_if_Customer_could_not_be_found()
            {
                //Arrange
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));

                //Act
                var result = Service.RemoveCustomerFromBroker(new CompanyKey(broker).KeyValue, new CustomerKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_Customer_is_not_of_Broker()
            {
                //Arrange
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));
                var customerKey = new CustomerKey((ICustomerKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems()));

                //Act
                var result = Service.RemoveCustomerFromBroker(new CompanyKey(broker).KeyValue, customerKey.KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerNotOfBroker);
            }

            [Test]
            public void Updates_Customer_Broker_to_RVCBroker_on_success()
            {
                //Arrange
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));
                var customerKey = new CustomerKey((ICustomerKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().SetBroker(broker)));

                //Act
                var result = Service.RemoveCustomerFromBroker(new CompanyKey(broker).KeyValue, customerKey.KeyValue);

                //Assert
                result.AssertSuccess();

                var customer = RVCUnitOfWork.CustomerRepository.FindByKey(customerKey, c => c.Broker);
                Assert.AreEqual(new CompanyKey(StaticCompanies.RVCBroker), new CompanyKey(customer.Broker));
            }
        }

        [TestFixture]
        public class CreateCustomerContract : SalesServiceTests
        {
            [Test]
            public void Returns_non_sucessful_result_if_ContractItems_is_null_or_empty()
            {
                //Arrange
                var parameters = new CreateContractParameters
                {
                    CustomerKey = new CustomerKey().KeyValue,
                    DefaultPickFromFacilityKey = new FacilityKey().KeyValue,
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2),
                };

                //Act
                var resultNull = Service.CreateCustomerContract(parameters);

                parameters.ContractItems = new IContractItem[0];
                var resultEmpty = Service.CreateCustomerContract(parameters);

                //Assert
                resultNull.AssertNotSuccess(UserMessages.ContractItemsRequired);
                resultEmpty.AssertNotSuccess(UserMessages.ContractItemsRequired);
            }

            [Test]
            public void Returns_non_sucessful_result_if_any_ContractItem_has_quantity_less_than_or_equal_to_zero()
            {
                //Arrange
                var parameters = new CreateContractParameters
                {
                    CustomerKey = new CustomerKey().KeyValue,
                    DefaultPickFromFacilityKey = new FacilityKey().KeyValue,
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2),
                    ContractItems = new List<IContractItem>
                        {
                            new ContractItemParameters
                            {
                                ChileProductKey = new ChileProductKey().KeyValue,
                                PackagingProductKey = new PackagingProductKey().KeyValue,
                                TreatmentKey = new InventoryTreatmentKey().KeyValue,
                            }
                        }
                };

                //Act
                (parameters.ContractItems.Single() as ContractItemParameters).Quantity = -1;
                var resultLessThan0 = Service.CreateCustomerContract(parameters);

                (parameters.ContractItems.Single() as ContractItemParameters).Quantity = 0;
                var resultEqual0 = Service.CreateCustomerContract(parameters);

                //Assert
                resultLessThan0.AssertNotSuccess(UserMessages.QuantityNotGreaterThanZero);
                resultEqual0.AssertNotSuccess(UserMessages.QuantityNotGreaterThanZero);
            }

            [Test]
            public void Returns_non_successful_result_if_TermBegin_date_is_greater_than_or_equal_to_TermEnd()
            {
                //Arrange
                var parameters = new CreateContractParameters
                    {
                        CustomerKey = new CustomerKey().KeyValue,
                        DefaultPickFromFacilityKey = new FacilityKey().KeyValue,
                        ContractItems = new List<IContractItem>
                            {
                                new ContractItemParameters
                                    {
                                        ChileProductKey = new ChileProductKey().KeyValue,
                                        PackagingProductKey = new PackagingProductKey().KeyValue,
                                        TreatmentKey = new InventoryTreatmentKey().KeyValue,
                                        Quantity = 10
                                    }
                            }
                    };

                //Act
                parameters.TermBegin = new DateTime(2012, 3, 2);
                parameters.TermEnd = new DateTime(2012, 3, 1);
                var resultGreaterThan = Service.CreateCustomerContract(parameters);

                parameters.TermBegin = new DateTime(2012, 3, 2);
                parameters.TermEnd = new DateTime(2012, 3, 2);
                var resultEqualTo = Service.CreateCustomerContract(parameters);

                //Assert
                resultGreaterThan.AssertNotSuccess(UserMessages.ContractTermMustBeginBeforeEnd);
                resultEqualTo.AssertNotSuccess(UserMessages.ContractTermMustBeginBeforeEnd);
            }

            [Test]
            public void Returns_non_successful_result_if_Customer_record_could_not_be_found()
            {
                //Act
                var result = Service.CreateCustomerContract(new CreateContractParameters
                {
                    CustomerKey = new CustomerKey().KeyValue,
                    DefaultPickFromFacilityKey = new FacilityKey().KeyValue,
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2),
                    ContractItems = new List<IContractItem>
                        {
                            new ContractItemParameters
                                {
                                    ChileProductKey = new ChileProductKey().KeyValue,
                                    PackagingProductKey = new PackagingProductKey().KeyValue,
                                    TreatmentKey = new InventoryTreatmentKey().KeyValue,
                                    Quantity = 10
                                }
                        }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_DefaultPickFromWarehouse_could_not_be_found()
            {
                //Arrange
                var customerKey = new CustomerKey((ICustomerKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker)));

                //Act
                var result = Service.CreateCustomerContract(new CreateContractParameters
                {
                    CustomerKey = customerKey.KeyValue,
                    DefaultPickFromFacilityKey = new FacilityKey().KeyValue,
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2),
                    ContractItems = new List<IContractItem>
                            {
                                new ContractItemParameters
                                    {
                                        ChileProductKey = new ChileProductKey().KeyValue,
                                        PackagingProductKey = new PackagingProductKey().KeyValue,
                                        TreatmentKey = new InventoryTreatmentKey().KeyValue,
                                        Quantity = 10
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.FacilityNotFound);
            }

            [Test]
            public void Creates_Contract_record_as_expected_on_success()
            {
                //Arrange
                var expectedYear = DateTime.UtcNow.Year;
                const int expectedSequence = 1;
                const ContractType expectedType = ContractType.Interim;
                const ContractStatus expectedStatus = ContractStatus.Confirmed;
                const string paymentTerms = "ONE MILLION $$$";
                var expectedAddress = new Address
                {
                    AddressLine1 = "line1",
                    City = "Coocamunga"
                };

                var termBegin = new DateTime(2012, 1, 2);
                var termEnd = new DateTime(2012, 3, 4);

                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker));
                var customerKey = new CustomerKey((ICustomerKey)customer);
                var brokerKey = new CompanyKey(customer.Broker);
                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>());

                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateCustomerContract(new CreateContractParameters
                {
                    UserToken = TestUser.UserName,
                    CustomerKey = customerKey.KeyValue,
                    DefaultPickFromFacilityKey = warehouseKey.KeyValue,
                    ContactAddress = expectedAddress,
                    ContractType = expectedType,
                    ContractStatus = expectedStatus,
                    PaymentTerms = paymentTerms,
                    TermBegin = termBegin,
                    TermEnd = termEnd,
                    ContractItems = new List<IContractItem>
                            {
                                new ContractItemParameters
                                    {
                                        ChileProductKey = chileProductKey.KeyValue,
                                        PackagingProductKey = packagingProductKey.KeyValue,
                                        TreatmentKey = treatmentKey.KeyValue,
                                        Quantity = 10
                                    }
                            }
                });

                //Assert
                result.AssertSuccess();

                var contract = RVCUnitOfWork.ContractRepository.Filter(c => true, c => c.Broker).Single();
                Assert.AreEqual(expectedYear, contract.ContractYear);
                Assert.AreEqual(expectedSequence, contract.ContractSequence);
                Assert.AreEqual(expectedType, contract.ContractType);
                Assert.AreEqual(expectedStatus, contract.ContractStatus);
                Assert.AreEqual(termBegin, contract.TermBegin);
                Assert.AreEqual(termEnd, contract.TermEnd);

                Assert.AreEqual(customerKey, contract);
                contract.ContactAddress.AssertEqual(expectedAddress);
                Assert.AreEqual(brokerKey, contract.Broker);
                Assert.AreEqual(warehouseKey, contract);
            }

            [Test]
            public void Creates_Contract_record_with_default_warehouse_of_Rincon_if_none_is_supplied()
            {
                //Arrange
                var customerKey = new CustomerKey((ICustomerKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker)));
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.CreateCustomerContract(new CreateContractParameters
                {
                    UserToken = TestUser.UserName,
                    CustomerKey = customerKey.KeyValue,
                    ContactAddress = new Address
                    {
                        AddressLine1 = "line1",
                        City = "Coocamunga"
                    },
                    ContractType = ContractType.Interim,
                    ContractStatus = ContractStatus.Confirmed,
                    PaymentTerms = "ONE MILLION $$$",
                    TermBegin = new DateTime(2012, 1, 2),
                    TermEnd = new DateTime(2012, 3, 4),
                    ContractItems = new List<IContractItem>
                                {
                                    new ContractItemParameters
                                        {
                                            ChileProductKey = chileProductKey.KeyValue,
                                            PackagingProductKey = packagingProductKey.KeyValue,
                                            TreatmentKey = treatmentKey.KeyValue,
                                            Quantity = 10
                                        }
                                }
                });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new FacilityKey(StaticFacilities.Rincon), new FacilityKey(RVCUnitOfWork.ContractRepository.All().Single()));
            }

            [Test]
            public void Creates_ContractItem_records_as_expected_on_success()
            {
                //Arrange
                var expectedYear = DateTime.UtcNow.Year;
                const int expectedSequence = 1;
                const int expectedItems = 2;

                const string expectedCode0 = "CHILEE01";
                const int quantity0 = 10;
                const bool useCustomerSpec0 = false;
                const double priceBase0 = 3.50;
                const double priceFreight0 = 0.10;
                const double priceTreatment0 = 0.20;
                const double priceWarehouse0 = 0.30;
                const double priceRbate0 = -0.01;

                const string expectedCode1 = "CODE OVERRIDE";
                const int quantity1 = 22;
                const bool useCustomerSpec1 = true;
                const double priceBase1 = 3.51;
                const double priceFreight1 = 1.21;
                const double priceTreatment1 = 1.31;
                const double priceWarehouse1 = 1.41;
                const double priceRbate1 = -1.11;

                var customerKey = new CustomerKey((ICustomerKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker)));

                var chileProductKey0 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductCode>(c => c.ConstrainByKeys(customerKey, chileProductKey0).Code = expectedCode0);
                var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey0 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                var chileProductKey1 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductCode>(c => c.ConstrainByKeys(customerKey, chileProductKey1));
                var packagingProductKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey1 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>());

                //Act
                var result = Service.CreateCustomerContract(new CreateContractParameters
                {
                    UserToken = TestUser.UserName,
                    CustomerKey = customerKey.KeyValue,
                    DefaultPickFromFacilityKey = warehouseKey.KeyValue,
                    ContractType = ContractType.Interim,
                    TermBegin = new DateTime(2012, 1, 2),
                    TermEnd = new DateTime(2012, 3, 4),
                    ContractItems = new List<IContractItem>
                        {
                            new ContractItemParameters
                            {
                                ChileProductKey = chileProductKey0.KeyValue,
                                PackagingProductKey = packagingProductKey0.KeyValue,
                                TreatmentKey = treatmentKey0.KeyValue,
                                Quantity = quantity0,
                                UseCustomerSpec = useCustomerSpec0,
                                PriceBase = priceBase0,
                                PriceFreight = priceFreight0,
                                PriceTreatment = priceTreatment0,
                                PriceWarehouse = priceWarehouse0,
                                PriceRebate = priceRbate0
                            },

                            new ContractItemParameters
                            {
                                ChileProductKey = chileProductKey1.KeyValue,
                                PackagingProductKey = packagingProductKey1.KeyValue,
                                TreatmentKey = treatmentKey1.KeyValue,
                                Quantity = quantity1,
                                UseCustomerSpec = useCustomerSpec1,
                                CustomerCodeOverride = expectedCode1,
                                PriceBase = priceBase1,
                                PriceFreight = priceFreight1,
                                PriceTreatment = priceTreatment1,
                                PriceWarehouse = priceWarehouse1,
                                PriceRebate = priceRbate1
                            },
                        }
                });

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.ContractItemRepository.All().Where(i => i.ContractYear == expectedYear && i.ContractSequence == expectedSequence).ToList();
                Assert.AreEqual(expectedItems, items.Count);

                var item = items.Single(i => chileProductKey0.Equals(i));
                Assert.AreEqual(packagingProductKey0, item);
                Assert.AreEqual(treatmentKey0, item);
                Assert.AreEqual(quantity0, item.Quantity);
                Assert.AreEqual(useCustomerSpec0, item.UseCustomerSpec);
                Assert.AreEqual(expectedCode0, item.CustomerProductCode);
                Assert.AreEqual(priceBase0, item.PriceBase);
                Assert.AreEqual(priceFreight0, item.PriceFreight);
                Assert.AreEqual(priceTreatment0, item.PriceTreatment);
                Assert.AreEqual(priceWarehouse0, item.PriceWarehouse);
                Assert.AreEqual(priceRbate0, item.PriceRebate);

                item = items.Single(i => chileProductKey1.Equals(i));
                Assert.AreEqual(packagingProductKey1, item);
                Assert.AreEqual(treatmentKey1, item);
                Assert.AreEqual(quantity1, item.Quantity);
                Assert.AreEqual(useCustomerSpec1, item.UseCustomerSpec);
                Assert.AreEqual(expectedCode1, item.CustomerProductCode);
                Assert.AreEqual(priceBase1, item.PriceBase);
                Assert.AreEqual(priceFreight1, item.PriceFreight);
                Assert.AreEqual(priceTreatment1, item.PriceTreatment);
                Assert.AreEqual(priceWarehouse1, item.PriceWarehouse);
                Assert.AreEqual(priceRbate1, item.PriceRebate);
            }
        }

        [TestFixture]
        public class UpdateCustomerContract : SalesServiceTests
        {
            [Test]
            public void Returns_non_sucessful_result_if_ContractItems_is_null_or_empty()
            {
                //Arrange
                var parameters = new Params
                {
                    ContractKey = new ContractKey().KeyValue,
                    CustomerKey = new CustomerKey().KeyValue,
                    BrokerKey = new CompanyKey().KeyValue,
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2),
                };

                //Act
                parameters.ContractItems = null;
                var resultNull = Service.UpdateCustomerContract(parameters);

                parameters.ContractItems = new IContractItem[0];
                var resultEmpty = Service.UpdateCustomerContract(parameters);

                //Assert
                resultNull.AssertNotSuccess(UserMessages.ContractItemsRequired);
                resultEmpty.AssertNotSuccess(UserMessages.ContractItemsRequired);
            }

            [Test]
            public void Returns_non_sucessful_result_if_any_ContractItem_has_quantity_less_than_or_equal_to_zero()
            {
                //Arrange
                var parameters = new Params
                {
                    ContractKey = new ContractKey().KeyValue,
                    CustomerKey = new CustomerKey().KeyValue,
                    BrokerKey = new CompanyKey().KeyValue,
                    DefaultPickFromWarehouseKey = new FacilityKey().KeyValue,
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2),
                    ContractItems = new List<IContractItem>
                        {
                            new ContractItemParameters
                            {
                                ChileProductKey = new ChileProductKey().KeyValue,
                                PackagingProductKey = new PackagingProductKey().KeyValue,
                                TreatmentKey = new InventoryTreatmentKey().KeyValue,
                            }
                        }
                };

                //Act
                (parameters.ContractItems.Single() as ContractItemParameters).Quantity = -1;
                var resultLessThan0 = Service.UpdateCustomerContract(parameters);

                (parameters.ContractItems.Single() as ContractItemParameters).Quantity = 0;
                var resultEqual0 = Service.UpdateCustomerContract(parameters);

                //Assert
                resultLessThan0.AssertNotSuccess(UserMessages.QuantityNotGreaterThanZero);
                resultEqual0.AssertNotSuccess(UserMessages.QuantityNotGreaterThanZero);
            }

            [Test]
            public void Returns_non_successful_result_if_TermBegin_date_is_greater_than_or_equal_to_TermEnd()
            {
                //Arrange
                var parameters = new Params
                {
                    ContractKey = new ContractKey().KeyValue,
                    CustomerKey = new CustomerKey().KeyValue,
                    BrokerKey = new CompanyKey().KeyValue,
                    ContractItems = new List<IContractItem>
                            {
                                new ContractItemParameters
                                    {
                                        ChileProductKey = new ChileProductKey().KeyValue,
                                        PackagingProductKey = new PackagingProductKey().KeyValue,
                                        TreatmentKey = new InventoryTreatmentKey().KeyValue,
                                        Quantity = 10
                                    }
                            }
                };

                //Act
                parameters.TermBegin = new DateTime(2012, 3, 2);
                parameters.TermEnd = new DateTime(2012, 3, 1);
                var resultGreaterThan = Service.UpdateCustomerContract(parameters);

                parameters.TermBegin = new DateTime(2012, 3, 2);
                parameters.TermEnd = new DateTime(2012, 3, 2);
                var resultEqualTo = Service.UpdateCustomerContract(parameters);

                //Assert
                resultGreaterThan.AssertNotSuccess(UserMessages.ContractTermMustBeginBeforeEnd);
                resultEqualTo.AssertNotSuccess(UserMessages.ContractTermMustBeginBeforeEnd);
            }

            [Test]
            public void Returns_non_sucessful_result_if_Contract_could_not_be_found()
            {
                //Act
                var result = Service.UpdateCustomerContract(new Params
                {
                    ContractKey = new ContractKey().KeyValue,
                    CustomerKey = new CustomerKey().KeyValue,
                    BrokerKey = new CompanyKey().KeyValue,
                    DefaultPickFromWarehouseKey = new FacilityKey().KeyValue,
                    ContractItems = new List<IContractItem>
                            {
                                new ContractItemParameters
                                    {
                                        ChileProductKey = new ChileProductKey().KeyValue,
                                        PackagingProductKey = new PackagingProductKey().KeyValue,
                                        TreatmentKey = new InventoryTreatmentKey().KeyValue,
                                        Quantity = 10
                                    }
                            },
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2)
                });

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerContractNotFound);
            }

            [Test]
            public void Returns_non_sucessful_result_if_the_new_Broker_is_not_currently_associated_with_the_Customer()
            {
                //Arrange
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));

                //Act
                var result = Service.UpdateCustomerContract(new Params
                {
                    ContractKey = new ContractKey(contract).KeyValue,
                    CustomerKey = new CustomerKey(contract).KeyValue,
                    BrokerKey = new CompanyKey(broker).KeyValue,
                    DefaultPickFromWarehouseKey = new FacilityKey(contract).KeyValue,
                    ContractItems = new List<IContractItem>
                                {
                                    new ContractItemParameters
                                        {
                                            ChileProductKey = new ChileProductKey().KeyValue,
                                            PackagingProductKey = new PackagingProductKey().KeyValue,
                                            TreatmentKey = new InventoryTreatmentKey().KeyValue,
                                            Quantity = 10
                                        }
                                },
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2)
                });

                //Assert
                result.AssertNotSuccess(UserMessages.ContractBrokerIsNotOfCustomer);
            }

            [Test]
            public void Will_update_Contract_record_as_expected_on_success()
            {
                //Arrange
                const ContractType expectedContractType = ContractType.Contract;
                const ContractStatus expectedStatus = ContractStatus.Confirmed;
                var expectedTermBegin = new DateTime(2012, 3, 1);
                var expectedTermEnd = new DateTime(2012, 3, 2);
                const string paymentTerms = "cash money";
                const string expectedNotesToPrint = "It is IMPERATIVE that these notes not be printed.";
                var expectedAddress = new Address
                {
                    AddressLine1 = "Line1",
                    City = "Badgers"
                };

                var contractKey = new ContractKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractType = ContractType.Quote, c => c.ContractStatus = ContractStatus.Pending));
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker), c => c.Company.SetCompanyTypes(CompanyType.Customer));
                var customerKey = new CustomerKey((ICustomerKey)customer);
                var brokerKey = new CompanyKey(customer.Broker);
                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>());

                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.UpdateCustomerContract(new Params
                {
                    UserToken = TestUser.UserName,
                    ContractKey = contractKey.KeyValue,
                    CustomerKey = customerKey.KeyValue,

                    ContactAddress = expectedAddress,
                    BrokerKey = brokerKey.KeyValue,
                    DefaultPickFromWarehouseKey = warehouseKey.KeyValue,

                    ContractType = expectedContractType,
                    ContractStatus = expectedStatus,
                    PaymentTerms = paymentTerms,
                    ContractItems = new List<IContractItem>
                            {
                                new ContractItemParameters
                                    {
                                        ChileProductKey = chileProductKey.KeyValue,
                                        PackagingProductKey = packagingProductKey.KeyValue,
                                        TreatmentKey = treatmentKey.KeyValue,
                                        Quantity = 10
                                    }
                            },
                    TermBegin = expectedTermBegin,
                    TermEnd = expectedTermEnd,
                    NotesToPrint = expectedNotesToPrint
                });

                //Assert
                result.AssertSuccess();
                var contract = RVCUnitOfWork.ContractRepository.Filter(contractKey.FindByPredicate, c => c.Broker).Single();
                Assert.AreEqual(customerKey, contract);
                contract.ContactAddress.AssertEqual(expectedAddress);
                Assert.AreEqual(brokerKey, contract.Broker);
                Assert.AreEqual(warehouseKey, contract);
                Assert.AreEqual(expectedContractType, contract.ContractType);
                Assert.AreEqual(expectedStatus, contract.ContractStatus);
                Assert.AreEqual(paymentTerms, contract.PaymentTerms);
                Assert.AreEqual(expectedTermBegin, contract.TermBegin);
                Assert.AreEqual(expectedTermEnd, contract.TermEnd);
                Assert.AreEqual(expectedNotesToPrint, contract.NotesToPrint);
            }

            [Test]
            public void Will_not_update_Warehouse_if_no_key_is_supplied()
            {
                //Arrange
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractType = ContractType.Quote, c => c.ContractStatus = ContractStatus.Pending);
                var warehouseKey = new FacilityKey(contract);
                var contractKey = new ContractKey(contract);
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker), c => c.Company.SetCompanyTypes(CompanyType.Customer));
                var customerKey = new CustomerKey((ICustomerKey)customer);
                var brokerKey = new CompanyKey(customer.Broker);
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.UpdateCustomerContract(new Params
                {
                    UserToken = TestUser.UserName,
                    ContractKey = contractKey.KeyValue,
                    CustomerKey = customerKey.KeyValue,

                    ContactAddress = new Address
                    {
                        AddressLine1 = "Line1",
                        City = "Badgers"
                    },
                    BrokerKey = brokerKey.KeyValue,
                    DefaultPickFromWarehouseKey = warehouseKey.KeyValue,

                    ContractType = ContractType.Contract,
                    ContractStatus = ContractStatus.Confirmed,
                    PaymentTerms = "cash money",
                    ContractItems = new List<IContractItem>
                            {
                                new ContractItemParameters
                                    {
                                        ChileProductKey = chileProductKey.KeyValue,
                                        PackagingProductKey = packagingProductKey.KeyValue,
                                        TreatmentKey = treatmentKey.KeyValue,
                                        Quantity = 10
                                    }
                            },
                    TermBegin = new DateTime(2012, 3, 1),
                    TermEnd = new DateTime(2012, 3, 2),
                    NotesToPrint = "It is IMPERATIVE that these notes not be printed."
                });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(warehouseKey, RVCUnitOfWork.ContractRepository.FindByKey(contractKey));
            }

            [Test]
            public void Will_set_ContractItem_records_as_expected_on_success()
            {
                //Arrange
                const int expectedItems = 2;

                const string expectedCode0 = "CHILEE01";
                const int quantity0 = 10;
                const bool useCustomerSpec0 = false;
                const double priceBase0 = 3.50;
                const double priceFreight0 = 0.10;
                const double priceTreatment0 = 0.20;
                const double priceWarehouse0 = 0.30;
                const double priceRbate0 = -0.01;

                const string expectedCode1 = "CODE OVERRIDE";
                const int quantity1 = 22;
                const bool useCustomerSpec1 = true;
                const double priceBase1 = 3.51;
                const double priceFreight1 = 1.21;
                const double priceTreatment1 = 1.31;
                const double priceWarehouse1 = 1.41;
                const double priceRbate1 = -1.11;

                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.Company.SetCompanyTypes(CompanyType.Customer));
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, customer.Broker).ContractStatus = ContractStatus.Pending, c => c.TermEnd = c.TermBegin.Value.AddDays(3));
                var contractKey = new ContractKey(contract);

                var chileProductKey0 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey0 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                var chileProductKey1 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var packagingProductKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var treatmentKey1 = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());

                //Act
                var result = Service.UpdateCustomerContract(new Params
                {
                    UserToken = TestUser.UserName,
                    ContractKey = contractKey.KeyValue,
                    CustomerKey = new CustomerKey(contract).KeyValue,
                    BrokerKey = new CompanyKey(contract.Broker).KeyValue,
                    DefaultPickFromWarehouseKey = new FacilityKey(contract).KeyValue,
                    TermBegin = contract.TermBegin,
                    TermEnd = contract.TermEnd,
                    ContractItems = new List<IContractItem>
                        {
                            new ContractItemParameters
                            {
                                ChileProductKey = chileProductKey0.KeyValue,
                                PackagingProductKey = packagingProductKey0.KeyValue,
                                TreatmentKey = treatmentKey0.KeyValue,
                                Quantity = quantity0,
                                UseCustomerSpec = useCustomerSpec0,
                                CustomerCodeOverride = expectedCode0,
                                PriceBase = priceBase0,
                                PriceFreight = priceFreight0,
                                PriceTreatment = priceTreatment0,
                                PriceWarehouse = priceWarehouse0,
                                PriceRebate = priceRbate0
                            },

                            new ContractItemParameters
                            {
                                ChileProductKey = chileProductKey1.KeyValue,
                                PackagingProductKey = packagingProductKey1.KeyValue,
                                TreatmentKey = treatmentKey1.KeyValue,
                                Quantity = quantity1,
                                UseCustomerSpec = useCustomerSpec1,
                                CustomerCodeOverride = expectedCode1,
                                PriceBase = priceBase1,
                                PriceFreight = priceFreight1,
                                PriceTreatment = priceTreatment1,
                                PriceWarehouse = priceWarehouse1,
                                PriceRebate = priceRbate1
                            }
                        }
                });

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.ContractRepository.FindByKey(contractKey, c => c.ContractItems).ContractItems.ToList();
                Assert.AreEqual(expectedItems, items.Count);

                var item = items.Single(i => chileProductKey0.Equals(i));
                Assert.AreEqual(packagingProductKey0, item);
                Assert.AreEqual(treatmentKey0, item);
                Assert.AreEqual(quantity0, item.Quantity);
                Assert.AreEqual(useCustomerSpec0, item.UseCustomerSpec);
                Assert.AreEqual(expectedCode0, item.CustomerProductCode);
                Assert.AreEqual(priceBase0, item.PriceBase);
                Assert.AreEqual(priceFreight0, item.PriceFreight);
                Assert.AreEqual(priceTreatment0, item.PriceTreatment);
                Assert.AreEqual(priceWarehouse0, item.PriceWarehouse);
                Assert.AreEqual(priceRbate0, item.PriceRebate);

                item = items.Single(i => chileProductKey1.Equals(i));
                Assert.AreEqual(packagingProductKey1, item);
                Assert.AreEqual(treatmentKey1, item);
                Assert.AreEqual(quantity1, item.Quantity);
                Assert.AreEqual(useCustomerSpec1, item.UseCustomerSpec);
                Assert.AreEqual(expectedCode1, item.CustomerProductCode);
                Assert.AreEqual(priceBase1, item.PriceBase);
                Assert.AreEqual(priceFreight1, item.PriceFreight);
                Assert.AreEqual(priceTreatment1, item.PriceTreatment);
                Assert.AreEqual(priceWarehouse1, item.PriceWarehouse);
                Assert.AreEqual(priceRbate1, item.PriceRebate);
            }

            private class Params : IUpdateCustomerContractParameters
            {
                public string UserToken { get; set; }
                public string ContractKey { get; internal set; }
                public string CustomerKey { get; internal set; }
                public string ContactName { get; internal set; }
                public string FOB { get; internal set; }
                public string DefaultPickFromWarehouseKey { get; internal set; }
                public Address ContactAddress { get; internal set; }
                public string BrokerKey { get; internal set; }
                public ContractType ContractType { get; internal set; }
                public ContractStatus ContractStatus { get; internal set; }
                public string PaymentTerms { get; internal set; }
                public string CustomerPurchaseOrder { get; internal set; }
                public DateTime ContractDate { get; internal set; }
                public DateTime? TermBegin { get; internal set; }
                public DateTime? TermEnd { get; internal set; }
                public string NotesToPrint { get; internal set; }
                public IEnumerable<IContractItem> ContractItems { get; internal set; }
            }
        }

        [TestFixture]
        public class SetContractsStatus : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_any_Contract_could_not_be_found()
            {
                //Arrange
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();

                //Act
                var result = Service.SetCustomerContractsStatus(new SetContractsStatusParameters
                    {
                        ContractKeys = new string[]
                            {
                                contract.ToContractKey(),
                                new ContractKey()
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerContractNotFound);
            }

            [Test]
            public void Sets_Contracts_statuses_as_expected()
            {
                //Arrange
                var contractKeys = new List<ContractKey>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractStatus = ContractStatus.Completed).ToContractKey(),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractStatus = ContractStatus.Pending).ToContractKey(),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractStatus = ContractStatus.Confirmed).ToContractKey()
                    };

                //Act
                var parameters = new SetContractsStatusParameters
                    {
                        ContractStatus = ContractStatus.Rejected,
                        ContractKeys = contractKeys.Select(k => k.KeyValue)
                    };
                var result = Service.SetCustomerContractsStatus(parameters);

                //Assert
                result.AssertSuccess();

                foreach(var key in contractKeys)
                {
                    var contract = RVCUnitOfWork.ContractRepository.FindByKey(key);
                    Assert.AreEqual(parameters.ContractStatus, contract.ContractStatus);
                }
            }
        }

        [TestFixture]
        public class RemoveCustomerContract : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Contract_does_not_exist()
            {
                //Act
                var result = Service.RemoveCustomerContract(new ContractKey(TestHelper.CreateObjectGraph<Contract>()));

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerContractNotFound);
            }

            [Test]
            public void Return_non_successful_result_if_ContractItems_have_associated_order_items()
            {
                //Arrange
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractItems = new List<ContractItem>
                    {
                        TestHelper.CreateObjectGraph<ContractItem>(),
                        TestHelper.CreateObjectGraph<ContractItem>(i => i.OrderItems = new List<SalesOrderItem>
                            {
                                TestHelper.CreateObjectGraph<SalesOrderItem>()
                            }),
                        TestHelper.CreateObjectGraph<ContractItem>()
                    });
                var contractKey = new ContractKey(contract);

                //Act
                var result = Service.RemoveCustomerContract(contractKey);

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerContractHasOrderItems);
            }

            [Test]
            public void Removes_Contract_and_item_records_on_success()
            {
                //Arrange
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractItems = new List<ContractItem>
                    {
                        TestHelper.CreateObjectGraph<ContractItem>(),
                        TestHelper.CreateObjectGraph<ContractItem>(),
                        TestHelper.CreateObjectGraph<ContractItem>()
                    });
                var contractKey = new ContractKey(contract);

                //Act
                var result = Service.RemoveCustomerContract(contractKey);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.ContractRepository.FindByKey(contractKey));
                contract.ContractItems.ForEach(i => Assert.IsNull(RVCUnitOfWork.ContractItemRepository.FindByKey(new ContractItemKey(i))));
            }

            [Test]
            public void Removes_Comments_Notebook_and_Notes_on_success()
            {
                //Arrange
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.Comments.Notes = new List<Note>
                    {
                        TestHelper.CreateObjectGraph<Note>(),
                        TestHelper.CreateObjectGraph<Note>(),
                        TestHelper.CreateObjectGraph<Note>()
                    });
                var contractKey = new ContractKey(contract);
                var notebookKey = new NotebookKey(contract);

                //Act
                var result = Service.RemoveCustomerContract(contractKey);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.NotebookRepository.FindByKey(notebookKey));
                Assert.IsEmpty(RVCUnitOfWork.NoteRepository.Filter(n => n.NotebookDate == notebookKey.NotebookKey_Date && n.NotebookSequence == notebookKey.NotebookKey_Sequence));
            }
        }

        [TestFixture]
        public class GetCustomerContract : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Contract_could_not_be_found()
            {
                //Act
                StartStopwatch();
                var result = Service.GetCustomerContract(new ContractKey().KeyValue);
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerContractNotFound);
            }

            [Test]
            public void Returns_Contract_as_expected()
            {
                //Arrange
                var contractDate = new DateTime(2012, 3, 29);
                var termBegin = new DateTime(2012, 1, 2);
                var termEnd = new DateTime(2222, 2, 2);
                const ContractType contractType = ContractType.Interim;
                const ContractStatus contractStatus = ContractStatus.Confirmed;
                const string paymentTerms = "20 fish net";
                const string notesToPrint = "Hi mom!";
                const string comments = "Very good.";

                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(
                    c => c.ContractDate = contractDate,
                    c => c.TermBegin = termBegin,
                    c => c.TermEnd = termEnd,
                    c => c.ContractType = contractType,
                    c => c.ContractStatus = contractStatus,
                    c => c.PaymentTerms = paymentTerms,
                    c => c.NotesToPrint = notesToPrint,
                    c => c.Comments.Notes = new List<Note> { TestHelper.CreateObjectGraph<Note>(n => n.Text = comments) });
                var contractKey = new ContractKey(contract);

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContract(contractKey.KeyValue);
                StopWatchAndWriteTime();

                //Asssert
                result.AssertSuccess();

                var contractResult = result.ResultingObject;
                Assert.AreEqual(contractKey.KeyValue, contractResult.CustomerContractKey);
                Assert.AreEqual(contractDate, contractResult.ContractDate);
                Assert.AreEqual(termBegin, contractResult.TermBegin);
                Assert.AreEqual(termEnd, contractResult.TermEnd);
                Assert.AreEqual(contractType, contractResult.ContractType);
                Assert.AreEqual(paymentTerms, contractResult.PaymentTerms);
                Assert.AreEqual(contractStatus, contractResult.ContractStatus);
                Assert.AreEqual(paymentTerms, contractResult.PaymentTerms);
                Assert.AreEqual(notesToPrint, contractResult.NotesToPrint);
                Assert.AreEqual(comments, contractResult.Comments.Notes.First().Text);
                Assert.AreEqual(contract.ContactName, contractResult.ContactName);
                Assert.AreEqual(contract.FOB, contractResult.FOB);

                contract.DefaultPickFromFacility.AssertEqual(contractResult.DefaultPickFromFacility);
                contract.ContactAddress.AssertEqual(contractResult.ContactAddress);
            }

            [Test]
            public void Returns_ContractItems_as_expected()
            {
                //Arrange
                const int expectedItems = 2;
                var contractKey = new ContractKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>());
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contractKey));
                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contractKey));

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContract(contractKey.KeyValue);
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();

                var items = result.ResultingObject.ContractItems.ToList();
                Assert.AreEqual(expectedItems, items.Count);
                contractItem0.AssertEqual(items.Single(i => new ContractItemKey(contractItem0).KeyValue == i.ContractItemKey));
                contractItem1.AssertEqual(items.Single(i => new ContractItemKey(contractItem1).KeyValue == i.ContractItemKey));
            }
        }

        [TestFixture]
        public class GetCustomerContracts : SalesServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_records_exist_in_database()
            {
                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(results);
            }

            [Test]
            public void Returns_summary_Contract_results_as_expected()
            {
                //Arrange
                StartStopwatch();

                const int expectedContracts = 3;
                var contract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var contract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var contract2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedContracts, results.Count);
                contract0.AssertEqual(results.Single(r => r.CustomerContractKey == new ContractKey(contract0).KeyValue));
                contract1.AssertEqual(results.Single(r => r.CustomerContractKey == new ContractKey(contract1).KeyValue));
                contract2.AssertEqual(results.Single(r => r.CustomerContractKey == new ContractKey(contract2).KeyValue));
            }

            [Test]
            public void Returns_TotalWeight_property_results_as_expected()
            {
                //Arrange
                StartStopwatch();

                const int quantity0_0 = 10;
                const int quantity0_1 = 20;
                const int quantity0_2 = 5;
                const double weight0_0 = 10.0;
                const double weight0_1 = 21.0;
                const double weight0_2 = 5.5;
                const double expectedTotalWeight0 = (quantity0_0 * weight0_0) + (quantity0_1 * weight0_1) + (quantity0_2 * weight0_2);

                const int quantity1_0 = 11;
                const int quantity1_1 = 22;
                const int quantity1_2 = 53;
                const double weight1_0 = 14.0;
                const double weight1_1 = 25.0;
                const double weight1_2 = 6.7;
                const double expectedTotalWeight1 = (quantity1_0 * weight1_0) + (quantity1_1 * weight1_1) + (quantity1_2 * weight1_2);

                var contract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contract0).Quantity = quantity0_0, c => c.PackagingProduct.Weight = weight0_0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contract0).Quantity = quantity0_1, c => c.PackagingProduct.Weight = weight0_1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contract0).Quantity = quantity0_2, c => c.PackagingProduct.Weight = weight0_2);

                var contract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contract1).Quantity = quantity1_0, c => c.PackagingProduct.Weight = weight1_0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contract1).Quantity = quantity1_1, c => c.PackagingProduct.Weight = weight1_1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contract1).Quantity = quantity1_2, c => c.PackagingProduct.Weight = weight1_2);

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedTotalWeight0, results.Single(r => r.CustomerContractKey == new ContractKey(contract0).KeyValue).SumWeight);
                Assert.AreEqual(expectedTotalWeight1, results.Single(r => r.CustomerContractKey == new ContractKey(contract1).KeyValue).SumWeight);
            }

            [Test]
            public void Returns_Contracts_as_expected_when_filtered_by_CustomerKey()
            {
                //Arrange
                StartStopwatch();

                const int expectedResults = 2;

                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Contracts = null, c => c.Company.Contacts = new List<Contact>
                    {
                        TestHelper.CreateObjectGraph<Contact>(n => n.Company = null, n => n.Addresses = new List<ContactAddress> { TestHelper.CreateObjectGraph<ContactAddress>(a => a.Contact = null) })
                    });
                var expectedContract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, customer.Company.Contacts.First().Addresses.First()));
                var expectedContract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, customer.Company.Contacts.First().Addresses.First()));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts(new FilterCustomerContractsParameters { CustomerKey = new CustomerKey((ICustomerKey)customer).KeyValue });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract0).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract1).KeyValue));
            }

            [Test]
            public void Returns_Contracts_as_expected_when_filtered_by_BrokerKey()
            {
                //Arrange
                StartStopwatch();

                const int expectedResults = 2;

                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));
                var expectedContract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(null, broker));
                var expectedContract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(null, broker));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts(new FilterCustomerContractsParameters { BrokerKey = new CompanyKey(broker).KeyValue });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract0).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract1).KeyValue));
            }

            [Test]
            public void Returns_Contracts_as_expected_when_filtered_by_ContractStatus()
            {
                //Arrange
                StartStopwatch();

                const int expectedResults = 2;
                const ContractStatus status = ContractStatus.Rejected;

                var expectedContract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractStatus = status);
                var expectedContract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractStatus = status);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractStatus = ContractStatus.Confirmed);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractStatus = ContractStatus.Pending);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ContractStatus = ContractStatus.Completed);

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts(new FilterCustomerContractsParameters { ContractStatus = status });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract0).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract1).KeyValue));
            }

            [Test]
            public void Returns_Contracts_as_expected_when_filtered_by_TermBeginRangeStart()
            {
                //Arrange
                StartStopwatch();

                const int expectedResults = 2;
                var rangeStart = new DateTime(2012, 3, 29);

                var expectedContract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeStart);
                var expectedContract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeStart.AddDays(1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeStart.AddDays(-1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeStart.AddDays(-2));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeStart.AddDays(-3));

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts(new FilterCustomerContractsParameters { TermBeginRangeStart = rangeStart });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract0).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract1).KeyValue));
            }

            [Test]
            public void Returns_Contracts_as_expected_when_filtered_by_TermBeginRangeEnd()
            {
                //Arrange
                StartStopwatch();

                const int expectedResults = 2;
                var rangeEnd = new DateTime(2012, 3, 29);

                var expectedContract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeEnd);
                var expectedContract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeEnd.AddDays(-1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeEnd.AddDays(1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeEnd.AddDays(2));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeEnd.AddDays(3));

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts(new FilterCustomerContractsParameters { TermBeginRangeEnd = rangeEnd });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract0).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract1).KeyValue));
            }

            [Test]
            public void Returns_Contracts_as_expected_when_filtered_by_TermBeginRangeStart_and_TermBeginRangeEnd()
            {
                //Arrange
                StartStopwatch();

                const int expectedResults = 3;
                var rangeStart = new DateTime(2012, 1, 1);
                var rangeEnd = new DateTime(2012, 3, 29);

                var expectedContract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeStart);
                var expectedContract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeStart.AddDays(10));
                var expectedContract2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeEnd);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeStart.AddDays(-1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermBegin = rangeEnd.AddDays(1));

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts(new FilterCustomerContractsParameters
                {
                    TermBeginRangeStart = rangeStart,
                    TermBeginRangeEnd = rangeEnd
                });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract0).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract1).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract2).KeyValue));
            }

            [Test]
            public void Returns_Contracts_as_expected_when_using_a_combination_of_filters()
            {
                //Arrange
                StartStopwatch();

                const int expectedResults = 3;
                const ContractStatus contractStatus = ContractStatus.Confirmed;
                var rangeStart = new DateTime(2012, 1, 1);
                var rangeEnd = new DateTime(2012, 3, 29);

                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker),
                    c => c.Company.Contacts = new List<Contact>
                        {
                            TestHelper.CreateObjectGraph<Contact>(n => n.Company = null, n => n.Addresses = new List<ContactAddress> { TestHelper.CreateObjectGraph<ContactAddress>(a => a.Contact = null) })
                        });
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));

                var expectedContract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, broker).SetTermDates(rangeStart, rangeEnd).ContractStatus = contractStatus);
                var expectedContract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, broker).SetTermDates(rangeStart.AddDays(10), rangeEnd).ContractStatus = contractStatus);
                var expectedContract2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, broker).SetTermDates(rangeEnd, rangeEnd.AddDays(10)).ContractStatus = contractStatus);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(null, broker).SetTermDates(rangeStart, rangeEnd).ContractStatus = contractStatus);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer).SetTermDates(rangeStart, rangeEnd).ContractStatus = contractStatus);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, broker).SetTermDates(rangeStart, rangeEnd).ContractStatus = ContractStatus.Pending);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, broker).SetTermDates(rangeStart.AddDays(-1), rangeEnd).ContractStatus = contractStatus);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customer, broker).SetTermDates(rangeEnd.AddDays(1), rangeEnd.AddDays(20)).ContractStatus = contractStatus);

                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetCustomerContracts(new FilterCustomerContractsParameters
                {
                    CustomerKey = new CustomerKey((ICustomerKey)customer).KeyValue,
                    BrokerKey = new CompanyKey(broker).KeyValue,
                    ContractStatus = contractStatus,
                    TermBeginRangeStart = rangeStart,
                    TermBeginRangeEnd = rangeEnd
                });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract0).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract1).KeyValue));
                Assert.IsNotNull(results.Single(r => r.CustomerContractKey == new ContractKey(expectedContract2).KeyValue));
            }
        }

        [TestFixture]
        public class GetDistinctContractPaymentTerms : SalesServiceTests
        {
            [Test]
            public void Returns_PaymentTerms_as_expected_on_success()
            {
                //Arrange
                const int expectedTerms = 3;
                const string terms0 = "Terms 0";
                const string terms1 = "Terms 1";
                const string terms2 = "Terms 2";

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.PaymentTerms = terms0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.PaymentTerms = terms0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.PaymentTerms = terms1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.PaymentTerms = terms1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.PaymentTerms = terms2);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.PaymentTerms = terms2);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.PaymentTerms = "");
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.PaymentTerms = null);

                //Act
                var result = Service.GetDistinctContractPaymentTerms();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(expectedTerms, results.Count);
                Assert.AreEqual(1, results.Count(t => t == terms0));
                Assert.AreEqual(1, results.Count(t => t == terms1));
                Assert.AreEqual(1, results.Count(t => t == terms2));
            }
        }

        [TestFixture]
        public class GetOrdersForCustomerContract : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Contract_could_not_be_found()
            {
                //Act
                var result = Service.GetOrdersForCustomerContract(new ContractKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerContractNotFound);
            }

            [Test]
            public void Returns_result_with_only_SalesOrders_that_have_a_SalesOrderItem_referencing_a_ContractItem_of_the_Contract()
            {
                //Arrange
                const int expectedOrders = 3;

                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));
                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));

                var order0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(order0, contractItem0));

                var order1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(order1, contractItem0));

                var order2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(order2, contractItem1));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                //Act
                var result = Service.GetOrdersForCustomerContract(contract.ToContractKey());

                //Assert
                result.AssertSuccess();

                var orders = result.ResultingObject.Orders.ToList();
                Assert.AreEqual(expectedOrders, orders.Count);
                Assert.IsNotNull(orders.Single(o => o.CustomerOrderKey == order0.ToSalesOrderKey().KeyValue));
                Assert.IsNotNull(orders.Single(o => o.CustomerOrderKey == order1.ToSalesOrderKey().KeyValue));
                Assert.IsNotNull(orders.Single(o => o.CustomerOrderKey == order2.ToSalesOrderKey().KeyValue));
            }

            [Test]
            public void Returns_result_with_SalesOrder_as_expected()
            {
                //Arrange
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.SalesOrderItems = new List<Data.Models.SalesOrderItem>
                    {
                        TestHelper.CreateObjectGraph<SalesOrderItem>(i => i.ConstrainByKeys(null, contractItem0))
                    });

                //Act
                var result = Service.GetOrdersForCustomerContract(contract.ToContractKey());

                //Assert
                result.AssertSuccess();
                order.AssertEqual(result.ResultingObject.Orders.Single());
            }

            [Test]
            public void Returns_result_with_SalesOrder_with_only_Items_that_reference_a_ContractItem_for_the_Contract()
            {
                //Arrange
                const int expectedItems = 2;

                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var contractItemKey0 = new ContractItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract)));
                var contractItemKey1 = new ContractItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract)));

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var orderItemKey0 = new SalesOrderItemKey((ISalesOrderItemKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Data.Models.SalesOrderItem>(i => i.ConstrainByKeys(order, contractItemKey0)));
                var orderItemKey1 = new SalesOrderItemKey((ISalesOrderItemKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Data.Models.SalesOrderItem>(i => i.ConstrainByKeys(order, contractItemKey1)));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(order));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(order));

                //Act
                var result = Service.GetOrdersForCustomerContract(contract.ToContractKey());

                //Assert
                result.AssertSuccess();

                var items = result.ResultingObject.Orders.Single().Items.ToList();
                Assert.AreEqual(expectedItems, items.Count);
                Assert.IsNotNull(items.Single(i => i.OrderItemKey == orderItemKey0.KeyValue && i.ContractItemKey == contractItemKey0.KeyValue));
                Assert.IsNotNull(items.Single(i => i.OrderItemKey == orderItemKey1.KeyValue && i.ContractItemKey == contractItemKey1.KeyValue));
            }

            [Test]
            public void Returns_result_with_CustomerContractOrderItem_Quantities_and_Weights_of_zero_if_no_items_have_been_picked_for_them()
            {
                //Arrange
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var contractItemKey0 = new ContractItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract)));
                var contractItemKey1 = new ContractItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract)));

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var orderItemKey0 = new SalesOrderItemKey((ISalesOrderItemKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(order, contractItemKey0)));
                var orderItemKey1 = new SalesOrderItemKey((ISalesOrderItemKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(order, contractItemKey1)));

                //Act
                var result = Service.GetOrdersForCustomerContract(new ContractKey(contract).KeyValue);

                //Assert
                result.AssertSuccess();

                var items = result.ResultingObject.Orders.Single().Items.ToList();

                var item = items.Single(i => i.OrderItemKey == orderItemKey0.KeyValue && i.ContractItemKey == contractItemKey0.KeyValue);
                Assert.AreEqual(0, item.TotalQuantityPicked);
                Assert.AreEqual(0, item.TotalWeightPicked);

                item = items.Single(i => i.OrderItemKey == orderItemKey1.KeyValue && i.ContractItemKey == contractItemKey1.KeyValue);
                Assert.AreEqual(0, item.TotalQuantityPicked);
                Assert.AreEqual(0, item.TotalWeightPicked);
            }

            [Test]
            public void Returns_result_with_CustomerContractOrderItems_as_expected()
            {
                //Arrange
                const int expectedItems = 2;

                const int weight0 = 100;
                const int quantity0_0 = 10;
                const int quantity0_1 = 20;
                const int totalQuantity0 = quantity0_0 + quantity0_1;
                const int expectedWeight0 = weight0 * totalQuantity0;
                const double priceBase0 = 1.0, priceFreight0 = 0.5, priceTreatment0 = 0.2, priceWarehouse0 = 0.1, priceRebate0 = 0.22;

                const int weight1 = 220;
                const int quantity1_0 = 22;
                const int quantity1_1 = 11;
                const int totalQuantity1 = quantity1_0 + quantity1_1;
                const int expectedWeight1 = weight1 * totalQuantity1;
                const double priceBase1 = 1.1, priceFreight1 = 1.5, priceTreatment1 = 1.2, priceWarehouse1 = 1.1, priceRebate1 = 1.22;

                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var contractItemKey0 = new ContractItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract)));
                var contractItemKey1 = new ContractItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract)));

                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                var orderItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(
                    i => i.ConstrainByKeys(order, contractItemKey0).InventoryPickOrderItem.PackagingProduct.Weight = weight0,
                    i => i.SetPrices(priceBase0, priceFreight0, priceTreatment0, priceWarehouse0, priceRebate0));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(p => p.SetToCustomerOrderItem(orderItem0).PickedInventoryItem.Quantity = quantity0_0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(p => p.SetToCustomerOrderItem(orderItem0).PickedInventoryItem.Quantity = quantity0_1);

                var orderItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(
                    i => i.ConstrainByKeys(order, contractItemKey1).InventoryPickOrderItem.PackagingProduct.Weight = weight1,
                    i => i.SetPrices(priceBase1, priceFreight1, priceTreatment1, priceWarehouse1, priceRebate1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(p => p.SetToCustomerOrderItem(orderItem1).PickedInventoryItem.Quantity = quantity1_0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(p => p.SetToCustomerOrderItem(orderItem1).PickedInventoryItem.Quantity = quantity1_1);

                //Act
                var result = Service.GetOrdersForCustomerContract(new ContractKey(contract).KeyValue);

                //Assert
                result.AssertSuccess();

                var items = result.ResultingObject.Orders.Single().Items.ToList();
                Assert.AreEqual(expectedItems, items.Count);

                var item = items.Single(i => i.ContractItemKey == contractItemKey0.KeyValue);
                orderItem0.AssertEqual(item);
                Assert.AreEqual(totalQuantity0, item.TotalQuantityPicked);
                Assert.AreEqual(expectedWeight0, item.TotalWeightPicked);

                item = items.Single(i => i.ContractItemKey == contractItemKey1.KeyValue);
                orderItem1.AssertEqual(item);
                Assert.AreEqual(totalQuantity1, item.TotalQuantityPicked);
                Assert.AreEqual(expectedWeight1, item.TotalWeightPicked);
            }
        }

        [TestFixture]
        public class GetContractShipmentSummary : SalesServiceTests
        {
            protected override bool SetupStaticRecords { get { return false; } }

            [Test]
            public void Returns_non_successful_result_if_Contract_could_not_be_found()
            {
                //Act
                StartStopwatch();
                var result = Service.GetContractShipmentSummary(new ContractKey());
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerContractNotFound);
            }

            [Test]
            public void Returns_ContractShipmentSummary_as_expected()
            {
                //Act
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>();
                var contractKey = new ContractKey(contract);

                var item0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract).PackagingProduct.Weight = 10, i => i.Quantity = 20);
                var item0Key = new ContractItemKey(item0);

                var item1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract).PackagingProduct.Weight = 5, i => i.Quantity = 100,
                    i => i.OrderItems = new List<SalesOrderItem>
                        {
                            TestHelper.CreateObjectGraph<SalesOrderItem>(o => o.PickedItems = new List<SalesOrderPickedItem>()),
                            TestHelper.CreateObjectGraph<SalesOrderItem>(o => o.Order.OrderStatus = SalesOrderStatus.Ordered, o => o.Order.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Shipped,
                                o => o.PickedItems = new List<SalesOrderPickedItem>
                                    {
                                        TestHelper.CreateObjectGraph<SalesOrderPickedItem>(p => p.PickedInventoryItem.PackagingProduct.Weight = 1, p => p.PickedInventoryItem.Quantity = 100),
                                        TestHelper.CreateObjectGraph<SalesOrderPickedItem>(p => p.PickedInventoryItem.PackagingProduct.Weight = 2, p => p.PickedInventoryItem.Quantity = 50),
                                    }),
                            TestHelper.CreateObjectGraph<SalesOrderItem>(o => o.Order.OrderStatus = SalesOrderStatus.Ordered, o => o.Order.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Unscheduled,
                                o => o.PickedItems = new List<SalesOrderPickedItem>
                                    {
                                        TestHelper.CreateObjectGraph<SalesOrderPickedItem>(p => p.PickedInventoryItem.PackagingProduct.Weight = 25, p => p.PickedInventoryItem.Quantity = 2),
                                    }),
                        });
                var item1Key = new ContractItemKey(item1);

                //Act
                StartStopwatch();
                var result = Service.GetContractShipmentSummary(contractKey);
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(result.ResultingObject.ContractKey, contractKey.KeyValue);
                Assert.AreEqual(contract.ContractStatus, result.ResultingObject.ContractStatus);

                var contractItem = result.ResultingObject.Items.Single(i => i.ContractItemKey == item0Key.KeyValue);
                Assert.AreEqual(item0.CustomerProductCode, contractItem.CustomerProductCode);
                Assert.AreEqual(item0.PriceBase, contractItem.BasePrice);
                Assert.AreEqual(0, contractItem.TotalWeightShipped);
                Assert.AreEqual(0, contractItem.TotalWeightPending);
                Assert.AreEqual(item0.Quantity * item0.PackagingProduct.Weight, contractItem.TotalWeight);
                Assert.AreEqual(item0.Quantity * item0.PackagingProduct.Weight, contractItem.TotalWeightRemaining);

                contractItem = result.ResultingObject.Items.Single(i => i.ContractItemKey == item1Key.KeyValue);
                Assert.AreEqual(item1.CustomerProductCode, contractItem.CustomerProductCode);
                Assert.AreEqual(item1.PriceBase, contractItem.BasePrice);
                Assert.AreEqual(200, contractItem.TotalWeightShipped);
                Assert.AreEqual(50, contractItem.TotalWeightPending);
                Assert.AreEqual(item1.Quantity * item1.PackagingProduct.Weight, contractItem.TotalWeight);
                Assert.AreEqual(250, contractItem.TotalWeightRemaining);
            }
        }

        [TestFixture]
        public class CloseExpiredContracts : SalesServiceTests
        {
            [Test]
            public void Sets_expired_confirmed_contract_statuses_to_completed()
            {
                //Arrange
                var now = DateTime.UtcNow;
                var contracts = new List<Contract>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermEnd = null),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermEnd = now.AddDays(-1), c => c.ContractStatus = ContractStatus.Pending),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermEnd = now.AddDays(1), c => c.ContractStatus = ContractStatus.Confirmed),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.TermEnd = now.AddDays(-1), c => c.ContractStatus = ContractStatus.Confirmed),
                    };

                //Act
                var result = Service.CompleteExpiredContracts();

                //Assert
                result.AssertSuccess();
                foreach(var contract in contracts)
                {
                    if(contract.TermEnd != null && now >= contract.TermEnd && contract.ContractStatus == ContractStatus.Confirmed)
                    {
                        Assert.AreEqual(ContractStatus.Completed, RVCUnitOfWork.ContractRepository.FindByKey(new ContractKey(contract)).ContractStatus);
                    }
                    else
                    {
                        Assert.AreNotEqual(ContractStatus.Completed, RVCUnitOfWork.ContractRepository.FindByKey(new ContractKey(contract)).ContractStatus);
                    }
                }
            }
        }

        [TestFixture]
        public class CreateSalesOrder : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Customer_could_not_be_found()
            {
                //Act
                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = new CustomerKey(),
                        BrokerKey = new CompanyKey(),
                        FacilitySourceKey = new FacilityKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerNotFound);
            }

            [Test]
            public void Creates_SalesOrder_record_as_expected_on_success()
            {
                //Arrange
                var expectedDate = DateTime.UtcNow.Date;
                const int expectedSequence = 1;
                var expectedUser = TestUser.UserName;
                const string customerPurchaseOrderNum = "1234";
                var orderReceived = new DateTime(2013, 1, 23);
                const string orderTakenBy = "Juanito";

                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker));
                var warehouseKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();
                var customerKey = customer.ToCustomerKey();
                var brokerKey = customer.Broker.ToCompanyKey();

                //Act
                var parameters = new CreateSalesOrderParameters
                    {
                        UserToken = expectedUser,
                        CustomerKey = customerKey.KeyValue,
                        BrokerKey = brokerKey,
                        FacilitySourceKey = warehouseKey,
                        InvoiceDate = new DateTime(2016, 1, 1),
                        InvoiceNotes = "notes",
                        FreightCharge = 1.0f,
                        HeaderParameters = new SetOrderHeaderParameters
                            {
                                CustomerPurchaseOrderNumber = customerPurchaseOrderNum,
                                DateOrderReceived = orderReceived,
                                OrderTakenBy = orderTakenBy
                            },
                        SetShipmentInformation = new SetShipmentInformationWithStatus
                            {
                                ShippingInstructions = new SetShippingInstructions
                                    {
                                        ShipmentDate = expectedDate
                                    }

                            }
                    };
                var result = Service.CreateSalesOrder(parameters);

                //Assert
                result.AssertSuccess();

                var order = RVCUnitOfWork.SalesOrderRepository.Filter(o => o.DateCreated == expectedDate && o.Sequence == expectedSequence,
                    o => o.InventoryShipmentOrder.Employee,
                    o => o.Broker,
                    o => o.SalesOrderItems,
                    o => o.InventoryShipmentOrder.ShipmentInformation,
                    o => o.InventoryShipmentOrder.SourceFacility,
                    o => o.InventoryShipmentOrder.DestinationFacility)
                    .Single();
                Assert.AreEqual(expectedUser, order.InventoryShipmentOrder.Employee.UserName);
                Assert.AreEqual(customerKey, order);
                Assert.AreEqual(brokerKey, order.Broker);
                Assert.AreEqual(warehouseKey, order.InventoryShipmentOrder.SourceFacility);
                Assert.AreEqual(customerPurchaseOrderNum, order.InventoryShipmentOrder.PurchaseOrderNumber);
                Assert.AreEqual(orderReceived, order.InventoryShipmentOrder.DateReceived);
                Assert.AreEqual(orderTakenBy, order.InventoryShipmentOrder.TakenBy);
                Assert.AreEqual(parameters.InvoiceDate, order.InvoiceDate);
                Assert.AreEqual(parameters.InvoiceNotes, order.InvoiceNotes);
                Assert.AreEqual(parameters.FreightCharge, order.FreightCharge);
                Assert.IsEmpty(order.SalesOrderItems);

                order.SoldTo.AssertEqual(null);
                order.InventoryShipmentOrder.ShipmentInformation.AssertEqual(parameters.SetShipmentInformation);
            }

            [Test]
            public void Creates_SalesOrder_record_with_SoldTo_ShippingLabel_as_expected_on_success()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker));
                var warehouseKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();
                var customerKey = customer.ToCustomerKey();
                var soldTo = TestHelper.CreateObjectGraph<ShippingLabel>();

                //Act
                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = customerKey,
                        BrokerKey = customer.Broker.ToCompanyKey(),
                        FacilitySourceKey = warehouseKey,

                        HeaderParameters = new SetOrderHeaderParameters(),
                        SetShipmentInformation = new SetShipmentInformationWithStatus
                            {
                                ShippingInstructions = new SetShippingInstructions
                                    {
                                        ShipFromOrSoldTo = soldTo
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.SalesOrderRepository.All().Single();
                order.SoldTo.AssertEqual(soldTo);
            }

            [Test]
            public void Creates_SalesOrder_with_ShipmentInformation_record_as_expected_on_success()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker));
                var warehouseKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();
                var customerKey = customer.ToCustomerKey();
                var setShipmentInformation = TestHelper.CreateObjectGraph<SetShipmentInformationWithStatus>();

                //Act
                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = customerKey,
                        BrokerKey = customer.Broker.ToCompanyKey(),
                        FacilitySourceKey = warehouseKey,

                        SetShipmentInformation = setShipmentInformation,
                        HeaderParameters = new SetOrderHeaderParameters()
                    });

                //Assert
                result.AssertSuccess();
                var order = RVCUnitOfWork.SalesOrderRepository.Filter(o => true, o => o.InventoryShipmentOrder.ShipmentInformation).Single();
                order.InventoryShipmentOrder.ShipmentInformation.Equals(setShipmentInformation);
            }

            [Test]
            public void Creates_SalesOrderItem_and_PickOrderItem_records_as_expected_on_success()
            {
                //Arrange
                const int expectedOrderItems = 2;

                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker),
                    c => c.Company.Contacts = new List<Contact>
                        {
                            TestHelper.CreateObjectGraph<Contact>(a => a.Addresses = new List<ContactAddress> { TestHelper.CreateObjectGraph<ContactAddress>() })
                        });
                var customerKey = customer.ToCustomerKey();

                var warehouseKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();
                var contractKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customerKey, customer.Broker)).ToContractKey();
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contractKey));
                var contractItemKey0 = contractItem0.ToContractItemKey();

                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contractKey));
                var contractItemKey1 = contractItem1.ToContractItemKey();

                //Act
                var paramItem0 = new SalesOrderItemParameters
                    {
                        ContractItemKey = contractItemKey0.KeyValue,
                        ProductKey = contractItem0.ToChileProductKey(),
                        PackagingKey = contractItem0.ToPackagingProductKey(),
                        TreatmentKey = contractItem0.ToInventoryTreatmentKey(),
                        Quantity = contractItem0.Quantity,
                        PriceBase = contractItem0.PriceBase,
                        PriceFreight = contractItem0.PriceFreight,
                        PriceTreatment = contractItem0.PriceTreatment,
                        PriceWarehouse = contractItem0.PriceWarehouse,
                        PriceRebate = contractItem0.PriceRebate,
                        CustomerLotCode = "LOTLOTLOT",
                        CustomerProductCode = "PRODUCTPRODUCTPRODUCT"
                    };

                var paramItem1 = new SalesOrderItemParameters
                    {
                        ContractItemKey = contractItemKey1.KeyValue,
                        ProductKey = contractItem1.ToChileProductKey(),
                        PackagingKey = contractItem1.ToPackagingProductKey(),
                        TreatmentKey = contractItem1.ToInventoryTreatmentKey(),
                        Quantity = contractItem1.Quantity,
                        PriceBase = contractItem1.PriceBase,
                        PriceFreight = contractItem1.PriceFreight,
                        PriceTreatment = contractItem1.PriceTreatment,
                        PriceWarehouse = contractItem1.PriceWarehouse,
                        PriceRebate = contractItem1.PriceRebate
                    };

                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = customerKey,
                        BrokerKey = customer.Broker.ToCompanyKey(),
                        FacilitySourceKey = warehouseKey,

                        HeaderParameters = new SetOrderHeaderParameters(),

                        OrderItems = new List<SalesOrderItemParameters>
                                {
                                    paramItem0,
                                    paramItem1
                                }
                    });

                //Assert
                result.AssertSuccess();
                var orderItems = RVCUnitOfWork.SalesOrderRepository.Filter(o => true,
                    o => o.SalesOrderItems.Select(i => i.ContractItem),
                    o => o.SalesOrderItems.Select(i => i.InventoryPickOrderItem))
                    .Single().SalesOrderItems.ToList();

                Assert.AreEqual(expectedOrderItems, orderItems.Count);
                orderItems.Single(i => contractItemKey0.Equals(i.ContractItem)).AssertEqual(paramItem0);
                orderItems.Single(i => contractItemKey1.Equals(i.ContractItem)).AssertEqual(paramItem1);
            }

            [Test, Issue("Broker was being set from that defined by the Customer association, but the need has changed" +
                         "to allow the user to specify the Broker on the order. -RI 2016-09-06",
                         References = new[] { "RVCADMIN-1281"})]
            public void Sets_Broker_reference_as_expected()
            {
                //Arrange
                var facility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));

                //Act
                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = customer.ToCustomerKey(),
                        BrokerKey = broker.ToCompanyKey(),
                        FacilitySourceKey = facility.ToFacilityKey(),

                        HeaderParameters = new SetOrderHeaderParameters()
                    });

                //Assert
                result.AssertSuccess();

                Assert.AreEqual(broker.Id, RVCUnitOfWork.SalesOrderRepository.Filter(c => true).Single().BrokerId);
            }

            [Test]
            public void CustomerOrder_requires_Customer_reference()
            {
                //Act
                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        IsMiscellaneous = false,
                        FacilitySourceKey = RinconFacility.ToFacilityKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerNotFound);
            }

            [Test]
            public void CustomerOrderItems_require_ContractItemKeys()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker),
                    c => c.Company.Contacts = new List<Contact>
                        {
                            TestHelper.CreateObjectGraph<Contact>(a => a.Addresses = new List<ContactAddress> { TestHelper.CreateObjectGraph<ContactAddress>() })
                        });
                var customerKey = customer.ToCustomerKey();

                var warehouseKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();
                var contractKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customerKey, customer.Broker)).ToContractKey();
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contractKey));
                var contractItemKey0 = contractItem0.ToContractItemKey();

                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contractKey));

                //Act
                var paramItem0 = new SalesOrderItemParameters
                    {
                        ContractItemKey = contractItemKey0.KeyValue,
                        ProductKey = contractItem0.ToChileProductKey(),
                        PackagingKey = contractItem0.ToPackagingProductKey(),
                        TreatmentKey = contractItem0.ToInventoryTreatmentKey(),
                        Quantity = contractItem0.Quantity,
                        PriceBase = contractItem0.PriceBase,
                        PriceFreight = contractItem0.PriceFreight,
                        PriceTreatment = contractItem0.PriceTreatment,
                        PriceWarehouse = contractItem0.PriceWarehouse,
                        PriceRebate = contractItem0.PriceRebate,
                        CustomerLotCode = "LOTLOTLOT",
                        CustomerProductCode = "PRODUCTPRODUCTPRODUCT"
                    };

                var paramItem1 = new SalesOrderItemParameters
                    {
                        ProductKey = contractItem1.ToChileProductKey(),
                        PackagingKey = contractItem1.ToPackagingProductKey(),
                        TreatmentKey = contractItem1.ToInventoryTreatmentKey(),
                        Quantity = contractItem1.Quantity,
                        PriceBase = contractItem1.PriceBase,
                        PriceFreight = contractItem1.PriceFreight,
                        PriceTreatment = contractItem1.PriceTreatment,
                        PriceWarehouse = contractItem1.PriceWarehouse,
                        PriceRebate = contractItem1.PriceRebate
                    };

                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = customerKey,
                        BrokerKey = customer.Broker.ToCompanyKey(),
                        FacilitySourceKey = warehouseKey,

                        HeaderParameters = new SetOrderHeaderParameters(),

                        OrderItems = new List<SalesOrderItemParameters>
                                    {
                                        paramItem0,
                                        paramItem1
                                    }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.ContractItemForCustomerNotFound);
            }

            [Test]
            public void MiscOrder_does_not_require_Customer_reference()
            {
                //Act
                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        IsMiscellaneous = true,
                        FacilitySourceKey = RinconFacility.ToFacilityKey(),
                        HeaderParameters = new SetOrderHeaderParameters()
                    });

                //Assert
                result.AssertSuccess();
                var salesOrderKey = KeyParserHelper.ParseResult<ISalesOrderKey>(result.ResultingObject).ResultingObject.ToSalesOrderKey();
                Assert.IsNull(RVCUnitOfWork.SalesOrderRepository.FindByKey(salesOrderKey, s => s.Customer).Customer);
            }

            [Test]
            public void MiscOrderItems_do_not_require_ContractItemKeys()
            {
                //Arrange
                var warehouseKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>();

                //Act
                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilitySourceKey = warehouseKey,
                        IsMiscellaneous = true,

                        HeaderParameters = new SetOrderHeaderParameters(),

                        OrderItems = new List<SalesOrderItemParameters>
                            {
                                new SalesOrderItemParameters
                                    {
                                        ProductKey = contractItem0.ToChileProductKey(),
                                        PackagingKey = contractItem0.ToPackagingProductKey(),
                                        TreatmentKey = contractItem0.ToInventoryTreatmentKey(),
                                        Quantity = contractItem0.Quantity,
                                        PriceBase = contractItem0.PriceBase,
                                        PriceFreight = contractItem0.PriceFreight,
                                        PriceTreatment = contractItem0.PriceTreatment,
                                        PriceWarehouse = contractItem0.PriceWarehouse,
                                        PriceRebate = contractItem0.PriceRebate,
                                        CustomerLotCode = "LOTLOTLOT",
                                        CustomerProductCode = "PRODUCTPRODUCTPRODUCT"
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
            }
        }

        [TestFixture]
        public class UpdateSalesOrder : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_SalesOrder_could_not_be_found()
            {
                //Act
                var result = Service.UpdateSalesOrder(new UpdateSalesOrderParameters
                    {
                        UserToken = "Jimmy",
                        SalesOrderKey = new SalesOrderKey(),
                        BrokerKey = new CompanyKey(),
                        FacilitySourceKey = new FacilityKey(),
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.SalesOrderNotFound);
            }

            [Test]
            public void Updates_SalesOrder_record_as_expected()
            {
                //Arrange
                const string expectedPO = "PO.";
                var expectedOrderReceived = new DateTime(2013, 3, 29);
                const string expectedOrderTakenBy = "Mr. Bojangles";
                var expectedUser = TestUser.UserName;
                var expectedWarehouseKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();

                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker));
                var currentContactKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>(c => c.ConstrainByKeys(customer)).ToContactKey();
                var salesOrderKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.ConstrainByKeys(customer, currentContactKey)).ToSalesOrderKey();

                //Act
                var parameters = new UpdateSalesOrderParameters
                    {
                        UserToken = expectedUser,
                        SalesOrderKey = salesOrderKey,
                        BrokerKey = customer.Broker.ToCompanyKey(),
                        FacilitySourceKey = expectedWarehouseKey,
                        HeaderParameters = new SetOrderHeaderParameters
                            {
                                CustomerPurchaseOrderNumber = expectedPO,
                                DateOrderReceived = expectedOrderReceived,
                                OrderTakenBy = expectedOrderTakenBy
                            },
                        SetShipmentInformation = new SetShipmentInformationWithStatus()
                    };
                var result = Service.UpdateSalesOrder(parameters);

                //Assert
                result.AssertSuccess();

                var salesOrder = RVCUnitOfWork.SalesOrderRepository.FindByKey(salesOrderKey, o => o.InventoryShipmentOrder.Employee, o => o.InventoryShipmentOrder.SourceFacility);
                Assert.AreEqual(expectedUser, salesOrder.InventoryShipmentOrder.Employee.UserName);
                Assert.AreEqual(expectedWarehouseKey, salesOrder.InventoryShipmentOrder.SourceFacility);
                Assert.AreEqual(expectedPO, salesOrder.InventoryShipmentOrder.PurchaseOrderNumber);
                Assert.AreEqual(expectedOrderReceived, salesOrder.InventoryShipmentOrder.DateReceived);
                Assert.AreEqual(expectedOrderTakenBy, salesOrder.InventoryShipmentOrder.TakenBy);
                Assert.AreEqual(parameters.CreditMemo, salesOrder.CreditMemo);
                Assert.AreEqual(parameters.FreightCharge, salesOrder.FreightCharge);
                Assert.AreEqual(parameters.InvoiceDate, salesOrder.InvoiceDate);
                Assert.AreEqual(parameters.InvoiceNotes, salesOrder.InvoiceNotes);
            }

            [Test]
            public void Updates_SalesOrder_SoldTo_ShippingLabel_as_expected_on_success()
            {
                //Arrange
                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var customerOrderKey = salesOrder.ToSalesOrderKey();
                var expectedSoldTo = TestHelper.CreateObjectGraph<ShippingLabel>();

                //Act
                var result = Service.UpdateSalesOrder(new UpdateSalesOrderParameters
                {
                    UserToken = TestUser.UserName,

                    SalesOrderKey = customerOrderKey,
                    BrokerKey = salesOrder.Broker.ToCompanyKey(),
                    FacilitySourceKey = salesOrder.InventoryShipmentOrder.SourceFacility.ToFacilityKey(),
                    HeaderParameters = new SetOrderHeaderParameters(),
                    SetShipmentInformation = new SetShipmentInformationWithStatus
                        {
                            ShippingInstructions = new SetShippingInstructions
                                {
                                    ShipFromOrSoldTo = expectedSoldTo
                                }
                        }
                });

                //Assert
                result.AssertSuccess();
                salesOrder = RVCUnitOfWork.SalesOrderRepository.FindByKey(customerOrderKey);
                salesOrder.SoldTo.AssertEqual(expectedSoldTo);
            }

            [Test]
            public void Updates_SalesOrder_ShipmentInformation_as_expected_on_success()
            {
                //Arrange
                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var salesOrderKey = salesOrder.ToSalesOrderKey();
                var expectedShipmentInformation = (ISetShipmentInformation)TestHelper.CreateObjectGraph<SetShipmentInformationWithStatus>();

                //Act
                var result = Service.UpdateSalesOrder(new UpdateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        
                        SalesOrderKey = salesOrderKey,
                        BrokerKey = salesOrder.Broker.ToCompanyKey(),
                        FacilitySourceKey = salesOrder.InventoryShipmentOrder.SourceFacility.ToFacilityKey(),
                        SetShipmentInformation = expectedShipmentInformation,
                        HeaderParameters = new SetOrderHeaderParameters()
                    });

                //Assert
                result.AssertSuccess();
                salesOrder = RVCUnitOfWork.SalesOrderRepository.FindByKey(salesOrderKey, o => o.InventoryShipmentOrder.ShipmentInformation);
                salesOrder.InventoryShipmentOrder.ShipmentInformation.AssertEqual(expectedShipmentInformation, salesOrder.SoldTo);
            }

            [Test]
            public void Will_not_remove_SalesOrderItem_records_on_success()
            {
                //Arrange
                const int orderItems = 3;
                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker));
                var salesOrderKey = salesOrder.ToSalesOrderKey();
                var salesOrderItemKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(salesOrder)).ToSalesOrderItemKey();
                var salesOrderItemKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(salesOrder)).ToSalesOrderItemKey();
                var salesOrderItemKey2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(salesOrder)).ToSalesOrderItemKey();

                //Act
                var result = Service.UpdateSalesOrder(new UpdateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        SalesOrderKey = salesOrderKey,
                        BrokerKey = salesOrder.Broker.ToCompanyKey(),
                        FacilitySourceKey = salesOrder.InventoryShipmentOrder.SourceFacility.ToFacilityKey(),
                        HeaderParameters = new SetOrderHeaderParameters(),
                        SetShipmentInformation = new SetShipmentInformationWithStatus(),
                    });

                //Assert
                result.AssertSuccess();
                salesOrder = RVCUnitOfWork.SalesOrderRepository.FindByKey(salesOrderKey, o => o.SalesOrderItems);

                var items = salesOrder.SalesOrderItems.ToList();
                Assert.AreEqual(orderItems, items.Count);
                Assert.AreEqual(1, items.Count(i => salesOrderItemKey0.Equals(i)));
                Assert.AreEqual(1, items.Count(i => salesOrderItemKey1.Equals(i)));
                Assert.AreEqual(1, items.Count(i => salesOrderItemKey2.Equals(i)));
            }

            [Test]
            public void Removes_all_SalesOrderItems_and_InventoryPickOrderItems_on_success_given_an_empty_collection_of_SalesOrderItems()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                var customerOrderItemKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(customerOrder)).ToSalesOrderItemKey();
                var pickOrderItemKey0 = customerOrderItemKey0.ToInventoryPickOrderItemKey();

                var customerOrderItemKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(customerOrder)).ToSalesOrderItemKey();
                var pickOrderItemKey1 = customerOrderItemKey1.ToInventoryPickOrderItemKey();

                var customerOrderItemKey2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(customerOrder)).ToSalesOrderItemKey();
                var pickOrderItemKey2 = customerOrderItemKey2.ToInventoryPickOrderItemKey();

                //Act
                var result = Service.UpdateSalesOrder(customerOrder.ToUpdateParameters(TestUser, o => o.OrderItems = new List<SalesOrderItemParameters>()));

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.SalesOrderItemRepository.FindByKey(customerOrderItemKey0));
                Assert.IsNull(RVCUnitOfWork.InventoryPickOrderItemRepository.FindByKey(pickOrderItemKey0));
                Assert.IsNull(RVCUnitOfWork.SalesOrderItemRepository.FindByKey(customerOrderItemKey1));
                Assert.IsNull(RVCUnitOfWork.InventoryPickOrderItemRepository.FindByKey(pickOrderItemKey1));
                Assert.IsNull(RVCUnitOfWork.SalesOrderItemRepository.FindByKey(customerOrderItemKey2));
                Assert.IsNull(RVCUnitOfWork.InventoryPickOrderItemRepository.FindByKey(pickOrderItemKey2));
            }

            [Test]
            public void Returns_non_sucessful_result_if_any_SalesOrderItem_references_a_ContractItem_that_does_not_belong_to_the_supplied_Customer()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customerOrder, customerOrder.Broker));
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));
                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>();

                //Act
                var result = Service.UpdateSalesOrder(customerOrder.ToUpdateParameters(TestUser, c => c.OrderItems = new List<SalesOrderItemParameters>
                    {
                        contractItem0.ToCustomerOrderItemParameters(),
                        contractItem1.ToCustomerOrderItemParameters()
                    }));

                //Assert
                result.AssertNotSuccess(UserMessages.ContractItemForCustomerNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_any_SalesOrderItem_references_a_ContractItem_with_different_ChileProductKeys()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customerOrder, customerOrder.Broker));
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));
                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));

                var result = Service.UpdateSalesOrder(customerOrder.ToUpdateParameters(TestUser, p => p.OrderItems = new List<SalesOrderItemParameters>
                    {
                        contractItem0.ToCustomerOrderItemParameters(),
                        contractItem1.ToCustomerOrderItemParameters(i => i.ProductKey = new ChileProductKey())
                    }));

                //Assert
                result.AssertNotSuccess(UserMessages.ExpectedChileProductKeyForContractItemNotEqualReceived);
            }

            [Test]
            public void Returns_successful_result_if_any_SalesOrderItem_references_a_ContractItem_with_different_PackagingProductKeys()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customerOrder, customerOrder.Broker));
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));
                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));
                var packaging = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();

                //Act
                var result = Service.UpdateSalesOrder(customerOrder.ToUpdateParameters(TestUser, c => c.OrderItems = new List<SalesOrderItemParameters>
                    {
                        contractItem0.ToCustomerOrderItemParameters(),
                        contractItem1.ToCustomerOrderItemParameters(i => i.PackagingKey = packaging.ToPackagingProductKey())
                    }));

                //Assert
                result.AssertSuccess();
            }

            [Test]
            public void Returns_successful_result_if_any_SalesOrderItem_references_a_ContractItem_with_different_InventoryTreatmentKeys()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                var contract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(customerOrder, customerOrder.Broker));
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));
                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract));

                //Act
                var result = Service.UpdateSalesOrder(customerOrder.ToUpdateParameters(TestUser, o => o.OrderItems = new List<SalesOrderItemParameters>
                    {
                        contractItem0.ToCustomerOrderItemParameters(),
                        contractItem1.ToCustomerOrderItemParameters(i => i.TreatmentKey = new InventoryTreatmentKey())
                    }));

                //Assert
                result.AssertSuccess();
            }

            [Test]
            public void Sets_SalesOrderItem_and_InventoryPickOrderItem_records_as_expected_on_success()
            {
                //Arrange
                const int expectedItems = 4;

                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker),
                    c => c.BrokerId = c.Customer.BrokerId);
                var contract0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(salesOrder, salesOrder.Customer.Broker));
                var contractItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(c => c.ConstrainByKeys(contract0));
                var salesOrderItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(c => c.ConstrainByKeys(salesOrder, contractItem0, contractItem0, contractItem0, contractItem0));
                var contractItemKey0 = contractItem0.ToContractItemKey();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(c => c.ConstrainByKeys(salesOrder));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(c => c.ConstrainByKeys(salesOrder));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(c => c.ConstrainByKeys(salesOrder));

                var contract1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(salesOrder, salesOrder.Customer.Broker));
                var contractItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract1));
                var contractItemKey1 = contractItem1.ToContractItemKey();

                var contract2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contract>(c => c.ConstrainByKeys(salesOrder, salesOrder.Customer.Broker));
                var contractItem2_0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract2));
                var contractItemKey2_0 = contractItem2_0.ToContractItemKey();
                var contractItem2_1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContractItem>(i => i.ConstrainByKeys(contract2));
                var contractItemKey2_1 = contractItem2_1.ToContractItemKey();

                //Act
                var paramItem0 = salesOrderItem0.ToCustomerOrderItemParameters();
                var paramItem1 = contractItem1.ToCustomerOrderItemParameters(i =>
                    {
                        i.Quantity = salesOrderItem0.InventoryPickOrderItem.Quantity + 20;
                        i.PriceBase = salesOrderItem0.PriceBase + 1.0;
                        i.PriceFreight = salesOrderItem0.PriceFreight;
                        i.PriceTreatment = salesOrderItem0.PriceTreatment;
                        i.PriceWarehouse = salesOrderItem0.PriceWarehouse;
                        i.PriceRebate = salesOrderItem0.PriceRebate - 1.0;
                    });
                var paramItem2 = contractItem2_0.ToCustomerOrderItemParameters();
                var paramItem3 = contractItem2_1.ToCustomerOrderItemParameters();

                var result = Service.UpdateSalesOrder(salesOrder.ToUpdateParameters(TestUser, c => c.OrderItems = new List<SalesOrderItemParameters>
                    {
                        paramItem0,
                        paramItem1,
                        paramItem2,
                        paramItem3
                    }));

                //Assert
                result.AssertSuccess();

                var items = RVCUnitOfWork.SalesOrderRepository.FindByKey(salesOrder.ToSalesOrderKey(),
                    c => c.SalesOrderItems.Select(i => i.ContractItem),
                    c => c.SalesOrderItems.Select(i => i.InventoryPickOrderItem)).SalesOrderItems.ToList();
                Assert.AreEqual(expectedItems, items.Count);
                items.Single(i => contractItemKey0.Equals(i.ContractItem)).AssertEqual(paramItem0);
                items.Single(i => contractItemKey1.Equals(i.ContractItem)).AssertEqual(paramItem1);
                items.Single(i => contractItemKey2_0.Equals(i.ContractItem)).AssertEqual(paramItem2);
                items.Single(i => contractItemKey2_1.Equals(i.ContractItem)).AssertEqual(paramItem3);
            }

            [Test, Issue("Updating order item customer codes should also update associated picked item codes. -RI 2016-09-05",
                References = new[] {"RVCADMIN-1274"})]
            public void Updating_SalesOrderItem_codes_will_update_associated_PickedInventoryItem_codes()
            {
                //Arrange
                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    o => o.Broker.SetCompanyTypes(CompanyType.Broker),
                    o => o.BrokerId = o.Customer.BrokerId);
                for(var n = 0; n < 3; ++n)
                {
                    TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i =>
                        {
                            i.ConstrainByKeys(salesOrder)
                             .SetToContractItem()
                             .ContractItem.Contract.ConstrainByKeys(salesOrder, salesOrder.Broker);
                            i.PickedItems = TestHelper.List<SalesOrderPickedItem>(2, l => l.ForEach(p => p.ConstrainByKeys(salesOrder)));
                        });
                }
                Assert.AreEqual(3, salesOrder.SalesOrderItems.Count);
                Assert.AreEqual(6, salesOrder.SalesOrderItems.SelectMany(i => i.PickedItems).Count());

                //Act
                var parameters = salesOrder.ToUpdateParameters(TestUser, p => p.OrderItems.ForEach((i, n) =>
                    {
                        i.CustomerLotCode = string.Format("LotCode{0}", n);
                        i.CustomerProductCode = string.Format("ProductCode{0}", n);
                    }));
                var result = Service.UpdateSalesOrder(parameters);

                //Assert
                result.AssertSuccess();
                salesOrder = RVCUnitOfWork.SalesOrderRepository.FindByKey(salesOrder.ToSalesOrderKey(),
                    o => o.SalesOrderItems.Select(i => i.InventoryPickOrderItem),
                    o => o.SalesOrderItems.Select(i => i.PickedItems.Select(p => p.PickedInventoryItem)));
                foreach(var item in parameters.OrderItems)
                {
                    var orderItem = salesOrder.SalesOrderItems.FirstOrDefault(i => i.InventoryPickOrderItem.ToProductKey().KeyValue == item.ProductKey);
                    Assert.AreEqual(item.CustomerLotCode, orderItem.InventoryPickOrderItem.CustomerLotCode);
                    Assert.AreEqual(item.CustomerProductCode, orderItem.InventoryPickOrderItem.CustomerProductCode);
                    foreach(var picked in orderItem.PickedItems)
                    {
                        Assert.AreEqual(item.CustomerLotCode, picked.PickedInventoryItem.CustomerLotCode);
                        Assert.AreEqual(item.CustomerProductCode, picked.PickedInventoryItem.CustomerProductCode);
                    }
                }
            }

            [Test, Issue("Broker was being set from that defined by the Customer association, but the need has changed" +
                     "to allow the user to specify the Broker on the order. -RI 2016-09-06",
                References = new[] { "RVCADMIN-1281"})]
            public void Sets_Broker_reference_as_expected()
            {
                //Arrange
                var facility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.OrderStatus = SalesOrderStatus.Ordered);
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));

                //Act
                var result = Service.UpdateSalesOrder(new UpdateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        SalesOrderKey = salesOrder.ToSalesOrderKey(),
                        BrokerKey = broker.ToCompanyKey(),
                        FacilitySourceKey = facility.ToFacilityKey(),

                        HeaderParameters = new SetOrderHeaderParameters()
                    });

                //Assert
                result.AssertSuccess();

                Assert.AreEqual(broker.Id, RVCUnitOfWork.SalesOrderRepository.Filter(c => true).Single().BrokerId);
            }
        }

        [TestFixture]
        public class DeleteSalesOrder : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_SalesOrder_does_not_exist()
            {
                //Act
                var result = Service.DeleteSalesOrder(new SalesOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.SalesOrderNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_CustomerOrder_has_been_shipped()
            {
                //Arrange
                var saleOrderKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    c => c.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Shipped)
                    .ToSalesOrderKey();

                //Act
                var result = Service.DeleteSalesOrder(saleOrderKey);

                //Assert
                result.AssertNotSuccess(UserMessages.CannotDeleteShippedShipmentOrder);
                Assert.IsNotNull(RVCUnitOfWork.SalesOrderRepository.FindByKey(saleOrderKey));
            }

            [Test]
            public void Removes_SalesOrder_and_associated_records_on_success()
            {
                //Arrange
                var allowances = new List<LotSalesOrderAllowance>();
                var orderItems = new List<SalesOrderItem>();
                var pickedItems = new List<SalesOrderPickedItem>();
                var salesOrderKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    c => c.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Scheduled,
                    c => c.LotAllowances = TestHelper.List<LotSalesOrderAllowance>(3, allowances.AddRange),
                    c => c.SalesOrderItems = TestHelper.List<SalesOrderItem>(3, orderItems.AddRange),
                    c => c.SalesOrderPickedItems = TestHelper.List<SalesOrderPickedItem>(3, (i, n) =>
                        {
                            i.PickedInventoryItem.SetCurrentLocationToSource();
                            pickedItems.Add(i);
                        }))
                    .ToSalesOrderKey();

                //Act
                var result = Service.DeleteSalesOrder(salesOrderKey);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.SalesOrderRepository.FindByKey(salesOrderKey));
                allowances.ForEach(i =>
                    {
                        var predicate = LotCustomerOrderAllowancePredicates.ByCustomerOrderKey(i);
                        Assert.IsNull(RVCUnitOfWork.LotSalesOrderAllowanceRepository.Filter(predicate).FirstOrDefault());
                    });
                orderItems.ForEach(i => Assert.IsNull(RVCUnitOfWork.SalesOrderItemRepository.FindByKey(i.ToSalesOrderItemKey())));
                pickedItems.ForEach(i => Assert.IsNull(RVCUnitOfWork.SalesOrderPickedItemRepository.FindByKey(i.ToCustomerOrderPickedItemKey())));
            }

            [Test]
            public void Returns_picked_items_back_to_Inventory()
            {
                //Arrange
                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    c => c.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Scheduled,
                    c => c.SalesOrderPickedItems = TestHelper.List<SalesOrderPickedItem>(3, (i, n) => i.PickedInventoryItem.SetCurrentLocationToSource()));
                var salesOrderKey = salesOrder.ToSalesOrderKey();
                var expectedInventory = salesOrder.SalesOrderPickedItems.ToDictionary(i => i.PickedInventoryItem.ToInventoryKey(), i => i.PickedInventoryItem.Quantity);

                //Act
                var result = Service.DeleteSalesOrder(salesOrderKey);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.SalesOrderRepository.FindByKey(salesOrderKey));
                foreach(var expected in expectedInventory)
                {
                    Assert.AreEqual(expected.Value, RVCUnitOfWork.InventoryRepository.FindByKey(expected.Key).Quantity);
                }
            }
        }

        [TestFixture]
        public class GetSalesOrder : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_SalesOrder_could_not_be_found()
            {
                //Act
                var result = TimedExecution(() => Service.GetSalesOrder(new SalesOrderKey().KeyValue));

                //Assert
                result.AssertNotSuccess(UserMessages.SalesOrderNotFound);
            }

            [Test]
            public void Will_find_SalesOrder_by_MoveNum_if_not_found_by_key()
            {
                //Arrange
                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker));

                //Act
                var result = TimedExecution(() => Service.GetSalesOrder(salesOrder.InventoryShipmentOrder.MoveNum.ToString()));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(salesOrder.InventoryShipmentOrder.MoveNum, result.ResultingObject.MoveNum);
            }

            [Test]
            public void Returns_SalesOrder_as_expected_on_success()
            {
                //Arrange
                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker));
                var salesOrderKey = salesOrder.ToSalesOrderKey();
                
                //Act
                var result = TimedExecution(() => Service.GetSalesOrder(salesOrderKey.KeyValue));

                //Assert
                result.AssertSuccess();
                salesOrder.AssertEqual(result.ResultingObject);
                Assert.IsEmpty(result.ResultingObject.PickOrder.PickOrderItems);
            }

            [Test]
            public void Returns_SalesOrderItems_as_expected_on_success()
            {
                //Arrange
                const int expectedItems = 2;

                var salesOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker));
                var salesOrderKey = salesOrder.ToSalesOrderKey();

                var item0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(salesOrder));
                var orderItemKey0 = item0.ToSalesOrderItemKey();

                var item1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(salesOrder));
                var orderItemKey1 = item1.ToSalesOrderItemKey();

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrder(salesOrderKey.KeyValue);
                var items = result.ResultingObject == null ? new List<ISalesOrderItemReturn>() : result.ResultingObject.PickOrder.PickOrderItems.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();

                Assert.AreEqual(expectedItems, items.Count);
                item0.AssertEqual(items.Single(i => i.OrderItemKey == orderItemKey0.KeyValue));
                item1.AssertEqual(items.Single(i => i.OrderItemKey == orderItemKey1.KeyValue));
            }

            [Test, Issue("Adding OrderItemKey member to picked items. -RI 2016-08-16",
                References = new[] { "RVCADMIN-1235" })]
            public void Returns_picked_items_with_order_item_keys_as_expected()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.SalesOrderItems = TestHelper.List<SalesOrderItem>(3, (i, n) =>
                        i.PickedItems = TestHelper.List<SalesOrderPickedItem>(2)
                    ));
                var picked = customerOrder.SalesOrderPickedItems.ToList();
                Assert.AreEqual(6, picked.Count);

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrder(customerOrder.ToSalesOrderKey());
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                picked.AssertEquivalent(result.ResultingObject.PickedInventory.PickedInventoryItems, i => i.ToPickedInventoryItemKey(), i => i.PickedInventoryItemKey, (e, r) =>
                    Assert.AreEqual(e.ToSalesOrderItemKey().KeyValue, r.OrderItemKey));
            }
        }

        [TestFixture]
        public class GetSalesOrders : SalesServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_CustomerOrder_records_exist()
            {
                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_results_as_expected_on_success()
            {
                //Arrange
                const int expectedResults = 3;

                var order0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var order1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                var order2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders();
                var results = result.ResultingObject == null ? new List<ISalesOrderSummaryReturn>() : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();

                Assert.AreEqual(expectedResults, results.Count);
                order0.AssertEqual(results.Single(r => order0.ToSalesOrderKey().KeyValue == r.MovementKey));
                order1.AssertEqual(results.Single(r => order1.ToSalesOrderKey().KeyValue == r.MovementKey));
                order2.AssertEqual(results.Single(r => order2.ToSalesOrderKey().KeyValue == r.MovementKey));
            }

            [Test]
            public void Returns_SalesOrders_of_specified_CustomerOrderStatus()
            {
                //Arrange
                const int expectedResults = 2;
                const SalesOrderStatus salesOrderStatus = SalesOrderStatus.Invoiced;

                var customerOrder0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.OrderStatus = salesOrderStatus);
                var orderKey0 = customerOrder0.ToSalesOrderKey();

                var customerOrder1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.OrderStatus = salesOrderStatus);
                var orderKey1 = customerOrder1.ToSalesOrderKey();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.OrderStatus = OrderStatus.Void);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.OrderStatus = SalesOrderStatus.Ordered);

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders(new FilterSalesOrdersParameters { SalesOrderStatus = salesOrderStatus });
                var results = result.ResultingObject == null ? new List<ISalesOrderSummaryReturn>() : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.MovementKey == orderKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.MovementKey == orderKey1.KeyValue));
            }

            [Test]
            public void Returns_SalesOrders_of_specified_Customer()
            {
                //Arrange
                const int expectedResults = 2;

                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker));
                var contact = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>(c => c.ConstrainByKeys(customer));

                var customerOrderKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.ConstrainByKeys(customer, contact)).ToSalesOrderKey();
                var customerOrderKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.ConstrainByKeys(customer, contact)).ToSalesOrderKey();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders(new FilterSalesOrdersParameters { CustomerKey = new CustomerKey((ICustomerKey)customer).KeyValue });
                var results = result.ResultingObject == null ? new List<ISalesOrderSummaryReturn>() : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.MovementKey == customerOrderKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.MovementKey == customerOrderKey1.KeyValue));
            }

            [Test]
            public void Returns_SalesOrders_of_specified_Broker()
            {
                //Arrange
                const int expectedResults = 2;

                var brokerKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker)));
                var customerOrderKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.ConstrainByKeys(null, brokerKey)).ToSalesOrderKey();
                var customerOrderKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.ConstrainByKeys(null, brokerKey)).ToSalesOrderKey();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>();

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders(new FilterSalesOrdersParameters { BrokerKey = brokerKey.KeyValue });
                var results = result.ResultingObject == null ? new List<ISalesOrderSummaryReturn>() : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.MovementKey == customerOrderKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.MovementKey == customerOrderKey1.KeyValue));
            }

            [Test]
            public void Returns_SalesOrders_with_OrderReceived_dates_within_the_specified_range()
            {
                //Arrange
                const int expectedResults = 3;
                var dateRangeStart = new DateTime(2012, 3, 29);
                var dateRageEnd = dateRangeStart.AddDays(10);

                var customerOrderKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.DateReceived = dateRangeStart).ToSalesOrderKey();
                var customerOrderKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.DateReceived = dateRangeStart.AddDays(5)).ToSalesOrderKey();
                var customerOrderKey2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.DateReceived = dateRageEnd).ToSalesOrderKey();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.InventoryShipmentOrder.DateReceived = dateRangeStart.AddDays(-1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.InventoryShipmentOrder.DateReceived = dateRageEnd.AddDays(1));

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders(new FilterSalesOrdersParameters { OrderReceivedRangeStart = dateRangeStart, OrderReceivedRangeEnd = dateRageEnd });
                var results = result.ResultingObject == null ? new List<ISalesOrderSummaryReturn>() : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => customerOrderKey0.KeyValue == r.MovementKey));
                Assert.IsNotNull(results.Single(r => customerOrderKey1.KeyValue == r.MovementKey));
                Assert.IsNotNull(results.Single(r => customerOrderKey2.KeyValue == r.MovementKey));
            }

            [Test]
            public void Returns_SalesOrder_with_OrderReceived_greater_than_or_equal_to_ScheduledShipDataRangeStart()
            {
                //Arrange
                const int expectedResults = 2;
                var dateRangeStart = new DateTime(2012, 3, 29);
                var customerOrderKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.DateReceived = dateRangeStart).ToSalesOrderKey();
                var customerOrderKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.DateReceived = dateRangeStart.AddDays(1000)).ToSalesOrderKey();

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders(new FilterSalesOrdersParameters { OrderReceivedRangeStart = dateRangeStart });
                var results = result.ResultingObject == null ? new List<ISalesOrderSummaryReturn>() : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => customerOrderKey0.KeyValue == r.MovementKey));
                Assert.IsNotNull(results.Single(r => customerOrderKey1.KeyValue == r.MovementKey));
            }

            [Test]
            public void Returns_SalesOrders_with_ShipmentDates_within_the_specified_range()
            {
                //Arrange
                const int expectedResults = 3;
                var dateRangeStart = new DateTime(2012, 3, 29);
                var dateRageEnd = dateRangeStart.AddDays(10);

                var customerOrderKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = dateRangeStart).ToSalesOrderKey();
                var customerOrderKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = dateRangeStart.AddDays(5)).ToSalesOrderKey();
                var customerOrderKey2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = dateRageEnd).ToSalesOrderKey();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = dateRangeStart.AddDays(-1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = dateRageEnd.AddDays(1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = null);

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders(new FilterSalesOrdersParameters { ScheduledShipDateRangeStart = dateRangeStart, ScheduledShipDateRangeEnd = dateRageEnd });
                var results = result.ResultingObject == null ? new List<ISalesOrderSummaryReturn>() : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => customerOrderKey0.KeyValue == r.MovementKey));
                Assert.IsNotNull(results.Single(r => customerOrderKey1.KeyValue == r.MovementKey));
                Assert.IsNotNull(results.Single(r => customerOrderKey2.KeyValue == r.MovementKey));
            }

            [Test]
            public void Returns_expected_results_given_multiple_filtering_parameters()
            {
                //Arrange
                const int expectedResults = 1;
                const SalesOrderStatus status = SalesOrderStatus.Invoiced;
                var orderReceivedStart = new DateTime(2012, 3, 29);
                var orderReceivedEnd = orderReceivedStart.AddDays(10);
                var shipmentScheduledStart = new DateTime(2013, 1, 1);
                var shipmentScheduledEnd = shipmentScheduledStart.AddDays(100);

                var customerKey = new CustomerKey((ICustomerKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.EmptyItems().Broker.SetCompanyTypes(CompanyType.Broker)));
                var brokerKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker)));

                var customerOrderKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    o => o.ConstrainByKeys(customerKey, brokerKey),
                    o => o.InventoryShipmentOrder.DateReceived = orderReceivedStart.AddDays(10),
                    o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = shipmentScheduledStart.AddDays(50),
                    o => o.OrderStatus = status).ToSalesOrderKey();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    o => o.ConstrainByKeys(null, brokerKey),
                    o => o.InventoryShipmentOrder.DateReceived = orderReceivedStart.AddDays(10),
                    o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = shipmentScheduledStart.AddDays(50),
                    o => o.OrderStatus = status);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    o => o.ConstrainByKeys(customerKey),
                    o => o.InventoryShipmentOrder.DateReceived = orderReceivedStart.AddDays(10),
                    o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = shipmentScheduledStart.AddDays(50),
                    o => o.OrderStatus = status);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    o => o.ConstrainByKeys(customerKey, brokerKey),
                    o => o.InventoryShipmentOrder.DateReceived = orderReceivedStart.AddDays(-1),
                    o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = shipmentScheduledStart.AddDays(50),
                    o => o.OrderStatus = status);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    o => o.ConstrainByKeys(customerKey, brokerKey),
                    o => o.InventoryShipmentOrder.DateReceived = orderReceivedStart.AddDays(10),
                    o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = null,
                    o => o.OrderStatus = status);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(
                    o => o.ConstrainByKeys(customerKey, brokerKey),
                    o => o.InventoryShipmentOrder.DateReceived = orderReceivedStart.AddDays(10),
                    o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate = shipmentScheduledStart.AddDays(50),
                    o => o.OrderStatus = SalesOrderStatus.Ordered,
                    o => o.InventoryShipmentOrder.OrderStatus = OrderStatus.Void);

                //Act
                StartStopwatch();
                var result = Service.GetSalesOrders(new FilterSalesOrdersParameters
                {
                    SalesOrderStatus = status,
                    OrderReceivedRangeStart = orderReceivedStart,
                    OrderReceivedRangeEnd = orderReceivedEnd,
                    ScheduledShipDateRangeStart = shipmentScheduledStart,
                    ScheduledShipDateRangeEnd = shipmentScheduledEnd,
                    CustomerKey = customerKey.KeyValue,
                    BrokerKey = brokerKey.KeyValue
                });
                var results = result.ResultingObject == null ? new List<ISalesOrderSummaryReturn>() : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single().MovementKey == customerOrderKey.KeyValue);
            }
        }

        [TestFixture]
        public class GetInventoryToPickForOrder : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_CustomerOrder_could_not_be_found()
            {
                //Act
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = new SalesOrderKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.SalesOrderNotFound);
            }

            [Test]
            public void Returns_Inventory_from_Warehouse_defined_on_SalesOrder()
            {
                //Arrange
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.SetShipFromWarehouse(warehouse));

                var expectedInventory = new Dictionary<string, bool>
                    {
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse)).ToInventoryKey(), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse)).ToInventoryKey(), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse)).ToInventoryKey(), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse).Lot.LotDefects = TestHelper.List<LotDefect>(1, (d, n) => d.Resolution = null)).ToInventoryKey(), true },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse).Lot.ProductionStatus = LotProductionStatus.Batched).ToInventoryKey(), false },
                        { TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(warehouse).Lot.ProductionStatus = LotProductionStatus.Batched).ToInventoryKey(), false }
                    };

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.ProductionStatus = LotProductionStatus.Produced);

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrder.ToSalesOrderKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                expectedInventory.AssertEquivalent(results, e => e.Key, r => r.InventoryKey, (e, r) => Assert.AreEqual(e.Value, r.ValidForPicking));
            }

            [Test]
            public void Returns_Inventory_for_SalesOrder_with_expected_ValidForPickingFlag()
            {
                //Arrange
                var expectedInventory = new Dictionary<string, bool>();

                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                var salesOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));
                var salesOrderKey = salesOrderItem.ToSalesOrderKey();

                var chileLot0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot0, salesOrderItem.InventoryPickOrderItem, null, salesOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey(), true);

                var chileLot1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot1, salesOrderItem.InventoryPickOrderItem, null, salesOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey(), true);

                var chileLot2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot2, salesOrderItem.InventoryPickOrderItem, null, salesOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey(), true);

                var chileLot3 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot3, salesOrderItem.InventoryPickOrderItem, null, salesOrderItem.InventoryPickOrderItem));

                var chileLot4 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.LotDefects = TestHelper.List<LotDefect>(1, (d, n) => d.Resolution = null));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot4, salesOrderItem.InventoryPickOrderItem, null, salesOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey(), true);

                var chileLot5 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().ProductionStatus = LotProductionStatus.Batched);
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot5, salesOrderItem.InventoryPickOrderItem, null, salesOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey(), false);

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = salesOrderKey
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                expectedInventory.AssertEquivalent(results, e => e.Key, r => r.InventoryKey, (e, r) => Assert.AreEqual(e.Value, r.ValidForPicking));
            }

            [Test, Issue("Active spec should be that of the CustomerOrderItem, not ContractItem.",
                References = new[] { "RVCADMIN-1177", "https://solutionhead.slack.com/archives/rvc/p1469045866000004" })]
            public void Returns_ValidForPicking_as_expected_by_CustomerOrderItem_product_spec()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = TestHelper.List<ChileProductAttributeRange>(1, (c, n) => c.SetValues(StaticAttributeNames.Asta, 5, 10)));
                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(customerOrderItem.Order, StaticAttributeNames.Asta, 6, 9, chileProduct).Active = true);

                var expected = new Dictionary<string, bool>
                    {
                        {
                            TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrderItem.Order.InventoryShipmentOrder.SourceFacility)
                                .Lot.SetChileLot()
                                .Attributes = TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 7.5)))
                            .ToInventoryKey(),
                            true
                        },
                        {
                            TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrderItem.Order.InventoryShipmentOrder.SourceFacility)
                                .Lot.SetChileLot()
                                .Attributes = TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 1)))
                            .ToInventoryKey(),
                            false
                        },
                        {
                            TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrderItem.Order.InventoryShipmentOrder.SourceFacility)
                                .Lot.SetChileLot())
                            .ToInventoryKey(),
                            false
                        }
                    };

                //Act
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.Items.AsEnumerable().Select(i =>
                    {
                        result.ResultingObject.Initializer.Initialize(i);
                        return i;
                    }).ToList();
                expected.AssertEquivalent(results, k => k.Key, k => k.InventoryKey, (e, r) => Assert.AreEqual(e.Value, r.ValidForPicking));
            }

            [Test]
            public void ValidForPicking_will_be_true_if_there_are_no_specs()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();

                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                var items = results
                    .Select(i =>
                        {
                            result.ResultingObject.Initializer.Initialize(i);
                            return i;
                        })
                    .ToList();
                Assert.IsTrue(items.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_false_if_product_spec_is_out_of_range()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.IsFalse(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_false_if_product_spec_is_out_of_range_and_has_unresolved_defect()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Asta).LotDefect.Resolution = null);
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.IsFalse(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_product_spec_is_out_of_range_but_has_resolved_defect()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Asta));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.IsTrue(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_product_spec_is_out_of_range_but_customer_spec_is_in_range()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(customerOrderItem.Order, StaticAttributeNames.Asta, 1, 3, chileProduct).Active = true);

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.IsTrue(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_false_if_customer_spec_is_out_of_range()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 3))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(customerOrderItem.Order, StaticAttributeNames.Asta, 1, 2, chileProduct).Active = true);

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 3))
                        });
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.False(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_false_if_customer_spec_is_out_of_range_has_resolution_but_product_spec_is_more_permissive()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 0, 3))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(customerOrderItem.Order, StaticAttributeNames.Asta, 1, 3, chileProduct).Active = true);

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 4))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Asta));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.False(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_customer_spec_is_out_of_range_has_resolution_and_product_spec_is_less_permissive()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(customerOrderItem.Order, StaticAttributeNames.Asta, 1, 3, chileProduct));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 4))
                        });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttributeDefect>(d => d.SetValues(chileLot, StaticAttributeNames.Asta));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.True(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_product_spec_is_out_of_range_but_has_contract_allowance()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var salesOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 4))
                        },
                    c => c.Lot.AddContractAllowance(salesOrderItem.ContractItem));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, salesOrderItem.InventoryPickOrderItem, null, salesOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = salesOrderItem.ToSalesOrderKey(),
                        OrderItemKey = salesOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.True(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_product_spec_is_out_of_range_but_has_customerOrder_allowance()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 4))
                        },
                    c => c.Lot.AddCustomerAllowance(customerOrderItem.Order));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.True(results.Single().ValidForPicking);
            }

            [Test]
            public void ValidForPicking_will_be_true_if_product_spec_is_out_of_range_but_has_customer_allowance()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();
                
                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 2))
                    });

                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick(),
                    c => c.Lot.Attributes = new List<LotAttribute>
                        {
                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(StaticAttributeNames.Asta, 4))
                        },
                    c => c.Lot.AddCustomerAllowance(customerOrderItem.Order));
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = customerOrderItem.ToSalesOrderKey(),
                        OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.True(results.Single().ValidForPicking);
            }

            [Test, Issue("Lots marked as 'Released' are not expecting any more attributes, so missing attributes are not considered invalid" +
                         "unless picking for a customer context that has an active spec that is more restrictive.",
                References = new[]
                    {
                        "RVCADMIN-1177",
                        "https://solutionhead.slack.com/archives/D03BRUYU4/p1469476017000062"
                    })]
            public void ValidForPicking_will_be_true_if_Released_attribute_is_missing_but_has_loose_product_spec()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();

                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 3))
                    });
                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(customerOrderItem.Order, StaticAttributeNames.Asta, 0, 4, chileProduct).Active = true);

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                {
                    OrderKey = customerOrderItem.ToSalesOrderKey(),
                    OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.True(results.Single().ValidForPicking);
            }

            [Test, Issue("Lots marked as 'Released' are not expecting any more attributes, so missing attributes are not considered invalid" +
                         "unless picking for a customer context that has an active spec that is more restrictive.",
                References = new[]
                    {
                        "RVCADMIN-1177",
                        "https://solutionhead.slack.com/archives/D03BRUYU4/p1469476017000062"
                    })]
            public void ValidForPicking_will_be_false_if_Relased_attribute_is_missing_but_has_non_loose_product_spec()
            {
                //Arrange
                var expectedInventory = new List<InventoryKey>();

                var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = new List<ChileProductAttributeRange>
                    {
                        TestHelper.CreateObjectGraph<ChileProductAttributeRange>(r => r.SetValues(StaticAttributeNames.Asta, 1, 3))
                    });
                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.InventoryPickOrderItem.SetProduct(chileProduct), i => i.Order.SetShipFromWarehouse(warehouse));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.SetValues(customerOrderItem.Order, StaticAttributeNames.Asta, 1, 2, chileProduct).Active = true);

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetProduct(chileProduct).Lot.SetChileLot().SetValidToPick());
                expectedInventory.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot, customerOrderItem.InventoryPickOrderItem, null, customerOrderItem.InventoryPickOrderItem, warehouse)).ToInventoryKey());

                //Act
                StartStopwatch();
                var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                {
                    OrderKey = customerOrderItem.ToSalesOrderKey(),
                    OrderItemKey = customerOrderItem.ToSalesOrderItemKey()
                });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Items.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                results.ForEach(i => result.ResultingObject.Initializer.Initialize(i));
                Assert.False(results.Single().ValidForPicking);
            }
        }

        [TestFixture]
        public class SetPickedInventory : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_CustomerOrder_could_not_be_found()
            {
                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(new SalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.SalesOrderNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_CustomerOrder_status_is_Invoiced()
            {
                //Arrange
                var customerOrderKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.OrderStatus = SalesOrderStatus.Invoiced).ToSalesOrderKey();

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickInvoicedCustomerOrder);
            }

            [Test]
            public void Returns_non_successful_result_if_CustomerOrder_status_is_Void()
            {
                //Arrange
                var customerOrderKey =TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.InventoryShipmentOrder.OrderStatus = OrderStatus.Void).ToSalesOrderKey();

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickVoidedCustomerOrder);
            }

            [Test]
            public void Returns_non_successful_result_if_attempting_to_reference_a_CustomerOrderItem_that_does_not_exist()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.OrderStatus = SalesOrderStatus.Ordered);
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, null, customerOrder.InventoryShipmentOrder.SourceFacility).Quantity = 100);

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrder.ToSalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        OrderItemKey = new SalesOrderItemKey(),
                                        InventoryKey = inventory.ToInventoryKey(),
                                        Quantity = 10
                                    }
                            }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickForCustomerOrderItem_DoesNotExist);
            }

            [Test]
            public void Returns_non_successful_result_if_Inventory_is_not_in_CustomerOrder_ShipFromWarehouse()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var customerOrderKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.OrderStatus = SalesOrderStatus.Ordered).ToSalesOrderKey();
                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(customerOrderKey).InventoryPickOrderItem.SetProduct(chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick().Quantity = 100);

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                {
                                    new SetPickedInventoryItemParameters
                                        {
                                            OrderItemKey = customerOrderItem.ToSalesOrderItemKey(),
                                            InventoryKey = inventory.ToInventoryKey(),
                                            Quantity = 10
                                        }
                                }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.SourceLocationMustBelongToFacility);
            }

            [Test]
            public void Returns_non_successful_result_if_Inventory_is_of_a_Lot_with_a_ProductionStatus_that_is_not_Complete()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker));
                var customerOrderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(customerOrder, chileProduct: chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(facility: customerOrder.InventoryShipmentOrder.SourceFacility).Lot.EmptyLot().ProductionStatus = LotProductionStatus.Batched, i => i.Quantity = 100);

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrder.ToSalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                {
                                    new SetPickedInventoryItemParameters
                                        {
                                            OrderItemKey = customerOrderItem.ToSalesOrderItemKey(),
                                            InventoryKey = inventory.ToInventoryKey(),
                                            Quantity = 1
                                        }
                                }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.CannotPickLotNotProduced);
            }

            [Test]
            public void Returns_successful_result_if_Inventory_is_of_a_Lot_with_an_unresolved_defect()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker));
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(chileProduct: chileProduct).ConstrainByKeys(customerOrder));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 100, i => i.SetValidToPick(customerOrder.InventoryShipmentOrder.SourceFacility).Lot.LotDefects =
                    TestHelper.List<LotDefect>(1, (d, n) => d.Resolution = null));

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrder.ToSalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                    {
                                        new SetPickedInventoryItemParameters
                                            {
                                                OrderItemKey = orderItem.ToSalesOrderItemKey(),
                                                InventoryKey = inventory.ToInventoryKey(),
                                                Quantity = 1
                                            }
                                    }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
            }

            [Test]
            public void Creates_CustomerOrderPickedItem_records_as_expected_on_success()
            {
                //Arrange
                const int expectedItems = 3;
                const int quantity0 = 10;
                const int quantity1 = 22;
                const int quantity2 = 303;

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.OrderStatus = SalesOrderStatus.Ordered);
                var customerOrderKey = customerOrder.ToSalesOrderKey();
                var customerOrderItemKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(customerOrder).InventoryPickOrderItem.SetProduct(chileProduct)).ToSalesOrderItemKey();
                var inventoryKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrder.InventoryShipmentOrder.SourceFacility).Quantity = quantity0 + 10).ToInventoryKey();
                var inventoryKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrder.InventoryShipmentOrder.SourceFacility).Quantity = quantity1 + 10).ToInventoryKey();
                var inventoryKey2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrder.InventoryShipmentOrder.SourceFacility).Quantity = quantity2 + 10).ToInventoryKey();

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters { OrderItemKey = customerOrderItemKey, InventoryKey = inventoryKey0.KeyValue, Quantity = quantity0 },
                                new SetPickedInventoryItemParameters { OrderItemKey = customerOrderItemKey, InventoryKey = inventoryKey1.KeyValue, Quantity = quantity1 },
                                new SetPickedInventoryItemParameters { OrderItemKey = customerOrderItemKey, InventoryKey = inventoryKey2.KeyValue, Quantity = quantity2 }
                            }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.SalesOrderRepository.FindByKey(customerOrderKey, o => o.SalesOrderPickedItems.Select(p => p.PickedInventoryItem))
                    .SalesOrderPickedItems.ToList();

                Assert.AreEqual(expectedItems, items.Count);
                Assert.AreEqual(quantity0, items.Single(i => inventoryKey0.Equals(i.PickedInventoryItem)).PickedInventoryItem.Quantity);
                Assert.AreEqual(quantity1, items.Single(i => inventoryKey1.Equals(i.PickedInventoryItem)).PickedInventoryItem.Quantity);
                Assert.AreEqual(quantity2, items.Single(i => inventoryKey2.Equals(i.PickedInventoryItem)).PickedInventoryItem.Quantity);
            }

            [Test]
            public void Modifies_existing_CustomerOrderPickedItem_records_as_expected_on_success()
            {
                //Arrange
                const int expectedItems = 2;

                const int currentQuantity0 = 100;
                const int currentQuantity1 = 200;
                const int currentQuantity2 = 300;

                const int newQuantity0 = 50;
                const int newQuantity1 = 300;

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.OrderStatus = SalesOrderStatus.Ordered);
                var customerOrderKey = customerOrder.ToSalesOrderKey();
                var customerOrderItemKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(customerOrder).InventoryPickOrderItem.SetProduct(chileProduct)).ToSalesOrderItemKey();

                var customerOrderPickedItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(
                    i => i.ConstrainByKeys(customerOrder).PickedInventoryItem.SetSourceWarehouse(customerOrder.InventoryShipmentOrder.SourceFacility).SetCurrentLocationToSource().Quantity = currentQuantity0,
                    i => i.PickedInventoryItem.Lot.SetValidToPick());

                var inventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrder.InventoryShipmentOrder.SourceFacility).Quantity = newQuantity1);
                var customerOrderPickedItem1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(
                    i => i.ConstrainByKeys(customerOrder).PickedInventoryItem.SetToInventory(inventory1).SetCurrentLocationToSource().Quantity = currentQuantity1);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.ConstrainByKeys(customerOrder.InventoryShipmentOrder).SetSourceWarehouse(customerOrder.InventoryShipmentOrder.SourceFacility).SetCurrentLocationToSource().Quantity = currentQuantity2,
                    i => i.Lot.SetValidToPick());

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        OrderItemKey = customerOrderItemKey,
                                        InventoryKey = customerOrderPickedItem0.PickedInventoryItem.ToInventoryKey(),
                                        Quantity = newQuantity0
                                    },
                                new SetPickedInventoryItemParameters
                                    {
                                        OrderItemKey = customerOrderItemKey,
                                        InventoryKey = customerOrderPickedItem1.PickedInventoryItem.ToInventoryKey(),
                                        Quantity = newQuantity1
                                    }
                            }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();

                var results = RVCUnitOfWork.SalesOrderRepository.FindByKey(customerOrderKey, o => o.SalesOrderPickedItems.Select(p => p.PickedInventoryItem)).SalesOrderPickedItems.ToList();

                Assert.AreEqual(expectedItems, results.Count);
                Assert.AreEqual(newQuantity0, results.Single(customerOrderPickedItem0.ToCustomerOrderPickedItemKey().Equals).PickedInventoryItem.Quantity);
                Assert.AreEqual(newQuantity1, results.Single(customerOrderPickedItem1.ToCustomerOrderPickedItemKey().Equals).PickedInventoryItem.Quantity);
            }

            [Test]
            public void Modifies_existing_Inventory_records_as_expected_on_success()
            {
                //Arrange
                const int inventoryToSubtractQuantity = 145;
                const int quantityToSubtract0 = 50;
                const int quantityToSubtract1 = 30;
                const int inventoryToSubtractNewQuantity = inventoryToSubtractQuantity - (quantityToSubtract0 + quantityToSubtract1);

                const int inventoryToAddQuantity = 555;
                const int quantityToAdd0 = 66;
                const int quantityToAdd1 = 77;
                const int inventoryToAddNewQuantity = inventoryToAddQuantity + (quantityToAdd0 + quantityToAdd1);

                const int inventoryToCreateQuantity = 524;

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.OrderStatus = SalesOrderStatus.Ordered);
                var customerOrderKey = customerOrder.ToSalesOrderKey();
                var customerOrderItemKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(customerOrder).InventoryPickOrderItem.SetProduct(chileProduct)).ToSalesOrderItemKey();

                var inventoryToRemove = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrder.InventoryShipmentOrder.SourceFacility));
                var inventoryToRemoveKey = inventoryToRemove.ToInventoryKey();

                var inventoryToSubtract = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrder.InventoryShipmentOrder.SourceFacility).Quantity = inventoryToSubtractQuantity);
                var inventoryToSubtractKey = inventoryToSubtract.ToInventoryKey();

                var inventoryToAdd = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(customerOrder.InventoryShipmentOrder.SourceFacility).Quantity = inventoryToAddQuantity);
                var inventoryToAddKey = inventoryToAdd.ToInventoryKey();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(p => p.ConstrainByKeys(customerOrder).PickedInventoryItem.SetToInventory(inventoryToAdd).SetCurrentLocationToSource().Quantity = quantityToAdd0 * 2);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(p => p.ConstrainByKeys(customerOrder).PickedInventoryItem.SetToInventory(inventoryToAdd).SetCurrentLocationToSource().Quantity = quantityToAdd1 * 2);

                var inventoryToCreateKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(p => p.ConstrainByKeys(customerOrder).PickedInventoryItem.SetSourceWarehouse(customerOrder.InventoryShipmentOrder.SourceFacility).SetCurrentLocationToSource().Quantity = inventoryToCreateQuantity).PickedInventoryItem);

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters { OrderItemKey = customerOrderItemKey, InventoryKey = inventoryToRemoveKey.KeyValue, Quantity = inventoryToRemove.Quantity },
                                new SetPickedInventoryItemParameters { OrderItemKey = customerOrderItemKey, InventoryKey = inventoryToSubtractKey.KeyValue, Quantity = quantityToSubtract0 },
                                new SetPickedInventoryItemParameters { OrderItemKey = customerOrderItemKey, InventoryKey = inventoryToSubtractKey.KeyValue, Quantity = quantityToSubtract1 },
                                new SetPickedInventoryItemParameters { OrderItemKey = customerOrderItemKey, InventoryKey = inventoryToAddKey.KeyValue, Quantity = quantityToAdd0 },
                                new SetPickedInventoryItemParameters { OrderItemKey = customerOrderItemKey, InventoryKey = inventoryToAddKey.KeyValue, Quantity = quantityToAdd1 }
                            }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();

                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToRemoveKey));
                Assert.AreEqual(inventoryToSubtractNewQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToSubtractKey).Quantity);
                Assert.AreEqual(inventoryToAddNewQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToAddKey).Quantity);
                Assert.AreEqual(inventoryToCreateQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToCreateKey).Quantity);
            }

            [Test]
            public void Modifies_existing_CustomerOrderPickedItem_to_reference_CustomerOrderItem_as_expected_on_success()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.OrderStatus = SalesOrderStatus.Ordered);
                var customerOrderKey = customerOrder.ToSalesOrderKey();

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                var customerOrderPickedItem0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(i => i.ConstrainByKeys(customerOrder).PickedInventoryItem.SetSourceWarehouse(customerOrder.InventoryShipmentOrder.SourceFacility).SetCurrentLocationToSource().ConstrainByKeys(null, chileLot).Quantity = 100);
                var customerOrderPickedItemKey0 = customerOrderPickedItem0.ToCustomerOrderPickedItemKey();

                var customerOrderItemKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(c => c.ConstrainByKeys(customerOrder, null, chileLot.ChileProduct, customerOrderPickedItem0.PickedInventoryItem, customerOrderPickedItem0.PickedInventoryItem)).ToSalesOrderItemKey();

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(customerOrderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = customerOrderPickedItem0.PickedInventoryItem.ToInventoryKey(),
                                        OrderItemKey = customerOrderItemKey.KeyValue,
                                        Quantity = 100
                                    }
                            }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var pickedItem = RVCUnitOfWork.SalesOrderPickedItemRepository.FindByKey(customerOrderPickedItemKey0, i => i.PickedInventoryItem);
                Assert.AreEqual(customerOrderItemKey, pickedItem);
            }

            [Test]
            public void Returns_non_successful_result_if_Inventory_is_out_of_ChileProduct_attribute_range()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(p => p.ProductAttributeRanges = TestHelper.List<ChileProductAttributeRange>(1, (r, n) => r.SetValues(StaticAttributeNames.Asta, 5, 10)));
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(chileProduct: chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(orderItem.Order).Lot.SetChileLot().Attributes =
                    TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 1)));

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(orderItem.ToSalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        OrderItemKey = orderItem.ToSalesOrderItemKey(),
                                        Quantity = 1
                                    }
                            }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.LotAttributeOutOfRequiredRange);
            }

            [Test]
            public void Returns_non_successful_result_if_Inventory_is_out_of_CustomerProduct_attribute_range()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.CustomerProductAttributeRanges = TestHelper.List<CustomerProductAttributeRange>(1, (r, n) => r.SetValues(customer, StaticAttributeNames.Asta, 5, 10)));
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(i => i.ConstrainByKeys(chileProduct: chileProduct).Order.ConstrainByKeys(customer));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(orderItem.Order).Lot.SetChileLot().Attributes =
                    TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 1)));

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(orderItem.ToSalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        OrderItemKey = orderItem.ToSalesOrderItemKey(),
                                        Quantity = 1
                                    }
                            }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertNotSuccess(UserMessages.LotAttributeOutOfRequiredRange);
            }

            [Test]
            public void Allows_picking_of_Inventory_that_is_in_range_of_spec()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(i => i.CustomerProductAttributeRanges =
                    TestHelper.List<CustomerProductAttributeRange>(1, (r, n) => r.SetValues(customer, StaticAttributeNames.Asta, 5, 10)));
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(o => o.Order.ConstrainByKeys(customer), i => i.InventoryPickOrderItem.SetProduct(chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(orderItem.Order).Lot.SetChileLot()
                    .Attributes = TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 5)));

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(orderItem.ToSalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                                {
                                    new SetPickedInventoryItemParameters
                                        {
                                            InventoryKey = inventory.ToInventoryKey(),
                                            OrderItemKey = orderItem.ToSalesOrderItemKey(),
                                            Quantity = 1
                                        }
                                }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var pickedItem = RVCUnitOfWork.SalesOrderItemRepository
                    .FindByKey(orderItem.ToSalesOrderItemKey(), i => i.PickedItems.Select(p => p.PickedInventoryItem))
                    .PickedItems.Single();
                Assert.AreEqual(1, pickedItem.PickedInventoryItem.Quantity);
            }

            [Test]
            public void Allows_picking_of_Inventory_that_is_out_of_range_but_has_a_LotAllowance()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(i => i.CustomerProductAttributeRanges =
                    TestHelper.List<CustomerProductAttributeRange>(1, (r, n) => r.SetValues(customer, StaticAttributeNames.Asta, 5, 10)));
                var orderItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderItem>(o => o.Order.ConstrainByKeys(customer), i => i.InventoryPickOrderItem.SetProduct(chileProduct));
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.SetValidToPick(orderItem.Order).Lot.SetChileLot()
                    .AddCustomerAllowance(orderItem.Order)
                    .Attributes = TestHelper.List<LotAttribute>(1, (a, n) => a.SetValues(StaticAttributeNames.Asta, 1)));

                //Act
                StartStopwatch();
                var result = Service.SetPickedInventory(orderItem.ToSalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new[]
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        InventoryKey = inventory.ToInventoryKey(),
                                        OrderItemKey = orderItem.ToSalesOrderItemKey(),
                                        Quantity = 1
                                    }
                            }
                    });
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                var pickedItem = RVCUnitOfWork.SalesOrderItemRepository
                    .FindByKey(orderItem.ToSalesOrderItemKey(), i => i.PickedItems.Select(p => p.PickedInventoryItem))
                    .PickedItems.Single();
                Assert.AreEqual(1, pickedItem.PickedInventoryItem.Quantity);
            }

            [Test, Issue("Vague task description makes it sounds like this isn't already happening," +
                         "but making sure anyways. -RI 2016-09-05" +
                         "Misunderstood task - picked item codes need to updated when associated order item codes are. -RI 2016-09-05",
                References = new [] { "RVCADMIN-1274" })]
            public void Modifies_customer_codes_as_expected()
            {
                //Arrange
                var customerOrder = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(c => c.Broker.SetCompanyTypes(CompanyType.Broker), c => c.OrderStatus = SalesOrderStatus.Ordered);
                var pickedItem = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrderPickedItem>(i => i.ConstrainByKeys(customerOrder).PickedInventoryItem.SetCurrentLocationToSource());

                //Act
                var result = Service.SetPickedInventory(customerOrder.ToSalesOrderKey(), new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new List<IPickedInventoryItemParameters>
                            {
                                new SetPickedInventoryItemParameters
                                    {
                                        CustomerLotCode = "LotCode!",
                                        CustomerProductCode = "ProductCode!",
                                        InventoryKey = pickedItem.PickedInventoryItem.ToInventoryKey(),
                                        OrderItemKey = pickedItem.ToSalesOrderItemKey(),
                                        Quantity = pickedItem.PickedInventoryItem.Quantity
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();

                var item = RVCUnitOfWork.SalesOrderPickedItemRepository.FindByKey(pickedItem.ToCustomerOrderPickedItemKey(), i => i.PickedInventoryItem);
                Assert.AreEqual("LotCode!", item.PickedInventoryItem.CustomerLotCode);
                Assert.AreEqual("ProductCode!", item.PickedInventoryItem.CustomerProductCode);
            }
        }

        [TestFixture]
        public class SetCustomerChileProductAttributeRange : SalesServiceTests
        {
            [Test]
            public void Creates_new_ChileProductAttributeRangeRecord_as_expected()
            {
                //Arrange
                const float expectedMin = 1.0f;
                const float expectedMax = 5.0f;
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var attributeName = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>();
                TestHelper.ResetContext();

                //Act
                var result = Service.SetCustomerChileProductAttributeRanges(new SetCustomerProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = customer.ToCustomerKey(),
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        AttributeRanges = new List<ISetCustomerProductAttributeRangeParameters>
                            {
                                new SetCustomerProductAttributeRangeParameters
                                    {
                                        AttributeNameKey = attributeName.ToAttributeNameKey(),
                                        RangeMin = expectedMin,
                                        RangeMax = expectedMax
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var range = RVCUnitOfWork.CustomerProductAttributeRangeRepository.Filter(CustomerProductAttributeRangePredicates.ByCustomerProduct(customer, chileProduct)).Single(r => r.AttributeShortName == attributeName.ShortName);
                Assert.IsTrue(range.Active);
                Assert.AreEqual(expectedMin, range.RangeMin);
                Assert.AreEqual(expectedMax, range.RangeMax);
                Assert.AreEqual(TestUser.EmployeeId, range.EmployeeId);
            }

            [Test]
            public void Updates_existing_CustomerProductAttributeRangeRecord_as_expected()
            {
                //Arrange
                const float expectedMin = 1.0f;
                const float expectedMax = 5.0f;
                var range = TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.Active = true);
                TestHelper.ResetContext();

                //Act
                var result = Service.SetCustomerChileProductAttributeRanges(new SetCustomerProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = range.Customer.ToCustomerKey(),
                        ChileProductKey = range.ChileProduct.ToChileProductKey(),
                        AttributeRanges = new List<ISetCustomerProductAttributeRangeParameters>
                            {
                                new SetCustomerProductAttributeRangeParameters
                                    {
                                        AttributeNameKey = range.ToAttributeNameKey(),
                                        RangeMin = expectedMin,
                                        RangeMax = expectedMax
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                range = RVCUnitOfWork.CustomerProductAttributeRangeRepository.Filter(CustomerProductAttributeRangePredicates.ByCustomerProduct(range.Customer, range.ChileProduct)).Single(r => r.AttributeShortName == range.AttributeShortName);
                Assert.IsTrue(range.Active);
                Assert.AreEqual(expectedMin, range.RangeMin);
                Assert.AreEqual(expectedMax, range.RangeMax);
                Assert.AreEqual(TestUser.EmployeeId, range.EmployeeId);
            }

            [Test]
            public void Sets_records_as_expected()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.ProductSpecs = TestHelper.List<CustomerProductAttributeRange>(3, (i, n) =>
                    i.SetValues(chileProduct: chileProduct)));
                var expected = customer.ProductSpecs.Take(2);

                var parameters = new SetCustomerProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = customer.ToCustomerKey(),
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        AttributeRanges = expected.Select(e => new SetCustomerProductAttributeRangeParameters
                            {
                                AttributeNameKey = e.ToAttributeNameKey(),
                                RangeMin = e.RangeMin,
                                RangeMax = e.RangeMax
                            })
                    };

                //Act
                var result = Service.SetCustomerChileProductAttributeRanges(parameters);

                //Assert
                result.AssertSuccess();
                var results = RVCUnitOfWork.CustomerProductAttributeRangeRepository.Filter(CustomerProductAttributeRangePredicates.ByCustomerProduct(customer, chileProduct)).ToList();
                expected.AssertEquivalent(results, e => e.ToAttributeNameKey(), r => r.ToAttributeNameKey(),
                    (e, r) =>
                        {
                            Assert.AreEqual(e.RangeMin, r.RangeMin);
                            Assert.AreEqual(e.RangeMax, r.RangeMax);
                        });
            }
        }

        [TestFixture]
        public class RemoveCustomerChileProductAttributeRange : SalesServiceTests
        {
            [Test]
            public void Removes_existing_CustomerChileProductAttributeRanges_as_expected_on_sucess()
            {
                //Arrange
                var product = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.ProductSpecs = TestHelper.List<CustomerProductAttributeRange>(3, (r, n) => r.ConstrainByKeys(null, product)));

                //Act
                var result = Service.RemoveCustomerChileProductAttributeRanges(new RemoveCustomerChileProductAttributeRangesParameters
                    {
                        CustomerKey = customer.ToCustomerKey(),
                        ChileProductKey = product.ToChileProductKey()
                    });

                //Assert
                result.AssertSuccess();
                var ranges = RVCUnitOfWork.CustomerProductAttributeRangeRepository.Filter(r => r.CustomerId == customer.Id).ToList();
                Assert.IsEmpty(ranges);
            }
        }

        [TestFixture]
        public class GetCustomerChileProductAttributeRanges : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_no_active_ProductAttribute_records_exist()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                //Act
                var result = Service.GetCustomerChileProductAttributeRanges(customer.ToCustomerKey(), chileProduct.ToChileProductKey());

                //Assert
                result.AssertNotSuccess(UserMessages.NoCustomerProductRangesFound);
            }

            [Test]
            public void Returns_CustomerChileProductAttributeRangeReturn_as_expected()
            {
                //Arrange
                var attribute0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.Active = true);
                var attribute1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductAttributeRange>(r => r.ConstrainByKeys(attribute0, attribute0).Active = true);
                var customerKey = attribute0.ToCustomerKey();
                var chileProductKey = attribute0.ToChileProductKey();

                //Act
                var result = Service.GetCustomerChileProductAttributeRanges(customerKey, chileProductKey);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(customerKey.KeyValue, result.ResultingObject.CustomerKey);
                Assert.AreEqual(chileProductKey.KeyValue, result.ResultingObject.ChileProduct.ProductKey);
                Assert.NotNull(result.ResultingObject.AttributeRanges.SingleOrDefault(r => r.AttributeShortName == attribute0.AttributeShortName));
                Assert.NotNull(result.ResultingObject.AttributeRanges.SingleOrDefault(r => r.AttributeShortName == attribute1.AttributeShortName));
            }
        }

        [TestFixture]
        public class GetCustomerChileProductsAttributeRanges : SalesServiceTests
        {
            [Test]
            public void Returns_empty_result_if_no_records_exist()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();

                //Act
                var result = Service.GetCustomerChileProductsAttributeRanges(customer.ToCustomerKey());

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_CustomerChileProductAttributeRangeReturns_as_expected()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.ProductSpecs = TestHelper.List<CustomerProductAttributeRange>(3, (i, n) => i.Active = n % 2 == 0));
                Assert.GreaterOrEqual(customer.ProductSpecs.Count, 1);

                //Act
                var result = Service.GetCustomerChileProductsAttributeRanges(customer.ToCustomerKey());

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.SelectMany(r => r.AttributeRanges).ToList();
                foreach(var expected in customer.ProductSpecs.Where(s => s.Active))
                {
                    var rangeKey = expected.ToCustomerProductAttributeRangeKey();
                    var received = results.FirstOrDefault(r => r.CustomerChileProductAttributeRangeKey == rangeKey.KeyValue);
                    Assert.AreEqual(expected.RangeMin, received.RangeMin);
                    Assert.AreEqual(expected.RangeMax, received.RangeMax);
                }
            }
        }

        [TestFixture]
        public class GetCustomerProductCode : SalesServiceTests
        {
            [Test]
            public void Returns_null_success_result_if_CustomerProductCode_does_not_return()
            {
                //Arrange
                var customerKey = new CustomerKey((ICustomerKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>());
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());

                //Act
                var result = Service.GetCustomerProductCode(customerKey, chileProductKey);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(result.ResultingObject);
            }

            [Test]
            public void Returns_CustomerProductCode_as_expected_on_success()
            {
                //Arrange
                var customerProductCode = TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductCode>();

                //Act
                var result = Service.GetCustomerProductCode(new CustomerKey(customerProductCode), new ChileProductKey(customerProductCode));

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(customerProductCode.Code, result.ResultingObject.Value);
            }
        }

        [TestFixture]
        public class SetCustomerProductCode : SalesServiceTests
        {
            [Test]
            public void Updates_CustomerProductCode_if_it_exists()
            {
                //Arrange
                const string code = "CodeCODE";
                var customerProductCode = TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerProductCode>();

                //Act
                var result = Service.SetCustomerProductCode(new CustomerKey(customerProductCode), new ChileProductKey(customerProductCode), code);

                //Assert
                result.AssertSuccess();
                customerProductCode = RVCUnitOfWork.CustomerProductCodeRepository.FindByKey(new CustomerProductCodeKey(customerProductCode));
                Assert.AreEqual(code, customerProductCode.Code);
            }

            [Test]
            public void Creates_CustomerProductCode_if_it_does_not_exist()
            {
                //Arrange
                const string code = "blargh";
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                //Act
                var result = Service.SetCustomerProductCode(new CustomerKey((ICustomerKey)customer), new ChileProductKey(chileProduct), code);

                //Assert
                result.AssertSuccess();
                var customerProductCode = RVCUnitOfWork.CustomerProductCodeRepository.FindByKey(new CustomerProductCodeKey(customer, chileProduct));
                Assert.AreEqual(code, customerProductCode.Code);
            }

            [Test]
            public void Returns_non_successful_result_if_Customer_does_not_exist()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                //Act
                var result = Service.SetCustomerProductCode(new CustomerKey(), new ChileProductKey(chileProduct), "code");

                //Assert
                result.AssertNotSuccess(UserMessages.CustomerNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_ChileProduct_does_not_exist()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();

                //Act
                var result = Service.SetCustomerProductCode(new CustomerKey((ICustomerKey)customer), new ChileProductKey(), "code");

                //Assert
                result.AssertNotSuccess(UserMessages.ChileProductNotFound);
            }
        }

        [TestFixture]
        public class PostInvoice : SalesServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_CustomerOrder_does_not_exist()
            {
                //Act
                var result = Service.PostInvoice(new SalesOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.SalesOrderNotFound);
            }

            [Test]
            public void Returns_on_successful_result_if_CustomerOrder_is_not_Shipped()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Unscheduled);

                //Act
                var result = Service.PostInvoice(order.ToSalesOrderKey());

                //Assert
                result.AssertNotSuccess(UserMessages.SalesOrderNotShipped);
            }

            [Test]
            public void Sets_CustomerOrder_status_to_Invoiced()
            {
                //Arrange
                var order = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesOrder>(o => o.OrderStatus = SalesOrderStatus.Ordered, o => o.InventoryShipmentOrder.ShipmentInformation.Status = ShipmentStatus.Shipped);

                //Act
                var result = Service.PostInvoice(order.ToSalesOrderKey());

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(SalesOrderStatus.Invoiced, RVCUnitOfWork.SalesOrderRepository.FindByKey(order.ToSalesOrderKey()).OrderStatus);
            }
        }

        [TestFixture]
        public class GetSalesQuotes : SalesServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_records_exist()
            {
                //Act
                var result = Service.GetSalesQuotes();
                var results = result.ResultingObject.ToList();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(results);
            }

            [Test]
            public void Returns_SalesQuotes_as_expected()
            {
                //Arrange
                var expected = new List<SalesQuote>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>()
                    };

                //Act
                var result = Service.GetSalesQuotes();
                var results = result.ResultingObject.ToList();

                //Assert
                expected.AssertEquivalent(results, e => e.ToSalesQuoteKey().KeyValue, r => r.SalesQuoteKey, SalesQuoteExtensions.AssertEqual);
            }

            [Test]
            public void Filters_SalesQuotes_as_expected()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(q => q.SetCustomer(null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(q => q.SetBroker(null));

                var expected = new List<SalesQuote>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(q => q.SetCustomer(customer).SetBroker(broker)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(q => q.SetCustomer(customer).SetBroker(broker))
                    };

                //Act
                var result = Service.GetSalesQuotes(new FilterSalesQuotesParameters
                    {
                        CustomerKey = customer.ToCustomerKey(),
                        BrokerKey = broker.ToCompanyKey()
                    });
                var results = result.ResultingObject.ToList();

                //Assert
                expected.AssertEquivalent(results, e => e.ToSalesQuoteKey().KeyValue, r => r.SalesQuoteKey, SalesQuoteExtensions.AssertEqual);
            }
        }

        [TestFixture]
        public class GetSalesQuote : SalesServiceTests
        {
            [Test]
            public void Returns_invalid_result_if_SalesQuote_could_not_be_found_by_number()
            {
                //Act
                var result = Service.GetSalesQuote(0);

                //Assert
                result.AssertNotSuccess(UserMessages.SalesQuoteNotFound_Num);
            }

            [Test]
            public void Returns_invalid_result_if_SalesQuote_could_not_be_found_by_key()
            {
                //Act
                var result = Service.GetSalesQuote(new SalesQuoteKey());

                //Assert
                result.AssertNotSuccess(UserMessages.SalesQuoteNotFound_Key);
            }

            [Test]
            public void Returns_SalesQuote_by_number_as_expected()
            {
                //Arrange
                var expected = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(q => q.Items = TestHelper.List<SalesQuoteItem>(3));

                //Act
                var result = Service.GetSalesQuote(expected.QuoteNum.Value);

                //Assert
                result.AssertSuccess();
                expected.AssertEqual(result.ResultingObject);
            }

            [Test]
            public void Returns_SalesQuote_by_key_as_expected()
            {
                //Arrange
                var expected = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(q => q.Items = TestHelper.List<SalesQuoteItem>(3));

                //Act
                var result = Service.GetSalesQuote(expected.ToSalesQuoteKey());

                //Assert
                result.AssertSuccess();
                expected.AssertEqual(result.ResultingObject);
            }
        }

        [TestFixture]
        public class SetSalesQuote : SalesServiceTests
        {
            [Test]
            public void Creates_SalesQuote_records_as_expected()
            {
                //Arrange
                var parameters = new SalesQuoteParameters
                    {
                        UserToken = TestUser.UserName,
                        QuoteDate = new DateTime(2016, 1, 1),
                        CalledBy = "called",
                        TakenBy = "taken",
                        ShipmentInformation = new SetShipmentInformationWithStatus
                            {
                                PalletQuantity = 123,
                                PalletWeight = 100.0f,
                                ShippingInstructions = new SetShippingInstructions
                                    {
                                        ShipFromOrSoldTo = new ShippingLabel
                                            {
                                                Name = "sold to"
                                            }
                                    }
                            },
                        Items = new List<ISalesQuoteItemParameters>
                            {
                                new SalesQuoteItemParameters
                                    {
                                        ProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Product>().ToProductKey(),
                                        PackagingKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey(),
                                        TreatmentKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>().ToInventoryTreatmentKey()
                                    }
                            }
                    };

                //Act
                var result = Service.SetSalesQuote(parameters);

                //Assert
                result.AssertSuccess();
                var salesQuote = GetSalesQuoteResult(result.ResultingObject);
                parameters.AssertEqual(salesQuote);
            }

            [Test]
            public void Returns_non_successful_result_if_SalesQuote_could_not_be_found()
            {
                //Act
                var result = Service.SetSalesQuote(new SalesQuoteParameters
                    {
                        UserToken = TestUser.UserName,
                        SalesQuoteNumber = 123
                    }, true);

                //Assert
                result.AssertNotSuccess(UserMessages.SalesQuoteNotFound_Num);
            }

            [Test]
            public void Updates_SalesQuote_records_as_expected()
            {
                //Arrange
                var salesQuote = TestHelper.CreateObjectGraphAndInsertIntoDatabase<SalesQuote>(q => q.Items = TestHelper.List<SalesQuoteItem>(3));
                var parameters = new SalesQuoteParameters
                    {
                        UserToken = TestUser.UserName,
                        SalesQuoteNumber = salesQuote.QuoteNum,
                        QuoteDate = new DateTime(2016, 1, 1),
                        DateReceived = null,
                        Items = new List<ISalesQuoteItemParameters>(salesQuote.Items.Take(1).Select(i => new SalesQuoteItemParameters
                            {
                                SalesQuoteItemKey = i.ToSalesQuoteItemKey(),
                                ProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Product>().ToProductKey(),
                                PackagingKey = i.PackagingProduct.ToPackagingProductKey(),
                                TreatmentKey = i.Treatment.ToInventoryTreatmentKey()
                            }))
                            {
                                new SalesQuoteItemParameters
                                    {
                                        ProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Product>().ToProductKey(),
                                        PackagingKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey(),
                                        TreatmentKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>().ToInventoryTreatmentKey()
                                    }
                            }
                    };

                //Act
                var result = Service.SetSalesQuote(parameters, true);

                //Assert
                result.AssertSuccess();
                parameters.AssertEqual(GetSalesQuoteResult(result.ResultingObject));
            }

            private SalesQuote GetSalesQuoteResult(string salesQuoteKey)
            {
                return RVCUnitOfWork.SalesQuoteRepository.FindByKey(KeyParserHelper.ParseResult<ISalesQuoteKey>(salesQuoteKey).ResultingObject.ToSalesQuoteKey(),
                    q => q.Employee,
                    q => q.SourceFacility,
                    q => q.Customer,
                    q => q.Broker,
                    q => q.ShipmentInformation,
                    q => q.Items.Select(i => i.Product),
                    q => q.Items.Select(i => i.PackagingProduct),
                    q => q.Items.Select(i => i.Treatment));
            }
        }
    }
}

// ReSharper restore InconsistentNaming