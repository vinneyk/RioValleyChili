using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IIntraWarehouseOrderKey
    {
        DateTime IntraWarehouseOrderKey_DateCreated { get; }

        int IntraWarehouseOrderKey_Sequence { get; }
    }
}