// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ChileLotProductionProjectors
    {
        internal static Expression<Func<ChileLotProduction, MillAndWetdownReturn>> SelectSummary()
        {
            return SelectBase();
        }

        internal static Expression<Func<ChileLotProduction, MillAndWetdownReturn>> SelectDetail(ILotUnitOfWork lotUnitOfWork)
        {
            var resultItem = LotProductionResultItemProjectors.SelectMillAndWetdownResultItem();
            var pickedItem = PickedInventoryItemProjectors.SelectMillAndWetdownPickedItem(lotUnitOfWork);

            return SelectBase().Merge(m => new MillAndWetdownReturn
            {
                ResultItems = m.Results.ResultItems.Select(i => resultItem.Invoke(i)),
                PickedItems = m.PickedInventory.Items.Select(i => pickedItem.Invoke(i))
            });
        }

        private static Expression<Func<ChileLotProduction, MillAndWetdownReturn>> SelectBase()
        {
            var lotKey = LotProjectors.SelectLotKey<ChileLotProduction>();
            var productKey = ProductProjectors.SelectProductKey();
            var productionLocationKey = LocationProjectors.SelectLocationKey();

            return m => new MillAndWetdownReturn
            {
                OutputChileLotKeyReturn = lotKey.Invoke(m),
                ChileProductKeyReturn = productKey.Invoke(m.ResultingChileLot.ChileProduct.Product),
                ProductionLineLocationKeyReturn = productionLocationKey.Invoke(m.Results.ProductionLineLocation),

                ShiftKey = m.Results.ShiftKey,
                ProductionLineDescription = m.Results.ProductionLineLocation.Description,
                ChileProductName = m.ResultingChileLot.ChileProduct.Product.Name,

                ProductionBegin = m.Results.ProductionBegin,
                ProductionEnd = m.Results.ProductionEnd,
                TotalProductionTimeMinutes = EntityFunctions.DiffMinutes(m.Results.ProductionBegin, m.Results.ProductionEnd) ?? 0,
                TotalWeightProduced = m.Results.ResultItems.Any() ? (int)m.Results.ResultItems.Sum(i => i.PackagingProduct.Weight * i.Quantity) : 0,
                TotalWeightPicked = m.PickedInventory.Items.Any() ? (int)m.PickedInventory.Items.Sum(i => i.PackagingProduct.Weight * i.Quantity) : 0
            };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup