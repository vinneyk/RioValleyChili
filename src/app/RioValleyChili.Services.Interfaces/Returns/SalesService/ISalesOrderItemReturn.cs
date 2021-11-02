namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ISalesOrderItemReturn : IPickOrderItemReturn
    {
        string ContractItemKey { get; }
        string ContractKey { get; }
        
        double PriceBase { get; }
        double PriceFreight { get; }
        double PriceTreatment { get; }
        double PriceWarehouse { get; }
        double PriceRebate { get; }
    }
}