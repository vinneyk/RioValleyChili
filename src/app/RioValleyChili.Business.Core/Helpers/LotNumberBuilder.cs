using System;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class LotNumberBuilder
    {
        public static LotNumberResult BuildLotNumber(ILotKey lotKey)
        {
            var date = lotKey.LotKey_DateCreated;
            var julianString = string.Format("{0:00}{1:000}", date.Year % 100, date.DayOfYear);
            var lotNumberString = string.Format("{0}{1}{2:00}", lotKey.LotKey_LotTypeId, julianString, lotKey.LotKey_DateSequence);

            int julian;
            if(!int.TryParse(julianString, out julian))
            {
                throw new FormatException(string.Format("Could not parse [{0}] into integer.", julianString));
            }

            int lotNumber;
            if(!int.TryParse(lotNumberString, out lotNumber))
            {
                throw new FormatException(string.Format("Could not parse [{0}] into integer.", lotNumberString));
            }

            return new LotNumberResult(lotNumber, julian);
        }
    }

    public struct LotNumberResult
    {
        public readonly int LotNumber;
        public readonly int Julian;

        public LotNumberResult(int lotNumber, int julian)
        {
            LotNumber = lotNumber;
            Julian = julian;
        }

        public static implicit operator int(LotNumberResult lotNumberResult)
        {
            return lotNumberResult.LotNumber;
        }

        public static implicit operator string(LotNumberResult lotNumberResult)
        {
            return lotNumberResult.ToString();
        }

        public override string ToString()
        {
            return LotNumber.ToString();
        }
    }
}