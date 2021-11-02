using System.ComponentModel.DataAnnotations;

namespace RioValleyChili.Data.Models
{
    public class LotStatusName
    {
        [Key]
        public virtual int Id { get; set; }

        [StringLength(25)]
        public virtual string Name { get; set; }
    }
}