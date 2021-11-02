using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;

namespace RioValleyChili.Data.Models
{
    public class LotDefectsInventoryResolutions
    {
        [Key]
        [Column(Order = 0, TypeName = "Date")]
        public virtual DateTime LotDateCreated { get; set; }

        [Key]
        [Column(Order = 1)]
        public virtual int LotDateSequence { get; set; }

        [Key]
        [Column(Order = 2)]
        [Obsolete("Use ProductionLineLocationTypeEnum until http://stackoverflow.com/q/12220956 is resolved.")]
        public virtual int LotTypeId
        {
            get { return (int)((LotTypeEnum)_lotType); }
            set { _lotType = (int)((LotTypeEnum)value); }
        }

        [Key]
        [Column(Order = 3)]
        public virtual int ResolutionsId { get; set; }

        [NotMapped]
        public LotTypeEnum LotTypeEnum
        {
            get { return (LotTypeEnum)_lotType; }
            set { _lotType = (int)value; }
        }

        private int _lotType;
    }
}