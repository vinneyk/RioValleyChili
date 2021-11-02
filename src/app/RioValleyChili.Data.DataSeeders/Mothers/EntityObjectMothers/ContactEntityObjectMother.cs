using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class ContactEntityObjectMother : EntityMotherLogBase<Models.Contact, ContactEntityObjectMother.CallbackParameters>
    {
        private readonly RioValleyChiliDataContext _newContext;

        public ContactEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            _newContext = newContext;
        }

        protected override IEnumerable<Models.Contact> BirthRecords()
        {
            var oldContactsByCompany = OldContext.CreateObjectSet<Contact>().GroupBy(c => c.Company_IA).ToList();

            foreach(var oldCompanyContacts in oldContactsByCompany)
            {
                var oldContactAddressesGroup = oldCompanyContacts.GroupBy(c => new ContactKey(c), ContactKey.ContactKeyEqualityComparer);
                var contactId = 0;
                foreach(var oldContactAddresses in oldContactAddressesGroup)
                {
                    if(oldCompanyContacts.Key == null)
                    {
                        foreach(var contact in oldCompanyContacts)
                        {
                            Log(new CallbackParameters(CallbackReason.NullCompany)
                                {
                                    Contact = contact
                                });
                        }
                        continue;
                    }

                    var companies = _newContext.Set<Models.Company>().Where(c => c.Name == oldCompanyContacts.Key).ToList();
                    if(companies.Count != 1)
                    {
                        foreach(var contact in oldCompanyContacts)
                        {
                            Log(new CallbackParameters(CallbackReason.MultipleCompanies)
                                {
                                    Contact = contact
                                });
                        }
                        continue;
                    }
                    var company = companies.Single();
                    
                    var addressId = 0;
                    contactId += 1;

                    var contactAddresses = new List<ContactAddress>();
                    foreach(var oldContactAddress in oldContactAddresses.Select(AddressHelper.ToAddress).Distinct(AddressHelper.AddressEqualityComparer))
                    {
                        addressId += 1;
                        contactAddresses.Add(new ContactAddress
                            {
                                CompanyId = company.Id,
                                ContactId = contactId,
                                AddressId = addressId,

                                Address = oldContactAddress
                            });
                    }

                    var name = oldContactAddresses.Key.Name;
                    if(name != null && name.Length > Constants.StringLengths.ContactName)
                    {
                        name = string.Concat(name.Take(Constants.StringLengths.ContactName));
                    }

                    var phoneNumber = oldContactAddresses.Key.PhoneNumber;
                    if(phoneNumber != null && phoneNumber.Length > Constants.StringLengths.PhoneNumber)
                    {
                        phoneNumber = string.Concat(phoneNumber.Take(Constants.StringLengths.PhoneNumber));
                    }

                    var email = oldContactAddresses.Key.EMailAddress;
                    if(email != null && email.Length > Constants.StringLengths.Email)
                    {
                        email = string.Concat(email.Take(Constants.StringLengths.Email));
                    }

                    yield return new Models.Contact
                        {
                            CompanyId = company.Id,
                            ContactId = contactId,

                            Name = name,
                            PhoneNumber = phoneNumber,
                            EMailAddress = email,

                            Addresses = contactAddresses
                        };
                }
            }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            NullCompany,
            MultipleCompanies,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public Contact Contact { get; set; }

            protected override CallbackReason ExceptionReason { get { return ContactEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return ContactEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return ContactEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case ContactEntityObjectMother.CallbackReason.Exception: return ReasonCategory.Error;
                        
                    case ContactEntityObjectMother.CallbackReason.NullCompany:
                    case ContactEntityObjectMother.CallbackReason.MultipleCompanies: return ReasonCategory.RecordSkipped;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}