using System;
using System.Globalization;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Helpers;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class SalesOrderKey : EntityKey<ISalesOrderKey>.With<DateTime, int>, IKey<SalesOrder>, ISalesOrderKey, IPickedInventoryKey, IInventoryPickOrderKey
    {
        public SalesOrderKey() : base(DataConstants.SqlMinDate, 0) { }

        public SalesOrderKey(ISalesOrderKey salesOrderKey) : base(salesOrderKey) { }

        public SalesOrderKey(IInventoryShipmentOrderKey inventoryShipmentOrderKey) : base(inventoryShipmentOrderKey.InventoryShipmentOrderKey_DateCreated, inventoryShipmentOrderKey.InventoryShipmentOrderKey_Sequence) { }

        public SalesOrderKey(IPickedInventoryKey pickedInventoryKey) : base(pickedInventoryKey.PickedInventoryKey_DateCreated, pickedInventoryKey.PickedInventoryKey_Sequence) { }

        public SalesOrderKey(IInventoryPickOrderKey inventoryPickOrderKey) : base(inventoryPickOrderKey.InventoryPickOrderKey_DateCreated, inventoryPickOrderKey.InventoryPickOrderKey_Sequence) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidCustomerOrderKey, inputValue);
        }

        protected override ISalesOrderKey ConstructKey(DateTime field0, int field1)
        {
            return new SalesOrderKey
                {
                    Field0 = field0,
                    Field1 = field1
                };
        }

        protected override With<DateTime, int> DeconstructKey(ISalesOrderKey key)
        {
            return new SalesOrderKey
                {
                    Field0 = key.SalesOrderKey_DateCreated,
                    Field1 = key.SalesOrderKey_Sequence
                };
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

        public Expression<Func<SalesOrder, bool>> FindByPredicate
        {
            get { return o => o.DateCreated == Field0 && o.Sequence == Field1; }
        }

        #region ISalesOrderKey

        public DateTime SalesOrderKey_DateCreated { get { return Field0; } }
        public int SalesOrderKey_Sequence { get { return Field1; } }

        #endregion

        #region IPickedInventoryKey

        public DateTime PickedInventoryKey_DateCreated { get { return Field0; } }
        public int PickedInventoryKey_Sequence { get { return Field1; } }

        #endregion

        #region IInventoryPickOrderKey

        public DateTime InventoryPickOrderKey_DateCreated { get { return Field0; } }
        public int InventoryPickOrderKey_Sequence { get { return Field1; } }

        #endregion
    }

    public static class ISalesOrderKeyExtensions
    {
        public static SalesOrderKey ToSalesOrderKey(this ISalesOrderKey k)
        {
            return new SalesOrderKey(k);
        }
    }
}