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
    public class InventoryAdjustmentItemKey : EntityKey<IInventoryAdjustmentItemKey>.With<DateTime, int, int>, IKey<InventoryAdjustmentItem>, IInventoryAdjustmentItemKey
    {
        public InventoryAdjustmentItemKey() { }

        public InventoryAdjustmentItemKey(IInventoryAdjustmentItemKey inventoryAdjustmentItemKey) : base(inventoryAdjustmentItemKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidInventoryAdjustmentItemKey, inputValue);
        }

        protected override IInventoryAdjustmentItemKey ConstructKey(DateTime field0, int field1, int field2)
        {
            return new InventoryAdjustmentItemKey { InventoryAdjustmentKey_AdjustmentDate = field0, InventoryAdjustmentKey_Sequence = field1, InventoryAdjustmetItemKey_Sequence = field2 };
        }

        protected override With<DateTime, int, int> DeconstructKey(IInventoryAdjustmentItemKey key)
        {
            return new InventoryAdjustmentItemKey { InventoryAdjustmentKey_AdjustmentDate = key.InventoryAdjustmentKey_AdjustmentDate, InventoryAdjustmentKey_Sequence = key.InventoryAdjustmentKey_Sequence, InventoryAdjustmetItemKey_Sequence = key.InventoryAdjustmetItemKey_Sequence };
        }

        protected override bool TryParseDateTime(string s, out object result)
        {
            DateTime dateTimeResult;
            var @return = DateTime.TryParseExact(s, "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateTimeResult);
            result = dateTimeResult;
            return @return;
        }

        protected override string DateTimeToString(DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        public Expression<Func<InventoryAdjustmentItem, bool>> FindByPredicate { get { return i => i.AdjustmentDate == Field0 && i.Sequence == Field1 && i.ItemSequence == Field2; } }

        public DateTime InventoryAdjustmentKey_AdjustmentDate { get { return Field0; } private set { Field0 = value; } }

        public int InventoryAdjustmentKey_Sequence { get { return Field1; } private set { Field1 = value; } }

        public int InventoryAdjustmetItemKey_Sequence { get { return Field2; } private set { Field2 = value; } }

        public static readonly InventoryAdjustmentKey Null = new InventoryAdjustmentKey();
    }
}