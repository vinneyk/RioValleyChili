using System;
using System.Collections.Generic;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionAdditiveInputs
    {
        public DateTime ProductionDate { get; set; }
        public string LotKey { get { return LotKeyReturn.LotKey; } }
        public ProductBaseReturn LotProduct { get; set; }
        public IEnumerable<ProductionPickedAdditive> PickedAdditiveItems { get; set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
    }

    internal class ProductionPickedAdditive
    {
        public string LotKey { get { return LotKeyReturn.LotKey; } }
        public double TotalPoundsPicked { get; set; }
        public string UserResultEntered { get; set; }

        public AdditiveProductReturn AdditiveProduct { get; set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
    }
}