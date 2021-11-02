namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotQualitySingleSummaryReturn : IAttributesByProductType
    {
        ILotQualitySummaryReturn LotSummary { get; }
    }
}