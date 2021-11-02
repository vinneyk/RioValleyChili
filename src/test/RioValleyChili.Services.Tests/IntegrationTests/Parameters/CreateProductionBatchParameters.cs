using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateProductionBatchParameters : ICreateProductionBatchParameters
    {
        public string UserToken { get; set; }
        public double BatchTargetWeight { get; set; }
        public double BatchTargetAsta { get; set; }
        public double BatchTargetScan { get; set; }
        public double BatchTargetScoville { get; set; }
        public string PackScheduleKey { get; set; }
        public string Notes { get; set; }
        public string[] Instructions { get; set; }

        public LotTypeEnum? LotType { get; set; }
        public DateTime? LotDateCreated { get; set; }
        public int? LotSequence { get; set; }
    }
}