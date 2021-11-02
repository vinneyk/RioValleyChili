using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class ProductionScheduleReportModel
    {
        public int Line
        {
            get
            {
                string street;
                int row;
                return LocationDescriptionHelper.GetStreetRow(ProductionLocation ?? "", out street, out row) ? row : -1;
            }
        }

        public DateTime Timestamp { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ProductionLocation { get; set; }
        public string DisplayProductionLocation { get { return (LocationDescriptionHelper.ParseToProductionLine(ProductionLocation) ?? "").ToUpper(); } }
        public string DisplayTimestamp { get { return string.Format("Created on {0}", Timestamp.ConvertUTCToLocal()); } }

        public IEnumerable<ProductionScheduleItemReportModel> ScheduledItems { get; set; }
    }

    public class ProductionScheduleItemReportModel
    {
        public IEnumerable<ProductionScheduleItemReportModel> This { get { return new List<ProductionScheduleItemReportModel> { this }; } }

        public string DisplayLine { get { return _parent.DisplayProductionLocation; } }

        public void Initialize(ProductionScheduleReportModel parent)
        {
            _parent = parent;
        }

        public bool FlushBefore { get; set; }
        public string FlushBeforeInstructions { get; set; }
        public string DisplayFlushBefore
        {
            get
            {
                if(FlushBefore)
                {
                    return string.IsNullOrWhiteSpace(FlushBeforeInstructions) ? "Flush Before" : FlushBeforeInstructions;
                }
                return null;
            }
        }

        public string PackScheduleKey { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string WorkType { get; set; }
        public IEnumerable<ProductionScheduleBatchReportModel> ProductionBatches { get; set; }

        public double? Granularity { get; set; }
        public double? Scan { get; set; }
        public string Instructions { get; set; }
        public DateTime? ProductionDeadline { get; set; }
        public string OrderNumber { get; set; }
        public IEnumerable<ProductionScheduleItemDetailReportModel> Details
        {
            get
            {
                return new List<ProductionScheduleItemDetailReportModel>
                    {
                        new ProductionScheduleItemDetailReportModel
                            {
                                Name = "Granulation:",
                                Value = Granularity == null ? "" : Granularity.ToString()
                            },
                        new ProductionScheduleItemDetailReportModel
                            {
                                Name = "Scan:",
                                Value = Scan == null ? "" : Scan.ToString()
                            },
                        new ProductionScheduleItemDetailReportModel
                            {
                                Name = "Instructions:",
                                Value = Instructions
                            },
                        new ProductionScheduleItemDetailReportModel
                            {
                                Name = "Date Due:",
                                Value = ProductionDeadline == null ? "" : ProductionDeadline.Value.ToString("dd-MMM")
                            },
                        new ProductionScheduleItemDetailReportModel
                            {
                                Name = "Order No.:",
                                Value = OrderNumber
                            }
                    };
            }
        }

        public int BatchCount { get { return ProductionBatches.Count(); } }

        public string PackagingProduct { get; set; }

        public bool FlushAfter { get; set; }
        public string FlushAfterInstructions { get; set; }
        public string DisplayFlushAfter
        {
            get
            {
                if(FlushAfter)
                {
                    return string.IsNullOrWhiteSpace(FlushAfterInstructions) ? "Flush After" : FlushAfterInstructions;
                }
                return null;
            }
        }

        private ProductionScheduleReportModel _parent;
    }

    public class ProductionScheduleBatchReportModel
    {
        public string LotNumber { get; set; }
    }

    public class ProductionScheduleItemDetailReportModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}