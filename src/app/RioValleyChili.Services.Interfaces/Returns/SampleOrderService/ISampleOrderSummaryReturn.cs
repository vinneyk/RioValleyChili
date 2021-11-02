using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISampleOrderSummaryReturn
    {
        string SampleRequestKey { get; }
        DateTime DateDue { get; }
        DateTime DateReceived { get; }
        DateTime? DateCompleted { get; }
        SampleOrderStatus Status { get; }

        ICompanyHeaderReturn RequestedByCompany { get; }
        ICompanyHeaderReturn Broker { get; }
        IUserSummaryReturn CreatedByUser { get; }
    }
}