using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class DeleteLotConductor
    {
        internal static readonly IEnumerable<Expression<Func<Lot, object>>> IncludePaths = new List<Expression<Func<Lot, object>>>
            {
                l => l.ChileLot,
                l => l.AdditiveLot,
                l => l.PackagingLot,
                l => l.Attributes,
                l => l.LotDefects.Select(d => d.Resolution),
                l => l.AttributeDefects,
                l => l.InputTransactions,
                l => l.OutputTransactions,
                l => l.ContractAllowances,
                l => l.SalesOrderAllowances,
                l => l.CustomerAllowances,
                l => l.History
            };

        internal static IEnumerable<Expression<Func<TSource, object>>> ConstructIncludePaths<TSource>(Expression<Func<TSource, Lot>> selectLot, params Expression<Func<TSource, object>>[] additionalIncludes)
        {
            return additionalIncludes.Concat(IncludePaths.Select(p => ((Expression<Func<TSource, object>>) (s => p.Invoke(selectLot.Invoke(s)))).ExpandAll()));
        }

        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal DeleteLotConductor(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult Delete(Lot lot)
        {
            if(lot.ChileLot != null)
            {
                _lotUnitOfWork.ChileLotRepository.Remove(lot.ChileLot);
            }
            if(lot.AdditiveLot != null)
            {
                _lotUnitOfWork.AdditiveLotRepository.Remove(lot.AdditiveLot);
            }
            if(lot.PackagingLot != null)
            {
                _lotUnitOfWork.PackagingLotRepository.Remove(lot.PackagingLot);
            }

            foreach(var attribute in lot.Attributes.ToList())
            {
                _lotUnitOfWork.LotAttributeRepository.Remove(attribute);
            }

            foreach(var defect in lot.LotDefects.ToList())
            {
                if(defect.Resolution != null)
                {
                    _lotUnitOfWork.LotDefectResolutionRepository.Remove(defect.Resolution);
                }
                _lotUnitOfWork.LotDefectRepository.Remove(defect);
            }

            foreach(var attributeDefect in lot.AttributeDefects.ToList())
            {
                _lotUnitOfWork.LotAttributeDefectRepository.Remove(attributeDefect);
            }

            foreach(var transaction in lot.OutputTransactions.Concat(lot.InputTransactions).ToList())
            {
                _lotUnitOfWork.InventoryTransactionsRepository.Remove(transaction);
            }

            foreach(var contractAllowance in lot.ContractAllowances.ToList())
            {
                _lotUnitOfWork.LotContractAllowanceRepository.Remove(contractAllowance);
            }

            foreach(var salesOrderAllowance in lot.SalesOrderAllowances.ToList())
            {
                _lotUnitOfWork.LotSalesOrderAllowanceRepository.Remove(salesOrderAllowance);
            }

            foreach(var customerAllowance in lot.CustomerAllowances.ToList())
            {
                _lotUnitOfWork.LotCustomerAllowanceRepository.Remove(customerAllowance);
            }

            foreach(var history in lot.History)
            {
                _lotUnitOfWork.LotHistoryRepository.Remove(history);
            }

            _lotUnitOfWork.LotRepository.Remove(lot);

            return new SuccessResult();
        }
    }
}