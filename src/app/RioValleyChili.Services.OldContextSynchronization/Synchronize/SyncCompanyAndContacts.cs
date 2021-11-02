using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using Company = RioValleyChili.Data.DataSeeders.Company;
using Contact = RioValleyChili.Data.DataSeeders.Contact;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncCompany)]
    public class SyncCompanyAndContacts : SyncCommandBase<ICompanyUnitOfWork, CompanyKey>
    {
        public SyncCompanyAndContacts(ICompanyUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<CompanyKey> getInput)
        {
            var companyKey = getInput();

            lock(Lock)
            {
                var company = UnitOfWork.CompanyRepository.FindByKey(companyKey,
                    c => c.CompanyTypes,
                    c => c.Contacts.Select(n => n.Addresses),
                    c => c.Customer.Broker,
                    c => c.Customer.Notes);

                bool commitNewContext;
                var oldCompany = SyncCompany(company, out commitNewContext);
                OldContext.SaveChanges();

                if(commitNewContext)
                {
                    UnitOfWork.Commit();
                }

                Console.WriteLine(ConsoleOutput.SynchedCompany, oldCompany.Company_IA);
            }
        }

        private Company SyncCompany(Data.Models.Company company, out bool commitNewContext)
        {
            commitNewContext = false;

            var oldCompany = OldContext.Companies
                .Select(c => new
                    {
                        company = c,
                        contacts = c.Contacts,
                        profiles = c.tblProfiles
                    })
                .Where(c => c.company.Company_IA == company.Name)
                .ToList()
                .Select(c => c.company)
                .FirstOrDefault();
            if(oldCompany == null)
            {
                oldCompany = new Company
                    {
                        Company_IA = company.Name,
                        EntryDate = company.TimeStamp.ConvertUTCToLocal().RoundMillisecondsForSQL(),
                        s_GUID = Guid.NewGuid(),
                    };
                OldContext.Companies.AddObject(oldCompany);
            }

            oldCompany.CType = GetCType(company.CompanyTypes.Select(c => c.CompanyTypeEnum));
            oldCompany.InActive = !company.Active;
            oldCompany.EmployeeID = company.EmployeeId;
            oldCompany.Serialized = SerializableCompany.Serialize(company);

            var oldIds = new List<int?>();
            var contactsToRemove = oldCompany.Contacts == null ? new List<Contact>() : oldCompany.Contacts.ToList();
            foreach(var contact in company.Contacts)
            {
                foreach(var address in contact.Addresses.DefaultIfEmpty(new ContactAddress
                    {
                        Address = new Address(),
                        OldContextID = contact.OldContextID
                    }))
                {
                    var oldContact = contactsToRemove.FirstOrDefault(c => c.ID == address.OldContextID);
                    if(oldContact != null)
                    {
                        contactsToRemove.Remove(oldContact);
                    }
                    else
                    {
                        oldContact = new Contact
                            {
                                Company_IA = company.Name,
                                Broker = company.Customer == null ? null : company.Customer.Broker.Name,
                                EntryDate = DateTime.Now.RoundMillisecondsForSQL(),
                                s_GUID = Guid.NewGuid()
                            };
                        OldContext.Contacts.AddObject(oldContact);
                        OldContext.SaveChanges();

                        address.OldContextID = oldContact.ID;
                        commitNewContext = true;
                    }
                    oldIds.Add(oldContact.ID);

                    oldContact.Contact_IA = contact.Name;
                    oldContact.Phone_IA = contact.PhoneNumber;
                    oldContact.EmailAddress_IA = contact.EMailAddress;

                    oldContact.AddrType = address.AddressDescription;
                    oldContact.Address1_IA = address.Address.AddressLine1;
                    oldContact.Address2_IA = address.Address.AddressLine2;
                    oldContact.Address3_IA = address.Address.AddressLine3;
                    oldContact.City_IA = address.Address.City;
                    oldContact.State_IA = address.Address.State;
                    oldContact.Zip_IA = address.Address.PostalCode;
                    oldContact.Country_IA = address.Address.Country;
                }

                if(!oldIds.Contains(contact.OldContextID))
                {
                    contact.OldContextID = oldIds.FirstOrDefault();
                    commitNewContext = true;
                }
            }

            contactsToRemove.ForEach(c => OldContext.Contacts.DeleteObject(c));

            if(company.Customer != null)
            {
                var entryDate = OldContext.tblProfiles.Select(n => n.EntryDate).DefaultIfEmpty(DateTime.Now.RoundMillisecondsForSQL()).Max();
                var notesToRemove = (oldCompany.tblProfiles == null ? new List<tblProfile>() : oldCompany.tblProfiles.ToList()).ToDictionary(p => p.EntryDate);
                foreach(var note in company.Customer.Notes)
                {
                    tblProfile oldNote;
                    if(note.EntryDate != null && notesToRemove.TryGetValue(note.EntryDate.Value, out oldNote))
                    {
                        notesToRemove.Remove(oldNote.EntryDate);
                    }
                    else
                    {
                        entryDate = entryDate.AddSeconds(1);
                        oldNote = new tblProfile
                            {
                                EntryDate = entryDate,
                                Company_IA = oldCompany.Company_IA,
                                s_GUID = Guid.NewGuid()
                            };
                        OldContext.tblProfiles.AddObject(oldNote);
                        note.EntryDate = oldNote.EntryDate;
                        commitNewContext = true;
                    }

                    oldNote.Boldit = note.Bold;
                    oldNote.ProfileType = note.Type;
                    oldNote.ProfileText = note.Text;
                }

                notesToRemove.Values.ForEach((n, i) => OldContext.tblProfiles.DeleteObject(n));
            }

            return oldCompany;
        }

        private static readonly object Lock = new object();

        private static string GetCType(IEnumerable<CompanyType> companyTypes)
        {
            return companyTypes.Select(c =>
                {
                    switch(c)
                    {
                        case CompanyType.Customer: return "Customer";
                        case CompanyType.Supplier: return "Ingredient";
                        case CompanyType.Dehydrator: return "Grower/Supplier";
                        case CompanyType.Broker: return "Broker";
                        case CompanyType.Freight: return "Freight";
                    }
                    return null;
                })
                .FirstOrDefault(c => c != null);
        }
    }
}