using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotCustomerOrderAllowanceProjectors
    {
        internal static Expression<Func<LotSalesOrderAllowance, LotCustomerOrderAllowanceReturn>> Select()
        {
            var customerOrderKeyReturn = SalesOrderProjectors.SelectKey();
            var customerKey = CustomerProjectors.SelectKey();

            return a => new LotCustomerOrderAllowanceReturn
                {
                    SalesOrderKeyReturn = customerOrderKeyReturn.Invoke(a.SalesOrder),
                    OrderNumber = a.SalesOrder.InventoryShipmentOrder.MoveNum,
                    CustomerKeyReturn = customerKey.Invoke(a.SalesOrder.Customer),
                    CustomerName = a.SalesOrder.Customer.Company.Name
                };
        }

        internal static Expression<Func<LotSalesOrderAllowance, SalesOrderKeyReturn>> SelectCustomerOrderKey()
        {
            return a => new SalesOrderKeyReturn
                {
                    SalesOrderKey_DateCreated = a.SalesOrderDateCreated,
                    SalesOrderKey_Sequence = a.SalesOrderSequence
                };
        }
    }
}