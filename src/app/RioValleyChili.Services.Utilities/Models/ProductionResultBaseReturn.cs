using System;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionResultBaseReturn : IProductionResultBaseReturn
    {
        public DateTime ProductionEndDate { get; internal set; }

        public string ProductionShiftKey { get; internal set; }

        public ILocationReturn ProductionLocation { get; internal set; }
    }
}