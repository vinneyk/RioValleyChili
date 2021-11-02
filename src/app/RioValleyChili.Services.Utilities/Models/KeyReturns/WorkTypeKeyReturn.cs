using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class WorkTypeKeyReturn : IWorkTypeKey
    {
        internal string WorkTypeKey { get { return new WorkTypeKey(this).KeyValue; } }

        public int WorkTypeKey_WorkTypeId { get; internal set; }
    }
}