using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryProductCommonReturn : IProductKey
    {
        public string ProductKey { get { return new ProductKey(this).KeyValue; } }

        public string ProductName { get; internal set; }

        public ProductTypeEnum? ProductType { get; internal set; }

        public int? ProductId { get; internal set; }

        public int ProductKey_ProductId { get { return (int) ProductId; } }
    }
}