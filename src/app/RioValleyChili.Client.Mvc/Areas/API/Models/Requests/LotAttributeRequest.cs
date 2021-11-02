namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class LotAttributeRequest
    {
        public string AttributeKey { get; set; }
        public LotAttributeInfoRequest AttributeInfo { get; set; }
        public DefectResolutionRequest Resolution { get; set; }
    }
}