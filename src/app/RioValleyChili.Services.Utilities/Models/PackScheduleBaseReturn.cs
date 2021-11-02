using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PackScheduleBaseReturn : IPackScheduleBaseReturn
    {
        public string PackScheduleKey { get { return PackScheduleKeyReturn == null ? null : PackScheduleKeyReturn.PackScheduleKey; } }
        public int? PSNum { get; internal set; }
        public string WorkType { get; internal set; }
        public string ProductionLineDescription { get; internal set; }
        internal PackScheduleKeyReturn PackScheduleKeyReturn { get; set; }
    }
}