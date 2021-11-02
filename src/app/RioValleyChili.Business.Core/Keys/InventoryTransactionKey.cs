using System;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class InventoryTransactionKey : DateSequenceKeyBase<IInventoryTransactionKey>, IInventoryTransactionKey, IKey<InventoryTransaction>
    {
        public InventoryTransactionKey() { }
        public InventoryTransactionKey(IInventoryTransactionKey key) : base(key) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidInventoryTransactionKey, inputValue);
        }

        protected override IInventoryTransactionKey ConstructKey(DateTime field0, int field1)
        {
            return new InventoryTransactionKey
                {
                    Field0 = field0,
                    Field1 = field1
                };
        }

        protected override With<DateTime, int> DeconstructKey(IInventoryTransactionKey key)
        {
            return new InventoryTransactionKey
                {
                    Field0 = key.InventoryTransactionKey_Date,
                    Field1 = key.InventoryTransactionKey_Sequence
                };
        }

        public Expression<Func<InventoryTransaction, bool>> FindByPredicate { get { return t => t.DateCreated == Field0 && t.Sequence == Field1; } }
        public DateTime InventoryTransactionKey_Date { get { return Field0; } }
        public int InventoryTransactionKey_Sequence { get { return Field1; } }
    }
}