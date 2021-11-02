// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class PickedInventoryProjectors
    {
        internal static Expression<Func<PickedInventory, PickedInventoryKeyReturn>> SelectPickedInventoryKey()
        {
            return p => new PickedInventoryKeyReturn
            {
                PickedInventoryKey_DateCreated = p.DateCreated,
                PickedInventoryKey_Sequence = p.Sequence
            };
        }

        internal static Expression<Func<PickedInventory, PickedInventoryReturn>> SelectSummary()
        {
            return SelectBase().Merge(i => new PickedInventoryReturn
                {
                    TotalQuantityPicked = i.Items.Any() ? i.Items.Sum(m => m.Quantity) : 0,
                    TotalWeightPicked = i.Items.Any() ? i.Items.Sum(m => m.Quantity * m.PackagingProduct.Weight) : 0.0
                });
        }

        internal static IEnumerable<Expression<Func<PickedInventory, PickedInventoryReturn>>> SplitSelectDetail(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate, ISalesUnitOfWork salesUnitOfWork = null)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            
            var attributeNames = AttributeNameProjectors.SelectActiveAttributeNames(inventoryUnitOfWork);
            var itemSelector = PickedInventoryItemProjectors.SplitSelect(inventoryUnitOfWork, currentDate, salesUnitOfWork);

            return new Projectors<PickedInventory, PickedInventoryReturn>
                {
                    SelectBase().Merge(i => new PickedInventoryReturn
                        {
                            AttributeNamesAndTypes = attributeNames.Invoke()
                        }),
                    { itemSelector, s => i => new PickedInventoryReturn
                        {
                            PickedInventoryItems = i.Items.Select(m => s.Invoke(m))
                        } }
                };
        }

        internal static Expression<Func<PickedInventory, IEnumerable<PickedAdditiveInventoryItem>>> SelectPickedAdditiveInventoryItem(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }

            var additiveLots = productionUnitOfWork.AdditiveLotRepository.All();

            return pickedInventory => additiveLots.Join(pickedInventory.Items,
                additiveLot => new { additiveLot.LotDateCreated, additiveLot.LotDateSequence, additiveLot.LotTypeId },
                item => new { item.LotDateCreated, item.LotDateSequence, item.LotTypeId },
                (additiveLot, item) => new PickedAdditiveInventoryItem
                    {
                        PickedInventoryItem = item,
                        AdditiveLot = additiveLot,
                        AdditiveProduct = additiveLot.AdditiveProduct,
                        AdditiveType = additiveLot.AdditiveProduct.AdditiveType,
                        Packaging = item.PackagingProduct
                    });
        }

        internal static Expression<Func<PickedInventory, IEnumerable<PickedChileInventoryItem>>> SelectPickedChileInventoryItem(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }

            var chileLots = productionUnitOfWork.ChileLotRepository.All();

            return pickedInventory => chileLots.Join(pickedInventory.Items,
                chileLot => new { chileLot.LotDateCreated, chileLot.LotDateSequence, chileLot.LotTypeId },
                item => new { item.LotDateCreated, item.LotDateSequence, item.LotTypeId },
                (chileLot, item) => new PickedChileInventoryItem
                    {
                        PickedInventoryItem = item,
                        ChileLot = chileLot,
                        Packaging = item.PackagingProduct
                    });
        }

        internal static Expression<Func<PickedInventory, IEnumerable<PickedPackagingInventoryItem>>> SelectPickedPackagingInventoryItem(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }

            var packagingLots = productionUnitOfWork.PackagingLotRepository.All();

            return pickedInventory => packagingLots.Join(pickedInventory.Items,
                packagingLot => new { packagingLot.LotDateCreated, packagingLot.LotDateSequence, packagingLot.LotTypeId },
                item => new { item.LotDateCreated, item.LotDateSequence, item.LotTypeId },
                (packagingLot, item) => new PickedPackagingInventoryItem
                    {
                        PickedInventoryItem = item,
                        PackagingLot = packagingLot,
                        PackagingProduct = packagingLot.PackagingProduct,
                        Product = packagingLot.PackagingProduct.Product
                    });
        }

        private static Expression<Func<PickedInventory, PickedInventoryReturn>> SelectBase()
        {
            var pickedInventoryKey = SelectPickedInventoryKey();

            return i => new PickedInventoryReturn
                {
                    PickedInventoryKeyReturn = pickedInventoryKey.Invoke(i)
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup