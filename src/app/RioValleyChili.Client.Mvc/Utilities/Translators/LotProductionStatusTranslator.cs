using System;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Utilities.Translators
{
    public static class LotProductionStatusTranslator
    {
        public static string GetDescription(this LotProductionStatus status)
        {
            switch (status)
            {
                case LotProductionStatus.Produced:
                    return "Produced";
                case LotProductionStatus.Batched:
                    return "Batched";
            }

            throw new InvalidOperationException("The GetDescription method does not support the LotProductionStatus \"{0}\".");
        }
    }
}