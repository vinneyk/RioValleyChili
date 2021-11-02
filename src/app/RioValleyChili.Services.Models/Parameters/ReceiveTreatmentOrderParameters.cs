using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class ReceiveTreatmentOrderParameters : IReceiveTreatmentOrderParameters
    {
        public string UserToken { get; set; }
        public string TreatmentOrderKey { get; set; }
        public string DestinationLocationKey { get; set; }
    }
}