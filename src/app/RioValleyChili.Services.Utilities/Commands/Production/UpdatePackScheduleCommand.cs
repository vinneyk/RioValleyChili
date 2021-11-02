using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Company;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Production
{
    internal class UpdatePackScheduleCommand
    {
        private readonly IProductionUnitOfWork _productionUnitOfWork;

        public UpdatePackScheduleCommand(IProductionUnitOfWork productionUnitOfWork)
        {
            if (productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        public IResult<PackSchedule> Execute(DateTime timestamp, UpdatePackScheduleCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            
            var packSchedule = _productionUnitOfWork.PackScheduleRepository.FindByKey(parameters.PackScheduleKey,
                p => p.ChileProduct,
                p => p.ProductionBatches.Select(b => b.Production.ResultingChileLot.Lot));
            if(packSchedule == null)
            {
                return new InvalidResult<PackSchedule>(null, string.Format(UserMessages.PackScheduleNotFound, parameters.PackScheduleKey.KeyValue));
            }

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

            if(!parameters.ChileProductKey.Equals(packSchedule))
            {
                if(packSchedule.ProductionBatches.Any())
                {
                    var chileProduct = _productionUnitOfWork.ChileProductRepository.FindByKey(parameters.ChileProductKey);
                    if(chileProduct == null)
                    {
                        return new InvalidResult<PackSchedule>(null, string.Format(UserMessages.ChileProductNotFound, parameters.ChileProductKey));
                    }

                    if(chileProduct.ChileState.ToLotType() != packSchedule.ChileProduct.ChileState.ToLotType())
                    {
                        return new InvalidResult<PackSchedule>(null, UserMessages.ChileProductDifferentLotType);
                    }

                    var completedBatch = packSchedule.ProductionBatches.FirstOrDefault(b => b.ProductionHasBeenCompleted);
                    if(completedBatch != null)
                    {
                        return new InvalidResult<PackSchedule>(null, string.Format(UserMessages.ProductionBatchAlreadyComplete, new LotKey(completedBatch)));
                    }

                    foreach(var batch in packSchedule.ProductionBatches)
                    {
                        batch.Production.ResultingChileLot.ChileProductId = parameters.ChileProductKey.ChileProductKey_ProductId;
                    }
                }
                packSchedule.ChileProductId = parameters.ChileProductKey.ChileProductKey_ProductId;
            }

            if(!parameters.PackagingProductKey.Equals(packSchedule))
            {
                foreach(var batch in packSchedule.ProductionBatches)
                {
                    batch.Production.ResultingChileLot.Lot.ReceivedPackagingProductId = parameters.PackagingProductKey.PackagingProductKey_ProductId;
                }
                packSchedule.PackagingProductId = parameters.PackagingProductKey.PackagingProductKey_ProductId;
            }

            packSchedule.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            packSchedule.TimeStamp = timestamp;

            packSchedule.ScheduledProductionDate = parameters.Parameters.ScheduledProductionDate;
            packSchedule.ProductionDeadline = parameters.Parameters.ProductionDeadline;
            packSchedule.ProductionLineLocationId = parameters.ProductionLocationKey.LocationKey_Id;
            packSchedule.SummaryOfWork = parameters.Parameters.SummaryOfWork;
            packSchedule.WorkTypeId = parameters.WorkTypeKey.WorkTypeKey_WorkTypeId;
            packSchedule.CustomerId = customer == null ? (int?) null : customer.Id;
            packSchedule.OrderNumber = parameters.Parameters.OrderNumber.TrimTruncate(Constants.StringLengths.OrderNumber);

            packSchedule.DefaultBatchTargetParameters = new ProductionBatchTargetParameters(parameters.Parameters);

            return new SuccessResult<PackSchedule>(packSchedule);
        }
    }
}