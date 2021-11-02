using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CreateProductionBatchReturn : ICreateProductionBatchReturn
    {
        public string ProductionBatchKey { get; internal set; }

        public string InstructionNotebookKey { get; internal set; }
    }
}