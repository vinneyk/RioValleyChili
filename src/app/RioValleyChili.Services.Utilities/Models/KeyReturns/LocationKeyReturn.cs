using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class LocationKeyReturn : ILocationKey
    {
        internal string LocationKey { get { return new LocationKey(this).KeyValue; } }

        public int LocationKey_Id { get; internal set; }
    }
}