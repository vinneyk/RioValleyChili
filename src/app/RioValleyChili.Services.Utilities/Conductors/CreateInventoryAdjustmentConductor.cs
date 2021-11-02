using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Notebook;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class CreateInventoryAdjustmentConductor
    {
        private readonly IInventoryUnitOfWork _inventoryUnitOfWork;

        internal CreateInventoryAdjustmentConductor(IInventoryUnitOfWork inventoryUnitOfWork)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            _inventoryUnitOfWork = inventoryUnitOfWork;
        }

        internal IResult<InventoryAdjustment> Execute(DateTime timeStamp, CreateInventoryAdjustmentConductorParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_inventoryUnitOfWork).GetEmployee(parameters.CreateInventoryAdjustmentParameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo((InventoryAdjustment) null);
            }
            var employee = employeeResult.ResultingObject;

            var createAdjustmentResult = CreateInventoryAdjustment(timeStamp, employee, parameters.CreateInventoryAdjustmentParameters.Comment);
            if(!createAdjustmentResult.Success)
            {
                return createAdjustmentResult.ConvertTo((InventoryAdjustment) null);
            }
            var inventoryAdjustment = createAdjustmentResult.ResultingObject;

            var createInventoryAdjustmentItemCommand = new CreateInventoryAdjustmentItemCommand(_inventoryUnitOfWork);
            var inventoryModifications = new List<ModifyInventoryParameters>();
            foreach(var adjustment in parameters.Items)
            {
                var createAdjustmentItemResult = createInventoryAdjustmentItemCommand.Execute(inventoryAdjustment, adjustment);
                if(!createAdjustmentItemResult.Success)
                {
                    return createAdjustmentItemResult.ConvertTo((InventoryAdjustment) null);
                }

                inventoryModifications.Add(new ModifyInventoryParameters(new InventoryKey(createAdjustmentItemResult.ResultingObject), adjustment.InventoryAdjustmentParameters.Adjustment));
            }

            var modifyInventoryResult = new ModifyInventoryCommand(_inventoryUnitOfWork).Execute(inventoryModifications,
                new InventoryTransactionParameters(employee, timeStamp, InventoryTransactionType.InventoryAdjustment, new InventoryAdjustmentKey(inventoryAdjustment)));
            if(!modifyInventoryResult.Success)
            {
                return modifyInventoryResult.ConvertTo((InventoryAdjustment) null);
            }
            
            return new SuccessResult<InventoryAdjustment>(inventoryAdjustment);
        }

        private IResult<InventoryAdjustment> CreateInventoryAdjustment(DateTime timestamp, Employee employee, string comment)
        {
            var notebookResult = CreateNotebook(timestamp, employee, comment);
            if(!notebookResult.Success)
            {
                return notebookResult.ConvertTo((InventoryAdjustment) null);
            }
            var notebook = notebookResult.ResultingObject;

            var newSequence = new EFUnitOfWorkHelper(_inventoryUnitOfWork).GetNextSequence(InventoryAdjustmentPredicates.ByAdjustmentDate(timestamp), a => a.Sequence);
            var newInventoryAdjustment = _inventoryUnitOfWork.InventoryAdjustmentRepository.Add(new InventoryAdjustment
            {
                AdjustmentDate = timestamp.Date,
                Sequence = newSequence,
                EmployeeId = employee.EmployeeId,
                TimeStamp = timestamp,

                NotebookDate = notebook.Date,
                NotebookSequence = notebook.Sequence
            });

            return new SuccessResult<InventoryAdjustment>(newInventoryAdjustment);
        }

        private IResult<Notebook> CreateNotebook(DateTime timestamp, Employee employee, string comment)
        {
            var createNotebookResult = new CreateNotebookCommand(_inventoryUnitOfWork).Execute(timestamp);
            if(!createNotebookResult.Success)
            {
                return createNotebookResult.ConvertTo((Notebook)null);
            }
            var notebook = createNotebookResult.ResultingObject;

            if(!string.IsNullOrWhiteSpace(comment))
            {
                var createNoteResult = new CreateNoteCommand(_inventoryUnitOfWork).Execute(notebook, timestamp, employee, comment);
                if(!createNoteResult.Success)
                {
                    return createNoteResult.ConvertTo((Notebook)null);
                }
            }

            return new SuccessResult<Notebook>(notebook);
        }
    }
}