using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryTransactionReturn : IInventoryTransactionReturn
    {
        public string EmployeeName { get; internal set; }
        public DateTime TimeStamp { get; internal set; }
        public InventoryTransactionType TransactionType { get; internal set; }
        public string SourceReference { get; internal set; }
        public int Quantity { get; internal set; }
        public double Weight { get; internal set; }
        public string ToteKey { get; internal set; }

        public string SourceLotKey { get { return SourceLotKeyReturn.LotKey; } }
        public string SourceLotVendorName { get; internal set; }
        public string SourceLotPurchaseOrderNumber { get; internal set; }
        public string SourceLotShipperNumber { get; internal set; }

        public string DestinationLotKey { get { return DestinationLotKeyReturn != null ? DestinationLotKeyReturn.LotKey : null; } }
        public string FacilityLocationDescription { get { return LocationDescriptionHelper.ParseToDisplayString(Location.Description); } }

        public IPackagingProductReturn SourceLotPackagingReceived { get; internal set; }
        public IInventoryProductReturn Product { get; internal set; }
        public IPackagingProductReturn Packaging { get; internal set; }
        public ILocationReturn Location { get; internal set; }
        public IInventoryTreatmentReturn Treatment { get; internal set; }

        internal LotKeyReturn SourceLotKeyReturn { get; set; }
        internal LotKeyReturn DestinationLotKeyReturn { get; set; }
    }

    internal class InventoryTransactionsByLotReturn : IInventoryTransactionsByLotReturn
    {
        public IInventoryProductReturn Product { get; internal set; }
        public IEnumerable<IInventoryTransactionReturn> InputItems { get; internal set; }
    }

    internal class PickedForBatchTransactionReturn : IInventoryTransactionReturn
    {
        public string EmployeeName { get; internal set; }
        public DateTime TimeStamp { get; internal set; }
        public InventoryTransactionType TransactionType { get; internal set; }
        public string SourceReference { get { return PackScheduleKeyReturn.PackScheduleKey; } }
        public int Quantity { get; internal set; }
        public double Weight { get; internal set; }
        public string ToteKey { get; internal set; }

        public string SourceLotKey { get { return SourceLotKeyReturn.LotKey; } }
        public string SourceLotVendorName { get; internal set; }
        public string SourceLotPurchaseOrderNumber { get; internal set; }
        public string SourceLotShipperNumber { get; internal set; }

        public string DestinationLotKey { get { return DestinationLotKeyReturn != null ? DestinationLotKeyReturn.LotKey : null; } }
        public string FacilityLocationDescription { get { return LocationDescriptionHelper.ParseToDisplayString(Location.Description); } }

        public IPackagingProductReturn SourceLotPackagingReceived { get; internal set; }
        public IInventoryProductReturn Product { get; internal set; }
        public IPackagingProductReturn Packaging { get; internal set; }
        public ILocationReturn Location { get; internal set; }
        public IInventoryTreatmentReturn Treatment { get; internal set; }

        internal LotKeyReturn SourceLotKeyReturn { get; set; }
        internal LotKeyReturn DestinationLotKeyReturn { get; set; }
        internal PackScheduleKeyReturn PackScheduleKeyReturn { get; set; }
    }
}