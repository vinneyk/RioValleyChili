using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class SampleOrderJournalExtensions
    {
        internal static void AssertEqual(this SampleOrderJournalEntry expected, ISampleOrderJournalEntryReturn result)
        {
            Assert.AreEqual(expected.ToSampleOrderJournalEntryKey().KeyValue, result.JournalEntryKey);
            Assert.AreEqual(expected.Date, result.Date);
            Assert.AreEqual(expected.Text, result.Text);
            expected.Employee.AssertEqual(result.CreatedByUser);
        }
    }
}