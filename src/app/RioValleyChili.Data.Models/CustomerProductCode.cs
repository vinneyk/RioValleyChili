using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class CustomerProductCode : ICustomerProductCodeKey
    {
        [Key, Column(Order = 0)]
        public virtual int CustomerId { get; set; }
        [Key, Column(Order = 1)]
        public virtual int ChileProductId { get; set; }

        [StringLength(Constants.StringLengths.CustomerProductCode)]
        public virtual string Code { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("ChileProductId")]
        public virtual ChileProduct ChileProduct { get; set; }

        #region Key Interface Implementations

        public int CustomerKey_Id { get { return CustomerId; } }
        public int ChileProductKey_ProductId { get { return ChileProductId; } }

        #endregion
    }
}