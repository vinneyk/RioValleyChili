using System;
using RioValleyChili.Core;

namespace RioValleyChili.Data.DataSeeders.Helpers
{
    internal static class ChileStateHelper
    {
        internal static ChileStateEnum GetChileState(int productTypeId)
        {
            switch (productTypeId)
            {
                case 0: 
                case 12:
                    return ChileStateEnum.OtherRaw;

                case 4:
                case 2:
                    return ChileStateEnum.WIP;

                case 1: return ChileStateEnum.Dehydrated;
                case 3: return ChileStateEnum.FinishedGoods;
            }

            throw new InvalidOperationException(string.Format("The productTypeId was not valid for translation into a ChileStateEnum. The value received was '{0}'.", productTypeId));
        }
    }
}