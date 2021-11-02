using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class SampleOrderItem : ISampleOrderItemKey
    {
        [Key, Column(Order = 0)]
        public virtual int SampleOrderYear { get; set; }
        [Key, Column(Order = 1)]
        public virtual int SampleOrderSequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ItemSequence { get; set; }

        public virtual int Quantity { get; set; }
        [StringLength(Constants.StringLengths.SampleItem)]
        public virtual string Description { get; set; }
        [StringLength(Constants.StringLengths.SampleItem)]
        public virtual string CustomerProductName { get; set; }

        public virtual int? ProductId { get; set; }

        public virtual DateTime? LotDateCreated { get; set; }
        public virtual int? LotDateSequence { get; set; }
        public virtual int? LotTypeId { get; set; }

        [Obsolete("For referencing old context. - RI 2016/9/13")]
        public DateTime? SampleDetailID { get; set; }

        [ForeignKey("SampleOrderYear, SampleOrderSequence")]
        public virtual SampleOrder SampleOrder { get; set; }
        [ForeignKey("SampleOrderYear, SampleOrderSequence, ItemSequence")]
        public virtual SampleOrderItemSpec Spec { get; set; }
        [ForeignKey("SampleOrderYear, SampleOrderSequence, ItemSequence")]
        public virtual SampleOrderItemMatch Match { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }

        int ISampleOrderKey.SampleOrderKey_Year { get { return SampleOrderYear; } }
        int ISampleOrderKey.SampleOrderKey_Sequence { get { return SampleOrderSequence; } }
        int ISampleOrderItemKey.SampleOrderItemKey_Sequence { get { return ItemSequence; } }
    }
}