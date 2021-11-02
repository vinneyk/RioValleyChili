// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ChileMaterialsReceivedProjectors
    {
        internal static Expression<Func<ChileMaterialsReceived, ChileMaterialsReceivedReturn>> SelectSummary()
        {
            return SelectBase().Merge(d => new ChileMaterialsReceivedReturn
                {
                    TotalLoad = d.Items.Any() ? d.Items.Sum(i => (int)(i.Quantity * i.PackagingProduct.Weight)) : 0
                });
        }

        internal static Expression<Func<ChileMaterialsReceived, ChileMaterialsReceivedReturn>> SelectDetail()
        {
            var selectItem = ChileMaterialsReceivedItemProjectors.Select();

            return SelectBase().Merge(m => new ChileMaterialsReceivedReturn
                {
                    ChileMaterialsReceivedType = m.ChileMaterialsReceivedType,
                    Items = m.Items.Select(i => selectItem.Invoke(i))
                });
        }

        private static Expression<Func<ChileMaterialsReceived, ChileMaterialsReceivedReturn>> SelectBase()
        {
            var key = LotProjectors.SelectLotKey<ChileMaterialsReceived>();
            var chileProduct = ProductProjectors.SelectChileProductSummary();
            var company = CompanyProjectors.SelectSummary();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return Projector<ChileMaterialsReceived>.To(m => new ChileMaterialsReceivedReturn
                {
                    DateReceived = m.DateReceived,
                    LoadNumber = m.LoadNumber,
                    PurchaseOrder = m.ChileLot.Lot.PurchaseOrderNumber,
                    ShipperNumber = m.ChileLot.Lot.ShipperNumber,

                    ChileProduct = chileProduct.Invoke(m.ChileProduct),
                    Supplier = company.Invoke(m.Supplier),
                    Treatment = treatment.Invoke(m.InventoryTreatment),

                    LotKeyReturn = key.Invoke(m),
                });
        }

        public static Expression<Func<ChileMaterialsReceived, ChileMaterialsReceivedRecapReturn>> SelectRecapReport()
        {
            var lotKey = LotProjectors.SelectLotKey<ChileMaterialsReceived>();

            return Projector<ChileMaterialsReceived>.To(m => new ChileMaterialsReceivedRecapReturn
                {
                    LotKeyReturn = lotKey.Invoke(m),
                    DateReceived = m.DateReceived,
                    LoadNumber = m.LoadNumber,
                    EmployeeName = m.Employee.UserName,
                    Supplier = m.Supplier.Name,
                    Product = m.ChileProduct.Product.Name,
                    PurchaseOrder = m.ChileLot.Lot.PurchaseOrderNumber,
                    ShipperNumber = m.ChileLot.Lot.ShipperNumber,

                    Items = m.Items
                        .OrderBy(i => i.ItemSequence)
                        .Select(i => new ChileMaterialsReceivedRecapItemReturn
                            {
                                Tote = i.ToteKey,
                                Quantity = i.Quantity,
                                Packaging = i.PackagingProduct.Product.Name,
                                Weight = i.PackagingProduct.Weight * i.Quantity,
                                Variety = i.ChileVariety,
                                LocaleGrown = i.GrowerCode,
                                LocationDescription = i.Location.Description
                            })
                });
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup