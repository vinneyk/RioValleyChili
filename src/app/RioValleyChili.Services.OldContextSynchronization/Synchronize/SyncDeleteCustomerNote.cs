using System;
using System.Linq;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.DeleteCustomerNote)]
    public class SyncDeleteCustomerNote : SyncCommandBase<ICompanyUnitOfWork, DateTime?>
    {
        public SyncDeleteCustomerNote(ICompanyUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<DateTime?> getInput)
        {
            var tblProfileKey = getInput();
            if(tblProfileKey != null)
            {
                var tblProfile = OldContext.tblProfiles.FirstOrDefault(t => t.EntryDate == tblProfileKey);
                if(tblProfile != null)
                {
                    OldContext.tblProfiles.DeleteObject(tblProfile);
                }

                OldContext.SaveChanges();

                Console.WriteLine(ConsoleOutput.DeletedTblProfile, tblProfileKey);
            }
        }
    }
}