namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface ISalesOrderItem
    {
        string ContractItemKey { get; }
        string ProductKey { get; }
        string PackagingKey { get; }
        string TreatmentKey { get; }

        int Quantity { get; }
        string CustomerLotCode { get; }
        string CustomerProductCode { get; }

        double PriceBase { get; }
        double PriceFreight { get; }
        double PriceTreatment { get; }
        double PriceWarehouse { get; }
        double PriceRebate { get; }
    }
}