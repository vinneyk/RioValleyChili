namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface ILotAttributeDefectReturn
    {
        string AttributeShortName { get; }

        double OriginalValue { get; }

        double OriginalMinLimit { get; }

        double OriginalMaxLimit { get; }
    }
}