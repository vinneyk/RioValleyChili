using System;
using System.Globalization;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class InventoryTreatmentKey : EntityKeyBase.Of<IInventoryTreatmentKey>, IKey<InventoryTreatment>, IInventoryTreatmentKey
    {
        #region Fields and Constructors

        private readonly int _inventoryTreatmentId;

        public InventoryTreatmentKey() {}

        public InventoryTreatmentKey(IInventoryTreatmentKey inventoryTreatmentKey)
            : this(inventoryTreatmentKey.InventoryTreatmentKey_Id) {}

        private InventoryTreatmentKey(int inventoryTreatmentId)
        {
            _inventoryTreatmentId = inventoryTreatmentId;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IInventoryTreatmentKey>

        protected override IInventoryTreatmentKey ParseImplementation(string keyValue)
        {
            return new InventoryTreatmentKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format("The value could not be parsed into an InventoryTreatmentKey.");
        }

        public override IInventoryTreatmentKey Default
        {
            get { return Null; }
        }

        protected override IInventoryTreatmentKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IInventoryTreatmentKey key)
        {
            return key.InventoryTreatmentKey_Id.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<InventoryTreatment>

        public Expression<Func<InventoryTreatment, bool>> FindByPredicate
        {
            get { return i => i.Id == _inventoryTreatmentId; }
        }

        #endregion

        #region Implementation of IInventoryTreatmentKey

        public int InventoryTreatmentKey_Id
        {
            get { return _inventoryTreatmentId; }
        }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as IInventoryTreatmentKey != null && Equals(obj as IInventoryTreatmentKey);
        }

        protected bool Equals(IInventoryTreatmentKey other)
        {
            return _inventoryTreatmentId == other.InventoryTreatmentKey_Id;
        }

        public override int GetHashCode()
        {
            return _inventoryTreatmentId;
        }

        #endregion

        public static IInventoryTreatmentKey Null = new NullInventoryTreatmentKey();

        private class NullInventoryTreatmentKey : IInventoryTreatmentKey
        {
            #region Implementation of IInventoryTreatmentKey

            public int InventoryTreatmentKey_Id { get { return 0; } }

            #endregion
        }
    }

    public static class IInventoryTreatmentKeyExtensions
    {
        public static InventoryTreatmentKey ToInventoryTreatmentKey(this IInventoryTreatmentKey k)
        {
            return new InventoryTreatmentKey(k);
        }
    }
}