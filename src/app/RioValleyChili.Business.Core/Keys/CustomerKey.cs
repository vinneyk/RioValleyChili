using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class CustomerKey : EntityKey<ICustomerKey>.With<int>, ICustomerKey, ICompanyKey, IKey<Customer>
    {
        public CustomerKey() { }

        public CustomerKey(ICustomerKey customerKey) : base(customerKey) { }

        public CustomerKey(ICompanyKey companyKey) : base(companyKey.CompanyKey_Id) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidCustomerKey, inputValue);
        }

        protected override ICustomerKey ConstructKey(int field0)
        {
            return new CustomerKey { CustomerKey_Id = field0 };
        }

        protected override With<int> DeconstructKey(ICustomerKey key)
        {
            return new CustomerKey { CustomerKey_Id = key.CustomerKey_Id };
        }

        public Expression<Func<Customer, bool>> FindByPredicate { get { return c => c.Id == Field0; } }
        public int CustomerKey_Id { get { return Field0; } private set { Field0 = value; } }
        public int CompanyKey_Id { get { return Field0; } }

        public static readonly ICustomerKey Null = new CustomerKey();
    }

    public static class ICustomerKeyExtensions
    {
        public static CustomerKey ToCustomerKey(this ICustomerKey k)
        {
            return new CustomerKey(k);
        }
    }
}