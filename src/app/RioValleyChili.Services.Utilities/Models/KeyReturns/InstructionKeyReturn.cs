using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class InstructionKeyReturn : IInstructionKey
    {
        internal string InstructionKey { get { return new InstructionKey(this).KeyValue; } }

        public int InstructionKey_InstructionId { get; internal set; }
    }
}