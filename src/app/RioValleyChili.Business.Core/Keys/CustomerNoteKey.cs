using System;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class CustomerNoteKey : EntityKey<ICustomerNoteKey>.With<int, int>, IKey<CustomerNote>, ICustomerNoteKey
    {
        public CustomerNoteKey() { }

        public CustomerNoteKey(ICustomerNoteKey key) : base(key) { }

        protected override ICustomerNoteKey ConstructKey(int field0, int field1)
        {
            return new CustomerNoteKey
                {
                    CustomerKey_Id = field0,
                    CustomerNoteKey_Id = field1
                };
        }

        protected override With<int, int> DeconstructKey(ICustomerNoteKey key)
        {
            return new CustomerNoteKey
                {
                    CustomerKey_Id = key.CustomerKey_Id,
                    CustomerNoteKey_Id = key.CustomerNoteKey_Id
                };
        }

        public Expression<Func<CustomerNote, bool>> FindByPredicate { get { return c => c.CustomerId == Field0 && c.NoteId == Field1; } }
        public int CustomerKey_Id { get { return Field0; } private set { Field0 = value; } }
        public int CustomerNoteKey_Id { get { return Field1; } private set { Field1 = value; } }
        public static readonly ICustomerNoteKey Null = new CustomerNoteKey();
    }

    public static class ICustomerNoteKeyExtensions
    {
        public static CustomerNoteKey ToCustomerNoteKey(this ICustomerNoteKey k)
        {
            return new CustomerNoteKey(k);
        }
    }
}