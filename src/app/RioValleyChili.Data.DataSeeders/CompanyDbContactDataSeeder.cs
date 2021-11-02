using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Logging;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Initialization;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders
{
    public class CompanyDbContactDataSeeder : DbContextDataSeederBase<RioValleyChiliDataContext>
    {
        public override void SeedContext(RioValleyChiliDataContext newContext)
        {
            new CoreDbContextDataSeeder().SeedContext(newContext);

            var consoleTicker = new ConsoleTicker();

            using(var oldContext = ContextsHelper.GetOldContext())
            {
                var companyResults = new CompanyAndContactsMother(oldContext, newContext, RVCDataLoadLoggerGate.CompanyAndContactsLoadLoggerCallback).BirthAll(() => consoleTicker.TickConsole("Loading Companies...")).ToList();
                consoleTicker.ReplaceCurrentLine("Loading Companies");
                var companies = new List<Models.Company>();
                var contacts = new List<Models.Contact>();
                var contactAddresses = new List<ContactAddress>();
                var customers = new List<Customer>();
                var customerNotes = new List<CustomerNote>();
                foreach(var result in companyResults)
                {
                    companies.Add(result.Company);
                    contacts.AddRange(result.Company.Contacts);
                    contactAddresses.AddRange(result.Company.Contacts.SelectMany(c => c.Addresses));

                    if(result.Customer != null)
                    {
                        customers.Add(result.Customer);
                        customerNotes.AddRange(result.Customer.Notes);
                    }
                }

                LoadRecords(newContext, companies, "\tcompanies", consoleTicker);
                LoadRecords(newContext, companies.SelectMany(c => c.CompanyTypes), "\tcompany types", consoleTicker);
                LoadRecords(newContext, contacts, "\tcontacts", consoleTicker);
                LoadRecords(newContext, contactAddresses, "\tcontact addresses", consoleTicker);
                LoadRecords(newContext, customers, "\tcustomers", consoleTicker);
                LoadRecords(newContext, customerNotes, "\tcustomer notes", consoleTicker);
            }
        }
    }
}