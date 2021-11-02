using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RioValleyChili.Data.Models
{
    public class LotHistory : LotKeyEmployeeIdentifiableBase
    {
        [Key, Column(Order = 4)]
        public virtual int Sequence { get; set; }

        public virtual string Serialized { get; set; }
    }
}