using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class LotCustomerAllowance : LotKeyEntityBase, ICustomerKey
    {
        [Key, Column(Order = 3)]
        public virtual int CustomerId { get; set; }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public int CustomerKey_Id { get { return CustomerId; } }
    }
}