using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.CompleteExpiredContracts)]
    public class SyncCompleteExpiredContracts : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, List<Contract>>
    {
        public SyncCompleteExpiredContracts(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<List<Contract>> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var contracts = getInput();
            if(contracts.Any())
            {
                foreach(var contract in contracts)
                {
                    var oldContract = OldContext.tblContracts.FirstOrDefault(c => c.ContractID == contract.ContractId);
                    if(oldContract == null)
                    {
                        throw new Exception(string.Format("Could not find tblContract[{0}] referenced by Contract[{1}]", contract.ContractId, new ContractKey(contract)));
                    }
                    oldContract.KStatus = ContractStatus.Completed.ToString();
                }

                OldContext.SaveChanges();

                Console.WriteLine(ConsoleOutput.CompletedExpiredContracts, contracts.Count);
            }
            else
            {
                Console.WriteLine(ConsoleOutput.NoExpiredContracts);
            }
        }
    }
}