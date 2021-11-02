using System;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Production
{
    internal class UpdateProductionBatchCommand
    {
        private readonly Data.Interfaces.UnitsOfWork.IProductionUnitOfWork _productionUnitOfWork;

        public UpdateProductionBatchCommand(Data.Interfaces.UnitsOfWork.IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        public IResult Execute(DateTime timestamp, UpdateProductionBatchCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var productionBatch = _productionUnitOfWork.ProductionBatchRepository.FindByKey(parameters.ProductionBatchKey,
                b => b.Production.ResultingChileLot.Lot);
            if(productionBatch == null)
            {
                return new InvalidResult(string.Format(UserMessages.ProductionBatchNotFound, parameters.ProductionBatchKey.KeyValue));
            }

            var employeeResult = new GetEmployeeCommand(_productionUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult;
            }

            productionBatch.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            productionBatch.TimeStamp = timestamp;
            productionBatch.TargetParameters = new ProductionBatchTargetParameters(parameters.Parameters);
            productionBatch.Production.ResultingChileLot.Lot.Notes = parameters.Parameters.Notes.TrimTruncate(Constants.StringLengths.LotNotes);

            return new SuccessResult();
        }
    }
}