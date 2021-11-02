using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class AdditiveTypeKeyReturn : IAdditiveTypeKey
    {
        internal string AdditiveTypeKey { get { return new AdditiveTypeKey(this).KeyValue; } }

        public int AdditiveTypeKey_AdditiveTypeId { get; internal set; }
    }
}