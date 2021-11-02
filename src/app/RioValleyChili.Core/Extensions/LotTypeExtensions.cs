using System;
using System.ComponentModel;
using System.Linq;

namespace RioValleyChili.Core.Extensions
{
    public static class LotTypeExtensions
    {
        public static ChileStateEnum ToChileState(this LotTypeEnum itemType)
        {
            switch(itemType)
            {
                case LotTypeEnum.FinishedGood: return ChileStateEnum.FinishedGoods;
                case LotTypeEnum.DeHydrated: return ChileStateEnum.Dehydrated;

                case LotTypeEnum.WIP: 
                case LotTypeEnum.Other: 
                    return ChileStateEnum.WIP;

                case LotTypeEnum.Raw: 
                case LotTypeEnum.GRP:
                    return ChileStateEnum.OtherRaw;
                default: throw new InvalidEnumArgumentException(string.Format("Cannot convert the InventoryItemTypeEnum value '{0}' to a ChileStateEnum.", itemType.ToString()));
            }
        }

        public static ProductTypeEnum ToProductType(this LotTypeEnum lotType)
        {
            switch(lotType)
            {
                case LotTypeEnum.WIP:
                case LotTypeEnum.Raw:
                case LotTypeEnum.GRP:
                case LotTypeEnum.FinishedGood:
                case LotTypeEnum.DeHydrated:
                case LotTypeEnum.Other:
                    return ProductTypeEnum.Chile;

                case LotTypeEnum.Additive:
                    return ProductTypeEnum.Additive;

                case LotTypeEnum.Packaging:
                    return ProductTypeEnum.Packaging;

                default: throw new InvalidEnumArgumentException(string.Format("Cannot convert the LotTypeEnum value '{0}' to a ProductTypeEnum.", lotType));
            }
        }

        public static int ToInt(this LotTypeEnum itemType)
        {
            return (int) itemType;
        }

        public static LotTypeEnum? ToLotType(this int lotTypeId)
        {
            return Enum.GetValues(typeof(LotTypeEnum)).Cast<LotTypeEnum>().SingleOrDefault(v => (int)v == lotTypeId);
        }
    }
}
