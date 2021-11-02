using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductBaseReturn : ProductKeyNameReturn, IProductReturn
    {
        public string ProductCode { get; internal set; }
        public bool IsActive { get; internal set; }

        public string ProductNameFull { get { return string.Format("{1} ({0})", ProductCode, ProductName); } }
        public string ProductCodeAndName { get { return string.Format("{0} - {1}", ProductCode, ProductName); } }
        public ProductTypeEnum ProductType { get; internal set; }
        public virtual LotTypeEnum LotType { get { return LotTypeEnum.Other; } }
    }
}