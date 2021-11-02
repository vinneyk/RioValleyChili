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
    public class InventoryShipmentOrderKey : EntityKey<IInventoryShipmentOrderKey>.With<DateTime, int>, IKey<InventoryShipmentOrder>, IInventoryShipmentOrderKey
    {
        public InventoryShipmentOrderKey()
        {
            InventoryShipmentOrderKey_DateCreated = DataConstants.SqlMinDate;
        }

        public InventoryShipmentOrderKey(IInventoryShipmentOrderKey inventoryShipmentOrderKey) : base(inventoryShipmentOrderKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidInventoryShipmentOrderKey, inputValue);
        }

        protected override IInventoryShipmentOrderKey ConstructKey(DateTime field0, int field1)
        {
            return new InventoryShipmentOrderKey
                {
                    InventoryShipmentOrderKey_DateCreated = field0,
                    InventoryShipmentOrderKey_Sequence = field1
                };
        }

        protected override With<DateTime, int> DeconstructKey(IInventoryShipmentOrderKey key)
        {
            return new InventoryShipmentOrderKey
                {
                    InventoryShipmentOrderKey_DateCreated = key.InventoryShipmentOrderKey_DateCreated,
                    InventoryShipmentOrderKey_Sequence = key.InventoryShipmentOrderKey_Sequence
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

        public Expression<Func<InventoryShipmentOrder, bool>> FindByPredicate
        {
            get { return (i => i.DateCreated == Field0 && i.Sequence == Field1); }
        }

        public DateTime InventoryShipmentOrderKey_DateCreated { get { return Field0; } private set { Field0 = value; } }
        public int InventoryShipmentOrderKey_Sequence { get { return Field1; } private set { Field1 = value; } }
    }

    public static class IInventoryShipmentOrderKeyExtensions
    {
        public static InventoryShipmentOrderKey ToInventoryShipmentOrderKey(this IInventoryShipmentOrderKey k)
        {
            return new InventoryShipmentOrderKey(k);
        }
    }
}