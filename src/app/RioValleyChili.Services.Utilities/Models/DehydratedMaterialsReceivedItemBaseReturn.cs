using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class DehydratedMaterialsReceivedItemBaseReturn : IDehydratedMaterialsReceivedItemBaseReturn
    {
        public string Dehydrator { get; internal set; }
        public string Variety { get; internal set; }
        public string ToteKey { get; internal set; }
        public string GrowerCode { get; internal set; }
    }
}