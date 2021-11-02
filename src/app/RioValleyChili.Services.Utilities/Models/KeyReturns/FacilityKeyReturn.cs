using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class FacilityKeyReturn : IFacilityKey
    {
        internal string FacilityKey { get { return new FacilityKey(this).KeyValue; } }

        public int FacilityKey_Id { get; internal set; }
    }
}