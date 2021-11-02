using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ContactAddressExtensions
    {
        internal static ContactAddress SetContact(this ContactAddress address, IContactKey contactKey)
        {
            if(address == null) { throw new ArgumentNullException("address"); }
            if(contactKey == null) { throw new ArgumentNullException("contactKey"); }

            address.Contact = null;
            address.CompanyId = contactKey.CompanyKey_Id;
            address.ContactId = contactKey.ContactKey_Id;

            return address;
        }

        internal static ContactAddress SetCompany(this ContactAddress contactAddress, ICompanyKey companyKey)
        {
            if(contactAddress == null) { throw new ArgumentNullException("contactAddress"); }

            contactAddress.CompanyId = companyKey.CompanyKey_Id;

            if(contactAddress.Contact != null)
            {
                contactAddress.Contact.ConstrainByKeys(companyKey);
            }

            return contactAddress;
        }

        internal static void AssertEqual(this ContactAddress contactAddress, IContactAddressReturn contactAddressReturn)
        {
            if(contactAddress == null) { throw new ArgumentNullException("contactAddress"); }
            if(contactAddressReturn == null) { throw new ArgumentNullException("contactAddressReturn"); }

            Assert.AreEqual(new ContactAddressKey(contactAddress).KeyValue, contactAddressReturn.ContactAddressKey);
            Assert.AreEqual(contactAddress.AddressDescription, contactAddressReturn.AddressDescription);
            contactAddress.Address.AssertEqual(contactAddressReturn.Address);
        }
    }
}