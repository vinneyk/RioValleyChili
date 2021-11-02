// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class InventoryTransactionProjectors
    {
        internal static Expression<Func<InventoryTransaction, InventoryTransactionReturn>> Select(ILotUnitOfWork lotUnitOfWork)
        {
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var product = LotProjectors.SelectDerivedProduct();
            var location = LocationProjectors.SelectLocation();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();
            var packaging = ProductProjectors.SelectPackagingProduct();

            return Projector<InventoryTransaction>.To(t => new InventoryTransactionReturn
                {
                    EmployeeName = t.Employee.UserName,
                    TimeStamp = t.TimeStamp,

                    TransactionType = t.TransactionType,
                    SourceReference = t.SourceReference,
                    Quantity = t.Quantity,
                    Weight = t.Quantity * t.PackagingProduct.Weight,
                    ToteKey = t.ToteKey,

                    SourceLotVendorName = new [] { t.SourceLot.Vendor }.Where(v => v != null).Select(v => v.Name).FirstOrDefault(),
                    SourceLotPurchaseOrderNumber = t.SourceLot.PurchaseOrderNumber,
                    SourceLotShipperNumber = t.SourceLot.ShipperNumber,

                    SourceLotPackagingReceived = packaging.Invoke(t.SourceLot.ReceivedPackaging),
                    Product = product.Invoke(t.SourceLot),
                    Packaging = packaging.Invoke(t.PackagingProduct),
                    Location = location.Invoke(t.Location),
                    Treatment = treatment.Invoke(t.Treatment),

                    SourceLotKeyReturn = lotKey.Invoke(t.SourceLot),
                    DestinationLotKeyReturn = new[] { t.DestinationLot}.Where(l => l != null).Select(l => lotKey.Invoke(l)).FirstOrDefault()
                });
        }
    }
}
// ReSharper restore ConvertClosureToMethodGroup