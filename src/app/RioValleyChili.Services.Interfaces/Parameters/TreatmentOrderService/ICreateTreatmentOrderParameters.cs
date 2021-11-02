using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService
{
    public interface ICreateTreatmentOrderParameters : ISetOrderParameters
    {
        string TreatmentKey { get; }
    }
}