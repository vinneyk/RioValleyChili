// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class IntraWarehouseOrderProjectors
    {
        internal static Expression<Func<IntraWarehouseOrder, IntraWarehouseOrderKeyReturn>> SelectIntraWarehouserOrderKey()
        {
            return o => new IntraWarehouseOrderKeyReturn
            {
                IntraWarehouseOrderKey_DateCreated = o.DateCreated,
                IntraWarehouseOrderKey_Sequence = o.Sequence
            };
        }

        internal static Expression<Func<IntraWarehouseOrder, IntraWarehouseOrderReturn>> SelectSummary()
        {
            var pickedInventory = PickedInventoryProjectors.SelectSummary();
            return SelectBase().Merge(o => new IntraWarehouseOrderReturn
                {
                    PickedInventorySummary = pickedInventory.Invoke(o.PickedInventory)
                });
        }

        internal static IEnumerable<Expression<Func<IntraWarehouseOrder, IntraWarehouseOrderReturn>>> SplitSelectDetail(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            var pickedInventory = PickedInventoryProjectors.SplitSelectDetail(inventoryUnitOfWork, currentDate);

            return new Projectors<IntraWarehouseOrder, IntraWarehouseOrderReturn>
                {
                    SelectBase().Merge(o => new IntraWarehouseOrderReturn
                    {
                        MovementDate = o.MovementDate
                    }),
                    { 
                        pickedInventory, s => o => new IntraWarehouseOrderReturn
                        {
                            PickedInventoryDetail = s.Invoke(o.PickedInventory),
                        } 
                    }
                };
        }

        private static Expression<Func<IntraWarehouseOrder, IntraWarehouseOrderReturn>> SelectBase()
        {
            var intraWarehouserOrderKey = SelectIntraWarehouserOrderKey();

            return o => new IntraWarehouseOrderReturn
            {
                IntraWarehouseOrderKeyReturn = intraWarehouserOrderKey.Invoke(o),

                TrackingSheetNumber = o.TrackingSheetNumber,
                OperatorName = o.OperatorName,
                DateCreated = o.MovementDate,
            };
        }
    }
}

// ReSharper enable ConvertClosureToMethodGroup