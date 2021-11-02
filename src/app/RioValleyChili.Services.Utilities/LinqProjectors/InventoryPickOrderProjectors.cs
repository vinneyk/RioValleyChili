// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class InventoryPickOrderProjectors
    {
        internal static Expression<Func<InventoryPickOrder, InventoryPickOrderKeyReturn>> SelectKey()
        {
            return o => new InventoryPickOrderKeyReturn
                {
                    InventoryPickOrderKey_DateCreated = o.DateCreated,
                    InventoryPickOrderKey_Sequence = o.Sequence
                };
        }

        internal static Expression<Func<InventoryPickOrder, InventoryPickOrderReturn>> SelectSummary()
        {
            var key = SelectKey();

            return Projector<InventoryPickOrder>.To(o => new InventoryPickOrderReturn
                {
                    InventoryPickOrderKeyReturn = key.Invoke(o),
                    TotalQuantity = o.Items.Any() ? o.Items.Sum(i => i.Quantity) : 0,
                    TotalWeight = o.Items.Any() ? o.Items.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0.0
                });
        }

        internal static IEnumerable<Expression<Func<InventoryPickOrder, InventoryPickOrderReturn>>> SplitSelectDetails()
        {
            var key = SelectKey();

            return new Projectors<InventoryPickOrder, InventoryPickOrderReturn>
                {
                    o => new InventoryPickOrderReturn
                        {
                            InventoryPickOrderKeyReturn = key.Invoke(o)
                        },
                    { InventoryPickOrderItemProjectors.SplitSelect(), s => o => new InventoryPickOrderReturn
                        {
                            PickOrderItems = o.Items.Select(i => s.Invoke(i))
                        } }
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup