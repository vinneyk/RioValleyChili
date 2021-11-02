using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Company;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Commands.Shipment;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetSalesQuoteConductor
    {
        internal SetSalesQuoteConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;
        }

        internal IResult<SalesQuote> Execute(DateTime timestamp, SalesQuoteParameters parameters)
        {
            var employeeResult = new GetEmployeeCommand(_inventoryShipmentOrderUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<SalesQuote>();
            }

            SalesQuote salesQuote;
            if(parameters.SalesQuoteNumber == null)
            {
                var date = timestamp.Date;
                var shipmentInfoResult = new CreateShipmentInformationCommand(_inventoryShipmentOrderUnitOfWork).Execute(date);
                if(!shipmentInfoResult.Success)
                {
                    return shipmentInfoResult.ConvertTo<SalesQuote>();
                }

                var sequence = new EFUnitOfWorkHelper(_inventoryShipmentOrderUnitOfWork).GetNextSequence<SalesQuote>(q => q.DateCreated == date, q => q.Sequence);
                var shipmentInformation = shipmentInfoResult.ResultingObject;
                var quoteNum = (date.Year * 100) - 1;
                salesQuote = _inventoryShipmentOrderUnitOfWork.SalesQuoteRepository.Add(new SalesQuote
                    {
                        DateCreated = date,
                        Sequence = sequence,
                        ShipmentInfoDateCreated = shipmentInformation.DateCreated,
                        ShipmentInfoSequence = shipmentInformation.Sequence,
                        ShipmentInformation = shipmentInformation,
                        Items = new List<SalesQuoteItem>(),

                        QuoteNum = _inventoryShipmentOrderUnitOfWork.SalesQuoteRepository.SourceQuery
                            .Select(q => q.QuoteNum)
                            .Where(q => q != null && q > quoteNum)
                            .DefaultIfEmpty(quoteNum)
                            .Max() + 1
                    });
            }
            else
            {
                salesQuote = _inventoryShipmentOrderUnitOfWork.SalesQuoteRepository
                    .Filter(q => q.QuoteNum == parameters.SalesQuoteNumber,
                        q => q.ShipmentInformation,
                        q => q.Items)
                    .FirstOrDefault();
                if(salesQuote == null)
                {
                    return new InvalidResult<SalesQuote>(null, string.Format(UserMessages.SalesQuoteNotFound_Num, parameters.SalesQuoteNumber));
                }
            }

            Facility sourceFacility = null;
            if(parameters.SourceFacilityKey != null)
            {
                sourceFacility = _inventoryShipmentOrderUnitOfWork.FacilityRepository.FindByKey(parameters.SourceFacilityKey);
                if(sourceFacility == null)
                {
                    return new InvalidResult<SalesQuote>(null, string.Format(UserMessages.FacilityNotFound, parameters.SourceFacilityKey));
                }
            }

            Company customer = null;
            var getCompanyCommand = new GetCompanyCommand(_inventoryShipmentOrderUnitOfWork);
            if(parameters.CustomerKey != null)
            {
                var customerResult = getCompanyCommand.Execute(parameters.CustomerKey.ToCompanyKey(), CompanyType.Customer);
                if(!customerResult.Success)
                {
                    return customerResult.ConvertTo<SalesQuote>();
                }
                customer = customerResult.ResultingObject;
            }

            Company broker = null;
            if(parameters.BrokerKey != null)
            {
                var brokerResult = getCompanyCommand.Execute(parameters.BrokerKey, CompanyType.Broker);
                if(!brokerResult.Success)
                {
                    return brokerResult.ConvertTo<SalesQuote>();
                }
                broker = brokerResult.ResultingObject;
            }

            salesQuote.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            salesQuote.TimeStamp = timestamp;

            salesQuote.SourceFacilityId = sourceFacility == null ? (int?) null : sourceFacility.Id;
            salesQuote.CustomerId = customer == null ? (int?) null : customer.CompanyKey_Id;
            salesQuote.BrokerId = broker == null ? (int?) null : broker.CompanyKey_Id;

            salesQuote.QuoteDate = parameters.Parameters.QuoteDate;
            salesQuote.DateReceived = parameters.Parameters.DateReceived;
            salesQuote.CalledBy = parameters.Parameters.CalledBy;
            salesQuote.TakenBy = parameters.Parameters.TakenBy;
            salesQuote.PaymentTerms = parameters.Parameters.PaymentTerms;

            ShippingLabel shipFromOrSoldTo = null;
            if(parameters.Parameters.ShipmentInformation != null && parameters.Parameters.ShipmentInformation.ShippingInstructions != null)
            {
                shipFromOrSoldTo = parameters.Parameters.ShipmentInformation.ShippingInstructions.ShipFromOrSoldTo;
            }
            salesQuote.SoldTo.SetShippingLabel(shipFromOrSoldTo);
            salesQuote.ShipmentInformation.SetShipmentInformation(parameters.Parameters.ShipmentInformation, false);

            var itemSequence = 0;
            var itemsToRemove = salesQuote.Items.ToDictionary(i => i.ToSalesQuoteItemKey(), i =>
                {
                    itemSequence = Math.Max(itemSequence, i.ItemSequence);
                    return i;
                });
            foreach(var item in parameters.Items)
            {
                SalesQuoteItem salesQuoteItem = null;
                if(item.SalesQuoteItemKey != null)
                {
                    if(itemsToRemove.TryGetValue(item.SalesQuoteItemKey, out salesQuoteItem))
                    {
                        itemsToRemove.Remove(item.SalesQuoteItemKey);
                    }
                    else
                    {
                        return new InvalidResult<SalesQuote>(null, string.Format(UserMessages.SalesQuoteItemNotFound, item.SalesQuoteItemKey));
                    }
                }
                else
                {
                    salesQuote.Items.Add(salesQuoteItem = new SalesQuoteItem
                        {
                            DateCreated = salesQuote.DateCreated,
                            Sequence = salesQuote.Sequence,
                            ItemSequence = ++itemSequence
                        });
                }

                salesQuoteItem.ProductId = item.ProductKey.ProductKey_ProductId;
                salesQuoteItem.PackagingProductId = item.PackagingProductKey.PackagingProductKey_ProductId;
                salesQuoteItem.TreatmentId = item.InventoryTreatmentKey.InventoryTreatmentKey_Id;
                salesQuoteItem.Quantity = item.Parameters.Quantity;
                salesQuoteItem.PriceBase = item.Parameters.PriceBase;
                salesQuoteItem.PriceFreight = item.Parameters.PriceFreight;
                salesQuoteItem.PriceTreatment = item.Parameters.PriceTreatment;
                salesQuoteItem.PriceWarehouse = item.Parameters.PriceWarehouse;
                salesQuoteItem.PriceRebate = item.Parameters.PriceRebate;
                salesQuoteItem.CustomerProductCode = item.Parameters.CustomerProductCode;
            }

            foreach(var item in itemsToRemove.Values)
            {
                _inventoryShipmentOrderUnitOfWork.SalesQuoteItemRepository.Remove(item);
            }

            return new SuccessResult<SalesQuote>(salesQuote);
        }

        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;
    }
}