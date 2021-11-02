using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotOutputTraceReturn : ILotOutputTraceReturn
    {
        public IEnumerable<string> LotPath { get; internal set; }
        public IEnumerable<ILotOutputTraceInputReturn> Inputs { get; internal set; }
        public IEnumerable<ILotOutputTraceOrdersReturn> Orders { get; internal set; }
    }

    internal class LotOutputTraceInputReturn : ILotOutputTraceInputReturn
    {
        public string LotKey { get; internal set; }
        public string Treatment { get; internal set; }
    }

    internal class LotOutputTraceOrdersReturn : ILotOutputTraceOrdersReturn
    {
        public string Treatment { get; internal set; }
        public int? OrderNumber { get; internal set; }
        public DateTime? ShipmentDate { get; internal set; }
        public string CustomerName { get; internal set; }
    }

    internal class LotOutputTraceSelect
    {
        internal LotKeyReturn LotKey { get; set; }
        internal IEnumerable<LotOutputTraceProductionSelect> ProductionOutput { get; set; }
        internal IEnumerable<LotOutputTraceOrderSelect> OrderOutput { get; set; }
    }

    internal class LotOutputTraceProductionSelect
    {
        internal LotKeyReturn DestinationLot { get; set; }
        internal IEnumerable<string> PickedTreatments { get; set; }
    }

    internal class LotOutputTraceOrderSelect
    {
        internal int? MoveNum { get; set; }
        internal DateTime? ShipmentDate { get; set; }
        internal string CustomerName { get; set; }
        internal IEnumerable<string> PickedTreatments { get; set; }
    }
}