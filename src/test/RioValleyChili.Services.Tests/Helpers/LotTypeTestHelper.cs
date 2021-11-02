using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Tests.Helpers
{
    static internal class LotTypeTestHelper
    {
        private static readonly List<LotTypeEnum> ChileLotTypes = Enum.GetValues(typeof(LotTypeEnum)).OfType<LotTypeEnum>().Where(t => t.IsChileLot()).ToList();

        internal static LotTypeEnum GetRandomChileLotType()
        {
            return ChileLotTypes[new Random().Next(0, ChileLotTypes.Count)];
        }
    }
}