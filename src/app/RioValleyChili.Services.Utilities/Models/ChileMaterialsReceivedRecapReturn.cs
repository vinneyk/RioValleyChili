using System;
using System.Collections.Generic;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ChileMaterialsReceivedRecapReturn : IChileMaterialsReceivedRecapReturn
    {
        public string LotKey { get { return LotKeyReturn.LotKey; } }
        public DateTime DateReceived { get; internal set; }
        public string LoadNumber { get; internal set; }
        public string EmployeeName { get; internal set; }
        public string Supplier { get; internal set; }
        public string Product { get; internal set; }
        public string PurchaseOrder { get; internal set; }
        public string ShipperNumber { get; internal set; }

        public IEnumerable<IChileMaterialsReceivedRecapItemReturn> Items { get; internal set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
    }

    internal class ChileMaterialsReceivedRecapItemReturn : IChileMaterialsReceivedRecapItemReturn
    {
        public string Tote { get; internal set; }
        public int Quantity { get; internal set; }
        public string Packaging { get; internal set; }
        public double Weight { get; internal set; }
        public string Variety { get; internal set; }
        public string LocaleGrown { get; internal set; }
        public string Location { get { return LocationDescriptionHelper.ParseToDisplayString(LocationDescription); } }

        internal string LocationDescription { get; set; }
    }
}