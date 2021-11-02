using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISampleOrderItemReturn
    {
        string ItemKey { get; }
        string CustomerProductName { get; }
        string LotKey { get; }
        string ProductKey { get; }
        ProductTypeEnum? ProductType { get; }
        int Quantity { get; }
        string Description { get; }

        ISampleOrderItemSpecReturn CustomerSpec { get; }
        ISampleOrderItemMatchReturn LabResults { get; }
    }
}