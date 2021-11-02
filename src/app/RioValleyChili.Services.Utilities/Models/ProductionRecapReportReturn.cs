using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionRecapReportReturn : IProductionRecapReportReturn
    {
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public IProductionRecapWeightGroup BatchWeights { get; internal set; }
        public IProductionRecapWeightGroup LineWeights { get; internal set; }
        public IProductionRecapTimeGroup BatchTimes { get; internal set; }
        public IProductionRecapTimeGroup LineTimes { get; internal set; }
        public IProductionRecapTestGroup BatchTests { get; internal set; }
        public IProductionRecapTestGroup LineTests { get; internal set; }

        public IProductionRecap_ByLineProduct LineProductWeights { get; internal set; }
        public IProductionRecap_LotGroupSection ProductDetails { get; internal set; }
        public IProductionRecap_LotGroupSection LineDetails { get; internal set; }

        public ProductionRecapReportReturn(DateTime startDate, DateTime endDate, List<ProductionRecapLot> productionResults)
        {
            StartDate = startDate.Date;
            EndDate = endDate.Date;

            BatchWeights = new ProductionRecapWeightGroup
                {
                    Items = productionResults.GroupBy(r => r.WorkType).Select(g => CreateWeightItem(g))
                        .OrderBy(i => i.Name).ToList()
                };
            LineWeights = new ProductionRecapWeightGroup
                {
                    Items = productionResults.GroupBy(r => r.ProductionLocation.Description).Select(g => CreateWeightItem(g, l => GetLineNumber(l.Key)))
                        .OrderBy(i => i.Name).ToList()
                };

            BatchTimes = new ProductionRecap_TimeGroup(productionResults.GroupBy(r => r.WorkType));
            LineTimes = new ProductionRecap_TimeGroup(productionResults.GroupBy(r => r.ProductionLocation.Description), GetLineNumber);

            BatchTests = new ProductionRecap_TestGroup(productionResults.GroupBy(r => r.WorkType));
            LineTests = new ProductionRecap_TestGroup(productionResults.GroupBy(r => r.ProductionLocation.Description), GetLineNumber);

            LineProductWeights = new ProductionRecap_LineProduct
                {
                    ItemsByLine = productionResults.GroupBy(r => r.ProductionLocation.Description)
                        .Select(CreateByLineGroup)
                        .OrderBy(m => m.Line).ToList()
                };

            ProductDetails = new ProductionRecap_LotGroupSection(productionResults.GroupBy(r => r.ProductType));
            LineDetails = new ProductionRecap_LotGroupSection(productionResults.GroupBy(r => r.ProductionLocation.Description), GetLineNumber);
        }

        private static ProductionRecap_LineProduct_ByLine CreateByLineGroup(IGrouping<string, ProductionRecapLot> byLine)
        {
            var lineGroup = new ProductionRecap_LineProduct_ByLine
                {
                    Line = GetLineNumber(byLine.Key),
                    ItemsByType = byLine.GroupBy(g => g.ProductType)
                        .Select(CreateByTypeGroup)
                        .OrderBy(m => m.Type).ToList()
                };
            return lineGroup;
        }

        private static ProductionRecap_LineProduct_ByLine_ByType CreateByTypeGroup(IGrouping<string, ProductionRecapLot> byType)
        {
            var typeGroup = new ProductionRecap_LineProduct_ByLine_ByType
                {
                    Type = byType.Key,
                    ItemsByProduct = byType.GroupBy(g => g.ChileProduct.ProductName).Select(g => CreateWeightItem(g))
                        .OrderBy(i => i.Name).ToList()
                };
            return typeGroup;
        }

        private static ProductionRecapWeightItem CreateWeightItem(IGrouping<string, ProductionRecapLot> lotGroup, Func<IGrouping<string, ProductionRecapLot>, string> selectName = null)
        {
            return new ProductionRecapWeightItem
                {
                    Name = selectName == null ? lotGroup.Key : selectName(lotGroup),
                    Target = (int)lotGroup.Sum(i2 => i2.TotalInputWeight),
                    Produced = (int)lotGroup.Sum(i2 => i2.TotalOutputWeight)
                };
        }

        private static string GetLineNumber(string locationDescription)
        {
            string street;
            int row;
            return LocationDescriptionHelper.GetStreetRow(locationDescription, out street, out row) ? row.ToString() : locationDescription;
        }
    }

    internal class ProductionRecapWeightItem : IProductionRecapWeightItem
    {
        public string Name { get; internal set; }
        public int Target { get; internal set; }
        public int Produced { get; internal set; }
    }

    internal class ProductionRecapWeightGroup : ProductionRecapWeightItem, IProductionRecapWeightGroup
    {
        public IEnumerable<IProductionRecapWeightItem> Items { get; internal set; }
    }

    internal class ProductionRecap_LineProduct_ByLine_ByType : IProductionRecap_ByLineProduct_ByLine_ByType
    {
        public string Type { get; internal set; }
        public IEnumerable<IProductionRecapWeightItem> ItemsByProduct { get; internal set; }
    }

    internal class ProductionRecap_LineProduct_ByLine : IProductionRecap_ByLineProduct_ByLine
    {
        public string Line { get; internal set; }
        public IEnumerable<IProductionRecap_ByLineProduct_ByLine_ByType> ItemsByType { get; internal set; }
    }

    internal class ProductionRecap_LineProduct : IProductionRecap_ByLineProduct
    {
        public IEnumerable<IProductionRecap_ByLineProduct_ByLine> ItemsByLine { get; internal set; }
    }

    internal class ProductionRecap_TestItem : IProductionRecapTestItem
    {
        public string Name { get; internal set; }
        public int Passed { get; internal set; }
        public int Failed { get; internal set; }
        public int NonCntrl { get; internal set; }
        public int InProc { get; internal set; }

        public ProductionRecap_TestItem(IGrouping<string, ProductionRecapLot> group, Func<string, string> processName)
        {
            Name = processName == null ? group.Key : processName(group.Key);

            var results = group.GroupBy(g => g.TestResult).ToDictionary(g => g.Key, g => g.ToList());
            Passed = WeightOf(TestResults.Pass, results);
            Failed = WeightOf(TestResults.Fail, results);
            NonCntrl = WeightOf(TestResults.NonCntrl, results);
            InProc = WeightOf(TestResults.InProc, results);
        }

        private static int WeightOf(TestResults testResults, IDictionary<TestResults, List<ProductionRecapLot>> lots)
        {
            List<ProductionRecapLot> results;
            if(!lots.TryGetValue(testResults, out results) || !results.Any())
            {
                return 0;
            }

            return (int) results.Sum(r => r.TotalOutputWeight);
        }
    }

    internal class ProductionRecap_TestGroup : IProductionRecapTestGroup
    {
        public IEnumerable<IProductionRecapTestItem> Items { get; internal set; }

        public ProductionRecap_TestGroup(IEnumerable<IGrouping<string, ProductionRecapLot>> groupBy, Func<string, string> processName = null)
        {
            Items = groupBy.Select(g => new ProductionRecap_TestItem(g, processName)).OrderBy(i => i.Name).ToList();
        }
    }

    internal class ProductionRecap_TimeItem : IProductionRecapTimeItem
    {
        public string Name { get; internal set; }
        public float Actual { get; internal set; }

        public ProductionRecap_TimeItem(IGrouping<string, ProductionRecapLot> lotGroup, Func<string, string> processName)
        {
            Name = processName == null ? lotGroup.Key : processName(lotGroup.Key);
            Actual = lotGroup.Any() ? lotGroup.Sum(g => g.ProductionTime) : 0.0f;
        }
    }

    internal class ProductionRecap_TimeGroup : IProductionRecapTimeGroup
    {
        public IEnumerable<IProductionRecapTimeItem> Items { get; internal set; }

        public ProductionRecap_TimeGroup(IEnumerable<IGrouping<string, ProductionRecapLot>> lotGroups, Func<string, string> processName = null)
        {
            Items = lotGroups.Select(g => new ProductionRecap_TimeItem(g, processName)).OrderBy(i => i.Name).ToList();
        }
    }

    internal class ProductionRecap_Lot : IProductionRecap_Lot
    {
        public string Name { get; internal set; }
        public string Lot { get; internal set; }
        public string Line { get; internal set; }
        public string Shift { get; internal set; }
        public string PSNum { get; internal set; }
        public string BatchType { get; internal set; }
        public string LotStat { get; internal set; }
        public int TargetWeight { get; internal set; }
        public int ProducedWeight { get; internal set; }
        public float ProductionTime { get; internal set; }

        public ProductionRecap_Lot(ProductionRecapLot lot)
        {
            Name = lot.ChileProduct.ProductName;
            Lot = lot.LotKey.LotKey;

            string street;
            int row;
            Line = LocationDescriptionHelper.GetStreetRow(lot.ProductionLocation.Description, out street, out row) ? row.ToString() : lot.ProductionLocation.Description;

            Shift = lot.Shift;
            PSNum = lot.ProductionBatch != null && lot.ProductionBatch.PSNum != null ? lot.ProductionBatch.PSNum.ToString() : "";
            BatchType = lot.WorkType;
            LotStat = lot.TestResult.ToString();
            TargetWeight = (int) lot.TotalInputWeight;
            ProducedWeight = (int) lot.TotalOutputWeight;
            ProductionTime = lot.ProductionTime;
        }
    }

    internal class ProductionRecap_LotGroup : IProductionRecap_LotGroup
    {
        public string Name { get; private set; }
        public IEnumerable<IProductionRecap_Lot> Items { get; internal set; }

        public ProductionRecap_LotGroup(IGrouping<string, ProductionRecapLot> lotGroup, Func<string, string> processName)
        {
            Name = processName == null ? lotGroup.Key : processName(lotGroup.Key);
            Items = lotGroup.Select(i => new ProductionRecap_Lot(i)).OrderBy(i => i.Lot).ToList();
        }
    }

    internal class ProductionRecap_LotGroupSection : IProductionRecap_LotGroupSection
    {
        public IEnumerable<IProductionRecap_LotGroup> Items { get; internal set; }

        public ProductionRecap_LotGroupSection(IEnumerable<IGrouping<string, ProductionRecapLot>> lotGroups, Func<string, string> processName = null)
        {
            Items = lotGroups.Select(g => new ProductionRecap_LotGroup(g, processName)).OrderBy(i => i.Name).ToList();
        }
    }
}