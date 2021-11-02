using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Data.DataSeeders.Utilities.Interfaces
{
    public interface ILotAttributes
    {
        int Lot { get; }
        int? TesterID { get; set; }
        DateTime? TestDate { get; set; }
        DateTime? EntryDate { get; set; }
        decimal? AoverB { get; set; }
        decimal? AvgAsta { get; set; }
        decimal? Coli { get; set; }
        decimal? EColi { get; set; }
        decimal? Gran { get; set; }
        decimal? H2O { get; set; }
        decimal? InsPrts { get; set; }
        decimal? Lead { get; set; }
        decimal? Mold { get; set; }
        decimal? RodHrs { get; set; }
        decimal? Sal { get; set; }
        decimal? Scan { get; set; }
        decimal? AvgScov { get; set; }
        decimal? TPC { get; set; }
        decimal? Yeast { get; set; }
        decimal? Ash { get; set; }
        decimal? AIA { get; set; }
        decimal? Ethox { get; set; }
        decimal? AToxin { get; set; }
        decimal? BI { get; set; }
        decimal? Gluten { get; set; }
    }

    public static class ILotAttributesExtensions
    {
        public static IEnumerable<LotAttributeWrapper> GetAttributes(this ILotAttributes lot, ILotAttributes oldestUntestedHistory)
        {
            return StaticAttributeNames.AttributeNames.Select(attribute => new LotAttributeWrapper(lot, oldestUntestedHistory, attribute));
        }

        public static void GetAccessors(this ILotAttributes lotAttributes, IAttributeNameKey attribute, out Func<decimal?> get, out Func<decimal?, decimal?> set)
        {
            var attributeNameKey = new AttributeNameKey(attribute);
            get = () => AttributeGetters[attributeNameKey](lotAttributes);
            set = v => AttributeSetters[attributeNameKey](lotAttributes, v);
        }

        public static Func<decimal?> AttributeGet(this ILotAttributes lotAttributes, IAttributeNameKey attributeNameKey)
        {
            Func<ILotAttributes, decimal?> getter;
            AttributeGetters.TryGetValue(new AttributeNameKey(attributeNameKey), out getter);
            if(getter == null)
            {
                return null;
            }
            return () => getter(lotAttributes);
        }

        public static Func<decimal?, decimal?> AttributeSet(this ILotAttributes lotAttributes, IAttributeNameKey attributeNameKey, bool rounded = false)
        {
            Func<ILotAttributes, decimal?, decimal?> setter;
            AttributeSetters.TryGetValue(new AttributeNameKey(attributeNameKey), out setter);
            if(setter == null)
            {
                return null;
            }

            if(rounded)
            {
                return v => setter(lotAttributes, Rounded(v, GetRoundDecimalPlaces(attributeNameKey)));
            }

            return v => setter(lotAttributes, v);
        }

        public class LotAttributeWrapper
        {
            public bool ActualValueRequired { get; private set; }
            public AttributeNameKey AttributeNameKey { get; private set; }
            public decimal? Value { get { return _get(); } set { _set(value); } }
            public bool Computed { get; private set; }

            private readonly Func<decimal?> _get;
            private readonly Func<decimal?, decimal?> _set;

            public LotAttributeWrapper(ILotAttributes lot, ILotAttributes untestedHistory, AttributeName attributeName)
            {
                ActualValueRequired = attributeName.ActualValueRequired;
                AttributeNameKey = new AttributeNameKey(attributeName);
                lot.GetAccessors(attributeName, out _get, out _set);

                Computed = lot.TestDate == null;
                if(untestedHistory != null)
                {
                    Computed = Value == untestedHistory.AttributeGet(attributeName)();
                }
            }
        }

        private static readonly Dictionary<AttributeNameKey, Func<ILotAttributes, decimal?>> AttributeGetters = new Dictionary<AttributeNameKey, Func<ILotAttributes, decimal?>>
            {
                { new AttributeNameKey(StaticAttributeNames.Asta), l => l.AvgAsta },
                { new AttributeNameKey(StaticAttributeNames.Scoville), l => l.AvgScov },
                { new AttributeNameKey(StaticAttributeNames.Scan), l => l.Scan },
                { new AttributeNameKey(StaticAttributeNames.H2O), l => l.H2O },
                { new AttributeNameKey(StaticAttributeNames.AB), l => l.AoverB },
                { new AttributeNameKey(StaticAttributeNames.Yeast), l => l.Yeast },
                { new AttributeNameKey(StaticAttributeNames.InsectParts), l => l.InsPrts },
                { new AttributeNameKey(StaticAttributeNames.ColiForms), l => l.Coli },
                { new AttributeNameKey(StaticAttributeNames.TPC), l => l.TPC },
                { new AttributeNameKey(StaticAttributeNames.Lead), l => l.Lead },
                { new AttributeNameKey(StaticAttributeNames.EColi), l => l.EColi },
                { new AttributeNameKey(StaticAttributeNames.Salmonella), l => l.Sal },
                { new AttributeNameKey(StaticAttributeNames.RodentHairs), l => l.RodHrs },
                { new AttributeNameKey(StaticAttributeNames.Granularity), l => l.Gran },
                { new AttributeNameKey(StaticAttributeNames.Mold), l => l.Mold },
                { new AttributeNameKey(StaticAttributeNames.Ash), l => l.Ash },
                { new AttributeNameKey(StaticAttributeNames.AIA), l => l.AIA },
                { new AttributeNameKey(StaticAttributeNames.Ethox), l => l.Ethox },
                { new AttributeNameKey(StaticAttributeNames.BI), l => l.BI },
                { new AttributeNameKey(StaticAttributeNames.AToxin), l => l.AToxin },
                { new AttributeNameKey(StaticAttributeNames.Gluten), l => l.Gluten }
            };

        private static decimal? Rounded(decimal? value, int places)
        {
            return value.HasValue ? (decimal?)Math.Round(value.Value, places) : null;
        }

        private static int GetRoundDecimalPlaces(IAttributeNameKey attributeNameKey)
        {
            int decimals;
            if(attributeNameKey == null || !AttributeDecimalPlaces.TryGetValue(new AttributeNameKey(attributeNameKey), out decimals))
            {
                return 0;
            }

            return decimals;
        }

        private static readonly Dictionary<AttributeNameKey, Func<ILotAttributes, decimal?, decimal?>> AttributeSetters = new Dictionary<AttributeNameKey, Func<ILotAttributes, decimal?, decimal?>>
            {
                { new AttributeNameKey(StaticAttributeNames.Asta), (l, v) => l.AvgAsta = v },
                { new AttributeNameKey(StaticAttributeNames.Scoville), (l, v) => l.AvgScov = v },
                { new AttributeNameKey(StaticAttributeNames.Scan), (l, v) => l.Scan = v },
                { new AttributeNameKey(StaticAttributeNames.H2O), (l, v) => l.H2O = v },
                { new AttributeNameKey(StaticAttributeNames.AB), (l, v) => l.AoverB = v },
                { new AttributeNameKey(StaticAttributeNames.Yeast), (l, v) => l.Yeast = v },
                { new AttributeNameKey(StaticAttributeNames.InsectParts), (l, v) => l.InsPrts = v },
                { new AttributeNameKey(StaticAttributeNames.ColiForms), (l, v) => l.Coli = v },
                { new AttributeNameKey(StaticAttributeNames.TPC), (l, v) => l.TPC = v },
                { new AttributeNameKey(StaticAttributeNames.Lead), (l, v) => l.Lead = v },
                { new AttributeNameKey(StaticAttributeNames.EColi), (l, v) => l.EColi = v },
                { new AttributeNameKey(StaticAttributeNames.Salmonella), (l, v) => l.Sal = v },
                { new AttributeNameKey(StaticAttributeNames.RodentHairs), (l, v) => l.RodHrs = v },
                { new AttributeNameKey(StaticAttributeNames.Granularity), (l, v) => l.Gran = v },
                { new AttributeNameKey(StaticAttributeNames.Mold), (l, v) => l.Mold = v },
                { new AttributeNameKey(StaticAttributeNames.Ash), (l, v) => l.Ash = v },
                { new AttributeNameKey(StaticAttributeNames.AIA), (l, v) => l.AIA = v },
                { new AttributeNameKey(StaticAttributeNames.Ethox), (l, v) => l.Ethox = v },
                { new AttributeNameKey(StaticAttributeNames.BI), (l, v) => l.BI = v },
                { new AttributeNameKey(StaticAttributeNames.AToxin), (l, v) => l.AToxin = v },
                { new AttributeNameKey(StaticAttributeNames.Gluten), (l, v) => l.Gluten = v }
            };

        private static readonly Dictionary<AttributeNameKey, int> AttributeDecimalPlaces = new Dictionary<AttributeNameKey, int>
            {
                { new AttributeNameKey(StaticAttributeNames.AB), 2 },
                { new AttributeNameKey(StaticAttributeNames.Ash), 2 },
                { new AttributeNameKey(StaticAttributeNames.AIA), 2 },
                { new AttributeNameKey(StaticAttributeNames.Granularity), 2 },
                { new AttributeNameKey(StaticAttributeNames.H2O), 2 },
                { new AttributeNameKey(StaticAttributeNames.Ethox), 2 },
                { new AttributeNameKey(StaticAttributeNames.EColi), 2 },
                { new AttributeNameKey(StaticAttributeNames.Salmonella), 2 },

                //todo: Verify addition of these decimals - RI 20150513
                { new AttributeNameKey(StaticAttributeNames.InsectParts), 2 },
                { new AttributeNameKey(StaticAttributeNames.RodentHairs), 2 }
            };
    }
}