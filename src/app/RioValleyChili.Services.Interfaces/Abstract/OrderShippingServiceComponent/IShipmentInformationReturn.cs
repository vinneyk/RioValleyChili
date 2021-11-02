namespace RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent
{
    public interface IShipmentInformationReturn : IShipmentSummaryReturn
    {
        int PalletQuantity { get; }
        double? PalletWeight { get; }

        IShippingInstructions ShippingInstructions { get; }
        ITransitInformation TransitInformation { get; }
    }
}