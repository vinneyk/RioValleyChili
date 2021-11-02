using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class WeightedLotAttributeReturn : LotAttributeReturn, IWeightedLotAttributeReturn
    {
        public double WeightedAverage { get; internal set; }
        public bool HasResolvedDefects { get; internal set; }
    }
}