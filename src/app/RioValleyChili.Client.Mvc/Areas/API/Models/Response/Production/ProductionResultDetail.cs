using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production
{
    public class ProductionResultDetail 
    {
        public DateTime ProductionEndDate { get; set; }
        public string ProductionShiftKey { get; set; }
        public string ProductionResultKey { get; set; }
        public string ProductionBatchKey { get; set; }
        public string OutputLotNumber { get; set; }
        public string ProductionLineKey { get; set; }
        public FacilityLocationResponse ProductionLine { get; set; }
        public string ChileProductKey { get; set; }
        public DateTime DateTimeEntered { get; set; }
        public DateTime ProductionStartDate { get; set; }
        public string ChileProductName { get; set; }
        public double TargetBatchWeight { get; set; }
        public string WorkType { get; set; }
        public string BatchStatus { get; set; }
        public IEnumerable<ProductionResultItem> ResultItems { get; set; }

        public class ProductionResultItem 
        {
            public string ProductionResultItemKey { get; set; }
            public PackagingProductResponse PackagingProduct { get; set; }
            public FacilityLocationResponse WarehouseLocation { get; set; }
            public InventoryTreatmentResponse Treatment { get; set; }
            public int Quantity { get; set; }
        }
    }
}