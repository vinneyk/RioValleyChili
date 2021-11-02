using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class InventoryTreatmentProjectors
    {
        internal static Expression<Func<InventoryTreatment, InventoryTreatmentKeyReturn>> SelectInventoryTreatmentKey()
        {
            return t => new InventoryTreatmentKeyReturn
            {
                InventoryTreatmentKey_Id = t.Id
            };
        }

        internal static Expression<Func<InventoryTreatment, InventoryTreatmentReturn>>  SelectInventoryTreatment()
        {
            var inventoryTreatmentKey = SelectInventoryTreatmentKey();

            return t => new InventoryTreatmentReturn
                {
                    InventoryTreatmentKeyReturn = inventoryTreatmentKey.Invoke(t),

                    TreatmentName = t.LongName,
                    TreatmentNameShort = t.ShortName
                };
        }
    }
}