using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class InventoryShipmentOrderCertificateOfAnalysis
    {
        public string ReportDestinationName { get { return !string.IsNullOrWhiteSpace(DestinationName) ? DestinationName : "Rio Valley Chili, Incorporated"; } }
        public string ReportMoveNum
        {
            get
            {
                return MovementNumber == null ? "" : OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ?
                    MovementNumber.Value.ToString() :
                    MovementNumber.Value.ToString("0000-000");
            }
        }

        public string DestinationName { get; set; }
        public string OrderKey { get; set; }
        public int? MovementNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public InventoryShipmentOrderTypeEnum OrderType { get; set; }
        public IEnumerable<InventoryShipmentOrderItemAnalysisReturn> Items { get; set; }

        public string OrderTypeString
        {
            get
            {
                switch(OrderType)
                {
                    case InventoryShipmentOrderTypeEnum.TreatmentOrder: return "Inventory Treatment Order";
                    case InventoryShipmentOrderTypeEnum.SalesOrder: return "Customer Order";
                    default: return "Inventory Transfer to other Warehouses";
                }
            }

        }

        public IEnumerable<ItemGroup> GroupedItems
        {
            get
            {
                return Items.GroupBy(m => m.LotProduct.ProductKey)
                    .Select(g => new ItemGroup
                        {
                            ProductCode = g.First().LotProduct.ProductCode,
                            FullProductName = g.First().LotProduct.FullProductName,
                            Items = g
                        });
            }
        }

        public class ItemGroup
        {
            public string ProductCode { get; set; }
            public string FullProductName { get; set; }
            public IEnumerable<InventoryShipmentOrderItemAnalysisReturn> Items { get; set; }
        }
    }

    public class InventoryShipmentOrderItemAnalysisReturn
    {
        public string LotKey { get; set; }
        public DateTime? ProductionDate { get; set; }
        public bool? LoBac { get; set; }
        public InventoryProductReturn LotProduct { get; set; }
        public InventoryTreatmentReturn TreatmentReturn { get; set; }
        public IEnumerable<LotAttributeReturn> Attributes { get; set; }
        public string Notes { get; set; }

        public double? Scan { get { return GetAttributeValue(Constants.ChileAttributeKeys.Scan); } }
        public double? Asta { get { return GetAttributeValue(Constants.ChileAttributeKeys.Asta); } }
        public double? H2O { get { return GetAttributeValue(Constants.ChileAttributeKeys.H2O); } }
        public double? Scov { get { return GetAttributeValue(Constants.ChileAttributeKeys.Scov); } }
        public double? Gluten { get { return GetAttributeValue(Constants.ChileAttributeKeys.Gluten); } }
        public double? TPC { get { return GetAttributeValue(Constants.ChileAttributeKeys.TPC); } }
        public double? Yeast { get { return GetAttributeValue(Constants.ChileAttributeKeys.Yeast); } }
        public double? ColiF { get { return GetAttributeValue(Constants.ChileAttributeKeys.ColiF); } }
        public double? EColi { get { return GetAttributeValue(Constants.ChileAttributeKeys.EColi); } }
        public double? Sal { get { return GetAttributeValue(Constants.ChileAttributeKeys.Sal); } }
        public double? AToxin { get { return GetAttributeValue(Constants.ChileAttributeKeys.AToxin); } }
        public double? InsP { get { return GetAttributeValue(Constants.ChileAttributeKeys.InsP); } }
        public double? RodHrs { get { return GetAttributeValue(Constants.ChileAttributeKeys.RodHrs); } }
        public double? Mold { get { return GetAttributeValue(Constants.ChileAttributeKeys.Mold); } }
        public double? Ash { get { return GetAttributeValue(Constants.ChileAttributeKeys.Ash); } }
        public double? AIA { get { return GetAttributeValue(Constants.ChileAttributeKeys.AIA); } }
        public double? BI { get { return GetAttributeValue(Constants.ChileAttributeKeys.BI); } }
        
        public string LoBacString { get { return (LoBac ?? false) ? "x" : ""; } }
        
        private double? GetAttributeValue(string key)
        {
            double value;
            return LotAttributes.TryGetValue(key, out value) ? value : (double?)null;
        }

        private Dictionary<string, double> LotAttributes
        {
            get { return _lotAttributes ?? (_lotAttributes = Attributes.ToDictionary(a => a.Key, a => a.Value)); }
        }
        private Dictionary<string, double> _lotAttributes;
    }

    public class LotAttributeReturn
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public DateTime AttributeDate { get; set; }
        public bool Computed { get; set; }
    }

    public class NoteReturn
    {
        public string NoteKey { get; set; }
        public DateTime NoteDate { get; set; }
        public string CreatedByUser { get; set; }
        public int Sequence { get; set; }
        public string Text { get; set; }
    }
}