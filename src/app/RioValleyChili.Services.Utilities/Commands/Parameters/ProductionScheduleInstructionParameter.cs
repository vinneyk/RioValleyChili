using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class ProductionScheduleInstructionParameter
    {
        public Instruction Instruction { get; private set; }

        public int Order { get; set; }

        public ProductionScheduleInstructionParameter(Instruction instruction, int order)
        {
            Instruction = instruction;
            Order = order;
        }
    }
}