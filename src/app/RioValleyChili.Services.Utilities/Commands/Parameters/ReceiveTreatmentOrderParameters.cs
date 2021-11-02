using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class ReceiveTreatmentOrderParameters
    {
        public IReceiveTreatmentOrderParameters Parameters;
        public TreatmentOrderKey TreatmentOrderKey;
        public LocationKey DestinationLocationKey;
    }
}