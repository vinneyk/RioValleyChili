using System;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.LotCommands;
using RioValleyChili.Services.Utilities.Commands.Notebook;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Production
{
    internal class CreateProductionBatchCommand
    {
        private readonly IProductionUnitOfWork _productionUnitOfWork;

        public CreateProductionBatchCommand(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        public IResult<ProductionBatch> Execute(DateTime timestamp, CreateProductionBatchCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var packSchedule = _productionUnitOfWork.PackScheduleRepository.FindByKey(parameters.PackScheduleKey, p => p.ChileProduct);
            if(packSchedule == null)
            {
                return new InvalidResult<ProductionBatch>(null, string.Format(UserMessages.PackScheduleNotFound, parameters.PackScheduleKey.KeyValue));
            }

            var employeeResult = new GetEmployeeCommand(_productionUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ProductionBatch>();
            }

            var createChileLotCommand = new CreateNewChileLotCommand(_productionUnitOfWork);
            var createChileLotResult = createChileLotCommand.Execute(new CreateNewChileLotCommandParameters
                {
                    EmployeeKey = employeeResult.ResultingObject,
                    TimeStamp = timestamp,
                    PackagingReceivedKey = packSchedule,
                    ChileProductKey = new ChileProductKey(packSchedule),
                    LotType = parameters.Parameters.LotType ?? packSchedule.ChileProduct.ChileState.ToLotType(),
                    LotDate = parameters.Parameters.LotDateCreated?.Date ?? timestamp,
                    LotSequence = parameters.Parameters.LotSequence,
                    SetLotProductionStatus = LotProductionStatus.Batched,
                    SetLotQualityStatus = LotQualityStatus.Pending
                });
            if(!createChileLotResult.Success)
            {
                return createChileLotResult.ConvertTo<ProductionBatch>();
            }
            var chileLot = createChileLotResult.ResultingObject;
            chileLot.Lot.Notes = parameters.Parameters.Notes.TrimTruncate(Constants.StringLengths.LotNotes);

            var productionResult = new CreateChileLotProductionCommand(_productionUnitOfWork).Execute(new CreateChileLotProduction
                {
                    TimeStamp = timestamp,
                    EmployeeKey = employeeResult.ResultingObject,
                    LotKey = chileLot,
                    ProductionType = ProductionType.ProductionBatch
                });
            if(!productionResult.Success)
            {
                return productionResult.ConvertTo<ProductionBatch>();
            }
            var production = productionResult.ResultingObject;

            var notebookResult = new CreateNotebookCommand(_productionUnitOfWork).Execute(timestamp, employeeResult.ResultingObject, parameters.Parameters.Instructions);
            if(!notebookResult.Success)
            {
                return notebookResult.ConvertTo<ProductionBatch>();
            }
            var notebook = notebookResult.ResultingObject;

            var productionBatch = new ProductionBatch
                {
                    EmployeeId = employeeResult.ResultingObject.EmployeeId,
                    TimeStamp = timestamp,

                    LotDateCreated = chileLot.LotDateCreated,
                    LotDateSequence = chileLot.LotDateSequence,
                    LotTypeId = chileLot.LotTypeId,

                    PackScheduleDateCreated = parameters.PackScheduleKey.PackScheduleKey_DateCreated,
                    PackScheduleSequence = parameters.PackScheduleKey.PackScheduleKey_DateSequence,

                    InstructionNotebook = notebook,
                    InstructionNotebookDateCreated = notebook.Date,
                    InstructionNotebookSequence = notebook.Sequence,

                    ProductionHasBeenCompleted = false,
                    TargetParameters = new ProductionBatchTargetParameters(parameters.Parameters),
                    Production = production
                };
            _productionUnitOfWork.ProductionBatchRepository.Add(productionBatch);

            return new SuccessResult<ProductionBatch>(productionBatch);
        }
    }
}