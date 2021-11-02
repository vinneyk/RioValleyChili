using System;
using System.Linq.Expressions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class InstructionProjectors
    {
        internal static Expression<Func<Instruction, InstructionKeyReturn>> SelectInstructionKey()
        {
            return i => new InstructionKeyReturn
                {
                    InstructionKey_InstructionId = i.Id
                };
        }
    }
}