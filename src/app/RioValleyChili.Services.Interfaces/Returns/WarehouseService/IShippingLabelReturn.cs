namespace RioValleyChili.Services.Interfaces.Returns.WarehouseService
{
    public interface IShippingLabelReturn
    {
        string Name { get; }
        string Phone { get; }
        string EMail { get; }
        string Fax { get; }
        string AddressLine1 { get; }
        string AddressLine2 { get; }
        string AddressLine3 { get; }
        string City { get; }
        string State { get; }
        string PostalCode { get; }
        string Country { get; }
    }
}