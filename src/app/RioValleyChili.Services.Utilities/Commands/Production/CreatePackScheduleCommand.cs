using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Helpers;
using RioValleyChili.Services.Utilities.Commands.Company;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;
using IProductionUnitOfWork = RioValleyChili.Data.Interfaces.UnitsOfWork.IProductionUnitOfWork;

namespace RioValleyChili.Services.Utilities.Commands.Production
{
    internal class CreatePackScheduleCommand
    {
        private readonly IProductionUnitOfWork _productionUnitOfWork;

        public CreatePackScheduleCommand(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        public IResult<PackSchedule> Execute(DateTime timestamp, CreatePackScheduleCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var dateCreated = parameters.Parameters.DateCreated?.Date ?? timestamp.Date;
            var sequence = parameters.Parameters.Sequence ?? new EFUnitOfWorkHelper(_productionUnitOfWork).GetNextSequence<PackSchedule>(p => p.DateCreated == dateCreated, p => p.SequentialNumber);
            var employeeResult = new GetEmployeeCommand(_productionUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<PackSchedule>();
            }

            Data.Models.Company customer = null;
            if(parameters.CustomerKey != null)
            {
                var companyResult = new GetCompanyCommand(_productionUnitOfWork).Execute(parameters.CustomerKey, CompanyType.Customer);
                if(!companyResult.Success)
                {
                    return companyResult.ConvertTo<PackSchedule>();
                }
                customer = companyResult.ResultingObject;
            }

            var packSchedule = _productionUnitOfWork.PackScheduleRepository.Add(new PackSchedule
                {
                    EmployeeId = employeeResult.ResultingObject.EmployeeId,
                    TimeStamp = timestamp,
                    PackSchID = DataConstants.SqlMinDate,
                    PSNum = parameters.Parameters.PSNum ?? _productionUnitOfWork.PackScheduleRepository.SourceQuery.Select(p => p.PSNum).Where(p => p != null).DefaultIfEmpty(0).Max().Value + 1,

                    DateCreated = dateCreated,
                    SequentialNumber = sequence,

                    WorkTypeId = parameters.WorkTypeKey.WorkTypeKey_WorkTypeId,
                    ChileProductId = parameters.ChileProductKey.ChileProductKey_ProductId,
                    PackagingProductId = parameters.PackagingProductKey.PackagingProductKey_ProductId,
                    ScheduledProductionDate = parameters.Parameters.ScheduledProductionDate,
                    ProductionDeadline = parameters.Parameters.ProductionDeadline,
                    ProductionLineLocationId = parameters.ProductionLocationKey.LocationKey_Id,
                    SummaryOfWork = parameters.Parameters.SummaryOfWork ?? "",
                    CustomerId = customer == null ? (int?)null : customer.Id,
                    OrderNumber = parameters.Parameters.OrderNumber.TrimTruncate(Constants.StringLengths.OrderNumber),

                    DefaultBatchTargetParameters = new ProductionBatchTargetParameters(parameters.Parameters)
                });

            return new SuccessResult<PackSchedule>(packSchedule);
        }
    }
}