using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductKeyNameReturn : IProductKeyNameReturn
    {
        public string ProductKey { get { return ProductKeyReturn.ProductKey; } }
        public string ProductName { get; internal set; }

        internal ProductKeyReturn ProductKeyReturn { get; set; }
    }
}