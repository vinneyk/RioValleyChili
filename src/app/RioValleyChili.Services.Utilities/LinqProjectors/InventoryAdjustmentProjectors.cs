// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class InventoryAdjustmentProjectors
    {
        internal static Expression<Func<InventoryAdjustment, InventoryAdjustmentKeyReturn>> SelectKey()
        {
            return a => new InventoryAdjustmentKeyReturn
                {
                    InventoryAdjustmentKey_AdjustmentDate = a.AdjustmentDate,
                    InventoryAdjustmentKey_Sequence = a.Sequence
                };
        }

        internal static IEnumerable<Expression<Func<InventoryAdjustment, InventoryAdjustmentReturn>>> SplitSelect(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }

            var key = SelectKey();
            var notebook = NotebookProjectors.Select();
            var selectItem = InventoryAdjustmentItemProjectors.Select(lotUnitOfWork);

            return new[]
                {
                    Projector<InventoryAdjustment>.To(a => new InventoryAdjustmentReturn
                        {
                            InventoryAdjustmentKeyReturn = key.Invoke(a),

                            AdjustmentDate = a.AdjustmentDate,
                            User = a.Employee.UserName,
                            TimeStamp = a.TimeStamp,

                            Notebook = notebook.Invoke(a.Notebook)
                        }),
                    Projector<InventoryAdjustment>.To(a => new InventoryAdjustmentReturn
                        {
                            Items = a.Items.Select(i => selectItem.Invoke(i))
                        })
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup