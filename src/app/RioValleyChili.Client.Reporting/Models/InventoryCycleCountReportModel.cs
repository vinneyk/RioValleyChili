using System;
using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Client.Reporting.Models
{
    public class InventoryCycleCountReportModel
    {
        public string FacilityName { get; set; }
        public string GroupName { get; set; }
        public DateTime ReportDateTime { get; set; }

        public double TotalGroupWeight { get { return Locations.Select(l => l.LocationWeight).DefaultIfEmpty(0).Sum(); } }
        public IEnumerable<InventoryCycleCountLocation> Locations { get; set; }

        public void Initialize()
        {
            var emptyInventory = new List<InventoryCycleCount>();
            for(var i = 0; i < 10; ++i)
            {
                emptyInventory.Add(new InventoryCycleCount());
            }
            
            foreach(var location in Locations)
            {
                location.Header = this;
                location.FacilityName = FacilityName;
                location.GroupName = GroupName;
                location.ReportDateTime = ReportDateTime;

                location.Inventory = location.Inventory.Concat(emptyInventory).ToList();
            }
        }
    }

    public class InventoryCycleCountLocation
    {
        public InventoryCycleCountReportModel Header { get; set; }

        public string FacilityName { get; set; }
        public string GroupName { get; set; }
        public DateTime ReportDateTime { get; set; }

        public string Location { get; set; }
        public int LocationQuantity { get { return Inventory.Select(i => i.Quantity ?? 0).DefaultIfEmpty(0).Sum(); } }
        public double LocationWeight { get { return Inventory.Select(i => i.Weight).DefaultIfEmpty(0).Sum(); } }
        public IEnumerable<InventoryCycleCount> Inventory { get; set; }
    }

    public class InventoryCycleCount
    {
        public string LotKey { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Packaging { get; set; }
        public string Treatment { get; set; }
        public int? Quantity { get; set; }
        public double Weight { get; set; }
        public string ProductDisplay { get { return string.IsNullOrWhiteSpace(ProductCode) ? ProductName : string.Format("{0} - {1}", ProductCode, ProductName); } }
    }
}