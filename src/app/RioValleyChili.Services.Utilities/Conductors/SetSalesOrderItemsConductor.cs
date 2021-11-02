using System;
using System.Collections.Generic;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Customer;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetSalesOrderItemsConductor
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal SetSalesOrderItemsConductor(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult<SalesOrder> SetOrderItems(SalesOrder salesOrder, List<SetSalesOrderItemParameters> newItems)
        {
            if(salesOrder == null) { throw new ArgumentNullException("salesOrder"); }
            if(newItems == null) { throw new ArgumentNullException("newItems"); }

            var validateResult = ValidateSalesOrderItemsHelper.Validate(salesOrder, newItems);
            if(!validateResult.Success)
            {
                return validateResult.ConvertTo<SalesOrder>();
            }

            var setOrderItemsResult = new SetSalesOrderItemsCommand(_salesUnitOfWork).Execute(salesOrder, newItems);
            if(!setOrderItemsResult.Success)
            {
                return setOrderItemsResult.ConvertTo<SalesOrder>();
            }

            return new SuccessResult<SalesOrder>(salesOrder);
        }
    }
}