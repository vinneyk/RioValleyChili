using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class SampleOrderItemMatch : ISampleOrderItemKey
    {
        [Key, Column(Order = 0)]
        public virtual int SampleOrderYear { get; set; }
        [Key, Column(Order = 1)]
        public virtual int SampleOrderSequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ItemSequence { get; set; }

        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string Gran { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string AvgAsta { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string AoverB { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string AvgScov { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string H2O { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string Scan { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string Yeast { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string Mold { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string Coli { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string TPC { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string EColi { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string Sal { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string InsPrts { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public virtual string RodHrs { get; set; }

        [StringLength(Constants.StringLengths.SampleOrderNotes)]
        public virtual string Notes { get; set; }

        [Obsolete("For referencing old context. - RI 2016/9/13")]
        public DateTime? RVCMatchID { get; set; }

        [ForeignKey("SampleOrderYear, SampleOrderSequence, ItemSequence")]
        public virtual SampleOrderItem Item { get; set; }

        int ISampleOrderKey.SampleOrderKey_Year { get { return SampleOrderYear; } }
        int ISampleOrderKey.SampleOrderKey_Sequence { get { return SampleOrderSequence; } }
        int ISampleOrderItemKey.SampleOrderItemKey_Sequence { get { return ItemSequence; } }
    }
}