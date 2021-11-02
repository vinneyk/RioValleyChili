using System;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ContactExtensions
    {
        internal static Contact ConstrainByKeys(this Contact contact, ICompanyKey companyKey)
        {
            if(contact == null) { throw new ArgumentNullException("contact"); }

            if(companyKey != null)
            {
                contact.Company = null;
                contact.CompanyId = companyKey.CompanyKey_Id;
            }

            return contact;
        }

        internal static void AssertEqual(this Contact contact, IContactSummaryReturn contactReturn)
        {
            if(contact == null) { throw new ArgumentNullException("contact"); }
            if(contactReturn == null) { throw new ArgumentNullException("contactReturn"); }

            Assert.AreEqual(new ContactKey(contact).KeyValue, contactReturn.ContactKey);
            Assert.AreEqual(new CompanyKey(contact).KeyValue, contactReturn.CompanyKey);
            Assert.AreEqual(contact.Name, contactReturn.Name);
            Assert.AreEqual(contact.PhoneNumber, contactReturn.PhoneNumber);
            Assert.AreEqual(contact.EMailAddress, contactReturn.EMailAddress);
            if(contact.Addresses == null)
            {
                if(contactReturn.Addresses != null)
                {
                    Assert.IsEmpty(contactReturn.Addresses);
                }
            }
            else
            {
                contact.Addresses.ForEach(a =>
                    {
                        var contactAddressKey = new ContactAddressKey(a);
                        a.Address.AssertEqual(contactReturn.Addresses.Single(r => contactAddressKey.KeyValue == r.ContactAddressKey).Address);
                    });
            }
        }
    }
}