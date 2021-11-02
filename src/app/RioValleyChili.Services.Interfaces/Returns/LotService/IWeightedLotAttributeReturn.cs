using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface IWeightedLotAttributeReturn : ILotAttributeReturn
    {
        double WeightedAverage { get; }
        bool HasResolvedDefects { get; }
    }
}