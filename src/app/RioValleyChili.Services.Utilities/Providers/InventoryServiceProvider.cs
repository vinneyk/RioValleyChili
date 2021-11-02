// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class InventoryServiceProvider : IUnitOfWorkContainer<IInventoryUnitOfWork>
    {
        IInventoryUnitOfWork IUnitOfWorkContainer<IInventoryUnitOfWork>.UnitOfWork { get { return _inventoryUnitOfWork; } }
        private readonly IInventoryUnitOfWork _inventoryUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public InventoryServiceProvider(IInventoryUnitOfWork inventoryUnitOfWork, ITimeStamper timeStamper)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            _inventoryUnitOfWork = inventoryUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        public IResult<IInventoryReturn> GetInventory(FilterInventoryParameters parameters = null)
        {
            InventoryPredicateBuilder.PredicateBuilderFilters filters = null;
            if(parameters != null)
            {
                var filterResults = parameters.ParseToPredicateBuilderFilters();
                if(!filterResults.Success)
                {
                    return filterResults.ConvertTo((IInventoryReturn) null);
                }
                filters = filterResults.ResultingObject;
            }
            
            return new GetInventoryCommand(_inventoryUnitOfWork).Execute(filters, _timeStamper.CurrentTimeStamp.Date);
        }

        public IResult<IEnumerable<KeyValuePair<string, double>>> CalculateAttributeWeightedAverages(IEnumerable<KeyValuePair<string, int>> inventoryKeysAndQuantites)
        {
            if(inventoryKeysAndQuantites == null) { throw new ArgumentNullException("inventoryKeysAndQuantites"); }

            var commandParameters = new Dictionary<IInventoryKey, int>();
            foreach(var item in inventoryKeysAndQuantites)
            {
                if(item.Value <= 0)
                {
                    return new InvalidResult<IEnumerable<KeyValuePair<string, double>>>(null, UserMessages.QuantityNotGreaterThanZero);
                }

                var keyResult = KeyParserHelper.ParseResult<IInventoryKey>(item.Key);
                if(!keyResult.Success)
                {
                    return keyResult.ConvertTo((IEnumerable<KeyValuePair<string, double>>)null);
                }
                commandParameters.Add(keyResult.ResultingObject, item.Value);
            }

            if(commandParameters.Count <= 0)
            {
                return new InvalidResult<IEnumerable<KeyValuePair<string, double>>>(null, UserMessages.EmptySet);
            }

            return new CalculateAttributeWeightedAveragesCommand(_inventoryUnitOfWork).Execute(commandParameters);
        }

        [SynchronizeOldContext(NewContextMethod.ReceiveInventory)]
        public IResult<string> ReceiveInventory(IReceiveInventoryParameters parameters)
        {
            var parsedParametersResult = parameters.ToParsedParameters();
            if(!parsedParametersResult.Success)
            {
                return parsedParametersResult.ConvertTo<string>();
            }

            var result = new ReceiveInventoryConductor(_inventoryUnitOfWork).ReceiveInventory(_timeStamper.CurrentTimeStamp, parsedParametersResult.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _inventoryUnitOfWork.Commit();

            var lotKey = result.ResultingObject.ToLotKey();
            return SyncParameters.Using(new SuccessResult<string>(lotKey), lotKey);
        }

        public IResult<IQueryable<IInventoryTransactionReturn>> GetInventoryReceived(GetInventoryReceivedParameters parameters)
        {
            var predicate = InventoryTransactionPredicates.ByTransactionType(InventoryTransactionType.ReceiveInventory);

            if(parameters != null)
            {
                if(parameters.LotKey != null)
                {
                    var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
                    if(!lotKeyResult.Success)
                    {
                        return lotKeyResult.ConvertTo<IQueryable<IInventoryTransactionReturn>>();
                    }

                    predicate = predicate.And(InventoryTransactionPredicates.BySourceLot(lotKeyResult.ResultingObject.ToLotKey()));
                }
                else if(parameters.LotType != null)
                {
                    predicate = predicate.And(InventoryTransactionPredicates.BySourceLotType(parameters.LotType.Value));
                }

                if(parameters.DateReceivedStart != null)
                {
                    predicate = predicate.And(InventoryTransactionPredicates.ByDateReceivedRangeStart(parameters.DateReceivedStart.Value));
                }

                if(parameters.DateReceivedEnd != null)
                {
                    predicate = predicate.And(InventoryTransactionPredicates.ByDateReceivedRangeEnd(parameters.DateReceivedEnd.Value));
                }
            }
            
            var select = InventoryTransactionProjectors.Select(_inventoryUnitOfWork);

            var results = _inventoryUnitOfWork.InventoryTransactionsRepository
                .Filter(predicate)
                .AsExpandable()
                .Select(select);

            return new SuccessResult<IQueryable<IInventoryTransactionReturn>>(results);
        }

        [Issue("Need to manually add transactions for PickedInventoryItems for ProductionBatches that have not had results entered.",
            Todo = "Remove manual addition of transaction reocrds once they are being logged while being picked. -RI 2016-06-14",
            References = new[] { "RVCADMIN-1153"},
            Flags = IssueFlags.TodoWhenAccessFreedom)]
        public IResult<IQueryable<IInventoryTransactionReturn>> GetInventoryTransactions(string lotKey)
        {
            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<IQueryable<IInventoryTransactionReturn>>();
            }
            var parsedLotKey = lotKeyResult.ResultingObject.ToLotKey();

            var predicate = InventoryTransactionPredicates.BySourceLot(parsedLotKey);
            var select = InventoryTransactionProjectors.Select(_inventoryUnitOfWork);

            var query = _inventoryUnitOfWork.InventoryTransactionsRepository.Filter(predicate).Select(select);

            var pickedPredicate = PickedInventoryItemPredicates.FilterByLotKey(parsedLotKey);
            var pickedSelect = PickedInventoryItemProjectors.SelectTransaction(_inventoryUnitOfWork);
            var pickedResults = _inventoryUnitOfWork.ProductionBatchRepository
                .Filter(b => new[] { b.Production.Results }.All(r => r == null))
                .AsExpandable()
                .SelectMany(b => b.Production.PickedInventory.Items
                                    .Where(i => pickedPredicate.Invoke(i))
                                    .Select(i => pickedSelect.Invoke(i, b)));

            var results = query.ToList<IInventoryTransactionReturn>().Concat(pickedResults).AsQueryable();
            return new SuccessResult<IQueryable<IInventoryTransactionReturn>>(results);
        }

        [Issue("Need to manually add transactions for PickedInventoryItems for ProductionBatches that have not had results entered.",
            Todo = "Remove manual addition of transaction records once they are being logged while being picked. -RI 2016-06-14",
            References = new[] { "RVCADMIN-1153" },
            Flags = IssueFlags.TodoWhenAccessFreedom)]
        public IResult<IInventoryTransactionsByLotReturn> GetLotInputTransactions(string lotKey)
        {
            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<IInventoryTransactionsByLotReturn>();
            }
            var parsedLotKey = lotKeyResult.ResultingObject.ToLotKey();

            var product = _inventoryUnitOfWork.LotRepository.FilterByKey(parsedLotKey)
                .Select(LotProjectors.SelectDerivedProduct())
                .FirstOrDefault();

            var predicate = InventoryTransactionPredicates.ByDestinationLot(parsedLotKey);
            var select = InventoryTransactionProjectors.Select(_inventoryUnitOfWork);

            var query = _inventoryUnitOfWork.InventoryTransactionsRepository.Filter(predicate).Select(select).ToList();

            var batchPredicate = ProductionBatchPredicates.ByLotKey(parsedLotKey);
            var pickedSelect = PickedInventoryItemProjectors.SelectTransaction(_inventoryUnitOfWork);
            var pickedResults = _inventoryUnitOfWork.ProductionBatchRepository
                .Filter(batchPredicate.And(b => new[] { b.Production.Results }.All(r => r == null)))
                .AsExpandable()
                .SelectMany(b => b.Production.PickedInventory.Items.Select(i => pickedSelect.Invoke(i, b)));

            var results = query.ToList<IInventoryTransactionReturn>().Concat(pickedResults).AsQueryable();

            return new SuccessResult<IInventoryTransactionsByLotReturn>(new InventoryTransactionsByLotReturn
                {
                    Product = product,
                    InputItems = results
                });
        }

        public IResult<IInventoryCycleCountReportReturn> GetInventoryCycleCountReport(string facilityKey, string groupName)
        {
            var facilityKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(facilityKey);
            if(!facilityKeyResult.Success)
            {
                return facilityKeyResult.ConvertTo<IInventoryCycleCountReportReturn>();
            }

            var parsedFacilityKey = facilityKeyResult.ResultingObject.ToFacilityKey();
            var predicate = parsedFacilityKey.FindByPredicate;
            var select = FacilityProjectors.SelectInventoryCycleCount(groupName);
            var facility = _inventoryUnitOfWork.FacilityRepository.Filter(predicate).Select(select).FirstOrDefault();
            if(facility == null)
            {
                return new InvalidResult<IInventoryCycleCountReportReturn>(null, string.Format(UserMessages.FacilityNotFound, facilityKey));
            }

            facility.Initialize();

            return new SuccessResult<IInventoryCycleCountReportReturn>(facility);
        }

        public IResult<Dictionary<string, string>> GetFacilityKeys()
        {
            var facilities = _inventoryUnitOfWork.FacilityRepository.SourceQuery.Select(FacilityProjectors.Select(false, false)).ToList();
            return new SuccessResult<Dictionary<string, string>>(facilities.ToDictionary(f => f.FacilityKey, f => f.FacilityName));
        }

        public IResult<IEnumerable<ILocationGroupNameReturn>> GetFacilityLocationGroups(string facilityKey)
        {
            var facilityKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(facilityKey);
            if(!facilityKeyResult.Success)
            {
                return facilityKeyResult.ConvertTo<IEnumerable<ILocationGroupNameReturn>>();
            }

            var locations = _inventoryUnitOfWork.LocationRepository
                .Filter(LocationPredicates.ByFacilityKey(facilityKeyResult.ResultingObject))
                .Select(LocationProjectors.SelectGroupName()).ToList()
                .DistinctBy(l => l.LocationGroupName).ToList();
            return new SuccessResult<IEnumerable<ILocationGroupNameReturn>>(locations);
        }
    }
}
// ReSharper restore ConvertClosureToMethodGroup