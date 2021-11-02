using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class CustomerProductAttributeRange : EmployeeIdentifiableBase, ICustomerProductAttributeRangeKey, IAttributeRange
    {
        [Key, Column(Order = 0)]
        public virtual int CustomerId { get; set; }
        [Key, Column(Order = 1)]
        public virtual int ChileProductId { get; set; }
        [Key, Column(Order = 3), StringLength(10)]
        public virtual string AttributeShortName { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("ChileProductId")]
        public virtual ChileProduct ChileProduct { get; set; }
        [ForeignKey("AttributeShortName")]
        public virtual AttributeName AttributeName { get; set; }

        public virtual double RangeMin { get; set; }
        public virtual double RangeMax { get; set; }
        public virtual bool Active { get; set; }

        #region Key Interface Implementations

        public int CustomerKey_Id { get { return CustomerId; } }
        public int ChileProductKey_ProductId { get { return ChileProductId; } }
        public string AttributeNameKey_ShortName { get { return AttributeShortName; } }

        #endregion
    }
}