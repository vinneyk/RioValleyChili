using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class InventoryTreatmentForAttribute : IInventoryTreatmentForAttributeKey
    {
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int TreatmentId { get; set; }
        [Key, Column(Order = 2), StringLength(10)]
        public virtual string AttributeShortName { get; set; }

        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment Treatment { get; set; }
        [ForeignKey("AttributeShortName")]
        public virtual AttributeName AttributeName { get; set; }

        #region Key Interface Implementations

        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }
        public string AttributeNameKey_ShortName { get { return AttributeShortName; } }

        #endregion

    }
}