using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class AdditiveType : IAdditiveTypeKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        [Required, StringLength(50)]
        public string Description { get; set; }

        #region Implementation of IAdditiveTypeKey

        public int AdditiveTypeKey_AdditiveTypeId { get { return Id; } }

        #endregion
    }
}
