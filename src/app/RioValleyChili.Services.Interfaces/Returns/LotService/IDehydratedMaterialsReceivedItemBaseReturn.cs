using System;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    [Obsolete("Kill it")]
    public interface IDehydratedMaterialsReceivedItemBaseReturn
    {
        string Variety { get; }
        string ToteKey { get; }
        string GrowerCode { get; }
    }
}