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
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Customer;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetInterWarehouseOrderPickedInventoryConductor
    {
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentUnitOfWork;

        public SetInterWarehouseOrderPickedInventoryConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentUnitOfWork)
        {
            if(inventoryShipmentUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentUnitOfWork"); }
            _inventoryShipmentUnitOfWork = inventoryShipmentUnitOfWork;
        }
        
        internal IResult UpdatePickedInventory(InventoryShipmentOrderKey orderKey, IUserIdentifiable user, DateTime timestamp, List<PickedInventoryParameters> setPickedInventoryItems)
        {
            var employeeResult = new GetEmployeeCommand(_inventoryShipmentUnitOfWork).GetEmployee(user);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<InventoryShipmentOrder>();
            }

            var orderResult = GetOrder(orderKey);
            if(!orderResult.Success)
            {
                return orderResult;
            }

            orderResult.ResultingObject.PickedInventory.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            orderResult.ResultingObject.PickedInventory.TimeStamp = timestamp;

            var pickedInventoryModifications = PickedInventoryHelper.CreateModifyPickedInventoryItemParameters(orderResult.ResultingObject.PickedInventory, setPickedInventoryItems);
            var validatorResult = ValidateModifyPickedInventoryItems(orderResult.ResultingObject, pickedInventoryModifications);
            if(!validatorResult.Success)
            {
                return validatorResult.ConvertTo<SalesOrder>();
            }

            var modifyPickedResult = new ModifyPickedInventoryItemsCommand(_inventoryShipmentUnitOfWork).Execute(orderResult.ResultingObject.PickedInventory, pickedInventoryModifications);
            if(!modifyPickedResult.Success)
            {
                return modifyPickedResult;
            }

            return new ModifyInventoryCommand(_inventoryShipmentUnitOfWork).Execute(pickedInventoryModifications.Select(p => p.ToModifySourceInventoryParameters()).ToList(), null);
        }

        private IResult<InventoryShipmentOrder> GetOrder(InventoryShipmentOrderKey orderKey)
        {
            var inventoryShipmentOrder = _inventoryShipmentUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey,
                i => i.ShipmentInformation,
                i => i.SourceFacility,
                i => i.PickedInventory.Items.Select(t => t.FromLocation),
                i => i.PickedInventory.Items.Select(t => t.CurrentLocation),
                i => i.InventoryPickOrder.Items.Select(t => t.Product),
                i => i.InventoryPickOrder.Items.Select(t => t.Customer));
            if(inventoryShipmentOrder == null)
            {
                return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, orderKey));
            }

            if(inventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Scheduled && inventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Unscheduled)
            {
                return new InvalidResult<InventoryShipmentOrder>(null, string.Format(UserMessages.CannotPickShipment, inventoryShipmentOrder.ShipmentInformation.Status));
            }

            return new SuccessResult<InventoryShipmentOrder>(inventoryShipmentOrder);
        }

        private IResult ValidateModifyPickedInventoryItems(InventoryShipmentOrder order, IEnumerable<ModifyPickedInventoryItemParameters> items)
        {
            var validator = PickedInventoryValidator.ForInterWarehouseOrder(order.SourceFacility);
            var orderItems = order.InventoryPickOrder.Items.ToDictionary(i => i.ToInventoryPickOrderItemKey());

            foreach(var item in items)
            {
                InventoryPickOrderItem orderItem = null;
                if(item.OrderItemKey != null && !orderItems.TryGetValue(item.OrderItemKey, out orderItem))
                {
                    return new InvalidResult(string.Format(UserMessages.CannotPickForOrderItem_DoesNotExist, item.OrderItemKey));
                }

                if(item.NewQuantity > item.OriginalQuantity)
                {
                    Dictionary<string, Inventory> inventory;
                    var validResult = validator.ValidateItems(_inventoryShipmentUnitOfWork.InventoryRepository, new[] { item.InventoryKey }, out inventory);

                    IChileProductKey chileProduct;
                    if(orderItem != null)
                    {
                        chileProduct = ChileProductKey.FromProductKey(orderItem.Product);
                    }
                    else
                    {
                        var lotKey = inventory.Single().Value.ToLotKey();
                        chileProduct = _inventoryShipmentUnitOfWork.ChileLotRepository.FindByKey(lotKey);
                        if(chileProduct == null)
                        {
                            return new InvalidResult(string.Format(UserMessages.ChileLotNotFound, lotKey));
                        }
                    }

                    IDictionary<AttributeNameKey, ChileProductAttributeRange> productSpec;
                    IDictionary<AttributeNameKey, CustomerProductAttributeRange> customerSpec;
                    var specResult = new GetProductSpecCommand(_inventoryShipmentUnitOfWork).Execute(chileProduct, orderItem == null ? null : orderItem.Customer, out productSpec, out customerSpec);
                    if(!specResult.Success)
                    {
                        return specResult;
                    }

                    var inventoryItem = inventory.Values.Single();
                    var context = new PickingValidatorContext(productSpec, customerSpec, null, null, orderItem == null ? null : orderItem.Customer);
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