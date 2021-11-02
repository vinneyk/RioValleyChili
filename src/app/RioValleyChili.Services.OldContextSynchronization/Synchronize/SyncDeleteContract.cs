using System;
using System.Linq;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.DeleteContract)]
    public class SyncDeleteContract : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, int?>
    {
        public SyncDeleteContract(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<int?> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var contractId = getInput();
            if(contractId == null)
            {
                return;
            }

            var contract = OldContext.tblContracts.Include("tblContractDetails").FirstOrDefault(c => c.ContractID == contractId.Value);
            foreach(var detail in contract.tblContractDetails.ToList())
            {
                OldContext.tblContractDetails.DeleteObject(detail);
            }
            OldContext.tblContracts.DeleteObject(contract);
            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.RemovedContract, contractId);
        }
    }
}