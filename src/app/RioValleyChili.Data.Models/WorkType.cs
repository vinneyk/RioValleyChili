using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class WorkType : EntityBase, IWorkTypeKey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        [Required]
        [StringLength(35)]
        public virtual string Description { get; set; }

        #region Implementation of IWorkTypeKey

        public int WorkTypeKey_WorkTypeId { get { return Id; } }

        #endregion
    }
}