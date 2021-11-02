using RioValleyChili.Services.Interfaces.Returns.ProductionService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class WorkTypeReturn : IWorkTypeReturn
    {
        public string Description { get; set; }
        public string WorkTypeKey { get { return WorkTypeKeyReturn.WorkTypeKey; } }

        internal WorkTypeKeyReturn WorkTypeKeyReturn { get; set; }
    }
}