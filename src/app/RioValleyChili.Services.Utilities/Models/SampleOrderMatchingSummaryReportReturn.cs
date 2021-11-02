using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderMatchingSummaryReportReturn : ISampleOrderMatchingSummaryReportReturn
    {
        public string SampleOrderKey { get { return SampleOrderKeyReturn.SampleOrderKey; } }
        public string Company { get; internal set; }
        public string Contact { get; internal set; }

        public IEnumerable<ISampleOrderMatchingItemReturn> Items { get; internal set; }

        internal SampleOrderKeyReturn SampleOrderKeyReturn { get; set; }
    }

    internal class SampleOrderMatchingItemReturn : ISampleOrderMatchingItemReturn
    {
        public string SampleMatch { get; internal set; }

        public string AstaColorSpec { get { return GetSpecRange(s => s.AstaMin, s => s.AstaMax); } }
        public string MoistureSpec
        {
            get
            {
                var water = GetSpecValue(s => s.WaterActivityMax);
                return water == null ? "n/a" : string.Format("{0:0.00}%", water.Value * 100);
            }
        }
        public string ParticleSpec { get { return ""; } }
        public string SurfaceColorSpec { get { return GetSpecRange(s => s.ScanMin, s => s.ScanMax); } }
        public string ABSpec
        {
            get
            {
                var ab = GetSpecValue(s => s.AoverB);
                return ab == null ? "n/a" : string.Format("{0:0.00}", ab.Value);
            }
        }
        public string SHUSpec { get { return GetSpecRange(s => s.ScovMin, s => s.ScovMax); } }
        public string TPCSpec { get { return GetContaminant(s => s.TPCMax); } }
        public string YeastSpec { get { return GetContaminant(s => s.YeastMax); } }
        public string MoldSpec { get { return GetContaminant(s => s.MoldMax); } }
        public string ColiformsSpec { get { return GetContaminant(s => s.ColiformsMax); } }
        public string EColiSpec { get { return GetContaminant(s => s.EColiMax); } }
        public string SalmonellaSpec { get { return GetContaminant(s => s.SalMax); } }

        public string AstaColorSample { get { return GetMatch(m => m.AvgAsta); } }
        public string MoistureSample { get { return GetMatch(m => m.H2O); } }
        public string ParticleSample { get { return GetMatch(m => m.Gran); } }
        public string SurfaceColorSample { get { return GetMatch(m => m.Scan); } }
        public string ABSample { get { return GetMatch(m => m.AoverB); } }
        public string SHUSample { get { return GetMatch(m => m.AvgScov); } }
        public string TPCSample { get { return GetMatch(m => m.TPC); } }
        public string YeastSample { get { return GetMatch(m => m.Yeast); } }
        public string MoldSample { get { return GetMatch(m => m.Mold); } }
        public string ColiformsSample { get { return GetMatch(m => m.Coli); } }
        public string EColiSample { get { return GetMatch(m => m.EColi); } }
        public string SalmonellaSample { get { return GetMatch(m => m.Sal); } }

        public string AstaColorMatch { get { return GetLotAttribute(StaticAttributeNames.Asta); } }
        public string MoistureMatch { get { return GetLotAttribute(StaticAttributeNames.H2O); } }
        public string ParticleMatch { get { return GetLotAttribute(StaticAttributeNames.Granularity); } }
        public string SurfaceColorMatch { get { return GetLotAttribute(StaticAttributeNames.Scan); } }
        public string ABMatch { get { return GetLotAttribute(StaticAttributeNames.AB); } }
        public string SHUMatch { get { return GetLotAttribute(StaticAttributeNames.Scoville); } }
        public string TPCMatch { get { return GetLotAttribute(StaticAttributeNames.TPC); } }
        public string YeastMatch { get { return GetLotAttribute(StaticAttributeNames.Yeast); } }
        public string MoldMatch { get { return GetLotAttribute(StaticAttributeNames.Mold); } }
        public string ColiformsMatch { get { return GetLotAttribute(StaticAttributeNames.ColiForms); } }
        public string EColiMatch { get { return GetLotAttribute(StaticAttributeNames.EColi); } }
        public string SalmonellaMatch { get { return GetLotAttribute(StaticAttributeNames.Salmonella); } }

        public string Status { get { return OrderStatus.ToDisplayString(); } }
        public string LotNumber { get { return LotKeyReturn == null ? null : LotNumberBuilder.BuildLotNumber(LotKeyReturn).ToString(); } }
        public string QuantitySent { get { return string.Format("{0} - {1}", Quantity, Description); } }
        public string ProductName { get; set; }
        public string Employee { get; set; }
        public DateTime Received { get; set; }
        public DateTime SampleDate { get; set; }
        public DateTime? Completed { get; set; }

        internal SampleOrderItemSpec Spec { get; set; }
        internal SampleOrderItemMatch Match { get; set; }
        internal IEnumerable<LotAttribute> LotAttributes { get; set; }

        internal SampleOrderStatus OrderStatus { get; set; }
        internal int Quantity { get; set; }
        internal string Description { get; set; }
        internal LotKeyReturn LotKeyReturn { get; set; }

        private string GetSpecRange(Func<SampleOrderItemSpec, double?> selectMin, Func<SampleOrderItemSpec, double?> selectMax)
        {
            var min = GetSpecValue(selectMin);
            var max = GetSpecValue(selectMax);
            if(min != null || max != null)
            {
                return string.Format("{0}-{1}", min ?? 0, max ?? 0);
            }

            return "n/a";
        }

        private string GetContaminant(Func<SampleOrderItemSpec, double?> selectMax)
        {
            var value = GetSpecValue(selectMax);
            return value == null ? "n/a" : string.Format("<{0}", value.Value);
        }

        private double? GetSpecValue(Func<SampleOrderItemSpec, double?> selectValue)
        {
            return Spec == null ? null : selectValue(Spec);
        }

        private string GetMatch(Func<SampleOrderItemMatch, string> select)
        {
            if(Match != null)
            {
                var value = select(Match);
                if(value != null)
                {
                    return value;
                }
            }

            return "n/a";
        }

        private string GetLotAttribute(IAttributeNameKey attribute)
        {
            if(LotAttributes != null)
            {
                var key = attribute.ToAttributeNameKey();
                var attr = LotAttributes.FirstOrDefault(a => key.Equals(a));
                if(attr != null)
                {
                    return attr.AttributeValue.ToString("0.####");
                }
            }

            return "n/a";
        }
    }
}