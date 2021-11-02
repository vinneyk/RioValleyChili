using System;
using System.Linq;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class SetCustomerChileProductAttributeRangeCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        public SetCustomerChileProductAttributeRangeCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        public IResult Execute(DateTime timeStamp, SetCustomerProductAttributeRangesParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters "); }

            var employeeResult = new GetEmployeeCommand(_salesUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult;
            }

            var predicate = CustomerProductAttributeRangePredicates.ByCustomerProduct(parameters.CustomerKey, parameters.ChileProductKey);
            var existing = _salesUnitOfWork.CustomerProductAttributeRangeRepository.Filter(predicate)
                .ToList()
                .ToDictionary(r => r.AttributeShortName);
            foreach(var item in parameters.AttributeRanges)
            {
                bool update;
                CustomerProductAttributeRange range;
                if(!existing.TryGetValue(item.AttributeNameKey.AttributeNameKey_ShortName, out range))
                {
                    update = true;
                    range = _salesUnitOfWork.CustomerProductAttributeRangeRepository.Add(new CustomerProductAttributeRange
                        {
                            CustomerId = parameters.CustomerKey.CustomerKey_Id,
                            ChileProductId = parameters.ChileProductKey.ChileProductKey_ProductId,
                            AttributeShortName = item.AttributeNameKey.AttributeNameKey_ShortName,
                            Active = true
                        });
                }
                else
                {
                    existing.Remove(item.AttributeNameKey.AttributeNameKey_ShortName);
                    update = range.RangeMin != item.Parameters.RangeMin || range.RangeMax != item.Parameters.RangeMax;
                }

                if(update)
                {
                    range.EmployeeId = employeeResult.ResultingObject.EmployeeId;
                    range.RangeMin = item.Parameters.RangeMin;
                    range.RangeMax = item.Parameters.RangeMax;
                    range.TimeStamp = timeStamp;
                }
            }

            foreach(var item in existing.Values)
            {
                _salesUnitOfWork.CustomerProductAttributeRangeRepository.Remove(item);
            }

            return new SuccessResult<CustomerProductAttributeRange>();
        }
    }
}