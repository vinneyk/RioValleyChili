namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface IContractItem
    {
        string ChileProductKey { get; }

        string PackagingProductKey { get; }

        string TreatmentKey { get; }

        bool UseCustomerSpec { get; }

        string CustomerCodeOverride { get; }

        int Quantity { get; }

        double PriceBase { get; }

        double PriceFreight { get; }

        double PriceTreatment { get; }

        double PriceWarehouse { get; }

        double PriceRebate { get; }
    }
}