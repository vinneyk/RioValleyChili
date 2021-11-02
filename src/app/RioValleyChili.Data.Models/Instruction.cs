using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using Solutionhead.DataAnnotations;

namespace RioValleyChili.Data.Models
{
    public class Instruction : IInstructionKey
    {
        //todo: create MultipleColumnIndexAttribute class and change InstructionText index to a MultipleColumnIndex including TypeId 

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        [SingleColumnIndex(true)]
        [StringLength(Constants.StringLengths.InstructionText)]
        public virtual string InstructionText { get; set; }
        
        public virtual InstructionType InstructionType { get; set; }

        #region Implementation of IInstructionKey

        public int InstructionKey_InstructionId { get { return Id; } }

        #endregion
    }
}
