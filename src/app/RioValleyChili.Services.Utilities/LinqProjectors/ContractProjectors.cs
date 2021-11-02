// ReSharper disable ConvertClosureToMethodGroup
// ReSharper disable RedundantCast
// ReSharper disable ConstantNullCoalescingCondition

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ContractProjectors
    {
        internal static Expression<Func<Contract, ContractKeyReturn>> SelectKey()
        {
            return c => new ContractKeyReturn
                {
                    ContractKey_Year = c.ContractYear,
                    ContractKey_Sequence = c.ContractSequence
                };
        }

        internal static IEnumerable<Expression<Func<Contract, ContractDetailReturn>>> SelectDetail()
        {
            var item = ContractItemProjectors.Select();
            var notebook = NotebookProjectors.Select();

            return SelectContractBase().Select(c => c.Merge(Projector<Contract>.To(n => new ContractDetailReturn { })))
                .ToAppendedList(Projector<Contract>.To(c => new ContractDetailReturn
                    {
                        NotesToPrint = c.NotesToPrint,
                        ContactAddress = c.ContactAddress,
                        Comments = notebook.Invoke(c.Comments)
                    }))
                .ToAppendedList(item.Select(itemProjector => Projector<Contract>.To(c => new ContractDetailReturn
                    {
                        ContractItems = c.ContractItems.Select(i => itemProjector.Invoke(i))
                    })));
        }
        
        internal static IEnumerable<Expression<Func<Contract, ContractSummaryReturn>>> SelectSummary()
        {
            return SelectContractBase().Select(c => c.Merge(Projector<Contract>.To(n => new ContractSummaryReturn { })))
                .ToAppendedList(c => new ContractSummaryReturn
                    {
                        AverageBasePrice = c.ContractItems.Any() ? c.ContractItems.Average(i => i.PriceBase) : 0.0,
                        AverageTotalPrice = c.ContractItems.Any() ? c.ContractItems.Average(i => i.PriceBase + i.PriceFreight + i.PriceTreatment + i.PriceWarehouse - i.PriceRebate) : 0.0,
                        SumQuantity = c.ContractItems.Any() ? c.ContractItems.Sum(i => i.Quantity) : 0.0,
                        SumWeight = c.ContractItems.Any() ? c.ContractItems.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0.0,
                        SumValue = c.ContractItems.Any() ? c.ContractItems.Sum(i => (i.Quantity * i.PackagingProduct.Weight) * (i.PriceBase + i.PriceFreight + i.PriceTreatment + i.PriceWarehouse - i.PriceRebate)) : 0.0
                    });
        }

        internal static Expression<Func<Contract, ContractOrdersReturn>> SelectContractOrders(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }

            var key = SelectKey();
            var contractOrderPredicate = CustomerOrderPredicates.ByContract();
            var contractOrder = SalesOrderProjectors.SelectCustomerContractOrder();

            var customerOrders = salesUnitOfWork.SalesOrderRepository.All();

            return c => new ContractOrdersReturn
                {
                    ContracKeyReturn = key.Invoke(c),
                    Orders = customerOrders.Where(o => contractOrderPredicate.Invoke(o, c)).Select(o => contractOrder.Invoke(o, c))
                };
        }

        internal static IEnumerable<Expression<Func<Contract, ContractShipmentSummaryReturn>>> SplitSelectShipmentSummary()
        {
            var key = SelectKey();

            return ContractItemProjectors.SplitSelectShipmentSummary()
                .Select(p => Projector<Contract>.To(c => new ContractShipmentSummaryReturn
                    {
                        CustomerName = c.Customer.Company.Name,
                        ContractType = c.ContractType,
                        Items = c.ContractItems.Select(i => p.Invoke(i))
                    }))
                .ToAppendedList(Projector<Contract>.To(c => new ContractShipmentSummaryReturn
                    {
                        ContractKeyReturn = key.Invoke(c),
                        ContractNumber = c.ContractId,
                        ContractStatus = c.ContractStatus,
                        TermBegin = c.TermBegin,
                        TermEnd = c.TermEnd
                    }));
        }

        private static IEnumerable<Expression<Func<Contract, ContractBaseReturn>>> SelectContractBase()
        {
            var key = SelectKey();
            var company = CompanyProjectors.SelectSummary();
            var warehouse = FacilityProjectors.Select(false, true);

            return new[]
                {
                    Projector<Contract>.To(c => new ContractBaseReturn
                        {
                            ContractKeyReturn = key.Invoke(c),

                            CustomerPurchaseOrder = c.CustomerPurchaseOrder,
                            ContractNumber = c.ContractId,
                            ContractDate = c.ContractDate,
                            TermBegin = c.TermBegin,
                            TermEnd = c.TermEnd,
                            PaymentTerms = c.PaymentTerms,
                            ContactName = c.ContactName,
                            FOB = c.FOB,

                            ContractType = c.ContractType,
                            ContractStatus = c.ContractStatus,

                            DefaultPickFromFacility = warehouse.Invoke(c.DefaultPickFromFacility)
                        }),
                    Projector<Contract>.To(c => new ContractBaseReturn
                        {
                            Customer = company.Invoke(c.Customer.Company),
                            Broker = company.Invoke(c.Broker),
                        })
                };
        }
    }
}

// ReSharper restore ConstantNullCoalescingCondition
// ReSharper restore RedundantCast
// ReSharper restore ConvertClosureToMethodGroup