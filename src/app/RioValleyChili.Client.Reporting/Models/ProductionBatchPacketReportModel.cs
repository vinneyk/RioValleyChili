using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Reporting.Models
{
    public class ProductionBatchPacketReportModel
    {
        public ProductionBatchPacketReportHeader Header { get; set; }
        public IEnumerable<KeyValuePair<string, IEnumerable<PickedChileInventoryItem>>> PickedChileInventoryItemsByChileType { get; set; }
        public IEnumerable<KeyValuePair<string, IEnumerable<PickedAdditiveInventoryItem>>> PickedAdditiveInventoryItemsByAdditiveType { get; set; }
        public IEnumerable<PickedPackagingInventoryItem> PickedPackagingInventoryItems { get; set; }

        public IEnumerable<KeyValuePair<int, string>> BatchInstructions { get; set; }
        public IEnumerable<ExpectedOutput> ExpectedOutputs { get; set; }

        public class ExpectedOutput
        {
            public string PackagingName { get; set; }
        }

        public class ProductionBatchPacketReportHeader
        {
            public string BatchType { get; set; }
            public string LotNumber { get; set; }
            public int PSNum { get; set; }
            public string PackScheduleKey { get; set; }
            public DateTime PackScheduleDate { get; set; }
            public string ProductNameDisplay { get; set; }
            public string Description { get; set; }

            public double TargetWeight { get; set; }
            public double TargetAsta { get; set; }
            public double TargetScoville { get; set; }
            public double TargetScan { get; set; }

            public double CalculatedAsta { get; set; }
            public double CalculatedScoville { get; set; }
            public double CalculatedScan { get; set; }

            public string BatchNotes { get; set; }
        }

        internal class ProductionBatchPacketReportBody
        {
            public IEnumerable<KeyValuePair<string, IEnumerable<PickedChileInventoryItem>>> PickedChileInventoryItemsByChileType { get; set; }
            public IEnumerable<KeyValuePair<string, PickedAdditiveInventoryItem>> PickedAdditiveInventoryItemsByAdditiveType { get; set; }
            public IEnumerable<PickedPackagingInventoryItem> PickedPackagingInventoryItems { get; set; }

            public IEnumerable<string> BatchInstructions { get; set; }
            public IEnumerable<string> ExpectedOutputs { get; set; }
        }

        public abstract class PickedInventoryItemBase
        {
            public string LotNumber { get; set; }
            public string ProductNameDisplay { get; set; }
            public int Quantity { get; set; }
            public string PickedFromWarehouseLocationName { get; set; }
        }

        public class PickedChileInventoryItem : PickedInventoryItemBase, IPickedPounds
        {
            public string PackagingName { get; set; }
            public string Treatment { get; set; }
            public double WeightPicked { get; set; }
            public string LotStatus { get; set; }
        }

        public class PickedAdditiveInventoryItem : PickedInventoryItemBase, IPickedPounds
        {
            public string PackagingName { get; set; }
            public double WeightPicked { get; set; }
        }

        public class PickedPackagingInventoryItem : PickedInventoryItemBase { }
    }

    public interface IPickedPounds
    {
        double WeightPicked { get; }
    }
}
