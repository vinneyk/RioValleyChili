using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class LotContractAllowance : LotKeyEntityBase, IContractKey
    {
        [Key, Column(Order = 3)]
        public virtual int ContractYear { get; set; }
        [Key, Column(Order = 4)]
        public virtual int ContractSequence { get; set; }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }
        [ForeignKey("ContractYear, ContractSequence")]
        public virtual Contract Contract { get; set; }

        public int ContractKey_Year { get { return ContractYear; } }
        public int ContractKey_Sequence { get { return ContractSequence; } }
    }
}