using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.DeleteCompanyContact)]
    public class SyncDeleteCompanyContact : SyncCommandBase<ICompanyUnitOfWork, List<int?>>
    {
        public SyncDeleteCompanyContact(ICompanyUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<List<int?>> getInput)
        {
            var contactIds = getInput().Where(c => c != null).Distinct().ToList();
            foreach(var contact in contactIds)
            {
                var oldContact = OldContext.Contacts.FirstOrDefault(c => c.ID == contact.Value);
                if(oldContact != null)
                {
                    OldContext.Contacts.DeleteObject(oldContact);
                }
            }

            OldContext.SaveChanges();

            contactIds.ForEach(i => Console.WriteLine(ConsoleOutput.DeletedContact, i));
        }
    }
}