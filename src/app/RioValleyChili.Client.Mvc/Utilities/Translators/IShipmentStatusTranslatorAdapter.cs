using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Utilities.Translators
{
    public interface IShipmentStatusTranslatorAdapter
    {
        ShipmentStatus ShipmentStatus { get; }

        string ShipmentStatusDescription { get; }
    }
}