using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionRecapLot
    {
        internal ProductionType ProductionType { get; set; }
        internal LotProductionStatus LotProductionStatus { get; set; }
        internal LotQualityStatus LotQualityStatus { get; set; }
        internal bool OutOfSpec { get; set; }
        internal DateTime ProductionBegin { get; set; }
        internal DateTime ProductionEnd { get; set; }
        internal double TotalInputWeight { get; set; }
        internal double TotalOutputWeight { get; set; }
        internal string Shift { get; set; }
        internal float ProductionTime { get { return (float)(ProductionEnd - ProductionBegin).TotalHours; } }
        internal ChileProductReturn ChileProduct { get; set; }
        internal LocationReturn ProductionLocation { get; set; }
        internal LotKeyReturn LotKey { get; set; }
        internal ProductionRecapBatch ProductionBatch { get; set; }
        internal IEnumerable<string> UnresolvedDefects { get; set; }

        internal string ProductType { get { return ProductionType == ProductionType.MillAndWetdown ? "WIP Material" : ChileProduct.ChileTypeDescription; } }
        internal string WorkType { get { return ProductionType == ProductionType.MillAndWetdown ? "WIP" : ProductionBatch.WorkType.Description; } }
        internal TestResults TestResult { get { return _testResult ?? (_testResult = DetermineTestResults()).Value; } }
        private TestResults? _testResult;

        private TestResults DetermineTestResults()
        {
            if(LotProductionStatus == LotProductionStatus.Batched)
            {
                return TestResults.InProc;
            }

            switch(LotQualityStatus)
            {
                case LotQualityStatus.Pending: return TestResults.InProc;

                case LotQualityStatus.Released:
                    if(!OutOfSpec)
                    {
                        return TestResults.Pass;
                    }
                    if(UnresolvedDefects.Select(d => d.ToUpper()).All(d => NonControllableDefects.Contains(d)))
                    {
                        return TestResults.NonCntrl;
                    }
                    break;
            }

            return TestResults.Fail;
        }

        private static readonly List<string> NonControllableDefects = new List<string>
            {
                "ASTA",
                "AB",
                "SCOVILLE"
            };
    }

    internal class ProductionRecapBatch
    {
        internal int? PSNum { get; set; }
        internal PackScheduleKeyReturn PackScheduleKeyReturn { get; set; }
        internal WorkTypeReturn WorkType { get; set; }
    }
}