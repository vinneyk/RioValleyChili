using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SampleOrderJournalEntryProjectors
    {
        internal static Expression<Func<SampleOrderJournalEntry, SampleOrderJournalEntryKeyReturn>> SelectKey()
        {
            return j => new SampleOrderJournalEntryKeyReturn
                {
                    SampleOrderKey_Year = j.SampleOrderYear,
                    SampleOrderKey_Sequence = j.SampleOrderSequence,
                    SampleOrderJournalEntryKey_Sequence = j.EntrySequence
                };
        }

        internal static Expression<Func<SampleOrderJournalEntry, SampleOrderJournalEntryReturn>> Select()
        {
            var key = SelectKey();
            var user = EmployeeProjectors.SelectSummary();

            return j => new SampleOrderJournalEntryReturn
                {
                    Date = j.Date,
                    Text = j.Text,
                    SampleOrderJournalEntryKeyReturn = key.Invoke(j),
                    CreatedByUser = user.Invoke(j.Employee)
                };
        }
    }
}