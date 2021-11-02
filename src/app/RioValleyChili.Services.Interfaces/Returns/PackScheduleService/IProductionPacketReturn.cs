using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IProductionPacketReturn
    {
        string PackScheduleKey { get; }
        int? PSNum { get; }

        DateTime DateCreated { get; }
        string ProductionLineDescription { get; }
        string SummaryOfWork { get; }

        IProductReturn ChileProduct { get; }
        IProductKeyNameReturn PackagingProduct { get; }
        IWorkTypeReturn WorkType { get; }
        ICompanyHeaderReturn Customer { get; }
        IEnumerable<IProductionPacketBatchReturn> Batches { get; }
    }
}