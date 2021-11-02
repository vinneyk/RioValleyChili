using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Client.Mvc.Extensions
{
    public static class WarehouseExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<IFacilityDetailReturn> warehouses, Expression<Func<IFacilityDetailReturn, bool>> selectedPredicate = null)
        {
            Expression<Func<IFacilityDetailReturn, bool>> defaultSelectedPredicate = warehouse => false;

            return warehouses.ToSelectList(
                w => w.FacilityName,
                w => w.FacilityKey,
                selectedPredicate ?? defaultSelectedPredicate);
        }
    }
}