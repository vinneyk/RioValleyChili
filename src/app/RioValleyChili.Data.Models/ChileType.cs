using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class ChileType : IChileTypeKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        [StringLength(50)]
        public virtual string Description { get; set; }

        #region Implementation of IChileTypeKey

        public int ChileTypeKey_ChileTypeId { get { return Id; } }

        #endregion
    }
}