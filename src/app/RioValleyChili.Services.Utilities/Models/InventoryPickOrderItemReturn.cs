using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryPickOrderItemReturn : IPickOrderItemReturn
    {
        public string OrderItemKey { get { return InventoryPickOrderItemKeyReturn.InventoryPickOrderItemKey; } }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string PackagingName { get; set; }
        public double PackagingWeight { get; set; }
        public string TreatmentNameShort { get; set; }
        public int Quantity { get; set; }
        public double TotalWeight { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }
        public string ProductKey { get { return ProductKeyReturn.ProductKey; } }
        public string PackagingProductKey { get { return PackagingProductKeyReturn.ProductKey; } }
        public string TreatmentKey { get { return InventoryTreatmentKeyReturn == null ? "" : InventoryTreatmentKeyReturn.InventoryTreatmentKey; } }
        public ICompanyHeaderReturn Customer { get; internal set; }

        internal InventoryPickOrderItemKeyReturn InventoryPickOrderItemKeyReturn { get; set; }
        internal ProductKeyReturn ProductKeyReturn { get; set; }
        internal ProductKeyReturn PackagingProductKeyReturn { get; set; }
        internal InventoryTreatmentKeyReturn InventoryTreatmentKeyReturn { get; set; }
    }
}