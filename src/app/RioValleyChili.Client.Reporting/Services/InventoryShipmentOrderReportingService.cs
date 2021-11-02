using System;
using System.ComponentModel;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public class InventoryShipmentOrderReportingService : ReportingService
    {
        private readonly IInventoryShipmentOrderService _inventoryShipmentOrderService;

        public InventoryShipmentOrderReportingService()
            : this(ResolveService<IInventoryShipmentOrderService>()) { }

        public InventoryShipmentOrderReportingService(IInventoryShipmentOrderService inventoryShipmentOrderService)
        {
            if(inventoryShipmentOrderService == null) { throw new ArgumentNullException("inventoryShipmentOrderService"); }
            _inventoryShipmentOrderService = inventoryShipmentOrderService;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public InternalOrderAcknowledgement GetWarehouseOrderAcknowledgement(string orderKey)
        {
            var result = _inventoryShipmentOrderService.GetInhouseShipmentOrderAcknowledgement(orderKey);
            var mapped = result.ResultingObject.Map().To<InternalOrderAcknowledgement>();
            return mapped;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public SalesOrderAcknowledgement GetCustomerOrderAcknowledgement(string orderKey)
        {
            var result = _inventoryShipmentOrderService.GetCustomerOrderAcknowledgement(orderKey);
            var mapped = result.ResultingObject.Map().To<SalesOrderAcknowledgement>();
            return mapped;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public WarehouseOrderPackingList GetPackingList(string orderKey)
        {
            var result = _inventoryShipmentOrderService.GetInventoryShipmentOrderPackingList(orderKey);
            var mapped = result.ResultingObject.Map().To<WarehouseOrderPackingList>();
            return mapped;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public WarehouseOrderBillOfLading GetBillOfLading(string orderKey)
        {
            var result = _inventoryShipmentOrderService.GetInventoryShipmentOrderBillOfLading(orderKey);
            var mapped = result.ResultingObject.Map().To<WarehouseOrderBillOfLading>();
            return mapped;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public WarehouseOrderPickSheet GetPickSheet(string orderKey)
        {
            var result = _inventoryShipmentOrderService.GetInventoryShipmentOrderPickSheet(orderKey);
            var mapped = result.ResultingObject.Map().To<WarehouseOrderPickSheet>();
            return mapped;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public InventoryShipmentOrderCertificateOfAnalysis GetCertificateOfAnalysis(string orderKey)
        {
            var result = _inventoryShipmentOrderService.GetInventoryShipmentOrderCertificateOfAnalysis(orderKey);
            var mapped = result.ResultingObject.Map().To<InventoryShipmentOrderCertificateOfAnalysis>();
            return mapped;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PendingOrderDetails GetPendingOrderDetails(DateTime startDate, DateTime endDate)
        {
            var result = _inventoryShipmentOrderService.GetPendingOrderDetails(startDate, endDate);
            var mapped = result.ResultingObject.Map().To<PendingOrderDetails>();
            return mapped;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public SalesOrderInvoice GetCustomerOrderInvoice(string orderKey)
        {
            var result = _inventoryShipmentOrderService.GetCustomerOrderInvoice(orderKey);
            var mapped = result.ResultingObject.Map().To<SalesOrderInvoice>();
            return mapped;
        }
    }
}