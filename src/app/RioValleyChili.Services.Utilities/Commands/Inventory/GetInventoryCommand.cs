using System;
using System.Data.Entity;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Utilities.Extensions;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class GetInventoryCommand
    {
        private readonly IInventoryUnitOfWork _inventoryUnitOfWork;

        internal GetInventoryCommand(IInventoryUnitOfWork inventoryUnitOfWork)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            _inventoryUnitOfWork = inventoryUnitOfWork;
        }

        internal IResult<IInventoryReturn> Execute(InventoryPredicateBuilder.PredicateBuilderFilters filters, DateTime currentDate)
        {
            var predicateResult = InventoryPredicateBuilder.BuildPredicate(_inventoryUnitOfWork, filters);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IInventoryReturn>();
            }

            var attributes = AttributeNameProjectors.SelectActiveAttributeNames(_inventoryUnitOfWork);
            var inventory = _inventoryUnitOfWork.InventoryRepository
                .Filter(predicateResult.ResultingObject)
                .SplitSelect(InventoryProjectors.SplitSelectInventorySummary(_inventoryUnitOfWork, currentDate));

            var inventoryReturn = new InventoryReturn
                {
                    AttributeNamesAndTypes = attributes.ExpandAll().Invoke(),
                    Inventory = inventory
                };

            if(!inventoryReturn.Inventory.Any())
            {
                if(filters != null && filters.LotKey != null)
                {
                    var lotKey = new LotKey(filters.LotKey);
                    if(!_inventoryUnitOfWork.OptimizeForReadonly().LotRepository
                        .All()
                        .AsExpandable()
                        .AsNoTracking()
                        .Any(lotKey.FindByPredicate))
                    {
                        return new InvalidResult<IInventoryReturn>(null, string.Format(UserMessages.LotNotFound, lotKey.KeyValue));
                    }
                }
            }

            return new SuccessResult<IInventoryReturn>(inventoryReturn);
        }
    }
}