using System;
using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Client.Reporting.Models
{
    public class ChileMaterialsReceivedRecapReportModel
    {
        public string LotKey { get; set; }
        public DateTime DateReceived { get; set; }
        public string LoadNumber { get; set; }
        public string EmployeeName { get; set; }
        public string Supplier { get; set; }
        public string Product { get; set; }
        public string PurchaseOrder { get; set; }
        public string ShipperNumber { get; set; }

        public int Totes { get { return Items.Count(i => !string.IsNullOrWhiteSpace(i.Tote)); } }

        public IEnumerable<ChileMaterialsReceivedRecapItemReportModel> Items { get; set; }
    }

    public class ChileMaterialsReceivedRecapItemReportModel
    {
        public string Tote { get; set; }
        public int Quantity { get; set; }
        public string Packaging { get; set; }
        public double Weight { get; set; }
        public string Variety { get; set; }
        public string LocaleGrown { get; set; }
        public string Location { get; set; }

        public int ToteCount { get { return !string.IsNullOrWhiteSpace(Tote) ? 1 : 0; } }
    }
}