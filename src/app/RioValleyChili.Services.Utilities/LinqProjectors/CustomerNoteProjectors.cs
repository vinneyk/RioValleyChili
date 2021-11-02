using System;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class CustomerNoteProjectors
    {
        internal static Expression<Func<CustomerNote, CustomerNoteKeyReturn>> SelectKey()
        {
            return n => new CustomerNoteKeyReturn
                {
                    CustomerKey_Id = n.CustomerId,
                    CustomerNoteKey_Id = n.NoteId
                };
        }

        internal static Expression<Func<CustomerNote, CustomerNoteReturn>> Select()
        {
            var key = SelectKey();
            return n => new CustomerNoteReturn
                {
                    CustomerNoteKeyReturn = key.Invoke(n),
                    Type = n.Type,
                    Text = n.Text,
                    Bold = n.Bold
                };
        }

        internal static Expression<Func<CustomerNote, CustomerCompanyNoteReturn>> SelectCustomerCompanyNote()
        {
            var key = SelectKey();
            var user = EmployeeProjectors.SelectSummary();

            return Projector<CustomerNote>.To(n => new CustomerCompanyNoteReturn
                {
                    NoteKeyReturn = key.Invoke(n),
                    DisplayBold = n.Bold,
                    NoteType = n.Type,
                    Text = n.Text,
                    TimeStamp = n.TimeStamp,
                    CreatedByUser = user.Invoke(n.Employee)
                });
        }
    }
}