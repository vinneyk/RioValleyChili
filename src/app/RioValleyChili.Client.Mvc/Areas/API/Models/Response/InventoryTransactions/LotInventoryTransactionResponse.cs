using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.InventoryTransactions
{
    public class LotInventoryTransactionResponse
    {
        public string EmployeeName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string TransactionSourceReferenceKey { get; set; }

        public string ProductName { get; set; }

        public string SourceLotKey { get; set; }
        public string SourceLotVendorName { get; set; }
        public string SourceLotPurchaseOrderNumber { get; set; }
        public string SourceLotShipperNumber { get; set; }

        public string DestinationLotKey { get; set; }
        public string Treatment { get; set; }
        public string ToteKey { get; set; }
        public string FacilityName { get; set; }
        public string FacilityLocationDescription { get; set; }

        public int Quantity { get; set; }
        public double Weight { get; set; }
    }

    public class ReceivedInventoryDetails
    {
        public string EnteredByUser { get; set; }

        public string LotKey { get; set; }

        public string ProductName { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string ShipperNumber { get; set; }

        public string VendorName { get; set; }

        public DateTime DateEntered { get; set; }

        public IEnumerable<ReceivedInventoryItem> InventoryItems { get; set; }
        public string PackagingReceived { get; set; }
    }

    public class ReceivedInventoryItem
    {
        public string Location { get; set; }

        public string FacilityName { get; set; }

        public string Treatment { get; set; }

        public string InventoryUnits { get; set; }
        
        public double Weight { get; set; }

        public int Quantity { get; set; }
    }
}