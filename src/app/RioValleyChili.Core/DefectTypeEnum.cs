using System;

namespace RioValleyChili.Core
{
    public enum DefectTypeEnum : short
    {
        ProductSpec = 0,
        BacterialContamination = 1,
        InHouseContamination = 2,
        [Obsolete("Considering for removal, pending confirmation. If critical usage is found, bring it up. - RI 2016-07-05")]
        ActionableDefect = 3
    }
}