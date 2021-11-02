using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class CreatePickedInventoryCommandParameters : ICreatePickedInventory
    {
        public IEmployeeKey EmployeeKey { get; set; }

        public DateTime TimeStamp { get; set; }

        public PickedReason PickedReason { get; set; }
    }
}