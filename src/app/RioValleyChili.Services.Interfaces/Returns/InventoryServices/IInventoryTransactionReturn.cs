using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IInventoryTransactionReturn
    {
        string EmployeeName { get; }
        DateTime TimeStamp { get; }
        InventoryTransactionType TransactionType { get; }
        string SourceReference { get; }
        int Quantity { get; }
        double Weight { get; }
        string ToteKey { get; }

        string SourceLotKey { get; }
        string SourceLotVendorName { get; }
        string SourceLotPurchaseOrderNumber { get; }
        string SourceLotShipperNumber { get; }

        string DestinationLotKey { get; }
        string FacilityLocationDescription { get; }

        IPackagingProductReturn SourceLotPackagingReceived { get; }
        IInventoryProductReturn Product { get; }
        IPackagingProductReturn Packaging { get; }
        ILocationReturn Location { get; }
        IInventoryTreatmentReturn Treatment { get; }
    }

    public interface IInventoryTransactionsByLotReturn
    {
        IInventoryProductReturn Product { get; }
        IEnumerable<IInventoryTransactionReturn> InputItems { get; }
    }
}