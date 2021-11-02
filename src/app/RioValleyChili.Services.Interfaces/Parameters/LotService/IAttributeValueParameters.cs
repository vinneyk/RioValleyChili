namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface IAttributeValueParameters
    {
        ILotAttributeInfoParameters AttributeInfo { get; }
        IDefectResolutionParameters Resolution { get; }
    }
}