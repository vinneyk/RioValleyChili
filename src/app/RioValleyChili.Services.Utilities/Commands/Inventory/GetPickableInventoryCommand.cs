using System;
using EF_Split_Projector;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class GetPickableInventoryCommand
    {
        private readonly IInventoryUnitOfWork _inventoryUnitOfWork;

        internal GetPickableInventoryCommand(IInventoryUnitOfWork inventoryUnitOfWork)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            _inventoryUnitOfWork = inventoryUnitOfWork;
        }

        internal IResult<PickableInventoryReturn> Execute(InventoryPredicateBuilder.PredicateBuilderFilters filters, DateTime currentDate, IInventoryValidator validator, bool includeAllowances)
        {
            var splitProjectors = InventoryProjectors.SplitSelectPickableInventorySummary(_inventoryUnitOfWork, currentDate, validator.ValidForPickingPredicates, includeAllowances);
            var predicateResult = InventoryPredicateBuilder.BuildPredicate(_inventoryUnitOfWork, filters, validator.InventoryFilters);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<PickableInventoryReturn>();
            }

            return new SuccessResult<PickableInventoryReturn>(new PickableInventoryReturn
                {
                    Items = _inventoryUnitOfWork.InventoryRepository
                        .Filter(predicateResult.ResultingObject)
                        .SplitSelect(splitProjectors)
                });
        }


    }
}