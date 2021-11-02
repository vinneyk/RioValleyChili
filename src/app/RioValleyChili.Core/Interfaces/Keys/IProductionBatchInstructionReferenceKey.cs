namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IProductionBatchInstructionReferenceKey : ILotKey
    {
        int ProductionBatchInstructionReferenceKey_InstructionOrder { get; }
    }
}