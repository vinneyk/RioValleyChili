using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class CreateChileLotProduction : ICreateChileLotProduction
    {
        public IEmployeeKey EmployeeKey { get; set; }

        public DateTime TimeStamp { get; set; }

        public PickedReason PickedReason { get { return PickedReason.Production; } }

        public ILotKey LotKey { get; set; }

        public ProductionType ProductionType { get; set; }
    }
}