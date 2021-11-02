namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public abstract class FilterInventoryForPickingContextParameters : FilterInventoryParametersBase
    {
        public abstract string PickingContextKey { get; }
    }

    public class FilterInventoryForShipmentOrderParameters : FilterInventoryForPickingContextParameters
    {
        public string OrderKey { get; set; }
        public string OrderItemKey { get; set; }
        public sealed override string PickingContextKey { get { return OrderKey; } }
    }

    public class FilterInventoryForBatchParameters : FilterInventoryForPickingContextParameters
    {
        public string BatchKey { get; set; }
        public string IngredientKey { get; set; }

        public sealed override string PickingContextKey { get { return BatchKey; } }
    }
}