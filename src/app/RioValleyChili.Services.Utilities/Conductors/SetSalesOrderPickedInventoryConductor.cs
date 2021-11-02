using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Customer;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetSalesOrderPickedInventoryConductor
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal SetSalesOrderPickedInventoryConductor(ISalesUnitOfWork salesSalesUnitOfWork)
        {
            if(salesSalesUnitOfWork == null) { throw new ArgumentNullException("salesSalesUnitOfWork"); }
            _salesUnitOfWork = salesSalesUnitOfWork;
        }

        internal IResult<SalesOrder> Execute(DateTime timestamp, ISalesOrderKey salesOrderKey, ISetPickedInventoryParameters setPickedInventory)
        {
            if(salesOrderKey == null) { throw new ArgumentNullException("salesOrderKey"); }
            if(setPickedInventory == null) { throw new ArgumentNullException("setPickedInventory"); }

            var employeeResult = new GetEmployeeCommand(_salesUnitOfWork).GetEmployee(setPickedInventory);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<SalesOrder>();
            }
            
            var customerOrderResult = GetSalesOrder(salesOrderKey);
            if(!customerOrderResult.Success)
            {
                return customerOrderResult;
            }
            var salesOrder = customerOrderResult.ResultingObject;

            salesOrder.InventoryShipmentOrder.PickedInventory.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            salesOrder.InventoryShipmentOrder.PickedInventory.TimeStamp = timestamp;

            var parsedPickedInventory = new List<PickedInventoryParameters>();
            foreach(var item in setPickedInventory.PickedInventoryItems)
            {
                var itemParseResult = item.ToParsedParameters(true);
                if(!itemParseResult.Success)
                {
                    return itemParseResult.ConvertTo<SalesOrder>();
                }
                parsedPickedInventory.Add(itemParseResult.ResultingObject);
            }
            
            var pickedInventoryModifications = PickedInventoryHelper.CreateModifyCustomerOrderPickedInventoryItemParameters(salesOrder, parsedPickedInventory);
            var validatorResult = ValidateModifyPickedInventoryItems(salesOrder, pickedInventoryModifications);
            if(!validatorResult.Success)
            {
                return validatorResult.ConvertTo<SalesOrder>();
            }

            var modifyPickedResult = new ModifySalesOrderPickedItemsCommand(_salesUnitOfWork).Execute(new PickedInventoryKey(salesOrder.InventoryShipmentOrder), pickedInventoryModifications);
            if(!modifyPickedResult.Success)
            {
                return modifyPickedResult.ConvertTo<SalesOrder>();
            }

            var inventoryModifications = pickedInventoryModifications.Select(p => p.ToModifySourceInventoryParameters());
            var modifyInventoryResult = new ModifyInventoryCommand(_salesUnitOfWork).Execute(inventoryModifications, null);
            if(!modifyInventoryResult.Success)
            {
                return modifyInventoryResult.ConvertTo<SalesOrder>();
            }

            return new SuccessResult<SalesOrder>(salesOrder);
        }

        private IResult<SalesOrder> GetSalesOrder(ISalesOrderKey salesOrderKey)
        {
            var orderKey = salesOrderKey.ToSalesOrderKey();
            var salesOrder = _salesUnitOfWork.SalesOrderRepository.FindByKey(orderKey,
                o => o.SalesOrderPickedItems.Select(i => i.PickedInventoryItem),
                o => o.SalesOrderItems.Select(i => i.ContractItem),
                o => o.SalesOrderItems.Select(i => i.InventoryPickOrderItem));
            if(salesOrder == null)
            {
                return new InvalidResult<SalesOrder>(null, string.Format(UserMessages.SalesOrderNotFound, salesOrderKey));
            }

            if(salesOrder.OrderStatus == SalesOrderStatus.Invoiced)
            {
                return new InvalidResult<SalesOrder>(null, string.Format(UserMessages.CannotPickInvoicedCustomerOrder, salesOrderKey));
            }

            salesOrder.InventoryShipmentOrder = _salesUnitOfWork.InventoryShipmentOrderRepository.FindByKey(salesOrder.ToInventoryShipmentOrderKey(),
                i => i.PickedInventory.Items.Select(n => n.CurrentLocation),
                i => i.SourceFacility.Locations,
                i => i.DestinationFacility.Locations);

            if(salesOrder.InventoryShipmentOrder.OrderStatus == OrderStatus.Void)
            {
                return new InvalidResult<SalesOrder>(null, string.Format(UserMessages.CannotPickVoidedCustomerOrder, salesOrderKey));
            }

            return new SuccessResult<SalesOrder>(salesOrder);
        }

        private IResult ValidateModifyPickedInventoryItems(SalesOrder salesOrder, IEnumerable<ModifySalesOrderPickedInventoryItemParameters> items)
        {
            var validator = PickedInventoryValidator.ForSalesOrder(salesOrder.InventoryShipmentOrder.SourceFacility);
            var orderItems = salesOrder.SalesOrderItems.ToDictionary(i => i.ToSalesOrderItemKey());

            foreach(var item in items)
            {
                SalesOrderItem orderItem;
                if(!orderItems.TryGetValue(item.SalesOrderItemKey, out orderItem))
                {
                    return new InvalidResult(string.Format(UserMessages.CannotPickForCustomerOrderItem_DoesNotExist, item.SalesOrderItemKey.KeyValue));
                }

                if(item.NewQuantity > item.OriginalQuantity)
                {
                    IDictionary<AttributeNameKey, ChileProductAttributeRange> productSpec;
                    IDictionary<AttributeNameKey, CustomerProductAttributeRange> customerSpec;
                    var specResult = new GetProductSpecCommand(_salesUnitOfWork).Execute(ChileProductKey.FromProductKey(orderItem.InventoryPickOrderItem),
                        orderItem.Order, out productSpec, out customerSpec);
                    if(!specResult.Success)
                    {
                        return specResult;
                    }

                    Dictionary<string, Inventory> inventory;
                    var validResult = validator.ValidateItems(_salesUnitOfWork.InventoryRepository, new[] { item.InventoryKey }, out inventory);
                    var inventoryItem = inventory.Values.Single();
                    var context = new PickingValidatorContext(productSpec, customerSpec, orderItem.ContractItem, orderItem, orderItem.Order);
                    var pickable = new PickingValidator(inventoryItem.Lot);
                    var pickableResult = pickable.ValidForPicking(context);

                    if(!validResult.Success || !pickableResult.Success)
                    {
                        return new InvalidResult(new[] { validResult, pickableResult }.CombineMessages());
                    }
                }
            }

            return new SuccessResult();
        }
    }
}