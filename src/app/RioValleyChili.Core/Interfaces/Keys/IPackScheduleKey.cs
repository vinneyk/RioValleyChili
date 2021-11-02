using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IPackScheduleKey
    {
        DateTime PackScheduleKey_DateCreated { get; }
        int PackScheduleKey_DateSequence { get; }
    }
}