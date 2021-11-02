using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class InventoryProductResponse : ProductResponse
    {
        public string ProductSubType { get; set; }
        public ProductTypeEnum? ProductType { get; set; }
        public string ProductNameFull {  get { return string.Format("{1} ({0})", ProductCode, ProductName); } }
        public string ProductCodeAndName { get { return string.Format("{0} - {1}", ProductCode, ProductName); } }
    }
}