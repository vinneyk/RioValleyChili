using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface ISalesOrderKey
    {
        DateTime SalesOrderKey_DateCreated { get; }
        int SalesOrderKey_Sequence { get; }
    }
}