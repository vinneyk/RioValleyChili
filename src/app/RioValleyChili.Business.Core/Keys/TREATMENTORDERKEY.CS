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
    public class TreatmentOrderKey : EntityKey<ITreatmentOrderKey>.With<DateTime, int>, IKey<TreatmentOrder>, ITreatmentOrderKey
    {
        #region Fields and Constructors.

        public TreatmentOrderKey()
        {
            InventoryShipmentOrderKey_DateCreated = DataConstants.SqlMinDate;
        }

        public TreatmentOrderKey(ITreatmentOrderKey treatmentOrderKey) : base(treatmentOrderKey) { }

        public TreatmentOrderKey(IInventoryShipmentOrderKey orderKey)
        {
            InventoryShipmentOrderKey_DateCreated = orderKey.InventoryShipmentOrderKey_DateCreated;
            InventoryShipmentOrderKey_Sequence = orderKey.InventoryShipmentOrderKey_Sequence;
        }

        #endregion

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidTreatmentOrderKey, inputValue);
        }

        protected override ITreatmentOrderKey ConstructKey(DateTime field0, int field1)
        {
            return new TreatmentOrderKey
            {
                InventoryShipmentOrderKey_DateCreated = field0,
                InventoryShipmentOrderKey_Sequence = field1
            };
        }

        protected override With<DateTime, int> DeconstructKey(ITreatmentOrderKey key)
        {
            return new TreatmentOrderKey
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

        #region Implementation of IKey<TreatmentOrder>.

        public Expression<Func<TreatmentOrder, bool>> FindByPredicate
        {
            get { return (t => t.DateCreated == Field0 && t.Sequence == Field1); }
        }

        #endregion

        #region Implementation of ITreatmentOrderKey.

        public DateTime InventoryShipmentOrderKey_DateCreated { get { return Field0; } set { Field0 = value; } }
        public int InventoryShipmentOrderKey_Sequence { get { return Field1; } set { Field1 = value; } }

        #endregion
    }

    public static class ITreatmentOrderKeyExtensions
    {
        public static TreatmentOrderKey ToTreatmentOrderKey(this ITreatmentOrderKey k)
        {
            return new TreatmentOrderKey(k);
        }
    }
}