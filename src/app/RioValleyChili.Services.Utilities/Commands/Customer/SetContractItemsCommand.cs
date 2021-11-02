using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;
namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class SetContractItemsCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal SetContractItemsCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        /// <param name="contract">A Contract able to navigate to Contract.Customer.ProductCodes.</param>
        internal IResult Execute(Contract contract, List<SetContractItemParameters> newItems)
        {
            if(contract == null) { throw new ArgumentNullException("contract"); }
            if(newItems == null) { throw new ArgumentNullException("newItems"); }

            var unitOfWorkHelper = new EFUnitOfWorkHelper(_salesUnitOfWork);
            var productCodes = (contract.Customer.ProductCodes ?? new CustomerProductCode[0]).ToList();

            var matches = (contract.ContractItems ?? new ContractItem[0]).BestMatches(newItems,
                (c, n) => n.ChileProductKey.Equals(c),
                (c, n) => n.PackagingProductKey.Equals(c),
                (c, n) => n.TreatmentKey.Equals(c),
                (c, n) => n.ContractItemParameters.UseCustomerSpec == c.UseCustomerSpec,
                (c, n) => n.ContractItemParameters.CustomerCodeOverride == c.CustomerProductCode,
                (c, n) => n.ContractItemParameters.Quantity == c.Quantity);

            foreach(var match in matches)
            {
                if(match.Item1 != null && match.Item2 != null)
                {
                    SetContractItem(match.Item1, match.Item2, productCodes);
                }
                else if(match.Item1 != null)
                {
                    _salesUnitOfWork.ContractItemRepository.Remove(match.Item1);
                }
                else if(match.Item2 != null)
                {
                    var nextItemSequence = unitOfWorkHelper.GetNextSequence<ContractItem>(i => i.ContractYear == contract.ContractYear && i.ContractSequence == contract.ContractSequence, i => i.ContractItemSequence);
                    SetContractItem(_salesUnitOfWork.ContractItemRepository.Add(new ContractItem
                        {
                            ContractYear = contract.ContractYear,
                            ContractSequence = contract.ContractSequence,
                            ContractItemSequence = nextItemSequence
                        }), match.Item2, productCodes);
                }
            }

            return new SuccessResult();
        }

        private static void SetContractItem(ContractItem contractItem, SetContractItemParameters newItem, IEnumerable<CustomerProductCode> productCodes)
        {
            contractItem.ChileProductId = newItem.ChileProductKey.ChileProductKey_ProductId;
            contractItem.PackagingProductId = newItem.PackagingProductKey.PackagingProductKey_ProductId;
            contractItem.TreatmentId = newItem.TreatmentKey.InventoryTreatmentKey_Id;
            contractItem.UseCustomerSpec = newItem.ContractItemParameters.UseCustomerSpec;
            contractItem.Quantity = newItem.ContractItemParameters.Quantity;
            contractItem.PriceBase = newItem.ContractItemParameters.PriceBase;
            contractItem.PriceFreight = newItem.ContractItemParameters.PriceFreight;
            contractItem.PriceTreatment = newItem.ContractItemParameters.PriceTreatment;
            contractItem.PriceWarehouse = newItem.ContractItemParameters.PriceWarehouse;
            contractItem.PriceRebate = newItem.ContractItemParameters.PriceRebate;

            contractItem.CustomerProductCode = newItem.ContractItemParameters.CustomerCodeOverride ??
                            productCodes.Where(c => newItem.ChileProductKey.Equals(c)).Select(c => c.Code).FirstOrDefault() ?? "";
        }
    }
}