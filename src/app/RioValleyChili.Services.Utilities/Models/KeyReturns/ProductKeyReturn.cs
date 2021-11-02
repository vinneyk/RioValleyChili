using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ProductKeyReturn : IProductKey
    {
        internal string ProductKey { get { return new ProductKey(this); } }

        public int ProductKey_ProductId { get; internal set; }
    }
}