using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class InventoryTreatment : IInventoryTreatmentKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        [StringLength(40)]
        public virtual string LongName { get; set; }
        [StringLength(10)]
        public virtual string ShortName { get; set; }

#warning This property needs to be mapped and data migration needs to be generated. Furthermore, update service methods, utilities, and commands as necessary.
        [NotMapped] 
        public virtual bool Active { get; set; }

        public ICollection<InventoryTreatmentForAttribute> AttributesTreated { get; set; }

        #region Implementation of IInventoryTreatmentKey

        public int InventoryTreatmentKey_Id { get { return Id; } }

        #endregion
    }
}