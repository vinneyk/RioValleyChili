using System;
using System.Globalization;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class SalesOrderPickedItemKey : EntityKey<ISalesOrderPickedItemKey>.With<DateTime, int, int>, IKey<SalesOrderPickedItem>, ISalesOrderPickedItemKey, IPickedInventoryItemKey
    {
        public SalesOrderPickedItemKey() { }

        public SalesOrderPickedItemKey(ISalesOrderPickedItemKey orderItemKey) : base(orderItemKey) { }

        public SalesOrderPickedItemKey(IPickedInventoryItemKey pickedInventoryItemKey) : base(pickedInventoryItemKey.PickedInventoryKey_DateCreated, pickedInventoryItemKey.PickedInventoryKey_Sequence, pickedInventoryItemKey.PickedInventoryItemKey_Sequence) { }

        public override string GetParseFailMessage(string inputValue)
        {
            return string.Format(UserMessages.InvalidCustomerOrderPickedItemKey, inputValue);
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

        protected override ISalesOrderPickedItemKey ConstructKey(DateTime field0, int field1, int field2)
        {
            return new SalesOrderPickedItemKey { Field0 = field0, Field1 = field1, Field2 = field2 };
        }

        protected override With<DateTime, int, int> DeconstructKey(ISalesOrderPickedItemKey key)
        {
            return new SalesOrderPickedItemKey { Field0 = key.SalesOrderKey_DateCreated, Field1 = key.SalesOrderKey_Sequence, Field2 = key.SalesOrderPickedItemKey_ItemSequence };
        }

        public Expression<Func<SalesOrderPickedItem, bool>> FindByPredicate
        {
            get { return i => i.DateCreated == Field0 && i.Sequence == Field1 && i.ItemSequence == Field2; }
        }

        #region ICustomerOrderPickedItemKey

        public DateTime SalesOrderKey_DateCreated { get { return Field0; } }

        public int SalesOrderKey_Sequence { get { return Field1; } }

        public int SalesOrderPickedItemKey_ItemSequence { get { return Field2; } }

        #endregion

        #region IPickedInventoryItemKey

        public DateTime PickedInventoryKey_DateCreated { get { return Field0; } }

        public int PickedInventoryKey_Sequence { get { return Field1; } }

        public int PickedInventoryItemKey_Sequence { get { return Field2; } }

        #endregion
    }

    public static class ICustomerOrderPickedItemKeyExtensions
    {
        public static SalesOrderPickedItemKey ToCustomerOrderPickedItemKey(this ISalesOrderPickedItemKey k)
        {
            return new SalesOrderPickedItemKey(k);
        }
    }
}