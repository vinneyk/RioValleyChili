using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Notebook;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class RemoveContractCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal RemoveContractCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult Execute(IContractKey contractKey, out int? contractId)
        {
            contractId = null;

            var key = new ContractKey(contractKey);
            var contract = _salesUnitOfWork.ContractRepository
                .FindByKey(key, c => c.ContractItems, c => c.Comments, c => c.Comments.Notes, c => c.ContractItems.Select(i => i.OrderItems));
            if(contract == null)
            {
                return new InvalidResult(string.Format(UserMessages.CustomerContractNotFound, key.KeyValue));
            }

            if(contract.ContractItems.SelectMany(i => i.OrderItems).Any())
            {
                return new InvalidResult(string.Format(UserMessages.CustomerContractHasOrderItems, key.KeyValue));
            }

            contractId = contract.ContractId;

            var deleteNotebookResult = new DeleteNotebookCommand(_salesUnitOfWork).Delete(contract.Comments);
            if(!deleteNotebookResult.Success)
            {
                return deleteNotebookResult;
            }

            var contractItems = contract.ContractItems.ToList();
            foreach(var item in contractItems)
            {
                _salesUnitOfWork.ContractItemRepository.Remove(item);
            }
            _salesUnitOfWork.ContractRepository.Remove(contract);

            return new SuccessResult();
        }
    }
}