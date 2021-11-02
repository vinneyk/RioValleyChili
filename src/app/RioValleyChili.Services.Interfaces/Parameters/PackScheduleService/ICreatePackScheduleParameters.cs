using System;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.PackScheduleService
{
    public interface ICreatePackScheduleParameters : IUserIdentifiable, IProductionBatchTargetParameters
    {
        string WorkTypeKey { get; }
        string ChileProductKey { get; }
        string PackagingProductKey { get; }
        string ProductionLineKey { get; }
        string CustomerKey { get; }

        DateTime ScheduledProductionDate { get; }
        DateTime? ProductionDeadline { get; }
        string OrderNumber { get; }
        string SummaryOfWork { get; }

        DateTime? DateCreated { get; }
        int? Sequence { get; }
        int? PSNum { get; }
    }
}