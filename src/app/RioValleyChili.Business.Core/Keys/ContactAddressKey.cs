using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class ContactAddressKey : EntityKey<IContactAddressKey>.With<int, int, int>, IKey<ContactAddress>, IContactAddressKey
    {
        public ContactAddressKey() { }

        public ContactAddressKey(IContactAddressKey contactAddressKey) : base(contactAddressKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidContactAddressKey, inputValue);
        }

        protected override IContactAddressKey ConstructKey(int field0, int field1, int field2)
        {
            return new ContactAddressKey { CompanyKey_Id = field0, ContactKey_Id = field1, ContactAddressKey_Id = field2 };
        }

        protected override With<int, int, int> DeconstructKey(IContactAddressKey key)
        {
            return new ContactAddressKey { CompanyKey_Id = key.CompanyKey_Id, ContactKey_Id = key.ContactKey_Id, ContactAddressKey_Id = key.ContactAddressKey_Id };
        }

        public Expression<Func<ContactAddress, bool>> FindByPredicate { get { return a => a.CompanyId == Field0 && a.ContactId == Field1 && a.AddressId == Field2; } }

        public int CompanyKey_Id { get { return Field0; } private set { Field0 = value; } }

        public int ContactKey_Id { get { return Field1; } private set { Field1 = value; } }

        public int ContactAddressKey_Id { get { return Field2; } private set { Field2 = value; } }

        public static IContactAddressKey Null = new ContactAddressKey { CompanyKey_Id = -1, ContactKey_Id = -1, ContactAddressKey_Id = -1 };
    }

    public static class IContactAddressKeyExtensions
    {
        public static ContactAddressKey ToContactAddressKey(this IContactAddressKey k)
        {
            return new ContactAddressKey(k);
        }
    }
}