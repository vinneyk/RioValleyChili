using System;
using System.Linq.Expressions;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ChileMaterialsReceivedItemProjectors
    {
        internal static Expression<Func<ChileMaterialsReceivedItem, ChileMaterialsReceivedItemKeyReturn>> SelectKey()
        {
            var lotKey = LotProjectors.SelectLotKey<ChileMaterialsReceivedItem>();
            return lotKey.Merge(i => new ChileMaterialsReceivedItemKeyReturn
                {
                    ChileMaterialsReceivedKey_ItemSequence = i.ItemSequence
                });
        }

        internal static Expression<Func<ChileMaterialsReceivedItem, ChileMaterialsReceivedItemReturn>> Select()
        {
            var key = SelectKey();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();
            var location = LocationProjectors.SelectLocation();

            return SelectBase().Merge(i => new ChileMaterialsReceivedItemReturn
                {
                    ChileMaterialsReceivedItemKeyReturn = key.Invoke(i),
                    Variety = i.ChileVariety,
                    Quantity = i.Quantity,
                    TotalWeight = (int) (i.Quantity * i.PackagingProduct.Weight),
                    PackagingProduct = packagingProduct.Invoke(i.PackagingProduct),
                    Location = location.Invoke(i.Location)
                });
        }

        internal static Expression<Func<ChileMaterialsReceivedItem, DehydratedInputReturn>> SelectInput()
        {
            var lotKey = LotProjectors.SelectLotKey<ChileMaterialsReceivedItem>();

            return SelectBase().Merge(i => new DehydratedInputReturn
                {
                    LotKeyReturn = lotKey.Invoke(i),
                    DehydratorName = i.ChileMaterialsReceived.Supplier.Name
                });
        }

        internal static Expression<Func<ChileMaterialsReceivedItem, DehydratedMaterialsReceivedItemBaseReturn>> SelectBase()
        {
            return i => new DehydratedMaterialsReceivedItemBaseReturn
                {
                    Variety = i.ChileVariety,
                    ToteKey = i.ToteKey,
                    GrowerCode = i.GrowerCode
                };
        }
    }
}