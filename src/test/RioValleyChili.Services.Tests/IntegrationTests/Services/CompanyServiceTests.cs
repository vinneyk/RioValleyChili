using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class CompanyServiceTests : ServiceIntegrationTestBase<CompanyService>
    {
        [TestFixture]
        public class CreateCompanyTests : CompanyServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_no_CompanyTypes_are_supplied()
            {
                //Act
                var nullResult = Service.CreateCompany(new CreateCompanyParameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyTypes = null
                    });
                var emptyResult = Service.CreateCompany(new CreateCompanyParameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyTypes = new List<CompanyType>()
                    });

                //Assert
                nullResult.AssertNotSuccess(UserMessages.CompanyTypesRequired);
                emptyResult.AssertNotSuccess(UserMessages.CompanyTypesRequired);
            }

            [Test]
            public void Creates_new_Company_record_without_a_parent_as_expected_on_success()
            {
                //Arrange
                var parameters = new CreateCompanyParameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyName = "Company Inc.",
                        CompanyTypes = new List<CompanyType> { CompanyType.Broker },
                        Active = false
                    };
                
                //Act
                var result = Service.CreateCompany(parameters);

                //Assert
                result.AssertSuccess();
                var company = RVCUnitOfWork.CompanyRepository.All().Select(c => new { company = c, c.CompanyTypes } ).Single(c => c.company.Name == parameters.CompanyName).company;
                Assert.IsTrue(parameters.CompanyTypes.All(t => company.CompanyTypes.Any(r => r.CompanyTypeEnum == t)));
                Assert.AreEqual(parameters.Active, company.Active);
            }

            [Test]
            public void Creates_CompanyTypeRecords_as_expected_on_success()
            {
                //Arrange
                const string companyName = "Banana Co.";
                var companyTypes = new List<CompanyType>
                    {
                        CompanyType.Broker,
                        CompanyType.Dehydrator,
                        CompanyType.Freight
                    };
                var parameters = new CreateCompanyParameters
                {
                    UserToken = TestUser.UserName,
                    CompanyName = companyName,
                    CompanyTypes = companyTypes,
                    Active = true
                };

                //Act
                var result = Service.CreateCompany(parameters);

                //Assert
                result.AssertSuccess();
                var companyResult = RVCUnitOfWork.CompanyRepository.All().Select(c => new { company = c, c.CompanyTypes }).SingleOrDefault(c => c.company.Name == companyName);
                Assert.NotNull(companyResult);
                var companyTypeRecords = companyResult.CompanyTypes.ToList();
                Assert.AreEqual(companyTypes.Count, companyTypeRecords.Count);
                companyTypes.ForEach(t => Assert.IsTrue(companyTypeRecords.Count(r => r.CompanyTypeEnum == t) == 1));
            }

            [Test]
            public void Does_not_create_associated_Customer_record_if_Customer_CompanyType_is_not_supplied()
            {
                //Arrange
                const string companyName = "Bebe";
                var parameters = new CreateCompanyParameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyName = companyName,
                        CompanyTypes = new List<CompanyType> { CompanyType.Broker },
                        Active = true
                    };

                //Act
                var result = Service.CreateCompany(parameters);

                //Assert
                result.AssertSuccess();
                var company = RVCUnitOfWork.CompanyRepository.All().SingleOrDefault(c => c.Name == companyName);
                Assert.NotNull(company);
                Assert.IsNull(RVCUnitOfWork.CustomerRepository.All().FirstOrDefault());
            }

            [Test]
            public void Creates_new_Customer_record_as_expected_if_Customer_CompanyType_is_supplied()
            {
                //Arrange
                var parameters = new CreateCompanyParameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyName = "Bebe",
                        CompanyTypes = new List<CompanyType> { CompanyType.Customer },
                        Active = true
                    };

                //Act
                var result = Service.CreateCompany(parameters);

                //Assert
                result.AssertSuccess();
                var customer = RVCUnitOfWork.CustomerRepository.Filter(c => true, c => c.Company, c => c.Broker, c => c.Company.CompanyTypes).Single(c => c.Company.Name == parameters.CompanyName);
                Assert.IsTrue(parameters.CompanyTypes.All(t => customer.Company.CompanyTypes.Any(r => r.CompanyTypeEnum == t)));
                Assert.AreEqual(parameters.Active, customer.Company.Active);
                Assert.AreEqual(StaticCompanies.RVCBroker.Id, customer.BrokerId);
                Assert.AreEqual(StaticCompanies.RVCBroker.Name, customer.Broker.Name);
            }

            [Test]
            public void Creates_new_Customer_record_with_Broker_as_expected()
            {
                //Arrange
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));
                var parameters = new CreateCompanyParameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyName = "Bebe",
                        CompanyTypes = new List<CompanyType> { CompanyType.Customer },
                        Active = true,
                        BrokerKey = broker.ToCompanyKey()
                    };

                //Act
                var result = Service.CreateCompany(parameters);

                //Assert
                result.AssertSuccess();
                var customer = RVCUnitOfWork.CustomerRepository.Filter(c => true, c => c.Company, c => c.Broker, c => c.Company.CompanyTypes).Single(c => c.Company.Name == parameters.CompanyName);
                Assert.IsTrue(parameters.CompanyTypes.All(t => customer.Company.CompanyTypes.Any(r => r.CompanyTypeEnum == t)));
                Assert.AreEqual(parameters.Active, customer.Company.Active);
                Assert.AreEqual(broker.Id, customer.BrokerId);
            }
        }

        [TestFixture]
        public class UpdateCompanyTests : CompanyServiceTests
        {
            private class Parameters : IUpdateCompanyParameters
            {
                public string UserToken { get; set; }
                public string CompanyKey { get; set; }
                public bool Active { get; set; }
                public string BrokerKey { get; set; }
                public IEnumerable<CompanyType> CompanyTypes { get; set; }
            }

            [Test]
            public void Returns_non_successful_result_if_Company_could_not_be_found()
            {
                //Act
                var result = Service.UpdateCompany(new Parameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyKey = new CompanyKey().KeyValue,
                        CompanyTypes = new List<CompanyType> { CompanyType.Broker }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CompanyNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_no_CompanyTypes_are_supplied()
            {
                //Arrange
                var companyKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>());

                //Act
                var resultNull = Service.UpdateCompany(new Parameters
                    {
                        CompanyKey = companyKey.KeyValue,
                        CompanyTypes = null
                    });
                var resultEmpty = Service.UpdateCompany(new Parameters
                    {
                        CompanyKey = companyKey.KeyValue,
                        CompanyTypes = new List<CompanyType>()
                    });

                //Assert
                resultNull.AssertNotSuccess(UserMessages.CompanyTypesRequired);
                resultEmpty.AssertNotSuccess(UserMessages.CompanyTypesRequired);
            }

            [Test]
            public void Sets_CompanyTypeRecords_as_expected()
            {
                //Arrange
                var companyKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker, CompanyType.Dehydrator)));
                var expectedTypes = new List<CompanyType>
                    {
                        CompanyType.Dehydrator,
                        CompanyType.Freight,
                        CompanyType.Supplier
                    };

                //Act
                var result = Service.UpdateCompany(new Parameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyKey = companyKey.KeyValue,
                        CompanyTypes = expectedTypes
                    });

                //Assert
                result.AssertSuccess();
                var companyTypes = RVCUnitOfWork.CompanyRepository.FindByKey(companyKey, c => c.CompanyTypes).CompanyTypes.ToList();
                Assert.AreEqual(expectedTypes.Count, companyTypes.Count);
                expectedTypes.ForEach(e => Assert.IsTrue(companyTypes.Count(t => t.CompanyTypeEnum == e) == 1));
            }

            [Test]
            public void Creates_Customer_record_as_expected_if_Customer_CompanyType_is_supplied()
            {
                //Arrange
                var companyKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker, CompanyType.Dehydrator)).ToCompanyKey();
                var expectedTypes = new List<CompanyType> { CompanyType.Customer };

                //Act
                var result = Service.UpdateCompany(new Parameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyKey = companyKey.KeyValue,
                        CompanyTypes = expectedTypes
                    });

                //Assert
                result.AssertSuccess();
                var customer = RVCUnitOfWork.CustomerRepository.FindByKey(companyKey);
                Assert.NotNull(customer);
            }

            [Test]
            public void Updates_Customer_record_as_expected_if_Customer_CompanyType_is_supplied()
            {
                //Arrange
                var companyKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Company.SetCompanyTypes(CompanyType.Customer)).ToCompanyKey();
                var broker = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));

                //Act
                var result = Service.UpdateCompany(new Parameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyKey = companyKey.KeyValue,
                        CompanyTypes = new List<CompanyType> { CompanyType.Customer },
                        BrokerKey = broker.ToCompanyKey()
                    });

                //Assert
                result.AssertSuccess();
                var customer = RVCUnitOfWork.CustomerRepository.FindByKey(companyKey);
                Assert.AreEqual(broker.Id, customer.BrokerId);
            }
        }

        [TestFixture]
        public class GetCompanyTests : CompanyServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_CompanyKey_could_not_be_parsed()
            {
                //Act
                var result = TimedExecution(() => Service.GetCompany("Bad-Company"), "Act");

                //Assert
                result.AssertNotSuccess(UserMessages.InvalidCompanyKey);
            }

            [Test]
            public void Returns_non_successful_result_if_Company_does_not_exist()
            {
                //Act
                var result = TimedExecution(() => Service.GetCompany(new CompanyKey().KeyValue), "Act");

                //Assert
                result.AssertNotSuccess(UserMessages.CompanyNotFound);
            }

            [Test]
            public void Returns_Company_result_as_expected()
            {
                //Arrange
                var company = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Supplier));
                var companyKey = company.ToCompanyKey();

                //Act
                var result = TimedExecution(() => Service.GetCompany(companyKey.KeyValue), "Act");

                //Assert
                result.AssertSuccess();
                company.AssertEqual(result.ResultingObject);
            }

            [Test]
            public void Returns_Customer_Company_result_as_expected()
            {
                //Arrange
                StartStopwatch();
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>(c => c.Company.SetCompanyTypes(CompanyType.Customer),
                    c => c.Broker.SetCompanyTypes(CompanyType.Broker));
                var companyKey = customer.ToCompanyKey();

                StopWatchAndWriteTime("Arrange");

                //Act
                var result = TimedExecution(() => Service.GetCompany(companyKey.KeyValue), "Act");

                //Assert
                result.AssertSuccess();
                customer.Company.AssertEqual(result.ResultingObject);
            }
        }

        [TestFixture]
        public class GetCompaniesTests : CompanyServiceTests
        {
            protected override bool SetupStaticRecords { get { return false; } }

            [Test]
            public void Returns_empty_results_if_no_Company_records_exist()
            {
                //Act
                var result = TimedExecution(() => Service.GetCompanies(), "Act");

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_all_Company_summaries_as_expected_if_no_filtering_is_specified()
            {
                //Arrange
                const int expectedResults = 3;
                var company0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>();
                var company1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>();
                var company2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>();

                //Act
                StartStopwatch();
                var result = Service.GetCompanies();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                company0.AssertEqual(results.SingleOrDefault(r => r.CompanyKey == new CompanyKey(company0).KeyValue));
                company1.AssertEqual(results.SingleOrDefault(r => r.CompanyKey == new CompanyKey(company1).KeyValue));
                company2.AssertEqual(results.SingleOrDefault(r => r.CompanyKey == new CompanyKey(company2).KeyValue));
            }

            [Test]
            public void Returns_all_Companies_of_the_specified_CompanyType()
            {
                //Arrange
                const int expectedResults = 3;
                var company0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Customer));
                var company1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Customer));
                var company2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Customer));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Broker));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator));

                //Act
                StartStopwatch();
                var result = Service.GetCompanies(new FilterCompanyParameters
                    {
                        CompanyType = CompanyType.Customer
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                company0.AssertEqual(results.SingleOrDefault(r => r.CompanyKey == new CompanyKey(company0).KeyValue));
                company1.AssertEqual(results.SingleOrDefault(r => r.CompanyKey == new CompanyKey(company1).KeyValue));
                company2.AssertEqual(results.SingleOrDefault(r => r.CompanyKey == new CompanyKey(company2).KeyValue));
            }
        }

        [TestFixture]
        public class CreateContactTests : CompanyServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Company_could_not_be_found()
            {
                //Act
                var result = Service.CreateContact(new CreateContactParameters
                    {
                        CompanyKey = new CompanyKey().KeyValue
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.CompanyNotFound);
            }

            [Test]
            public void Creates_new_Contact_record_as_expected_on_success()
            {
                //Arrange
                const string expectedName = "Batman";
                const string expectedPhone = "batsignal";
                const string expectedEmail = "bwayne@gmail.com";
                
                const string expectedAddressLine1 = "Big ol' mansion.";
                const string expectedAddressLine2 = "I dunno some cave I guess.";

                const string addressDescription1 = "Some Address";
                const string addressDescription2 = "Some Other Address";

                var company = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.Contacts = null);
                var companyKey = new CompanyKey(company);

                

                //Act
                var result = Service.CreateContact(new CreateContactParameters
                    {
                        CompanyKey = companyKey.KeyValue,
                        Name = expectedName,
                        PhoneNumber = expectedPhone,
                        EmailAddress = expectedEmail,
                        Addresses = new List<IContactAddressReturn>
                            {
                                new ContactAddressParameters
                                    {
                                        AddressDescription = addressDescription1,
                                        Address = new Address { AddressLine1 = expectedAddressLine1 }
                                    },
                                new ContactAddressParameters
                                    {
                                        AddressDescription = addressDescription2,
                                        Address = new Address { AddressLine1 = expectedAddressLine2 }
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var contact = RVCUnitOfWork.ContactRepository.Filter(c => true, c => c.Addresses).Single();
                var addresses = contact.Addresses.ToList();
                Assert.AreEqual(companyKey, contact);
                Assert.AreEqual(expectedName, contact.Name);
                Assert.AreEqual(expectedPhone, contact.PhoneNumber);
                Assert.AreEqual(expectedEmail, contact.EMailAddress);
                Assert.IsNotNull(addresses.Single(a => a.AddressDescription == addressDescription1 && a.Address.AddressLine1 == expectedAddressLine1));
                Assert.IsNotNull(addresses.Single(a => a.AddressDescription == addressDescription2 && a.Address.AddressLine1 == expectedAddressLine2));
            }

            [Test]
            public void Creates_new_Contact_record_with_no_Addresses()
            {
                //Arrange
                const string expectedName = "Batman";
                const string expectedPhone = "batsignal";
                const string expectedEmail = "bwayne@gmail.com";

                var company = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.Contacts = null);
                var companyKey = new CompanyKey(company);

                

                //Act
                var result = Service.CreateContact(new CreateContactParameters
                {
                    CompanyKey = companyKey.KeyValue,
                    Name = expectedName,
                    PhoneNumber = expectedPhone,
                    EmailAddress = expectedEmail,
                });

                //Assert
                result.AssertSuccess();
                var contact = RVCUnitOfWork.ContactRepository.Filter(c => true, c => c.Addresses).Single();
                Assert.IsEmpty(contact.Addresses);
                Assert.AreEqual(companyKey, contact);
                Assert.AreEqual(expectedName, contact.Name);
                Assert.AreEqual(expectedPhone, contact.PhoneNumber);
                Assert.AreEqual(expectedEmail, contact.EMailAddress);
            }
        }

        [TestFixture]
        public class UpdateContactTests : CompanyServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Contact_could_not_be_found()
            {
                //Act
                var result = Service.UpdateContact(new UpdateContactParameters
                    {
                        ContactKey = new ContactKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.ContactNotFound);
            }

            [Test]
            public void Removes_all_ContactAddresses_if_null_Addresses_is_passed()
            {
                //Arrange
                var contactKey = new ContactKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>(c => c.Addresses = null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContactAddress>(a => a.SetContact(contactKey));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContactAddress>(a => a.SetContact(contactKey));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContactAddress>(a => a.SetContact(contactKey));

                //Act
                var result = Service.UpdateContact(new UpdateContactParameters
                    {
                        ContactKey = contactKey
                    });

                //Assert
                result.AssertSuccess();
                var contact = RVCUnitOfWork.ContactRepository.FindByKey(contactKey, c => c.Addresses);
                Assert.IsEmpty(contact.Addresses);
            }

            [Test]
            public void Updates_Contact_as_expected_on_success()
            {
                //Arrange
                const int expectedAddresses = 2;
                const string expectedAddressDescription = "Desc";
                const string expectedName = "Jimmy Corrigan";
                const string expectedPhoneNumber = "lonelyNumber";
                const string expectedEmail = "probably none";

                var updatedAddress = new Address
                    {
                        AddressLine1 = "updated"
                    };

                var newAddress = new Address
                {
                    AddressLine1 = "new"
                };

                var contactKey = new ContactKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>(c => c.Addresses = null));
                var addressToUpdateKey = new ContactAddressKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContactAddress>(a => a.SetContact(contactKey)));
                var addressToRemoveKey = new ContactAddressKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContactAddress>(a => a.SetContact(contactKey)));

                //Act
                var result = Service.UpdateContact(new UpdateContactParameters
                    {
                        ContactKey = contactKey,
                        Name = expectedName,
                        PhoneNumber = expectedPhoneNumber,
                        EmailAddress = expectedEmail,
                        Addresses = new List<IContactAddressReturn>
                            {
                                new ContactAddressParameters
                                    {
                                        ContactAddressKey = addressToUpdateKey.KeyValue,
                                        AddressDescription = expectedAddressDescription,
                                        Address = updatedAddress
                                    },
                                new ContactAddressParameters
                                    {
                                        Address = newAddress
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();

                var contact = RVCUnitOfWork.ContactRepository.FindByKey(contactKey, c => c.Addresses);
                Assert.AreEqual(expectedName, contact.Name);
                Assert.AreEqual(expectedPhoneNumber, contact.PhoneNumber);
                Assert.AreEqual(expectedEmail, contact.EMailAddress);

                var addresses = contact.Addresses.ToList();
                Assert.AreEqual(expectedAddresses, addresses.Count);

                var contactAddress = addresses.Single(a => addressToUpdateKey.Equals(a));
                Assert.AreEqual(contactAddress.AddressDescription, expectedAddressDescription);
                contactAddress.Address.AssertEqual(updatedAddress);
                Assert.IsNull(addresses.SingleOrDefault(a => addressToRemoveKey.Equals(a)));
                addresses.Single(a => a.Address.AddressLine1 == newAddress.AddressLine1).Address.AssertEqual(newAddress);
            }
        }

        [TestFixture]
        public class GetContactsTests : CompanyServiceTests
        {
            [Test]
            public void Returns_empty_collection_if_no_Contacts_exist()
            {
                //Act
                var result = Service.GetContacts();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_empty_collection_if_no_Contacts_of_supplied_Company_exist()
            {
                //Arrange
                var companyKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.EmptyItems()));

                //Act
                var result = Service.GetContacts(new FilterContactsParameters { CompanyKey = companyKey.KeyValue });

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_all_Contacts_if_no_filtering_options_are_specified()
            {
                //Arrange
                const int expectedContacts = 3;
                var contact0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>();
                var contact1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>();
                var contact2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>();

                //Act
                var result = Service.GetContacts();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(expectedContacts, results.Count);
                
                contact0.AssertEqual(results.Single(r => new ContactKey(contact0).KeyValue == r.ContactKey));
                contact1.AssertEqual(results.Single(r => new ContactKey(contact1).KeyValue == r.ContactKey));
                contact2.AssertEqual(results.Single(r => new ContactKey(contact2).KeyValue == r.ContactKey));
            }

            [Test]
            public void Returns_only_Contacts_of_specified_Company()
            {
                //Arrange
                const int expectedContacts = 3;

                var companyKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.EmptyItems()));
                var contact0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>(c => c.ConstrainByKeys(companyKey));
                var contact1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>(c => c.ConstrainByKeys(companyKey));
                var contact2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>(c => c.ConstrainByKeys(companyKey));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>();

                //Act
                var result = Service.GetContacts(new FilterContactsParameters { CompanyKey = companyKey.KeyValue} );

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(expectedContacts, results.Count);

                contact0.AssertEqual(results.Single(r => new ContactKey(contact0).KeyValue == r.ContactKey));
                contact1.AssertEqual(results.Single(r => new ContactKey(contact1).KeyValue == r.ContactKey));
                contact2.AssertEqual(results.Single(r => new ContactKey(contact2).KeyValue == r.ContactKey));
            }
        }

        [TestFixture]
        public class GetContactTest : CompanyServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Contact_does_not_exist()
            {
                //Act
                var result = Service.GetContact(new ContactKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.ContactNotFound);
            }

            [Test]
            public void Returns_Contact_as_expected()
            {
                //Arrange
                var contact = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>();

                //Act
                var result = Service.GetContact(new ContactKey(contact).KeyValue);

                //Assert
                result.AssertSuccess();
                contact.AssertEqual(result.ResultingObject);
            }
        }

        [TestFixture]
        public class DeleteContactTests : CompanyServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Contact_does_not_exist()
            {
                //Act
                var result = Service.DeleteContact(new ContactKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.ContactNotFound);
            }

            [Test]
            public void Removes_Contact_and_associated_records_from_database_on_success()
            {
                //Arrange
                var contactKey = new ContactKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Contact>(c => c.Addresses = null));
                var addressKey0 = new ContactAddressKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContactAddress>(c => c.SetContact(contactKey)));
                var addressKey1 = new ContactAddressKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContactAddress>(c => c.SetContact(contactKey)));
                var addressKey2 = new ContactAddressKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ContactAddress>(c => c.SetContact(contactKey)));

                

                //Act
                var result = Service.DeleteContact(contactKey.KeyValue);

                //Assert
                result.AssertSuccess();

                Assert.IsNull(RVCUnitOfWork.ContactAddressRepository.FindByKey(addressKey0));
                Assert.IsNull(RVCUnitOfWork.ContactAddressRepository.FindByKey(addressKey1));
                Assert.IsNull(RVCUnitOfWork.ContactAddressRepository.FindByKey(addressKey2));
                Assert.IsNull(RVCUnitOfWork.ContactRepository.FindByKey(contactKey));
            }
        }

        [TestFixture]
        public class CreateCustomerNoteTests : CompanyServiceTests
        {
            [Test]
            public void Creates_CustomerNote_as_expected_on_success()
            {
                //Arrange
                var customer = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Customer>();

                //Act
                var parameters = new CreateCustomerNoteParameters
                    {
                        CustomerKey = customer.ToCustomerKey(),
                        UserToken = TestUser.UserName,
                        Bold = true,
                        Text = "The calm, cool face of the river asked me for a kiss.",
                        Type = "Suicide's Note"
                    };
                var result = Service.CreateCustomerNote(parameters);

                //Assert
                result.AssertSuccess();
                parameters.AssertEqual(RVCUnitOfWork.CustomerNoteRepository.Filter(n => true, n => n.Employee).SingleOrDefault());
            }
        }

        [TestFixture]
        public class UpdateCustomerNoteTests : CompanyServiceTests
        {
            [Test]
            public void Updates_CustomerNote_as_expected_on_success()
            {
                //Arrange
                var customerNote = TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerNote>();

                //Act
                var parameters = new UpdateCustomerNoteParameters
                    {
                        CustomerNoteKey = customerNote.ToCustomerNoteKey(),
                        UserToken = TestUser.UserName,
                        Bold = true,
                        Text = "The calm, cool face of the river asked me for a kiss.",
                        Type = "Suicide's Note"
                    };
                var result = Service.UpdateCustomerNote(parameters);

                //Assert
                result.AssertSuccess();
                parameters.AssertEqual(RVCUnitOfWork.CustomerNoteRepository.FindByKey(customerNote.ToCustomerNoteKey(), n => n.Employee));
            }
        }

        [TestFixture]
        public class DeleteCustomerNoteTEsts : CompanyServiceTests
        {
            [Test]
            public void Deletes_CustomerNote_record()
            {
                //Arrange
                var customerNote = TestHelper.CreateObjectGraphAndInsertIntoDatabase<CustomerNote>();

                //Act
                var result = Service.DeleteCustomerNote(customerNote.ToCustomerNoteKey());

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.CustomerNoteRepository.FindByKey(customerNote.ToCustomerNoteKey()));
            }
        }
    }
}
