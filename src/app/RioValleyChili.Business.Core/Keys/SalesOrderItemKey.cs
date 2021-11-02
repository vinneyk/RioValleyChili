using System;
using System.Globalization;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class SalesOrderItemKey : EntityKey<ISalesOrderItemKey>.With<DateTime, int, int>, IKey<SalesOrderItem>, ISalesOrderItemKey, IInventoryPickOrderItemKey
    {
        public SalesOrderItemKey() { }

        public SalesOrderItemKey(ISalesOrderItemKey orderItemKey) : base(orderItemKey) { }

        public SalesOrderItemKey(IInventoryPickOrderItemKey orderItemKey) : base(orderItemKey.InventoryPickOrderKey_DateCreated, orderItemKey.InventoryPickOrderKey_Sequence, orderItemKey.InventoryPickOrderItemKey_Sequence) { }

        public override string GetParseFailMessage(string inputValue)
        {
            return string.Format(UserMessages.InvalidCustomerOrderItemKey, inputValue);
        }

        protected override string DateTimeToString(DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        protected override bool TryParseDateTime(string s, out object result)
        {
            DateTime dateTime;
            var tryParse = DateTime.TryParseExact(s, "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateTime);
            result = dateTime;
            return tryParse;
        }

        protected override ISalesOrderItemKey ConstructKey(DateTime field0, int field1, int field2)
        {
            return new SalesOrderItemKey { Field0 = field0, Field1 = field1, Field2 = field2 };
        }

        protected override With<DateTime, int, int> DeconstructKey(ISalesOrderItemKey key)
        {
            return new SalesOrderItemKey { Field0 = key.SalesOrderKey_DateCreated, Field1 = key.SalesOrderKey_Sequence, Field2 = key.SalesOrderItemKey_ItemSequence };
        }

        public Expression<Func<SalesOrderItem, bool>> FindByPredicate
        {
            get { return i => i.DateCreated == Field0 && i.Sequence == Field1 && i.ItemSequence == Field2; }
        }

        #region ISalesOrderItemKey

        public DateTime SalesOrderKey_DateCreated { get { return Field0; } }
        public int SalesOrderKey_Sequence { get { return Field1; } }
        public int SalesOrderItemKey_ItemSequence { get { return Field2; } }

        #endregion

        #region IInventoryPickOrderItemKey

        public DateTime InventoryPickOrderKey_DateCreated { get { return Field0; } }
        public int InventoryPickOrderKey_Sequence { get { return Field1; } }
        public int InventoryPickOrderItemKey_Sequence { get { return Field2; } }

        #endregion
    }

    public static class ISalesOrderItemKeyExtensions
    {
        public static SalesOrderItemKey ToSalesOrderItemKey(this ISalesOrderItemKey k)
        {
            return new SalesOrderItemKey(k);
        }
    }
}