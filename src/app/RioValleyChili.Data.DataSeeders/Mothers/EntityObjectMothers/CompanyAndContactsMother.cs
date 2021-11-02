using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class CompanyAndContactsMother : EntityMotherLogBase<CompanyAndContactsMother.CompanyResult, CompanyAndContactsMother.CallbackParameters>
    {
        private readonly RioValleyChiliDataContext _newContext;
        private readonly NewContextHelper _newContextHelper;
        private NotebookFactory _notebookFactory;

        public CompanyAndContactsMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback = null) : base(oldContext, loggingCallback)
        {
            if(newContext == null) {  throw new ArgumentNullException("newContext"); }
            _newContext = newContext;
            _newContextHelper = new NewContextHelper(_newContext);
        }

        private Dictionary<string, Models.Company> _brokersByName;
        private Dictionary<string, List<Customer>> _customersPendingBroker;

        private enum EntityType
        {
            Company,
            Contact,
            Customer,
            CustomerNote
        }

        private readonly MotherLoadCount<EntityType> _loadCount = new MotherLoadCount<EntityType>();

        protected override IEnumerable<CompanyResult> BirthRecords()
        {
            _loadCount.Reset();

            _brokersByName = new Dictionary<string, Models.Company>();
            _customersPendingBroker = new Dictionary<string, List<Customer>>();
            _notebookFactory = NotebookFactory.Create(_newContext);

            var companies = SelectCompaniesToLoad(OldContext);
            var nextCompanyId = 1;
            foreach(var company in companies)
            {
                _loadCount.AddRead(EntityType.Company);

                SerializableCompany serialized;
                var newCompany = CreateNewCompany(company, nextCompanyId, out serialized);
                if(newCompany == null)
                {
                    continue;
                }

                if(!newCompany.CompanyTypes.Any())
                {
                    Log(new CallbackParameters(CallbackReason.CompanyTypeUndetermined)
                        {
                            Company = company
                        });
                }

                Customer newCustomer = null;
                if(newCompany.CompanyTypes.Any(t => t.CompanyTypeEnum == CompanyType.Customer))
                {
                    var broker = serialized != null && serialized.Customer != null && !string.IsNullOrWhiteSpace(serialized.Customer.Broker) ? serialized.Customer.Broker : company.CustomerBroker;
                    if(string.IsNullOrWhiteSpace(broker))
                    {
                        Log(new CallbackParameters(CallbackReason.CustomerDefaultBrokerAssigned)
                            {
                                Company = company
                            });
                        broker = Models.StaticRecords.StaticCompanies.RVCBroker.Name;
                    }
                    
                    newCustomer = CreateCustomer(newCompany, broker, company);
                }
                newCompany.Contacts = GetContacts(company.Contacts, newCompany).ToList();

                if(newCompany.CompanyTypes.Any(t => t.CompanyTypeEnum == CompanyType.Broker))
                {
                    RegisterBroker(newCompany.Name, newCompany);
                }

                var companyResult = new CompanyResult(newCompany, newCustomer);
                CountResults(companyResult);
                nextCompanyId += 1;
                yield return companyResult;
            }
            
            _customersPendingBroker.ForEach(c => c.Value.ForEach(u => Log(new CallbackParameters(CallbackReason.CustomerUnresolvedBroker)
                {
                    Customer = u
                })));

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private void CountResults(CompanyResult result)
        {
            if(result != null)
            {
                if(result.Company != null)
                {
                    _loadCount.AddLoaded(EntityType.Company);
                    _loadCount.AddLoaded(EntityType.Contact, (uint) result.Company.Contacts.Count);
                }
                if(result.Customer != null)
                {
                    _loadCount.AddLoaded(EntityType.Customer);
                    _loadCount.AddLoaded(EntityType.CustomerNote, (uint) result.Customer.Notes.Count);
                }
            }
        }

        private void RegisterBroker(string brokerName, Models.Company brokerCompany)
        {
            _brokersByName.Add(brokerName, brokerCompany);
            List<Customer> customers;
            if(_customersPendingBroker.TryGetValue(brokerCompany.Name, out customers))
            {
                customers.ForEach(c => c.BrokerId = brokerCompany.Id);
                _customersPendingBroker.Remove(brokerCompany.Name);
            }
        }

        private Models.Company CreateNewCompany(CompanyDTO company, int nextId, out SerializableCompany serialized)
        {
            serialized = null;
            if(company.EntryDate == null)
            {
                Log(new CallbackParameters(CallbackReason.NullEntryDate)
                    {
                        Company = company
                    });
                return null;
            }
            var entryDate = company.EntryDate.Value;
            
            var companyTypes = company.CompanyTypes.Where(t => t != null).Select(t => t.Value).ToList();
            serialized = SerializableCompany.Deserialize(company.Serialized);
            if(serialized != null)
            {
                if(serialized.Types != null)
                {
                    companyTypes.AddRange(serialized.Types);
                }
                if(serialized.Customer != null)
                {
                    companyTypes.Add(CompanyType.Customer);
                }
            }
            
            if(company.Name.EndsWith("isomedix", true, CultureInfo.InvariantCulture))
            {
                companyTypes.Add(CompanyType.TreatmentFacility);
            }
            else if(company.Name.StartsWith("sterigenics", true, CultureInfo.InvariantCulture))
            {
                companyTypes.Add(CompanyType.TreatmentFacility);
            }
            
            var newCompany = new Models.Company
                {
                    EmployeeId = company.EmployeeId ?? _newContextHelper.DefaultEmployee.EmployeeId,
                    TimeStamp = entryDate.ConvertLocalToUTC(),

                    Id = nextId,

                    Name = company.Name,
                    Active = !company.InActive,
                    CompanyTypes = companyTypes.Distinct().Select(t => new CompanyTypeRecord
                        {
                            CompanyId = nextId,
                            CompanyType = (int) t
                        }).ToList()
                };

            return newCompany;
        }

        private Customer CreateCustomer(Models.Company company, string broker, CompanyDTO oldCompany)
        {
            _loadCount.AddRead(EntityType.Customer);

            Models.Company brokerCompany;
            _brokersByName.TryGetValue(broker, out brokerCompany);

            var customer = new Customer
                {
                    Id = company.Id,
                    Company = company,
                    BrokerId = brokerCompany != null ? brokerCompany.Id : -1,
                    Notes = CreateCustomerNotes(company, oldCompany).ToList()
                };

            if(customer.BrokerId == -1)
            {
                List<Customer> customers;
                if(!_customersPendingBroker.TryGetValue(broker, out customers))
                {
                    _customersPendingBroker.Add(broker, customers = new List<Customer>());
                }
                customers.Add(customer);
            }

            return customer;
        }

        private IEnumerable<CustomerNote> CreateCustomerNotes(Models.Company newCompany, CompanyDTO oldCompany)
        {
            var noteId = 1;
            foreach(var profile in oldCompany.Profiles)
            {
                _loadCount.AddRead(EntityType.CustomerNote);

                yield return new CustomerNote
                    {
                        CustomerId = newCompany.Id,
                        NoteId = noteId++,
                        EmployeeId = _newContextHelper.DefaultEmployee.EmployeeId,
                        TimeStamp = profile.EntryDate.ConvertLocalToUTC(),

                        Type = profile.ProfileType,
                        Text = profile.ProfileText,
                        Bold = profile.Boldit,
                        EntryDate = profile.EntryDate
                    };
            }
        }

        private IEnumerable<Models.Contact> GetContacts(IEnumerable<ContactDTO> contacts, ICompanyKey companyKey)
        {
            var nextContactId = 1;
            foreach(var contact in contacts)
            {
                _loadCount.AddRead(EntityType.Contact);

                var newContact = new Models.Contact
                    {
                        CompanyId = companyKey.CompanyKey_Id,
                        ContactId = nextContactId++,
                        Name = contact.Contact_IA,
                        PhoneNumber = contact.Phone_IA,
                        EMailAddress = contact.EMailAddress_IA,
                        OldContextID = contact.OldContextID
                    };
                newContact.Addresses = GetContactAddresses(contact.Addresses, newContact).ToList();
                yield return newContact;
            }
        }

        private static IEnumerable<ContactAddress> GetContactAddresses(IEnumerable<ContactAddressDTO> addresses, IContactKey contactKey)
        {
            var nextAddressId = 1;
            return addresses.Select(a => new ContactAddress
                {
                    OldContextID = a.OldContextID,
                    CompanyId = contactKey.CompanyKey_Id,
                    ContactId = contactKey.ContactKey_Id,
                    AddressId = nextAddressId++,
                    AddressDescription = a.AddrType,

                    Address = new Address
                        {
                            AddressLine1 = a.Address1_IA,
                            AddressLine2 = a.Address2_IA,
                            AddressLine3 = a.Address3_IA,
                            City = a.City_IA,
                            State = a.State_IA,
                            PostalCode = a.Zip_IA,
                            Country = a.Country_IA
                        }
                });
        }

        public static List<CompanyDTO> SelectCompaniesToLoad(ObjectContext oldContext)
        {
            var companies = oldContext.CreateObjectSet<Company>();
            var profiles = oldContext.CreateObjectSet<tblProfile>();

            var contacts = oldContext.CreateObjectSet<Contact>();
            var tblContracts = oldContext.CreateObjectSet<tblContract>();
            var tblOrders = oldContext.CreateObjectSet<tblOrder>();
            var tblSamples = oldContext.CreateObjectSet<tblSample>();

            return contacts.Where(c => c.Broker != null).Select(c => new { Employee = c.EmployeeID, Name = c.Broker, Type = "Broker", c.EntryDate, InActive = false })
                .Concat(tblContracts.Where(c => c.Broker != null).Select(c => new { Employee = c.EmployeeID, Name = c.Broker, Type = "Broker", c.EntryDate, InActive = false }))
                .Concat(tblOrders.Where(o => o.Broker != null).Select(o => new { Employee = o.EmployeeID, Name = o.Broker, Type = "Broker", o.EntryDate, InActive = false }))
                .Concat(tblSamples.Where(s => s.Broker != null).Select(s => new { Employee = s.EmployeeID, Name = s.Broker, Type = "Broker", s.EntryDate, InActive = false }))
                .Concat(companies.Where(c => c.Company_IA != null).Select(c => new { Employee = c.EmployeeID, Name = c.Company_IA, Type = c.CType, c.EntryDate, c.InActive }))
                .Distinct().Select(c => new
                    {
                        c.Employee,
                        c.EntryDate,
                        c.Name,
                        c.InActive,
                        CompanyType =
                            c.Type.Contains("broker") ? CompanyType.Broker :
                            c.Type.Contains("customer") ? CompanyType.Customer :
                            c.Type.Contains("ingredient") || c.Type.Contains("packaging") ? CompanyType.Supplier :
                            c.Type.Contains("grower") || c.Type.Contains("supplier") ? CompanyType.Dehydrator :
                            c.Type.Contains("freight") ? CompanyType.Freight :
                                (CompanyType?)null
                    })
                .GroupBy(c => c.Name)
                .Select(c => new CompanyDTO
                    {
                        EmployeeId = c.OrderBy(d => d.EntryDate).Select(m => m.Employee).FirstOrDefault(e => e!= null),
                        EntryDate = c.Select(m => m.EntryDate).Where(d => d != null).OrderBy(d => d).FirstOrDefault(),
                        Name = c.Key,
                        InActive = c.Any(m => m.InActive),
                        _Profiles = profiles.Where(p => p.Company_IA == c.Key).Select(p => new ProfileDTO
                            {
                                EntryDate = p.EntryDate,
                                Boldit = p.Boldit,
                                ProfileType = p.ProfileType,
                                ProfileText = p.ProfileText
                            }),
                        _CompanyTypes = c.Select(s => s.CompanyType),
                        _Contacts = contacts.Where(n => n.Company_IA == c.Key)
                            .GroupBy(n => new
                                {
                                    n.Contact_IA,
                                    n.Phone_IA,
                                    n.EmailAddress_IA,
                                    n.Broker
                                })
                            .Select(n => new ContactDTO
                                {
                                    Contact_IA = n.Key.Contact_IA,
                                    Phone_IA = n.Key.Phone_IA,
                                    EMailAddress_IA = n.Key.EmailAddress_IA,
                                    Broker = n.Key.Broker,
                                    OldContextID = n.FirstOrDefault().ID,

                                    _Addresses = n.Select(a => new ContactAddressDTO
                                        {
                                            OldContextID = a.ID,
                                            AddrType = a.AddrType,
                                            Address1_IA = a.Address1_IA,
                                            Address2_IA = a.Address2_IA,
                                            Address3_IA = a.Address3_IA,
                                            City_IA = a.City_IA,
                                            State_IA = a.State_IA,
                                            Zip_IA = a.Zip_IA,
                                            Country_IA = a.Country_IA,
                                        })
                                }),

                        Serialized = companies.Where(m => m.Company_IA == c.Key).Select(m => m.Serialized).FirstOrDefault(),
                        CustomerBroker = contacts.Select(n => new CustomerBrokerDTO
                            {
                                CompanyIA = n.Company_IA,
                                EntryDate = n.EntryDate,
                                Broker = n.Broker
                            })
                            .Concat(tblContracts.Select(n => new CustomerBrokerDTO
                                {
                                    CompanyIA = n.Company_IA,
                                    EntryDate = n.EntryDate,
                                    Broker = n.Broker
                                }))
                            .Concat(tblOrders.Select(n => new CustomerBrokerDTO
                                {
                                    CompanyIA = n.Company_IA,
                                    EntryDate = n.EntryDate,
                                    Broker = n.Broker
                                }))
                            .Concat(tblSamples.Select(n => new CustomerBrokerDTO
                                {
                                    CompanyIA = n.Company_IA,
                                    EntryDate = n.EntryDate,
                                    Broker = n.Broker
                                }))
                            .Where(b => b.CompanyIA == c.Key && b.EntryDate != null && b.Broker != null)
                            .OrderByDescending(b => b.EntryDate.Value)
                            .Select(b => b.Broker)
                            .FirstOrDefault()
                    }).ToList();
        }

        #region DTOs

        public class CompanyDTO
        {
            public int? EmployeeId { get; set; }
            public DateTime? EntryDate { get; set; }
            public string Name { get; set; }
            public bool InActive { get; set; }

            public string Serialized { get; set; }
            public string CustomerBroker { get; set; }

            public List<CompanyType?> CompanyTypes { get { return _companyTypes ?? (_companyTypes = _CompanyTypes.ToList()); } }
            public IEnumerable<CompanyType?> _CompanyTypes { get; set; }

            public List<ContactDTO> Contacts { get { return _contacts ?? (_contacts = _Contacts.ToList()); } }
            public IEnumerable<ContactDTO> _Contacts { get; set; }

            public List<ProfileDTO> Profiles { get { return _profiles ?? (_profiles = _Profiles.ToList()); } }
            public IEnumerable<ProfileDTO> _Profiles { get; set; }

            private List<CompanyType?> _companyTypes;
            private List<ContactDTO> _contacts;
            private List<ProfileDTO> _profiles;
        }

        public class ContactDTO
        {
            public string Contact_IA { get; set; }
            public string Phone_IA { get; set; }
            public string EMailAddress_IA { get; set; }
            public string Broker { get; set; }
            public int OldContextID { get; set; }

            public IEnumerable<ContactAddressDTO> _Addresses { get; set; }

            public List<ContactAddressDTO> Addresses { get { return _addresses ?? (_addresses = _Addresses.ToList()); } }


            private List<ContactAddressDTO> _addresses;
        }

        public class ContactAddressDTO
        {
            public int OldContextID { get; set; }
            public string AddrType { get; set; }
            public string Address1_IA { get; set; }
            public string Address2_IA { get; set; }
            public string Address3_IA { get; set; }
            public string City_IA { get; set; }
            public string State_IA { get; set; }
            public string Zip_IA { get; set; }
            public string Country_IA { get; set; }
        }

        public class ProfileDTO
        {
            public DateTime EntryDate { get; set; }
            public bool Boldit { get; set; }
            public string ProfileType { get; set; }
            public string ProfileText { get; set; }
        }

        public class CustomerBrokerDTO
        {
            public string CompanyIA { get; set; }
            public DateTime? EntryDate { get; set; }
            public string Broker { get; set; }
        }

        #endregion

        public class CompanyResult
        {
            public readonly Models.Company Company = null;
            public readonly Customer Customer = null;

            public CompanyResult(Models.Company company)
            {
                Company = company;
            }

            public CompanyResult(Models.Company company, Customer customer)
            {
                Company = company;
                Customer = customer;
            }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            CustomerDefaultBrokerAssigned,
            CustomerUnresolvedBroker,
            NullEntryDate,
            CompanyTypeUndetermined,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public CompanyDTO Company { get; set; }

            public Customer Customer { get; set; }

            protected override CallbackReason ExceptionReason { get { return CompanyAndContactsMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return CompanyAndContactsMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return CompanyAndContactsMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case CompanyAndContactsMother.CallbackReason.Exception: return ReasonCategory.Error;
                    case CompanyAndContactsMother.CallbackReason.NullEntryDate: return ReasonCategory.RecordSkipped;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}