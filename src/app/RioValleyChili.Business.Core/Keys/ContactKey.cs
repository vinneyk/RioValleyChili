using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class ContactKey : EntityKey<IContactKey>.With<int, int>, IKey<Contact>, IContactKey
    {
        public ContactKey() { }

        public ContactKey(IContactKey ContactKey) : base(ContactKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidContactKey, inputValue);
        }

        protected override IContactKey ConstructKey(int field0, int field1)
        {
            return new ContactKey { CompanyKey_Id = field0, ContactKey_Id = field1 };
        }

        protected override With<int, int> DeconstructKey(IContactKey key)
        {
            return new ContactKey { CompanyKey_Id = key.CompanyKey_Id, ContactKey_Id = key.ContactKey_Id };
        }

        public Expression<Func<Contact, bool>> FindByPredicate { get { return c => c.CompanyId == Field0 && c.ContactId == Field1; } }

        public int CompanyKey_Id { get { return Field0; } private set { Field0 = value; } }

        public int ContactKey_Id { get { return Field1; } private set { Field1 = value; } }

        public static IContactKey Null = new ContactKey();
    }

    public static class IContactKeyExtensions
    {
        public static ContactKey ToContactKey(this IContactKey k)
        {
            return new ContactKey(k);
        }
    }
}