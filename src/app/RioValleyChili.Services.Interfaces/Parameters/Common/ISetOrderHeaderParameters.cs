using System;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public interface ISetOrderHeaderParameters
    {
        string CustomerPurchaseOrderNumber { get; }
        DateTime? DateOrderReceived { get; }
        string PaymentTerms { get; }
        string OrderRequestedBy { get; }
        string OrderTakenBy { get; }
    }
}