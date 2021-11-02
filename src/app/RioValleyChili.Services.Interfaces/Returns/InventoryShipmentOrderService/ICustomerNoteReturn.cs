namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface ICustomerNoteReturn
    {
        string CustomerNoteKey { get; }
        string Type { get; }
        string Text { get; }
        bool Bold { get; }
    }
}