using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class CustomerProductAttributeRangeKey : EntityKey<ICustomerProductAttributeRangeKey>.With<int, int, string>, IKey<CustomerProductAttributeRange>, ICustomerProductAttributeRangeKey
    {
        public CustomerProductAttributeRangeKey() { }

        public CustomerProductAttributeRangeKey(ICustomerProductAttributeRangeKey key) : base(key) { }

        public CustomerProductAttributeRangeKey(ICustomerKey customerKey, IChileProductKey chileProductKey, IAttributeNameKey attributeNameKey) : base(customerKey.CustomerKey_Id, chileProductKey.ChileProductKey_ProductId, attributeNameKey.AttributeNameKey_ShortName) { }

        protected override ICustomerProductAttributeRangeKey ConstructKey(int field0, int field1, string field2)
        {
            return new CustomerProductAttributeRangeKey { CustomerKey_Id = field0, ChileProductKey_ProductId = field1, AttributeNameKey_ShortName = field2 };
        }

        protected override With<int, int, string> DeconstructKey(ICustomerProductAttributeRangeKey key)
        {
            return new CustomerProductAttributeRangeKey { CustomerKey_Id = key.CustomerKey_Id, ChileProductKey_ProductId = key.ChileProductKey_ProductId, AttributeNameKey_ShortName = key.AttributeNameKey_ShortName };
        }

        public Expression<Func<CustomerProductAttributeRange, bool>> FindByPredicate { get { return s => s.CustomerId == Field0 && s.ChileProductId == Field1 && s.AttributeShortName == Field2; } }

        public int CustomerKey_Id { get { return Field0; } private set { Field0 = value; } }

        public int ChileProductKey_ProductId { get { return Field1; } private set { Field1 = value; } }

        public string AttributeNameKey_ShortName { get { return Field2; } private set { Field2 = value; } }

        public static ICustomerProductAttributeRangeKey Null = new CustomerProductAttributeRangeKey();
    }

    public static class ICustomerProductAttributeRangeKeyExtensions
    {
        public static CustomerProductAttributeRangeKey ToCustomerProductAttributeRangeKey(this ICustomerProductAttributeRangeKey rangeKey)
        {
            return new CustomerProductAttributeRangeKey(rangeKey);
        }
    }
}