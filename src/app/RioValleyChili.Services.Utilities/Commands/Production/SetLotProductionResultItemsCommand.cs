using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Production
{
    internal class SetLotProductionResultItemsCommand
    {
        private readonly IProductionUnitOfWork _productionUnitOfWork;

        internal SetLotProductionResultItemsCommand(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null)
            {
                throw new ArgumentNullException("productionUnitOfWork");
            }
            _productionUnitOfWork = productionUnitOfWork;
        }

        internal IResult Execute(LotProductionResults productionResults, IEnumerable<IProductionResultItemParameters> resultItems, ref List<ModifyInventoryParameters> inventoryModifications)
        {
            var nextSequence = productionResults.ResultItems.Select(i => i.ResultItemSequence).DefaultIfEmpty(0).Max() + 1;
            var existingItems = productionResults.ResultItems.ToDictionary(i => new InventoryKey(i));
            inventoryModifications = inventoryModifications ?? new List<ModifyInventoryParameters>();

            foreach(var item in resultItems)
            {
                if(item.Quantity <= 0)
                {
                    return new InvalidResult(UserMessages.QuantityNotGreaterThanZero);
                }

                var inventoryKey = new InventoryKey(productionResults, item.PackagingProductKey, item.LocationKey, item.InventoryTreatmentKey, "");
                if(inventoryModifications.Any(m => m.InventoryKey.Equals(inventoryKey) && m.ModifyQuantity > 0))
                {
                    return new InvalidResult(string.Format(UserMessages.ProductionResultAlreadyPendingAddition, inventoryKey));
                }

                LotProductionResultItem resultItem;
                if(existingItems.TryGetValue(inventoryKey, out resultItem))
                {
                    existingItems.Remove(inventoryKey);
                }
                else
                {
                    resultItem = _productionUnitOfWork.LotProductionResultItemsRepository.Add(new LotProductionResultItem
                        {
                            LotDateCreated = inventoryKey.LotKey_DateCreated,
                            LotDateSequence = inventoryKey.LotKey_DateSequence,
                            LotTypeId = inventoryKey.LotKey_LotTypeId,
                            ResultItemSequence = nextSequence++,
                            Quantity = 0
                        });
                }

                var deltaQuantity = item.Quantity - resultItem.Quantity;
                if(deltaQuantity != 0)
                {
                    inventoryModifications.Add(new ModifyInventoryParameters(inventoryKey, deltaQuantity));
                }

                resultItem.PackagingProductId = inventoryKey.PackagingProductKey_ProductId;
                resultItem.LocationId = inventoryKey.LocationKey_Id;
                resultItem.TreatmentId = inventoryKey.InventoryTreatmentKey_Id;
                resultItem.Quantity = item.Quantity;
            }

            foreach(var item in existingItems.Values)
            {
                _productionUnitOfWork.LotProductionResultItemsRepository.Remove(item);
                inventoryModifications.Add(new ModifyInventoryParameters(item, -item.Quantity));
            }

            return new SuccessResult();
        }
    }
}