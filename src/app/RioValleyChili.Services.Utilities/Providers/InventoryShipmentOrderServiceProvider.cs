using System;
using System.Collections.Generic;
using System.Linq;
using EF_Projectors.Extensions;
using EF_Split_Projector;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class InventoryShipmentOrderServiceProvider : IUnitOfWorkContainer<IInventoryShipmentOrderUnitOfWork>
    {
        #region Fields and Constructors.

        IInventoryShipmentOrderUnitOfWork IUnitOfWorkContainer<IInventoryShipmentOrderUnitOfWork>.UnitOfWork { get { return _inventoryShipmentOrderUnitOfWork; } }
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public InventoryShipmentOrderServiceProvider(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork, ITimeStamper timeStamper)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        #endregion

        public IResult<IQueryable<IShipmentOrderSummaryReturn>> GetShipments()
        {
            var query = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.All().SplitSelect(InventoryShipmentOrderProjectors.SplitSelectSummary());
            return new SuccessResult<IQueryable<IShipmentOrderSummaryReturn>>(query);
        }

        [SynchronizeOldContext(NewContextMethod.SetShipmentInformation)]
        public IResult SetShipmentInformation(ISetInventoryShipmentInformationParameters parameters)
        {
            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult;
            }

            var result = new SetShipmentInformationConductor(_inventoryShipmentOrderUnitOfWork).SetShipmentInformation(parametersResult.ResultingObject);
            if(!result.Success)
            {
                return result;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var key = new InventoryShipmentOrderKey(result.ResultingObject);
            return SyncParameters.Using(new SuccessResult<string>(key), key);
        }

        [SynchronizeOldContext(NewContextMethod.Post)]
        public IResult<string> Post(IPostParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<string>();
            }

            var postResult = new PostInventoryShipmentOrderConductor(_inventoryShipmentOrderUnitOfWork).Post(parsedParameters.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!postResult.Success)
            {
                return postResult.ConvertTo<string>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();
            var key = parsedParameters.ResultingObject.InventoryShipmentOrderKey;
            return SyncParameters.Using(new SuccessResult<string>(key), key);
        }

        public IResult<IInternalOrderAcknowledgementReturn> GetInventoryShipmentOrderAcknowledgement(string orderKey)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(orderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<IInternalOrderAcknowledgementReturn>();
            }

            var predicate = orderKeyResult.ResultingObject.ToInventoryShipmentOrderKey().FindByPredicate;
            var select = InventoryShipmentOrderProjectors.SplitSelectAcknowledgement(_inventoryShipmentOrderUnitOfWork.SalesOrderRepository.All());

            var order = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository
                .Filter(predicate)
                .SplitSelect(select).FirstOrDefault();
            if(order == null)
            {
                return new InvalidResult<IInternalOrderAcknowledgementReturn>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, orderKey));
            }

            return new SuccessResult<IInternalOrderAcknowledgementReturn>(order);
        }

        public IResult<ISalesOrderAcknowledgementReturn> GetCustomerOrderAcknowledgement(string orderKey)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<ISalesOrderKey>(orderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<ISalesOrderAcknowledgementReturn>();
            }

            var predicate = new SalesOrderKey(orderKeyResult.ResultingObject).FindByPredicate;
            var select = SalesOrderProjectors.SplitSelectAcknowledgement();

            var order = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository
                .Filter(predicate)
                .SplitSelect(select).FirstOrDefault();
            if(order == null)
            {
                return new InvalidResult<ISalesOrderAcknowledgementReturn>(null, string.Format(UserMessages.SalesOrderNotFound, orderKey));
            }

            return new SuccessResult<ISalesOrderAcknowledgementReturn>(order);
        }

        public IResult<IInventoryShipmentOrderPackingListReturn> GetInventoryShipmentOrderPackingList(string orderKey)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(orderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<IInventoryShipmentOrderPackingListReturn>();
            }

            var predicate = orderKeyResult.ResultingObject.ToInventoryShipmentOrderKey().FindByPredicate;
            var select = InventoryShipmentOrderProjectors.SplitSelectPackingList();

            var order = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.Filter(predicate).SplitSelect(select).FirstOrDefault();
            if(order == null)
            {
                return new InvalidResult<IInventoryShipmentOrderPackingListReturn>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, orderKey));
            }

            if(order.OrderType == InventoryShipmentOrderTypeEnum.SalesOrder)
            {
                var customerOrderKey = new SalesOrderKey(orderKeyResult.ResultingObject);
                var customerOrder = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository.FindByKey(customerOrderKey);
                if(customerOrder == null)
                {
                    return new InvalidResult<IInventoryShipmentOrderPackingListReturn>(null, string.Format(UserMessages.SalesOrderNotFound, customerOrder));
                }

                order.ShipFromOrSoldToShippingLabel = customerOrder.SoldTo;
            }

            return new SuccessResult<IInventoryShipmentOrderPackingListReturn>(order);
        }

        public IResult<IInventoryShipmentOrderBillOfLadingReturn> GetInventoryShipmentOrderBillOfLading(string orderKey)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(orderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<IInventoryShipmentOrderBillOfLadingReturn>();
            }

            var predicate = new InventoryShipmentOrderKey(orderKeyResult.ResultingObject).FindByPredicate;
            var select = InventoryShipmentOrderProjectors.SplitSelectBillOfLading();

            var order = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.Filter(predicate).SplitSelect(select).FirstOrDefault();
            if(order == null)
            {
                return new InvalidResult<IInventoryShipmentOrderBillOfLadingReturn>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, orderKey));
            }

            return new SuccessResult<IInventoryShipmentOrderBillOfLadingReturn>(order);
        }

        public IResult<IInventoryShipmentOrderPickSheetReturn> GetInventoryShipmentOrderPickSheet(string orderKey)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(orderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<IInventoryShipmentOrderPickSheetReturn>();
            }

            var predicate = new InventoryShipmentOrderKey(orderKeyResult.ResultingObject).FindByPredicate;
            var select = InventoryShipmentOrderProjectors.SplitSelectPickSheet().Merge();
            
            var order = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.Filter(predicate).Select(select).FirstOrDefault();
            if(order == null)
            {
                return new InvalidResult<IInventoryShipmentOrderPickSheetReturn>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, orderKey));
            }

            if(order.OrderType == InventoryShipmentOrderTypeEnum.SalesOrder)
            {
                var customerOrderKey = new SalesOrderKey(orderKeyResult.ResultingObject);
                var customerOrder = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository.FindByKey(customerOrderKey);
                if(customerOrder == null)
                {
                    return new InvalidResult<IInventoryShipmentOrderPickSheetReturn>(null, string.Format(UserMessages.SalesOrderNotFound, customerOrderKey));
                }

                order.ShipFromOrSoldToShippingLabel = customerOrder.SoldTo;
            }

            return new SuccessResult<IInventoryShipmentOrderPickSheetReturn>(order);
        }

        public IResult<IInventoryShipmentOrderCertificateOfAnalysisReturn> GetInventoryShipmentOrderCertificateOfAnalysis(string orderKey)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(orderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<IInventoryShipmentOrderCertificateOfAnalysisReturn>();
            }

            var predicate = orderKeyResult.ResultingObject.ToInventoryShipmentOrderKey().FindByPredicate;
            var select = InventoryShipmentOrderProjectors.SplitSelectCertificateOfAnalysisReturn();

            var order = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.Filter(predicate).SplitSelect(select).FirstOrDefault();
            if(order == null)
            {
                return new InvalidResult<IInventoryShipmentOrderCertificateOfAnalysisReturn>(null, string.Format(UserMessages.InventoryShipmentOrderNotFound, orderKey));
            }

            if(order.OrderType == InventoryShipmentOrderTypeEnum.SalesOrder)
            {
                var customerOrderKey = new SalesOrderKey(orderKeyResult.ResultingObject);
                var customerOrder = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository.FindByKey(customerOrderKey,
                    o => o.Customer.Company);
                if(customerOrder == null)
                {
                    return new InvalidResult<IInventoryShipmentOrderCertificateOfAnalysisReturn>(null, string.Format(UserMessages.SalesOrderNotFound, customerOrderKey));
                }

                order.DestinationName = customerOrder.Customer == null ? null : customerOrder.Customer.Company.Name;
            }

            return new SuccessResult<IInventoryShipmentOrderCertificateOfAnalysisReturn>(order);
        }

        public IResult<IPendingOrderDetails> GetPendingOrderDetails(DateTime startDate, DateTime endDate)
        {
            return new PendingOrderDetailsConductor(_inventoryShipmentOrderUnitOfWork).Get(startDate, endDate);
        }

        public IResult<ISalesOrderInvoice> GetCustomerOrderInvoice(string orderKey)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<ISalesOrderKey>(orderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<ISalesOrderInvoice>();
            }

            return new CustomerOrderInvoiceConductor(_inventoryShipmentOrderUnitOfWork).Get(orderKeyResult.ResultingObject.ToSalesOrderKey());
        }

        //todo: Void all order types in one method? How to synch? -RI 2015/11/18
        public IResult VoidOrder(string orderKey)
        {
            throw new NotImplementedException();
        }

        public IResult<IEnumerable<string>> GetShipmentMethods()
        {
            var shipmentMethods = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository
                .All()
                .Select(o => o.ShipmentInformation.ShipmentMethod)
                .Distinct()
                .ToList()
                .Where(s => !string.IsNullOrWhiteSpace(s));
            return new SuccessResult<IEnumerable<string>>(shipmentMethods);
        }
    }
}