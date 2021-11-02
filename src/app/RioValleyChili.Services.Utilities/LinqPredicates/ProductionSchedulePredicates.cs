using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ProductionSchedulePredicates
    {
        internal static Expression<Func<ProductionSchedule, bool>> ByProductionDate(DateTime productionDate)
        {
            productionDate = productionDate.Date;
            return p => p.ProductionDate == productionDate;
        }

        internal static Expression<Func<ProductionSchedule, bool>> ByProductionLineLocationKey(ILocationKey locationKey)
        {
            return p => p.ProductionLineLocationId == locationKey.LocationKey_Id;
        }
    }
}