using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class AttributeValueParameters : IAttributeValueParameters
    {
        public AttributeInfoParameters AttributeInfo { get; set; }
        public DefectResolutionParameters Resolution { get; set; }

        ILotAttributeInfoParameters IAttributeValueParameters.AttributeInfo { get { return AttributeInfo; } }
        IDefectResolutionParameters IAttributeValueParameters.Resolution { get { return Resolution; } }
    }
}