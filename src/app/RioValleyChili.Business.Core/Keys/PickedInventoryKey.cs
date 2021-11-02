using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class PickedInventoryKey : DateSequenceKeyBase<IPickedInventoryKey>, IKey<PickedInventory>, IPickedInventoryKey
    {
        public PickedInventoryKey() { }

        public PickedInventoryKey(IPickedInventoryKey pickedInventoryKey) : base(pickedInventoryKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidPickedInventoryKey, inputValue);
        }

        protected override IPickedInventoryKey ConstructKey(DateTime field0, int field1)
        {
            return new PickedInventoryKey
                {
                    PickedInventoryKey_DateCreated = field0,
                    PickedInventoryKey_Sequence = field1
                };
        }

        protected override With<DateTime, int> DeconstructKey(IPickedInventoryKey key)
        {
            return new PickedInventoryKey
                {
                    PickedInventoryKey_DateCreated = key.PickedInventoryKey_DateCreated,
                    PickedInventoryKey_Sequence = key.PickedInventoryKey_Sequence
                };
        }

        public Expression<Func<PickedInventory, bool>> FindByPredicate { get { return (i => i.DateCreated == Field0 && i.Sequence == Field1); } }
        public DateTime PickedInventoryKey_DateCreated { get { return Field0; } private set { Field0 = value; } }
        public int PickedInventoryKey_Sequence { get { return Field1; } private set { Field1 = value; } }
    }
}