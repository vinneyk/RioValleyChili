using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerNoteReturn : ICustomerNoteReturn
    {
        public string CustomerNoteKey { get { return CustomerNoteKeyReturn.CustomerNoteKey; } }
        public string Type { get; set; }
        public string Text { get; set; }
        public bool Bold { get; set; }

        internal CustomerNoteKeyReturn CustomerNoteKeyReturn { get; set; }

    }
}