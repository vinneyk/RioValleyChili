using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderItemReturn : ISampleOrderItemReturn
    {
        public string ItemKey { get { return SampleOrderItemKeyReturn.SampleOrderItemKey; } }
        public string LotKey { get { return LotKeyReturn == null ? null : LotKeyReturn.LotKey; } }
        public string ProductKey { get { return ProductKeyReturn == null ? null : ProductKeyReturn.ProductKey; } }
        public ProductTypeEnum? ProductType { get; internal set; }

        public string CustomerProductName { get; internal set; }
        public int Quantity { get; internal set; }
        public string Description { get; internal set; }

        public ISampleOrderItemSpecReturn CustomerSpec { get; internal set; }
        public ISampleOrderItemMatchReturn LabResults { get; internal set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
        internal ProductKeyReturn ProductKeyReturn { get; set; }
        internal SampleOrderItemKeyReturn SampleOrderItemKeyReturn { get; set; }
    }
}