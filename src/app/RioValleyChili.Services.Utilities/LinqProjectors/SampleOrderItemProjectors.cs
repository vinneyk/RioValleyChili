// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SampleOrderItemProjectors
    {
        internal static Expression<Func<SampleOrderItem, SampleOrderItemKeyReturn>> SelectKey()
        {
            return i => new SampleOrderItemKeyReturn
                {
                    SampleOrderKey_Year = i.SampleOrderYear,
                    SampleOrderKey_Sequence = i.SampleOrderSequence,
                    SampleOrderItemKey_Sequence = i.ItemSequence
                };
        }

        internal static Expression<Func<SampleOrderItem, SampleOrderItemReturn>> Select()
        {
            var key = SelectKey();
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var productKey = ProductProjectors.SelectProductKey();
            var spec = SampleOrderItemSpecProjectors.Select();
            var match = SampleOrderItemMatchProjectors.Select();

            return i => new SampleOrderItemReturn
                {
                    CustomerProductName = i.CustomerProductName,
                    Quantity = i.Quantity,
                    Description = i.Description,

                    CustomerSpec = new[]{ i.Spec}.Where(s => s != null).Select(s => spec.Invoke(s)).FirstOrDefault(),
                    LabResults = new[] { i.Match }.Where(m => m != null).Select(m => match.Invoke(m)).FirstOrDefault(),

                    SampleOrderItemKeyReturn = key.Invoke(i),
                    LotKeyReturn = new[] { i.Lot }.Where(l => l != null).Select(l => lotKey.Invoke(l)).FirstOrDefault(),
                    ProductKeyReturn = new[] { i.Product }.Where(p => p != null).Select(p => productKey.Invoke(p)).FirstOrDefault(),
                    ProductType = new[] { i.Product }.Where(p => p != null).Select(p => (ProductTypeEnum?)p.ProductType).FirstOrDefault()
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup