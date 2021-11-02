using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using Solutionhead.Data;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Helpers
{
    [Issue("Client expressed desire to see inventory that was still invalid to pick in some cases." +
               "Implemented by using some validation rules to intialize ValidToPick flag during projection instead of filtering Inventory," +
               "and taking that into consideration in IPickableInventoryInitializer implementations. - RI 2016-10-05",
               References = new[] { "RVCADMIN-1332" })]
    internal interface IInventoryValidator
    {
        IEnumerable<Expression<Func<Inventory, bool>>> InventoryFilters { get; }
        IEnumerable<Expression<Func<Inventory, bool>>> ValidForPickingPredicates { get; }
        IResult ValidateItems(IRepository<Inventory> inventoryRepository, IEnumerable<IInventoryKey> inventoryKeys);
        IResult ValidateItems(IRepository<Inventory> inventoryRepository, IEnumerable<InventoryKey> inventoryKeys);
        IResult ValidateItems(IRepository<Inventory> inventoryRepository, IEnumerable<InventoryKey> inventoryKeys, out Dictionary<string, Inventory> inventory);
    }

    internal class PickedInventoryValidator : List<PickedItemValidationRule>, IInventoryValidator
    {
        private readonly Expression<Func<Inventory, object>>[] _includes;

        private PickedInventoryValidator(params Expression<Func<Inventory, object>>[] includes)
        {
            _includes = includes;
        }

        IEnumerable<Expression<Func<Inventory, bool>>> IInventoryValidator.InventoryFilters { get { return this.Where(v => v.Predicate != null && v.InventoryFilter).Select(v => v.Predicate); } }
        IEnumerable<Expression<Func<Inventory, bool>>> IInventoryValidator.ValidForPickingPredicates { get { return this.Where(v => v.Predicate != null && !v.InventoryFilter).Select(v => v.Predicate); } }

        IResult IInventoryValidator.ValidateItems(IRepository<Inventory> inventoryRepository, IEnumerable<IInventoryKey> inventoryKeys)
        {
            return ((IInventoryValidator) this).ValidateItems(inventoryRepository, inventoryKeys.Select(i => new InventoryKey(i)));
        }

        IResult IInventoryValidator.ValidateItems(IRepository<Inventory> inventoryRepository, IEnumerable<InventoryKey> inventoryKeys)
        {
            Dictionary<string, Inventory> inventory;
            return ((IInventoryValidator)this).ValidateItems(inventoryRepository, inventoryKeys, out inventory);
        }

        IResult IInventoryValidator.ValidateItems(IRepository<Inventory> inventoryRepository, IEnumerable<InventoryKey> inventoryKeys, out Dictionary<string, Inventory> inventory)
        {
            var invalidResults = new List<IResult>();
            
            var keys = inventoryKeys.ToList();
            var predicate = PredicateHelper.OrPredicates(keys.Select(k => k.FindByPredicate));
            inventory = inventoryRepository.Filter(predicate, _includes).ToDictionary(i => new InventoryKey(i).KeyValue, i => i);
            foreach(var key in keys)
            {
                Inventory inventoryItem;
                if(!inventory.TryGetValue(key.KeyValue, out inventoryItem))
                {
                    invalidResults.Add(new InvalidResult(string.Format(UserMessages.InventoryItemNotFound, key)));
                    continue;
                }

                invalidResults.AddRange(this.Select(v => v.Validate(inventoryItem)).Where(v => !v.Success));
            }

            if(invalidResults.Any())
            {
                return new InvalidResult(string.Join("\n", invalidResults.Select(r => r.Message).Distinct()));
            }

            return new SuccessResult();
        }

        internal void Add(Expression<Func<Inventory, bool>> validation, Func<Inventory, IResult> invalidResult, bool inventoryFilter = true)
        {
            Add(new PickedItemValidationRule(validation.ExpandAll(), invalidResult, inventoryFilter));
        }

        internal static readonly IInventoryValidator ForProductionBatch = new PickedInventoryValidator(i => i.Location.Facility, i => i.Lot)
            {
                { i => i.Location.Active, i => new InvalidResult(string.Format(UserMessages.InventoryInactiveLocation, i.ToInventoryKey())) },
                { i => !i.Location.Locked, i => new InvalidResult(string.Format(UserMessages.InventoryLocationLocked, i.ToInventoryKey())) },
                { i => i.Location.FacilityId == GlobalKeyHelpers.RinconFacilityKey.FacilityKey_Id, i => new InvalidResult(string.Format(UserMessages.InventoryInvalidFacility, i.ToInventoryKey(), i.Location.Facility.Name)) },
                { i => i.Lot.QualityStatus == LotQualityStatus.Released, i => new InvalidResult(string.Format(UserMessages.CannotPickLotQualityState, i.ToLotKey(), i.Lot.QualityStatus)) },
                { i => i.Lot.ProductionStatus == LotProductionStatus.Produced, i => new InvalidResult(string.Format(UserMessages.CannotPickLotNotProduced, i.ToLotKey())) },
                { i => i.Lot.Hold == null, i => new InvalidResult(string.Format(UserMessages.CannotPickLotOnHold, new LotKey(i), i.Lot.Hold)) }
            };

        internal static readonly IInventoryValidator ForMillAndWetdown = new PickedInventoryValidator(i => i.Location.Facility, i => i.Lot)
            {
                { i => i.Location.Active, i => new InvalidResult(string.Format(UserMessages.InventoryInactiveLocation, i.ToInventoryKey())), false },
                { i => i.Location.FacilityId == GlobalKeyHelpers.RinconFacilityKey.FacilityKey_Id, i => new InvalidResult(string.Format(UserMessages.InventoryInvalidFacility, i.ToInventoryKey(), i.Location.Facility.Name)) },
                { InventoryPredicates.ByLotStatus(LotQualityStatus.Released, LotQualityStatus.Pending), i => new InvalidResult(string.Format(UserMessages.CannotPickLotQualityState, i.ToLotKey(), i.Lot.QualityStatus)), false },
                { i => i.Lot.Hold == null, i => new InvalidResult(string.Format(UserMessages.CannotPickLotOnHold, i.ToLotKey(), i.Lot.Hold)), false }
            };

        internal static IInventoryValidator ForInterWarehouseOrder(IFacilityKey sourceFacility)
        {
            var facilityKey = sourceFacility.ToFacilityKey();
            var facilityPredicate = facilityKey.FindByPredicate;

            return new PickedInventoryValidator(i => i.Location.Facility,
                i => i.Lot.CustomerAllowances,
                i => i.Lot.ContractAllowances,
                i => i.Lot.SalesOrderAllowances,
                i => i.Lot.Attributes,
                i => i.Lot.LotDefects.Select(d => d.Resolution),
                i => i.Lot.AttributeDefects.Select(d => d.LotDefect.Resolution))
                {
                    { i => i.Location.Active, i => new InvalidResult(string.Format(UserMessages.InventoryInactiveLocation, i.ToInventoryKey())), false },
                    { i => !i.Location.Locked, i => new InvalidResult(string.Format(UserMessages.InventoryLocationLocked, i.ToInventoryKey())), false },
                    { InventoryPredicates.ByLotStatus(LotQualityStatus.Released), i => new InvalidResult(string.Format(UserMessages.CannotPickLotQualityState, i.ToLotKey(), i.Lot.QualityStatus)), false },
                    { InventoryPredicates.ByLotProductionStatus(LotProductionStatus.Produced), i => new InvalidResult(string.Format(UserMessages.CannotPickLotNotProduced, i.ToLotKey())), false },
                    { i => i.Lot.Hold == null || i.Lot.Hold == LotHoldType.HoldForCustomer, i => new InvalidResult(string.Format(UserMessages.CannotPickLotOnHold, i.ToLotKey(), i.Lot.Hold)), false },
                    { i => facilityPredicate.Invoke(i.Location.Facility), i => new InvalidResult(string.Format(UserMessages.SourceLocationMustBelongToFacility, facilityKey)) }
                };
        }

        internal static IInventoryValidator ForTreatmentOrder(IFacilityKey sourceFacility, IInventoryTreatmentKey treatmentKey, ITreatmentOrderUnitOfWork unitOfWork)
        {
            var facilityKey = sourceFacility.ToFacilityKey();
            var facilityPredicate = facilityKey.FindByPredicate;

            return new PickedInventoryValidator(i => i.Location.Facility, i => i.Lot.Inventory.Select(n => n.Treatment))
                {
                    { i => i.Location.Active, i => new InvalidResult(string.Format(UserMessages.InventoryInactiveLocation, i.ToInventoryKey())), false },
                    { i => !i.Location.Locked, i => new InvalidResult(string.Format(UserMessages.InventoryLocationLocked, i.ToInventoryKey())), false },
                    { i => i.Lot.QualityStatus != LotQualityStatus.Rejected, i => new InvalidResult(string.Format(UserMessages.CannotPickLotQualityState, i.ToLotKey(), i.Lot.QualityStatus)), false },
                    { i => facilityPredicate.Invoke(i.Location.Facility), i => new InvalidResult(string.Format(UserMessages.SourceLocationMustBelongToFacility, facilityKey)) },
                    { ValidForTreatmentOrder(treatmentKey, unitOfWork), i => new InvalidResult(string.Format(UserMessages.LotConflictingInventoryTreatment, i.ToLotKey())) }
                };
        }

        internal static IInventoryValidator ForSalesOrder(IFacilityKey sourceFacility)
        {
            var facilityKey = sourceFacility.ToFacilityKey();
            var facilityPredicate = facilityKey.FindByPredicate;

            return new PickedInventoryValidator(i => i.Location.Facility,
                i => i.Lot.CustomerAllowances,
                i => i.Lot.ContractAllowances,
                i => i.Lot.SalesOrderAllowances,
                i => i.Lot.Attributes,
                i => i.Lot.LotDefects.Select(d => d.Resolution),
                i => i.Lot.AttributeDefects.Select(d => d.LotDefect.Resolution))
                {
                    { i => i.Location.Active, i => new InvalidResult(string.Format(UserMessages.InventoryInactiveLocation, i.ToInventoryKey())), false },
                    { i => !i.Location.Locked, i => new InvalidResult(string.Format(UserMessages.InventoryLocationLocked, i.ToInventoryKey())), false },
                    { InventoryPredicates.ByLotStatus(LotQualityStatus.Released), i => new InvalidResult(string.Format(UserMessages.CannotPickLotQualityState, i.ToLotKey(), i.Lot.QualityStatus)), false },
                    { i => i.Lot.Hold == null || i.Lot.Hold == LotHoldType.HoldForCustomer, i => new InvalidResult(string.Format(UserMessages.CannotPickLotOnHold, i.ToLotKey(), i.Lot.Hold)), false },
                    { i => facilityPredicate.Invoke(i.Location.Facility), i => new InvalidResult(string.Format(UserMessages.SourceLocationMustBelongToFacility, facilityKey)) },
                    { InventoryPredicates.ByLotProductionStatus(LotProductionStatus.Produced), i => new InvalidResult(string.Format(UserMessages.CannotPickLotNotProduced, i.ToLotKey())), false }
                };
        }

        private static Expression<Func<Inventory, bool>> ValidForTreatmentOrder(IInventoryTreatmentKey treatment, ITreatmentOrderUnitOfWork unitOfWork)
        {
            var lotsAreEqual = LotProjectors.SelectLotsAreEqual();
            var validTreatment = treatment.ToInventoryTreatmentKey().FindByPredicate
                .Or(Data.Models.StaticRecords.StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey().FindByPredicate);

            var treatmentOrders = unitOfWork.TreatmentOrderRepository.All();
            var lots = unitOfWork.LotRepository.All();

            return i => lots
                .Where(l => lotsAreEqual.Invoke(i.Lot, l))
                .SelectMany(l => l.Inventory.Select(n => n.Treatment).Concat(l.PickedInventory.Select(p => p.Treatment)))
                .Concat(treatmentOrders
                            .Where(o =>
                                o.InventoryShipmentOrder.OrderStatus != OrderStatus.Void &&
                                o.InventoryShipmentOrder.PickedInventory.Items.Any(l => lotsAreEqual.Invoke(l.Lot, i.Lot)))
                            .Select(o => o.Treatment))
                .All(t => validTreatment.Invoke(t));
        }
    }

    internal class PickedItemValidationRule
    {
        public readonly Expression<Func<Inventory, bool>> Predicate;
        public readonly bool InventoryFilter;
        private readonly Func<Inventory, bool> _ruleDelegate;
        private readonly Func<Inventory, IResult> _resultDelegate;

        internal PickedItemValidationRule(Expression<Func<Inventory, bool>> predicate, Func<Inventory, IResult> resultDelegate, bool inventoryFilter)
        {
            Predicate = predicate;
            _ruleDelegate = Predicate.Compile();
            _resultDelegate = resultDelegate;
            InventoryFilter = inventoryFilter;
        }

        internal IResult Validate(Inventory inventory)
        {
            if(_ruleDelegate != null)
            {
                return _ruleDelegate(inventory) ? new SuccessResult() : _resultDelegate(inventory);
            }
            return _resultDelegate(inventory);
        }
    }
}