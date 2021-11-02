using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class AdditiveTypeReturn : IAdditiveTypeReturn
    {
        public string AdditiveTypeKey { get { return AdditiveTypeKeyReturn.AdditiveTypeKey; } }

        public string AdditiveTypeDescription { get; internal set; }

        internal AdditiveTypeKeyReturn AdditiveTypeKeyReturn { get; set; }
    }
}