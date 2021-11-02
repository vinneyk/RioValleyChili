using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ChileTypeSummaryReturn : IChileTypeSummaryReturn
    {
        public virtual string ChileTypeKey { get { return ChileTypeKeyReturn.ChileTypeKey; } }

        public virtual string ChileTypeDescription { get; set; }

        internal ChileTypeKeyReturn ChileTypeKeyReturn { get; set; }
    }
}
