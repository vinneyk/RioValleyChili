using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces
{
    public interface ICreatePickedInventory
    {
        IEmployeeKey EmployeeKey { get; }

        DateTime TimeStamp { get; }

        PickedReason PickedReason { get; }
    }
}