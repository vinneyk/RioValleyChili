using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PackScheduleDetailReturn : PackScheduleSummaryReturn, IPackScheduleDetailReturn
    {
        public string PackagingProductKey { get { return PackagingProductKeyReturn.ProductKey; } }

        public string PackagingProductName { get; internal set; }

        public double PackagingWeight { get; internal set; }

        public string SummaryOfWork { get; internal set; }

        public IEnumerable<IProductionBatchSummaryReturn> ProductionBatches { get; internal set; }

        internal ProductKeyReturn PackagingProductKeyReturn { get; set; }
    }
}