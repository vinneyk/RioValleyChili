using System;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IPackScheduleSummaryReturn : IPackScheduleBaseParametersReturn
    {
        DateTime DateCreated { get; }

        DateTime ScheduledProductionDate { get; }

        DateTime? ProductionDeadline { get; }

        string WorkTypeKey { get; }

        string ChileProductKey { get; }

        string ChileProductName { get; }

        string ProductionLineKey { get; }

        string OrderNumber { get; }

        ICompanyHeaderReturn Customer { get; }
    }
}