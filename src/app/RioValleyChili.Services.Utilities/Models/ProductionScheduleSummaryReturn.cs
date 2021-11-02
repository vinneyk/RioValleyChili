using System;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionScheduleSummaryReturn : IProductionScheduleSummaryReturn
    {
        public string ProductionScheduleKey { get { return ProductionScheduleKeyReturn.ProductionScheduleKey; } }
        public DateTime ProductionDate { get; internal set; }
        public ILocationReturn ProductionLine { get; internal set; }

        internal ProductionScheduleKeyReturn ProductionScheduleKeyReturn { get; set; }
    }
}