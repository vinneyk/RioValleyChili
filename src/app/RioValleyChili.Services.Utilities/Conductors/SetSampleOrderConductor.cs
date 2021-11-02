using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetSampleOrderConductor
    {
        private readonly ISampleOrderUnitOfWork _sampleOrderUnitOfWork;

        internal SetSampleOrderConductor(ISampleOrderUnitOfWork sampleOrderUnitOfWork)
        {
            _sampleOrderUnitOfWork = sampleOrderUnitOfWork;
        }

        internal IResult<SampleOrder> Execute(DateTime timeStamp, SetSampleOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_sampleOrderUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<SampleOrder>();
            }

            SampleOrder sampleOrder;
            if(parameters.SampleOrderKey != null)
            {
                sampleOrder = _sampleOrderUnitOfWork.SampleOrderRepository.FindByKey(parameters.SampleOrderKey,
                    o => o.Items.Select(i => i.Spec),
                    o => o.Items.Select(i => i.Match));
                if(sampleOrder == null)
                {
                    return new InvalidResult<SampleOrder>(null, string.Format(UserMessages.SampleOrderNotFound, parameters.SampleOrderKey));
                }
            }
            else
            {
                var year = timeStamp.Year;
                var sequence = new EFUnitOfWorkHelper(_sampleOrderUnitOfWork).GetNextSequence<SampleOrder>(o => o.Year == year, o => o.Sequence);

                sampleOrder = _sampleOrderUnitOfWork.SampleOrderRepository.Add(new SampleOrder
                    {
                        Year = year,
                        Sequence = sequence,

                        Items = new List<SampleOrderItem>()
                    });
            }

            return SetSampleOrder(employeeResult.ResultingObject, timeStamp, sampleOrder, parameters);
        }

        private IResult<SampleOrder> SetSampleOrder(Employee employee, DateTime timeStamp, SampleOrder sampleOrder, SetSampleOrderParameters parameters)
        {
            Customer requestCustomer = null;
            if(parameters.RequestCustomerKey != null)
            {
                requestCustomer = _sampleOrderUnitOfWork.CustomerRepository.FindByKey(parameters.RequestCustomerKey);
                if(requestCustomer == null)
                {
                    return new InvalidResult<SampleOrder>(null, string.Format(UserMessages.CustomerNotFound, parameters.RequestCustomerKey));
                }
            }

            Company broker = null;
            if(parameters.BrokerKey != null)
            {
                broker = _sampleOrderUnitOfWork.CompanyRepository.FindByKey(parameters.BrokerKey, b => b.CompanyTypes);
                if(broker == null)
                {
                    return new InvalidResult<SampleOrder>(null, string.Format(UserMessages.CompanyNotFound, parameters.BrokerKey));
                }

                if(broker.CompanyTypes.All(t => t.CompanyTypeEnum != CompanyType.Broker))
                {
                    return new InvalidResult<SampleOrder>(null, string.Format(UserMessages.CompanyNotOfType, parameters.BrokerKey, CompanyType.Broker));
                }
            }
            
            sampleOrder.EmployeeId = employee.EmployeeId;
            sampleOrder.TimeStamp = timeStamp;

            sampleOrder.DateDue = parameters.Parameters.DateDue ?? parameters.Parameters.DateReceived;
            sampleOrder.DateReceived = parameters.Parameters.DateReceived;
            sampleOrder.DateCompleted = parameters.Parameters.DateCompleted;

            sampleOrder.Status = parameters.Parameters.Status;
            sampleOrder.Active = parameters.Parameters.Active;

            sampleOrder.Comments = parameters.Parameters.Comments;
            sampleOrder.PrintNotes = parameters.Parameters.PrintNotes;
            sampleOrder.Volume = parameters.Parameters.Volume;

            sampleOrder.ShipmentMethod = parameters.Parameters.ShipmentMethod;
            sampleOrder.FOB = parameters.Parameters.FOB;
            sampleOrder.ShipToCompany = parameters.Parameters.ShipToCompany;
            sampleOrder.ShipTo = new ShippingLabel().SetShippingLabel(parameters.Parameters.ShipToShippingLabel);
            sampleOrder.Request = new ShippingLabel().SetShippingLabel(parameters.Parameters.RequestedByShippingLabel);
            
            sampleOrder.RequestCustomerId = requestCustomer == null ? (int?) null : requestCustomer.Id;
            sampleOrder.BrokerId = broker == null ? (int?) null : broker.Id;

            var itemSequence = sampleOrder.Items.Select(i => i.ItemSequence).DefaultIfEmpty(0).Max() + 1;
            var existingItems = sampleOrder.Items.ToList();
            foreach(var item in parameters.Items)
            {
                SampleOrderItem orderItem;
                if(item.SampleOrderItemKey != null)
                {
                    orderItem = existingItems.FirstOrDefault(item.SampleOrderItemKey.FindByPredicate.Compile());
                    if(orderItem == null)
                    {
                        return new InvalidResult<SampleOrder>(null, string.Format(UserMessages.SampleOrderItemNotFound, item.SampleOrderItemKey));
                    }

                    existingItems.Remove(orderItem);
                }
                else
                {
                    orderItem = _sampleOrderUnitOfWork.SampleOrderItemRepository.Add(new SampleOrderItem
                        {
                            SampleOrderYear = sampleOrder.Year,
                            SampleOrderSequence = sampleOrder.Sequence,
                            ItemSequence = itemSequence++
                        });
                }

                Product product = null;
                if(item.ProductKey != null)
                {
                    product = _sampleOrderUnitOfWork.ProductRepository.FindByKey(item.ProductKey);
                    if(product == null)
                    {
                        return new InvalidResult<SampleOrder>(null, string.Format(UserMessages.ProductNotFound, item.ProductKey));
                    }
                }

                Lot lot = null;
                if(item.LotKey != null)
                {
                    lot = _sampleOrderUnitOfWork.LotRepository.FindByKey(item.LotKey);
                    if(lot == null)
                    {
                        return new InvalidResult<SampleOrder>(null, string.Format(UserMessages.LotNotFound, item.LotKey));
                    }
                }

                orderItem.Quantity = item.Parameters.Quantity;
                orderItem.Description = item.Parameters.Description;
                orderItem.CustomerProductName = item.Parameters.CustomerProductName;

                orderItem.ProductId = product == null ? (int?)null : product.Id;

                if(lot == null)
                {
                    orderItem.LotDateCreated = null;
                    orderItem.LotDateSequence = null;
                    orderItem.LotTypeId = null;
                }
                else
                {
                    orderItem.LotDateCreated = lot.LotDateCreated;
                    orderItem.LotDateSequence = lot.LotDateSequence;
                    orderItem.LotTypeId = lot.LotTypeId;
                }
            }

            foreach(var item in existingItems)
            {
                if(item.Match != null)
                {
                    _sampleOrderUnitOfWork.SampleOrderItemMatchRepository.Remove(item.Match);
                }

                if(item.Spec != null)
                {
                    _sampleOrderUnitOfWork.SampleOrderItemSpecRepository.Remove(item.Spec);
                }

                _sampleOrderUnitOfWork.SampleOrderItemRepository.Remove(item);
            }

            return new SuccessResult<SampleOrder>(sampleOrder);
        }
    }
}