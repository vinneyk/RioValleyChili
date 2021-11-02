namespace RioValleyChili.Core
{
    public enum LotTypeEnum
    {
        Raw = 0,
        DeHydrated = 1,
        WIP = 2,
        FinishedGood = 3,
        Additive = 4,
        Packaging = 5,
        Other = 11,
        GRP = 12
    }

    public static class LotTypeExtensions
    {
        public static bool IsChileLot(this LotTypeEnum lotType)
        {
            switch(lotType)
            {
                case LotTypeEnum.Raw:
                case LotTypeEnum.DeHydrated:
                case LotTypeEnum.WIP:
                case LotTypeEnum.FinishedGood:
                case LotTypeEnum.Other:
                case LotTypeEnum.GRP:
                    return true;

                default: return false;
            }
        }
    }
}