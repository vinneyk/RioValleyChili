using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class DehydratedInputReturn : DehydratedMaterialsReceivedItemBaseReturn, IDehydratedInputReturn
    {
        public string LotKey { get { return LotKeyReturn.LotKey; } }

        public string DehydratorName { get; internal set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
    }
}