namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface ICreateProductionBatchReturn
    {
        string ProductionBatchKey { get; }

        string InstructionNotebookKey { get; }
    }
}