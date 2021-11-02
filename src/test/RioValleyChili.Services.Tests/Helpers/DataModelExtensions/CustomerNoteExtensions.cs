using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class CustomerNoteExtensions
    {
        internal static void AssertEqual(this CustomerNote expected, ICustomerCompanyNoteReturn result)
        {
            Assert.AreEqual(expected.ToCustomerNoteKey().KeyValue, result.NoteKey);
            Assert.AreEqual(expected.Bold, result.DisplayBold);
            Assert.AreEqual(expected.Type, result.NoteType);
            Assert.AreEqual(expected.Text, result.Text);
            Assert.AreEqual(expected.TimeStamp, result.TimeStamp);
            expected.Employee.AssertEqual(result.CreatedByUser);
        }

        internal static void AssertEqual(this ICreateCustomerNoteParameters expected, CustomerNote result)
        {
            Assert.AreEqual(expected.CustomerKey, result.ToCustomerKey().KeyValue);
            Assert.AreEqual(expected.UserToken, result.Employee.UserName);
            Assert.AreEqual(expected.Text, result.Text);
            Assert.AreEqual(expected.Type, result.Type);
            Assert.AreEqual(expected.Bold, result.Bold);
        }

        internal static void AssertEqual(this IUpdateCustomerNoteParameters expected, CustomerNote result)
        {
            Assert.AreEqual(expected.CustomerNoteKey, result.ToCustomerNoteKey().KeyValue);
            Assert.AreEqual(expected.UserToken, result.Employee.UserName);
            Assert.AreEqual(expected.Text, result.Text);
            Assert.AreEqual(expected.Type, result.Type);
            Assert.AreEqual(expected.Bold, result.Bold);
        }
    }
}