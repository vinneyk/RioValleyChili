using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ContactAddressProjectors
    {
        internal static Expression<Func<ContactAddress, ContactAddressKeyReturn>> SelectKey()
        {
            return a => new ContactAddressKeyReturn
                {
                    CompanyKey_Id = a.CompanyId,
                    ContactKey_Id = a.ContactId,
                    ContactAddressKey_Id = a.AddressId
                };
        }

        internal static Expression<Func<ContactAddress, ContactAddressReturn>> Select()
        {
            var key = SelectKey();

            return a => new ContactAddressReturn
                {
                    ContactAddressKeyReturn = key.Invoke(a),
                    AddressDescription = a.AddressDescription,
                    Address = a.Address
                };
        }
    }
}