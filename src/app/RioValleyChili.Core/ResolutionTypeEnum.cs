using System;

namespace RioValleyChili.Core
{
    public enum ResolutionTypeEnum : short
    {
        DataEntryCorrection = 0,
        ReworkPerformed,
        Treated,
        Retest,
        AcceptedByUser,
        [Obsolete("Use AcceptedByUser instead.")]
        AcceptedByDataLoad,
        InvalidValue
    }
}