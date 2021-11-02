using System;

namespace RioValleyChili.Core
{
    public enum InstructionType
    {
        ProductionBatchInstruction,
        [Obsolete("Pretty sure this should be removed. -RI 2016-10-24")]
        ProductionScheduleInstruction
    }
}