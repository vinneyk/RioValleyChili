using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;

namespace LotAttributeFix
{
    public class LotAttributesCopy : ILotAttributes
    {
        public decimal? this[IAttributeNameKey k]
        {
            get { return _wrappedAttributes[new AttributeNameKey(k)].Value; }
            set { _wrappedAttributes[new AttributeNameKey(k)].Value = value; }
        }

        public int Lot { get; private set; }
        public int? TesterID { get; set; }
        public DateTime? TestDate { get; set; }
        public DateTime? EntryDate { get; set; }
        public decimal? AoverB { get; set; }
        public decimal? AvgAsta { get; set; }
        public decimal? Coli { get; set; }
        public decimal? EColi { get; set; }
        public decimal? Gran { get; set; }
        public decimal? H2O { get; set; }
        public decimal? InsPrts { get; set; }
        public decimal? Lead { get; set; }
        public decimal? Mold { get; set; }
        public decimal? RodHrs { get; set; }
        public decimal? Sal { get; set; }
        public decimal? Scan { get; set; }
        public decimal? AvgScov { get; set; }
        public decimal? TPC { get; set; }
        public decimal? Yeast { get; set; }
        public decimal? Ash { get; set; }
        public decimal? AIA { get; set; }
        public decimal? Ethox { get; set; }
        public decimal? AToxin { get; set; }
        public decimal? BI { get; set; }
        public decimal? Gluten { get; set; }

        private readonly IDictionary<string, ILotAttributesExtensions.LotAttributeWrapper> _wrappedAttributes;

        public LotAttributesCopy(ILotAttributes source)
        {
            _wrappedAttributes = this.GetAttributes(null).ToDictionary(a => a.AttributeNameKey.ToString());
            foreach(var attribute in _wrappedAttributes)
            {
                attribute.Value.Value = source.AttributeGet(attribute.Value.AttributeNameKey)();
            }
        }
    }
}