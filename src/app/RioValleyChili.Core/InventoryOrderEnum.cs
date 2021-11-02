namespace RioValleyChili.Core
{
    //todo: discuss renaming this enum to InventoryPickingContext. - VK 8/6/2014
    public enum InventoryOrderEnum
    {
        Unknown = 0,
        WarehouseMovements = 1,
        TransWarehouseMovements = 2,
        Treatments = 3,
        ProductionBatch = 4,
        CustomerOrder = 5,
    }
}