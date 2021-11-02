using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    public static class LotExtensions
    {
        public static string ToMaterialType(this ILotKey lot)
        {
            switch((LotTypeEnum)lot.LotKey_LotTypeId)
            {
                case LotTypeEnum.Raw:
                case LotTypeEnum.DeHydrated:
                case LotTypeEnum.WIP:
                case LotTypeEnum.FinishedGood:
                    return "Base";
                case LotTypeEnum.Additive:
                    return "Ingredient";
                case LotTypeEnum.Packaging:
                    return "Packaging";
            }
            return "";
        }
    }
}