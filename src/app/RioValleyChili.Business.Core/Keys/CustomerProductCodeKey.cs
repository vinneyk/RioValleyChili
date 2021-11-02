using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class CustomerProductCodeKey : EntityKey<ICustomerProductCodeKey>.With<int, int>, IKey<CustomerProductCode>, ICustomerProductCodeKey
    {
        public CustomerProductCodeKey() { }

        public CustomerProductCodeKey(ICustomerProductCodeKey key) : base(key) { }

        public CustomerProductCodeKey(ICustomerKey customerKey, IChileProductKey chileProductKey) : base(customerKey.CustomerKey_Id, chileProductKey.ChileProductKey_ProductId) { }

        protected override ICustomerProductCodeKey ConstructKey(int field0, int field1)
        {
            return new CustomerProductCodeKey { CustomerKey_Id = field0, ChileProductKey_ProductId = field1 };
        }

        protected override With<int, int> DeconstructKey(ICustomerProductCodeKey key)
        {
            return new CustomerProductCodeKey { CustomerKey_Id = key.CustomerKey_Id, ChileProductKey_ProductId = key.ChileProductKey_ProductId };
        }

        public Expression<Func<CustomerProductCode, bool>> FindByPredicate { get { return c => c.CustomerId == Field0 && c.ChileProductId == Field1; } }

        public int CustomerKey_Id { get { return Field0; } private set { Field0 = value; } }

        public int ChileProductKey_ProductId { get { return Field1; } private set { Field1 = value; } }

        public static ICustomerProductCodeKey Null = new CustomerProductCodeKey();
    }
}