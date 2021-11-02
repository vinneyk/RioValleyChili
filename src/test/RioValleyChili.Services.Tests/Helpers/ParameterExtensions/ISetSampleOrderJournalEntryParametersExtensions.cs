using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISetSampleOrderJournalEntryParametersExtensions
    {
        internal static void AssertEqual(this ISetSampleOrderJournalEntryParameters expected, SampleOrderJournalEntry result)
        {
            if(!string.IsNullOrWhiteSpace(expected.JournalEntryKey))
            {
                Assert.AreEqual(expected.JournalEntryKey, result.ToSampleOrderJournalEntryKey().KeyValue);
            }

            Assert.AreEqual(expected.UserToken, result.Employee.UserName);
            Assert.AreEqual(expected.Date, result.Date);
            Assert.AreEqual(expected.Text, result.Text);
        }
    }
}