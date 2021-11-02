using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryShipmentOrderItemAnalysisReturn : IInventoryShipmentOrderItemAnalysisReturn
    {
        public string LotKey { get { return LotKeyReturn.LotKey; } }
        public DateTime? ProductionDate { get; internal set; }
        public bool? LoBac { get; internal set; }
        public string Notes { get; internal set; }

        public IInventoryProductReturn LotProduct { get; internal set; }
        public IInventoryTreatmentReturn TreatmentReturn { get; internal set; }
        public IEnumerable<ILotAttributeReturn> Attributes { get; internal set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
    }
}