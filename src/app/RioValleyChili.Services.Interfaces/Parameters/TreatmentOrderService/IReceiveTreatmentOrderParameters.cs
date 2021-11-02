using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService
{
    public interface IReceiveTreatmentOrderParameters : IUserIdentifiable
    {
        string TreatmentOrderKey { get; }
        string DestinationLocationKey { get; }
    }
}