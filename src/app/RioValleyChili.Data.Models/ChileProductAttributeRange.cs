using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class ChileProductAttributeRange : EmployeeIdentifiableBase, IChileProductAttributeRangeKey, IAttributeRange
    {
        [Key, Column(Order = 0)]
        public virtual int ChileProductId { get; set; }
        [Key, Column(Order = 1), StringLength(Constants.StringLengths.AttributeShortName)]
        public virtual string AttributeShortName { get; set; }

        public virtual double RangeMin { get; set; }
        public virtual double RangeMax { get; set; }

        [ForeignKey("ChileProductId")]
        public virtual ChileProduct ChileProduct { get; set; }
        [ForeignKey("AttributeShortName")]
        public virtual AttributeName AttributeName { get; set; }

        #region IChileProductAttributeRangeKey interface implementation.

        public int ChileProductKey_ProductId { get { return ChileProductId; } }
        public string AttributeNameKey_ShortName { get { return AttributeShortName; } }

        #endregion
    }
}