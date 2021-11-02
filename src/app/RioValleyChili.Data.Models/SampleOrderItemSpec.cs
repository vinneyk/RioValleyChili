using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class SampleOrderItemSpec : ISampleOrderItemKey
    {
        [Key, Column(Order = 0)]
        public virtual int SampleOrderYear { get; set; }
        [Key, Column(Order = 1)]
        public virtual int SampleOrderSequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ItemSequence { get; set; }

        public virtual double? AstaMin { get; set; }
        public virtual double? AstaMax { get; set; }
        public virtual double? MoistureMin { get; set; }
        public virtual double? MoistureMax { get; set; }
        public virtual double? WaterActivityMin { get; set; }
        public virtual double? WaterActivityMax { get; set; }
        public virtual double? Mesh { get; set; }
        public virtual double? AoverB { get; set; }
        public virtual double? ScovMin { get; set; }
        public virtual double? ScovMax { get; set; }
        public virtual double? ScanMin { get; set; }
        public virtual double? ScanMax { get; set; }
        public virtual double? TPCMin { get; set; }
        public virtual double? TPCMax { get; set; }
        public virtual double? YeastMin { get; set; }
        public virtual double? YeastMax { get; set; }
        public virtual double? MoldMin { get; set; }
        public virtual double? MoldMax { get; set; }
        public virtual double? ColiformsMin { get; set; }
        public virtual double? ColiformsMax { get; set; }
        public virtual double? EColiMin { get; set; }
        public virtual double? EColiMax { get; set; }
        public virtual double? SalMin { get; set; }
        public virtual double? SalMax { get; set; }

        [StringLength(Constants.StringLengths.SampleOrderNotes)]
        public virtual string Notes { get; set; }

        [Obsolete("For referencing old context. - RI 2016/9/13")]
        public DateTime? CustSpecID { get; set; }

        [ForeignKey("SampleOrderYear, SampleOrderSequence, ItemSequence")]
        public virtual SampleOrderItem Item { get; set; }

        int ISampleOrderKey.SampleOrderKey_Year { get { return SampleOrderYear; } }
        int ISampleOrderKey.SampleOrderKey_Sequence { get { return SampleOrderSequence; } }
        int ISampleOrderItemKey.SampleOrderItemKey_Sequence { get { return ItemSequence; } }
    }
}