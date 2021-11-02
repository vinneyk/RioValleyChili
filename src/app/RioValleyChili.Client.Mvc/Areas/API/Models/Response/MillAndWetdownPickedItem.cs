using System;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class MillAndWetdownPickedItem 
    {
        public string PickedInventoryItemKey { get; set; }
        public string InventoryKey { get; set; }
        public string LotKey { get; set; }
        public DateTime LotDateCreated { get; set; }
        public string ToteKey { get; set; }
        public int QuantityPicked { get; set; }
        public int TotalWeightPicked { get; set; }
        public InventoryProductResponse Product { get; set; }
        public PackagingProductResponse PackagingProduct { get; set; }
        public FacilityLocationResponse Location { get; set; }
    }
}