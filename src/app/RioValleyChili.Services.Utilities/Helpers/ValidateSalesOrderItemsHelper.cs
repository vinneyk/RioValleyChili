using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Helpers
{
    internal static class ValidateSalesOrderItemsHelper
    {
        internal static IResult Validate(SalesOrder order, List<SetSalesOrderItemParameters> orderItems)
        {
            if(orderItems == null) { throw new ArgumentNullException("orderItems"); }

            var contractItems = order.Customer == null ? new List<ContractItem>() : order.Customer.Contracts.SelectMany(r => r.ContractItems).ToList();
            foreach(var orderItem in orderItems.Where(i => order.InventoryShipmentOrder.OrderType != InventoryShipmentOrderTypeEnum.MiscellaneousOrder || i.ContractItemKey != null))
            {
                var contractItem = contractItems.FirstOrDefault(c => orderItem.ContractItemKey != null && orderItem.ContractItemKey.Equals(c));
                if(contractItem == null)
                {
                    return new InvalidResult(string.Format(UserMessages.ContractItemForCustomerNotFound, orderItem.ContractItemKey, order.Customer == null ? null : order.Customer.ToCustomerKey()));
                }

                if(order.BrokerId != contractItem.Contract.BrokerId)
                {
                    return new InvalidResult(string.Format(UserMessages.SalesOrderItemBrokerConflict));
                }

                if(!orderItem.ProductKey.Equals(new ProductKey(contractItem.ToChileProductKey())))
                {
                    return new InvalidResult(string.Format(UserMessages.ExpectedChileProductKeyForContractItemNotEqualReceived, contractItem.ToChileProductKey(), orderItem.ContractItemKey, orderItem.ProductKey));
                }
            }

            return new SuccessResult();
        }
    }
}