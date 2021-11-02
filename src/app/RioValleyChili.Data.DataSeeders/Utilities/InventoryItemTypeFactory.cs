using RioValleyChili.Core;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    class InventoryItemTypeFactory
    {
        internal static LotTypeEnum BuildInventoryItemTypeFromProdTypeID(int pTypeId)
        {
            return (LotTypeEnum)pTypeId;
        }
    }
}
