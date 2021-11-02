namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IInventoryTreatmentReturn
    {
        string TreatmentKey { get; }
        string TreatmentName { get; }
        string TreatmentNameShort { get; }
    }
}