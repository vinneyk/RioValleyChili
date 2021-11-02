using System;

namespace RioValleyChili.Core.Extensions
{
    public static class ChileStateExtensions
    {
        public static LotTypeEnum ToLotType(this ChileStateEnum chileState)
        {
            switch(chileState)
            {
                case ChileStateEnum.WIP: return LotTypeEnum.WIP;
                case ChileStateEnum.FinishedGoods: return LotTypeEnum.FinishedGood;
                case ChileStateEnum.Dehydrated: return LotTypeEnum.DeHydrated;
                case ChileStateEnum.OtherRaw: return LotTypeEnum.Raw;
                default: throw new InvalidOperationException(String.Format("There is no translation defined to transform chileStateEnum value '{0}' to a LotTypeEnum.", chileState));
            }
        }

        public static string ToChileStateName(this ChileStateEnum chileState)
        {
            switch (chileState)
            {
                case ChileStateEnum.WIP: return "WIP";
                case ChileStateEnum.FinishedGoods: return "Finished Goods";
                case ChileStateEnum.Dehydrated: return "Dehydrated";
                case ChileStateEnum.OtherRaw: return "Other Raw";
                default: throw new ArgumentOutOfRangeException("chileState");
            }
        }
    }
}