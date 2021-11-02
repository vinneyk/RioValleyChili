using System.ComponentModel.DataAnnotations;

namespace RioValleyChili.Data.Models
{
    public class LotStatusValue
    {
        [Key]
        public virtual int Id { get; set; }

        [StringLength(25)]
        public virtual string Description { get; set; }
    }
}