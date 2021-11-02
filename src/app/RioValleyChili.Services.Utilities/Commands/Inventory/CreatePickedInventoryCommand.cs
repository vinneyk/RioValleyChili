using System;
using System.Collections.Generic;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class CreatePickedInventoryCommand
    {
        private readonly IPickedInventoryUnitOfWork _pickedInventoryUnitOfWork;

        public CreatePickedInventoryCommand(IPickedInventoryUnitOfWork pickedInventoryUnitOfWork)
        {
            if(pickedInventoryUnitOfWork == null) { throw new ArgumentNullException("pickedInventoryUnitOfWork"); }
            _pickedInventoryUnitOfWork = pickedInventoryUnitOfWork;
        }

        public IResult<PickedInventory> Execute(ICreatePickedInventory parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var dateCreated = parameters.TimeStamp.Date;
            var sequence = new EFUnitOfWorkHelper(_pickedInventoryUnitOfWork).GetNextSequence<PickedInventory>(i => i.DateCreated == dateCreated, i => i.Sequence);

            var pickedInventory = new PickedInventory
                {
                    EmployeeId = parameters.EmployeeKey.EmployeeKey_Id,
                    TimeStamp = parameters.TimeStamp,

                    DateCreated = dateCreated,
                    Sequence = sequence,
                    PickedReason = parameters.PickedReason,
                    Archived = false,
                    Items = new List<PickedInventoryItem>()
                };

            _pickedInventoryUnitOfWork.PickedInventoryRepository.Add(pickedInventory);

            return new SuccessResult<PickedInventory>(pickedInventory);
        }
    }
}