using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncContractsStatus)]
    public class SyncContractsStatus : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, List<ContractKey>>
    {
        public SyncContractsStatus(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<List<ContractKey>> getInput)
        {
            var contractKeys = getInput();
            if(!contractKeys.Any())
            {
                return;
            }

            var predicate = contractKeys.Aggregate(PredicateBuilder.False<Contract>(), (c, n) => c.Or(n.FindByPredicate)).Expand();
            var contracts = UnitOfWork.ContractRepository.Filter(predicate).ToList();

            var missing = contractKeys.FirstOrDefault(k => contracts.Count(k.FindByPredicate.Compile()) < 1);
            if(missing != null)
            {
                throw new Exception(string.Format("Could not find Contract[{0}]", missing));
            }

            var status = contracts.Select(c => c.ContractStatus).Distinct().Single();
            var contractIds = contracts.Select(c => c.ContractId).ToArray();
            var oldContracts = OldContext.tblContracts.Where(c => contractIds.Contains(c.ContractID)).ToList();

            var oldMissing = contractIds.FirstOrDefault(c => oldContracts.All(n => n.ContractID != c));
            if(oldMissing != null)
            {
                throw new Exception(string.Format("Could not find tblContract[{0}]", oldMissing));
            }

            oldContracts.ForEach(n => n.KStatus = status.ToString());

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.SyncContractsStatus, string.Join(",", oldContracts.Select(c => c.ContractID.ToString())));
        }
    }
}