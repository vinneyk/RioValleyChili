using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Commands.Shipment;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class CreateSalesOrderConductor : SalesOrderConductorBase
    {
        internal CreateSalesOrderConductor(ISalesUnitOfWork salesUnitOfWork) : base(salesUnitOfWork) { }

        internal IResult<SalesOrder> Execute(DateTime timestamp, CreateSalesOrderConductorParameters parameters)
        {
            if(timestamp == null) { throw new ArgumentNullException("timestamp"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(SalesUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<SalesOrder>();
            }

            Customer customer = null;
            if(parameters.CustomerKey != null)
            {
                customer = SalesUnitOfWork.CustomerRepository.FindByKey(parameters.CustomerKey,
                    c => c.Company.Contacts,
                    c => c.Contracts.Select(r => r.ContractItems));
            }
            
            if(customer == null && !parameters.CreateParameters.IsMiscellaneous)
            {
                return new InvalidResult<SalesOrder>(null, string.Format(UserMessages.CustomerNotFound, parameters.CustomerKey));
            }

            var pickedInventoryResult = new CreatePickedInventoryCommand(SalesUnitOfWork).Execute(new CreatePickedInventoryCommandParameters
                {
                    EmployeeKey = employeeResult.ResultingObject.ToEmployeeKey(),
                    PickedReason = PickedReason.SalesOrder,
                    TimeStamp = timestamp
                });
            if(!pickedInventoryResult.Success)
            {
                pickedInventoryResult.ConvertTo<SalesOrder>();
            }
            var pickedInventory = pickedInventoryResult.ResultingObject;
            pickedInventory.Items = null;

            var pickOrderResult = new CreateInventoryPickOrderCommand(SalesUnitOfWork).Execute(pickedInventory);
            if(!pickOrderResult.Success)
            {
                return pickOrderResult.ConvertTo<SalesOrder>();
            }
            var pickOrder = pickOrderResult.ResultingObject;
            pickOrder.Items = null;

            var shipmentInformationResult = new CreateShipmentInformationCommand(SalesUnitOfWork).Execute(timestamp.Date);
            if(!shipmentInformationResult.Success)
            {
                return shipmentInformationResult.ConvertTo<SalesOrder>();
            }
            var shipmentInfo = shipmentInformationResult.ResultingObject;

            var salesOrder = new SalesOrder
                {
                    DateCreated = pickedInventory.DateCreated,
                    Sequence = pickedInventory.Sequence,

                    CustomerId = customer == null ? (int?)null : customer.Id,
                    BrokerId = customer == null ? (int?)null : customer.BrokerId,

                    InventoryShipmentOrder = new InventoryShipmentOrder
                        {
                            DateCreated = pickedInventory.DateCreated,
                            Sequence = pickedInventory.Sequence,
                            EmployeeId = employeeResult.ResultingObject.EmployeeId,
                            TimeStamp = timestamp,

                            OrderType = parameters.CreateParameters.IsMiscellaneous ? InventoryShipmentOrderTypeEnum.MiscellaneousOrder : InventoryShipmentOrderTypeEnum.SalesOrder,
                            ShipmentInfoDateCreated = shipmentInfo.DateCreated,
                            ShipmentInfoSequence = shipmentInfo.Sequence,
                            ShipmentInformation = shipmentInfo,

                            MoveNum = SalesUnitOfWork.SalesOrderRepository.SourceQuery.Select(s => s.InventoryShipmentOrder.MoveNum).Where(n => n != null).DefaultIfEmpty(0).Max() + 1
                        },

                    Customer = customer,
                    OrderStatus = SalesOrderStatus.Ordered
                };

            var setResult = SetSalesOrder(salesOrder, timestamp, parameters);
            if(!setResult.Success)
            {
                return setResult;
            }

            salesOrder = SalesUnitOfWork.SalesOrderRepository.Add(salesOrder);

            return new SuccessResult<SalesOrder>(salesOrder);
        }
    }

    internal class UpdateSalesOrderConductor : SalesOrderConductorBase
    {
        internal UpdateSalesOrderConductor(ISalesUnitOfWork salesUnitOfWork) : base(salesUnitOfWork) { }

        internal IResult<SalesOrder> Execute(DateTime timeStamp, UpdateSalesOrderCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var salesOrder = SalesUnitOfWork.SalesOrderRepository.FindByKey(parameters.SalesOrderKey,
                c => c.InventoryShipmentOrder.ShipmentInformation,
                c => c.Customer.Company.Contacts,
                c => c.Customer.Contracts.Select(n => n.ContractItems),
                c => c.SalesOrderItems.Select(i => i.InventoryPickOrderItem),
                c => c.SalesOrderItems.Select(i => i.ContractItem),
                c => c.SalesOrderItems.Select(i => i.PickedItems.Select(p => p.PickedInventoryItem)));
            if(salesOrder == null)
            {
                return new InvalidResult<SalesOrder>(null, string.Format(UserMessages.SalesOrderNotFound, parameters.SalesOrderKey.KeyValue));
            }

            salesOrder.CreditMemo = parameters.UpdateParameters.CreditMemo;
            return SetSalesOrder(salesOrder, timeStamp, parameters);
        }
    }

    internal abstract class SalesOrderConductorBase
    {
        protected readonly ISalesUnitOfWork SalesUnitOfWork;

        protected SalesOrderConductorBase(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            SalesUnitOfWork = salesUnitOfWork;
        }

        protected IResult<SalesOrder> SetSalesOrder(SalesOrder salesOrder, DateTime timeStamp, SetSalesOrderParametersBase parameters)
        {
            if(salesOrder == null) { throw new ArgumentNullException("salesOrder"); }
            if(timeStamp == null) { throw new ArgumentNullException("timeStamp"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(SalesUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<SalesOrder>();
            }
            
            if(parameters.BrokerKey != null)
            {
                if(salesOrder.BrokerId != parameters.BrokerKey.CompanyKey_Id)
                {
                    var broker = SalesUnitOfWork.CompanyRepository.FindByKey(parameters.BrokerKey, c => c.CompanyTypes);
                    if(broker == null)
                    {
                        return new InvalidResult<SalesOrder>(null, string.Format(UserMessages.CompanyNotFound, parameters.BrokerKey));
                    }

                    if(broker.CompanyTypes.All(t => t.CompanyTypeEnum != CompanyType.Broker))
                    {
                        return new InvalidResult<SalesOrder>(null, string.Format(UserMessages.CompanyNotOfType, parameters.BrokerKey, CompanyType.Broker));
                    }

                    salesOrder.Broker = null;
                    salesOrder.BrokerId = broker.Id;
                }
            }
            else
            {
                salesOrder.Broker = null;
                salesOrder.BrokerId = null;
            }

            salesOrder.InventoryShipmentOrder.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            salesOrder.InventoryShipmentOrder.TimeStamp = timeStamp;

            salesOrder.InventoryShipmentOrder.SourceFacility = null;
            salesOrder.InventoryShipmentOrder.SourceFacilityId = parameters.ShipFromFacilityKey.FacilityKey_Id;
            salesOrder.InventoryShipmentOrder.DestinationFacilityId = null;

            salesOrder.InventoryShipmentOrder.PurchaseOrderNumber = parameters.Parameters.HeaderParameters.CustomerPurchaseOrderNumber;
            salesOrder.InventoryShipmentOrder.DateReceived = parameters.Parameters.HeaderParameters.DateOrderReceived;
            salesOrder.InventoryShipmentOrder.RequestedBy = parameters.Parameters.HeaderParameters.OrderRequestedBy;
            salesOrder.InventoryShipmentOrder.TakenBy = parameters.Parameters.HeaderParameters.OrderTakenBy;

            salesOrder.PaymentTerms = parameters.Parameters.HeaderParameters.PaymentTerms;
            salesOrder.PreShipmentSampleRequired = parameters.Parameters.PreShipmentSampleRequired;
            salesOrder.InvoiceDate = parameters.Parameters.InvoiceDate;
            salesOrder.InvoiceNotes = parameters.Parameters.InvoiceNotes;
            salesOrder.FreightCharge = parameters.Parameters.FreightCharge;

            if(parameters.Parameters.SetShipmentInformation != null && parameters.Parameters.SetShipmentInformation.ShippingInstructions != null)
            {
                salesOrder.SetSoldTo(parameters.Parameters.SetShipmentInformation.ShippingInstructions.ShipFromOrSoldTo);
            }
            else
            {
                salesOrder.SetSoldTo(null);
            }
            salesOrder.InventoryShipmentOrder.ShipmentInformation.SetShipmentInformation(parameters.Parameters.SetShipmentInformation, false);

            if(parameters.OrderItems != null)
            {
                var setItemsResult = new SetSalesOrderItemsConductor(SalesUnitOfWork).SetOrderItems(salesOrder, parameters.OrderItems);
                if(!setItemsResult.Success)
                {
                    return setItemsResult.ConvertTo<SalesOrder>();
                }
            }

            return new SuccessResult<SalesOrder>(salesOrder);
        }
    }
}