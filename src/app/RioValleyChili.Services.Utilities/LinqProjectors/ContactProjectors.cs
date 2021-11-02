// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ContactProjectors
    {
        internal static Expression<Func<Contact, ContactKeyReturn>> SelectKey()
        {
            return c => new ContactKeyReturn
                {
                    CompanyKey_Id = c.CompanyId,
                    ContactKey_Id = c.ContactId
                };
        }

        internal static Expression<Func<Contact, ContactSummaryReturn>> SelectSummary()
        {
            var key = SelectKey();
            var address = ContactAddressProjectors.Select();

            return c => new ContactSummaryReturn
                {
                    ContactKeyReturn = key.Invoke(c),

                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    EMailAddress = c.EMailAddress,
                    Addresses = c.Addresses.Select(a => address.Invoke(a))
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup