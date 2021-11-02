using System;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class CustomerOrderExtensions
    {
        internal static SalesOrder ConstrainByKeys(this SalesOrder order, ICustomerKey customerKey, ICompanyKey brokerKey = null)
        {
            if(order == null) { throw new ArgumentNullException("order"); }

            if(customerKey != null)
            {
                order.Customer = null;
                order.CustomerId = customerKey.CustomerKey_Id;
            }

            if(brokerKey != null)
            {
                order.Broker = null;
                order.BrokerId = brokerKey.CompanyKey_Id;
            }

            return order;
        }

        internal static SalesOrder SetShipFromWarehouse(this SalesOrder salesOrder, IFacilityKey facilityKey)
        {
            if(salesOrder == null) { throw new ArgumentNullException("salesOrder"); }
            if(facilityKey == null) { throw new ArgumentNullException("facilityKey"); }

            salesOrder.InventoryShipmentOrder.SourceFacility = null;
            salesOrder.InventoryShipmentOrder.SourceFacilityId = facilityKey.FacilityKey_Id;

            return salesOrder;
        }

        internal static void AssertEqual(this SalesOrder expected, ISalesOrderSummaryReturn result)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }
            if(result == null) { throw new ArgumentNullException("result"); }

            Assert.AreEqual(expected.ToSalesOrderKey().KeyValue, result.MovementKey);
            Assert.AreEqual(expected.InventoryShipmentOrder.MoveNum, result.MoveNum);
            Assert.AreEqual(expected.OrderStatus, result.SalesOrderStatus);
            Assert.AreEqual(expected.PaymentTerms, result.PaymentTerms);
            Assert.AreEqual(expected.InventoryShipmentOrder.DateReceived, result.DateOrderReceived);
            Assert.AreEqual(expected.CreditMemo, result.CreditMemo);
            Assert.AreEqual(expected.ToCustomerKey().KeyValue, result.Customer.CompanyKey);
            Assert.AreEqual(expected.Broker.ToCompanyKey().KeyValue, result.Broker.CompanyKey);
        }

        internal static void AssertEqual(this SalesOrder expected, ISalesOrderDetailReturn result)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }
            if(result == null) { throw new ArgumentNullException("result"); }

            Assert.AreEqual(expected.ToSalesOrderKey().KeyValue, result.MovementKey);
            Assert.AreEqual(expected.InventoryShipmentOrder.MoveNum, result.MoveNum);
            Assert.AreEqual(expected.Broker.ToCompanyKey().KeyValue, result.Broker.CompanyKey);
            Assert.AreEqual(expected.InventoryShipmentOrder.SourceFacility.ToFacilityKey().KeyValue, result.OriginFacility.FacilityKey);
            Assert.AreEqual(expected.InventoryShipmentOrder.ToShipmentInformationKey().KeyValue, result.Shipment.ShipmentKey);
            Assert.AreEqual(InventoryOrderEnum.CustomerOrder, result.Shipment.InventoryOrderEnum);
            Assert.AreEqual(expected.OrderStatus, result.SalesOrderStatus);
            Assert.AreEqual(expected.InventoryShipmentOrder.TakenBy, result.OrderTakenBy);
            Assert.AreEqual(expected.InventoryShipmentOrder.DateReceived, result.DateOrderReceived);
            Assert.AreEqual(expected.PaymentTerms, result.PaymentTerms);
            Assert.AreEqual(expected.CreditMemo, result.CreditMemo);
            Assert.AreEqual(expected.InvoiceDate, result.InvoiceDate);
            Assert.AreEqual(expected.InvoiceNotes, result.InvoiceNotes);
            Assert.AreEqual(expected.FreightCharge, result.FreightCharge);
            Assert.AreEqual(expected.InventoryShipmentOrder.PurchaseOrderNumber, result.PurchaseOrderNumber);
            Assert.AreEqual(expected.InventoryShipmentOrder.ShipmentInformation.ShipmentMethod, result.Shipment.TransitInformation.ShipmentMethod);
            expected.SoldTo.AssertEqual(result.ShipFromReplace);
        }

        internal static void AssertEqual(this SalesOrder expected, ICustomerContractOrderReturn result)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }
            if(result == null) { throw new ArgumentNullException("result"); }

            Assert.AreEqual(expected.ToSalesOrderKey().KeyValue, result.CustomerOrderKey);
            Assert.AreEqual(expected.OrderStatus, result.SalesOrderStatus);
            Assert.AreEqual(expected.InventoryShipmentOrder.ShipmentInformation.Status, result.ShipmentStatus);
        }

        internal static UpdateSalesOrderParameters ToUpdateParameters(this SalesOrder salesOrder, Employee user = null, Action<UpdateSalesOrderParameters> initialize = null)
        {
            var parameters = new UpdateSalesOrderParameters
                {
                    UserToken = user == null ? null : user.UserName,
                    SalesOrderKey = salesOrder.ToSalesOrderKey(),
                    BrokerKey = salesOrder.Broker.ToCompanyKey(),
                    FacilitySourceKey = salesOrder.InventoryShipmentOrder.SourceFacility.ToFacilityKey(),
                    PreShipmentSampleRequired = salesOrder.PreShipmentSampleRequired,
                    HeaderParameters = salesOrder.InventoryShipmentOrder.ToOrderHeaderParameters(),
                    SetShipmentInformation = salesOrder.InventoryShipmentOrder.ShipmentInformation.ToSetShipmentInformationWithStatus(),
                    OrderItems = salesOrder.SalesOrderItems == null ? null : salesOrder.SalesOrderItems.Select(i => i.ToCustomerOrderItemParameters())
                };

            if(initialize != null)
            {
                initialize(parameters);
            }

            return parameters;
        }
    }
}