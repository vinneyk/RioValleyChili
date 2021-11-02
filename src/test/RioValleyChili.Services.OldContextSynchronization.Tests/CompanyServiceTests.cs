using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class CompanyServiceTests
    {
        [TestFixture]
        public class CreateCompanyUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, CompanyKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncCompany; } }
        }

        [TestFixture]
        public class CreateCompany : SynchronizeOldContextIntegrationTestsBase<CompanyService>
        {
            [Test]
            public void Creates_new_Company_record()
            {
                //Arrange
                var companyName = Guid.NewGuid().ToString();
                companyName = companyName.Substring(0, Math.Min(companyName.Length, Constants.StringLengths.CompanyName));

                //Act
                var result = Service.CreateCompany(new CreateCompanyParameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyName = companyName,
                        Active = true,
                        CompanyTypes = new[]
                            {
                                CompanyType.Freight
                            }
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var companyKey = GetKeyFromConsoleString(ConsoleOutput.SynchedCompany);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var company = oldContext.Companies.FirstOrDefault(c => c.Company_IA == companyKey);
                    Assert.AreEqual("Freight", company.CType);
                }
            }
        }

        [TestFixture]
        public class CreateContact : SynchronizeOldContextIntegrationTestsBase<CompanyService>
        {
            [Test]
            public void Creates_new_Contact_records()
            {
                //Arrange
                var company = RVCUnitOfWork.CompanyRepository
                    .Filter(c => c.Contacts.All(n => n.Name != "contact0"))
                    .FirstOrDefault();
                if(company == null)
                {
                    Assert.Inconclusive("No suitable Company to test.");
                }

                //Act
                var result = Service.CreateContact(new CreateContactParameters
                    {
                        UserToken = TestUser.UserName,
                        CompanyKey = company.ToCompanyKey(),
                        Name = "contact0",
                        EmailAddress = "oh@my.exclamationPoint",
                        PhoneNumber = "nope",
                        Addresses = new List<IContactAddressReturn>
                            {
                                new ContactAddressParameters
                                    {
                                        AddressDescription = "addr1"
                                    },
                                new ContactAddressParameters
                                    {
                                        AddressDescription = "addr2"
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var companyKey = GetKeyFromConsoleString(ConsoleOutput.SynchedCompany);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var contacts = oldContext.Contacts.Where(c => c.Company_IA == companyKey && c.Contact_IA == "contact0").ToList();
                    Assert.IsNotNull(contacts.SingleOrDefault(c => c.AddrType == "addr1"));
                    Assert.IsNotNull(contacts.SingleOrDefault(c => c.AddrType == "addr2"));
                }
            }
        }

        [TestFixture]
        public class Update : SynchronizeOldContextIntegrationTestsBase<CompanyService>
        {
            [Test]
            public void Updates_Contact_record()
            {
                //Arrange
                var contact = RVCUnitOfWork.ContactRepository
                    .Filter(n => n.Name != "removedAddresses" && n.OldContextID != null && n.Addresses.Count > 1)
                    .FirstOrDefault();
                if(contact == null)
                {
                    Assert.Inconclusive("No suitable Contact to test.");
                }
                
                //Act
                var result = Service.UpdateContact(new UpdateContactParameters
                    {
                        ContactKey = contact.ToContactKey(),
                        UserToken = TestUser.UserName,
                        Name = "removedAddresses",
                        EmailAddress = "oh@my.exclamationPoint",
                        PhoneNumber = "nope"
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var companyKey = GetKeyFromConsoleString(ConsoleOutput.SynchedCompany);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var oldContact = oldContext.Contacts.SingleOrDefault(c => c.Company_IA == companyKey && c.Contact_IA == "removedAddresses");
                    Assert.AreEqual(null, oldContact.Address1_IA);
                }
            }
        }

        [TestFixture]
        public class Delete : SynchronizeOldContextIntegrationTestsBase<CompanyService>
        {
            [Test]
            public void Deletes_Contact_record()
            {
                //Arrange
                var contact = RVCUnitOfWork.ContactRepository.Filter(c => c.OldContextID != null).FirstOrDefault();
                if(contact == null)
                {
                    Assert.Inconclusive("No suitable Contact to test.");
                }
                var oldId = contact.OldContextID;

                //Act
                var result = Service.DeleteContact(contact.ToContactKey());

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                using(var oldContext = new RioAccessSQLEntities())
                {
                    Assert.IsNull(oldContext.Contacts.FirstOrDefault(n => n.ID == oldId));
                }
            }
        }
    }
}