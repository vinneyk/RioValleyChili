using RioValleyChili.Core;

namespace RioValleyChili.Data.DataSeeders.Helpers
{
    public static class LotHelper
    {
        public static LotHoldType? GetHoldStatus(LotStat? lotStat, out string description)
        {
            LotHoldType? hold = null;
            description = null;

            switch(lotStat)
            {
                case LotStat.Completed_Hold:
                    hold = LotHoldType.HoldForCustomer;
                    description = GetHoldDescription(lotStat.Value);
                    break;

                case LotStat.InProcess_Hold:
                    hold = LotHoldType.HoldForAdditionalTesting;
                    description = GetHoldDescription(lotStat.Value);
                    break;

                case LotStat._09Hold:
                    hold = LotHoldType.HoldForAdditionalTesting;
                    description = GetHoldDescription(lotStat.Value);
                    break;

                case LotStat.TBT:
                    hold = LotHoldType.HoldForTreatment;
                    description = GetHoldDescription(lotStat.Value);
                    break;
            }

            return hold;
        }

        private static string GetHoldDescription(LotStat lotStat)
        {
            return string.Format("Set by data load from LotStat[{0}].", lotStat);
        }
    }
}
