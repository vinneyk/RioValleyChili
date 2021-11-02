using System;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionResultsService
{
    public interface IProductionResultBaseReturn
    {
        DateTime ProductionEndDate { get; }

        string ProductionShiftKey { get; }

        ILocationReturn ProductionLocation { get; }
    }
}