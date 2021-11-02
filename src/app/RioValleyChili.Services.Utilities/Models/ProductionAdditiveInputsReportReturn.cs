using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionAdditiveInputsReportReturn : IProductionAdditiveInputsReportReturn
    {
        public DateTime ProductionStart { get; private set; }
        public DateTime ProductionEnd { get; private set; }
        public IEnumerable<IProductionAdditiveInputs_ByDateReturn> ByDates { get; private set; }

        internal ProductionAdditiveInputsReportReturn(DateTime start, DateTime end, IEnumerable<ProductionAdditiveInputs> inputs)
        {
            ProductionStart = start;
            ProductionEnd = end;

            ByDates = inputs.GroupBy(i => i.ProductionDate.Date)
                .Select(byDates => new ProductionAdditiveInputs_ByDateReturn
                    {
                        ProductionDate = byDates.Key,
                        Totals = byDates.SelectMany(d => d.PickedAdditiveItems)
                            .GroupBy(a => a.AdditiveProduct.AdditiveTypeDescription)
                            .Select(byType => new ProductionAdditiveInputs_Totals
                                {
                                    AdditiveType = byType.Key,
                                    TotalPoundsPicked = byType.Sum(a => a.TotalPoundsPicked)
                                }),
                        Lots = byDates.Select(d => new ProductionLotAdditiveInputs
                                {
                                    LotKey = d.LotKey,
                                    Product = d.LotProduct.ProductCodeAndName,
                                    Additives = d.PickedAdditiveItems
                                        .GroupBy(a => a.AdditiveProduct.AdditiveTypeDescription)
                                        .Select(byType => new ProductionAdditiveInputs_ByAdditiveTypeReturn
                                            {
                                                AdditiveType = byType.Key,
                                                PickedItems = byType.Select(a => new ProductionAdditiveInputPicked
                                                    {
                                                        AdditiveType = byType.Key,
                                                        LotKey = a.LotKey,
                                                        TotalPoundsPicked = (int) a.TotalPoundsPicked,
                                                        UserResultEntered = a.UserResultEntered
                                                    })
                                            })
                                })
                    });
        }
    }

    internal class ProductionAdditiveInputs_ByDateReturn : IProductionAdditiveInputs_ByDateReturn
    {
        public DateTime ProductionDate { get; internal set; }
        public IEnumerable<IProductionLotAdditiveInputs> Lots { get; internal set; }
        public IEnumerable<IProductionAdditiveInputs_Totals> Totals { get; internal set; }
    }

    internal class ProductionLotAdditiveInputs : IProductionLotAdditiveInputs
    {
        public string LotKey { get; internal set; }
        public string Product { get; internal set; }
        public IEnumerable<IProductionAdditiveInputs_ByAdditiveTypeReturn> Additives { get; internal set; }
    }

    internal class ProductionAdditiveInputs_ByAdditiveTypeReturn : IProductionAdditiveInputs_ByAdditiveTypeReturn
    {
        public string AdditiveType { get; internal set; }
        public IEnumerable<IProductionAdditiveInputPicked> PickedItems { get; internal set; }
    }

    internal class ProductionAdditiveInputPicked : IProductionAdditiveInputPicked
    {
        public string AdditiveType { get; internal set; }
        public string LotKey { get; internal set; }
        public int TotalPoundsPicked { get; internal set; }
        public string UserResultEntered { get; internal set; }
    }

    internal class ProductionAdditiveInputs_Totals : IProductionAdditiveInputs_Totals
    {
        public string AdditiveType { get; internal set; }
        public double TotalPoundsPicked { get; internal set; }
    }
}