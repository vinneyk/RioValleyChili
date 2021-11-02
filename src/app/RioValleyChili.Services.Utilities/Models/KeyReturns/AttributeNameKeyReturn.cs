using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class AttributeNameKeyReturn : IAttributeNameKey
    {
        internal string AttributeNameKey { get { return new AttributeNameKey(this).KeyValue; } }

        public string AttributeNameKey_ShortName { get; internal set; }
    }
}