using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IShipmentInformationKey
    {
        DateTime ShipmentInfoKey_DateCreated { get; }

        int ShipmentInfoKey_Sequence { get;  }
    }
}