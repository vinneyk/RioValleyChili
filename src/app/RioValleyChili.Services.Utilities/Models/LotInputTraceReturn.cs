using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotInputTraceReturn : ILotInputTraceReturn
    {
        public IEnumerable<string> LotPath { get; internal set; }
        public string Treatment { get; internal set; }
    }

    internal class LotInputTraceSelect
    {
        public LotKeyReturn LotKey { get; set; }
        public IEnumerable<LotInputTracePickedSelect> Inputs { get; set; }
    }

    internal class LotInputTracePickedSelect
    {
        public LotKeyReturn PickedLotKey { get; set; }
        public IEnumerable<string> PickedTreatments { get; set; }
    }
}