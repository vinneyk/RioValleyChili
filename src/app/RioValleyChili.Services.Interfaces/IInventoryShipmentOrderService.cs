using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IInventoryShipmentOrderService
    {
        IResult<IQueryable<IShipmentOrderSummaryReturn>> GetShipments();
        IResult SetShipmentInformation(ISetInventoryShipmentInformationParameters parameters);
        IResult Post(IPostParameters parameters);
        IResult<IInternalOrderAcknowledgementReturn> GetInhouseShipmentOrderAcknowledgement(string orderKey);
        IResult<ISalesOrderAcknowledgementReturn> GetCustomerOrderAcknowledgement(string orderKey);
        IResult<IInventoryShipmentOrderPackingListReturn> GetInventoryShipmentOrderPackingList(string orderKey);
        IResult<IInventoryShipmentOrderBillOfLadingReturn> GetInventoryShipmentOrderBillOfLading(string orderKey);
        IResult<IInventoryShipmentOrderPickSheetReturn> GetInventoryShipmentOrderPickSheet(string orderKey);
        IResult<IInventoryShipmentOrderCertificateOfAnalysisReturn> GetInventoryShipmentOrderCertificateOfAnalysis(string orderKey);
        IResult<IPendingOrderDetails> GetPendingOrderDetails(DateTime startDate, DateTime endDate);
        IResult<ISalesOrderInvoice> GetCustomerOrderInvoice(string orderKey);
        IResult VoidOrder(string orderKey);
        IResult<IEnumerable<string>> GetShipmentMethods();
    }
}