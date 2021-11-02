using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryProductReturn : IInventoryProductReturn, IProductKey
    {
        public string ProductKey { get { return this.ToProductKey(); } }
        public string ProductName { get; internal set; }
        public ProductTypeEnum? ProductType { get; internal set; }
        public string ProductSubType { get; internal set; }
        public string ProductCode { get; internal set; }
        public bool IsActive { get; internal set; }
        public int? ProductId { get; internal set; }

        public int ProductKey_ProductId { get { return (int) ProductId; } }

        internal string ConstructCodeAndName()
        {
            return string.IsNullOrWhiteSpace(ProductCode) ? ProductName : string.Format("{0} - {1}", ProductCode, ProductName);
        }
    }
}