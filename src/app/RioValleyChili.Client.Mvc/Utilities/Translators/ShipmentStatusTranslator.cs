namespace RioValleyChili.Client.Mvc.Utilities.Translators
{
    public class ShipmentStatusTranslator
    {
        public static string TranslateStatus(IShipmentStatusTranslatorAdapter adapter)
        {
            return adapter.ShipmentStatus.ToString();
        }
    }
}