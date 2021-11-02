using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface ILotKey
    {
        DateTime LotKey_DateCreated { get; }

        int LotKey_DateSequence { get; }

        int LotKey_LotTypeId { get; }
    }
}