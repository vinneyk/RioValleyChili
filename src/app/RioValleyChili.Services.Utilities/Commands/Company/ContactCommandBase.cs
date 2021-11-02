using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Company
{
    internal abstract class ContactCommandBase
    {
        protected readonly ICompanyUnitOfWork CompanyUnitOfWork;

        internal ContactCommandBase(ICompanyUnitOfWork companyUnitOfWork)
        {
            if(companyUnitOfWork == null) { throw new ArgumentNullException("companyUnitOfWork"); }
            CompanyUnitOfWork = companyUnitOfWork;
        }

        internal IResult<Contact> SetContact(Contact contact, ContactCommandParametersBase parameters)
        {
            if(contact == null) { throw new ArgumentNullException("contact"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            contact.Name = parameters.BaseParameters.Name;
            contact.PhoneNumber = parameters.BaseParameters.PhoneNumber;
            contact.EMailAddress = parameters.BaseParameters.EmailAddress;

            var addressesResult = SetAddresses(contact, parameters.SetAddresses);
            if(!addressesResult.Success)
            {
                return addressesResult.ConvertTo<Contact>();
            }

            return new SuccessResult<Contact>(contact);
        }
        
        private IResult SetAddresses(Contact contact, IEnumerable<SetContactAddressParameters> setAddresses)
        {
            if(contact == null) { throw new ArgumentNullException("contact"); }
            if(setAddresses == null) { throw new ArgumentNullException("setAddresses"); }

            var setAddressesWithKeys = setAddresses.Where(s => s.ContactAddressKey != null);
            var contactAddressesToRemove = contact.Addresses.Where(a => !setAddressesWithKeys.Any(s => s.ContactAddressKey.Equals(a))).ToList();

            foreach(var address in contactAddressesToRemove)
            {
                contact.Addresses.Remove(address);
                CompanyUnitOfWork.ContactAddressRepository.Remove(address);
            }

            var lastAddressSequence = contact.Addresses.Any() ? contact.Addresses.Max(a => a.AddressId) : 0;
            foreach(var setAddress in setAddresses)
            {
                ContactAddress contactAddress;
                if(setAddress.ContactAddressKey != null)
                {
                    contactAddress = contact.Addresses.SingleOrDefault(a => setAddress.ContactAddressKey.Equals(a));
                    if(contactAddress == null)
                    {
                        return new InvalidResult(string.Format(UserMessages.ContactAddressNotFound, setAddress.ContactAddressKey.KeyValue));
                    }
                }
                else
                {
                    contactAddress = new ContactAddress
                        {
                            CompanyId = contact.CompanyId,
                            AddressId = ++lastAddressSequence
                        };
                    contact.Addresses.Add(contactAddress);
                    CompanyUnitOfWork.ContactAddressRepository.Add(contactAddress);
                }

                contactAddress.AddressDescription = setAddress.ContactAddress.AddressDescription ?? "";
                contactAddress.Address = setAddress.ContactAddress.Address ?? new Address();
            }

            return new SuccessResult();
        }
    }

}