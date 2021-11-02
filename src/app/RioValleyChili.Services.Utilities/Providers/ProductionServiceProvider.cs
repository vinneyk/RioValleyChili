using System;
using System.Collections.Generic;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Production;
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
    public class ProductionServiceProvider : IUnitOfWorkContainer<IProductionUnitOfWork>
    {
        #region Fields and Constructors.

        IProductionUnitOfWork IUnitOfWorkContainer<IProductionUnitOfWork>.UnitOfWork { get { return _productionUnitOfWork; } }
        private readonly IProductionUnitOfWork _productionUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public ProductionServiceProvider(IProductionUnitOfWork productionUnitOfWork, ITimeStamper timeStamper)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }

            _productionUnitOfWork = productionUnitOfWork;
            _timeStamper = timeStamper;
        }

        #endregion

        [SynchronizeOldContext(NewContextMethod.CreatePackSchedule)]
        public IResult<string> CreatePackSchedule(ICreatePackScheduleParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult.ConvertTo<string>();
            }

            var result = new CreatePackScheduleCommand(_productionUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _productionUnitOfWork.Commit();

            var packScheduleKey = result.ResultingObject.ToPackScheduleKey();
            return SyncParameters.Using(new SuccessResult<string>(packScheduleKey), new SyncCreatePackScheduleParameters
                {
                    PackScheduleKey = packScheduleKey,
                    UseSuppliedPSNum = parametersResult.ResultingObject.Parameters.PSNum != null
                });
        }

        [SynchronizeOldContext(NewContextMethod.UpdatePackSchedule)]
        public IResult<string> UpdatePackSchedule(IUpdatePackScheduleParameters parameters) 
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult.ConvertTo<string>();
            }

            var result = new UpdatePackScheduleCommand(_productionUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _productionUnitOfWork.Commit();

            var packScheduleKey = result.ResultingObject.ToPackScheduleKey();
            return SyncParameters.Using(new SuccessResult<string>(packScheduleKey), packScheduleKey);
        }

        [SynchronizeOldContext(NewContextMethod.DeletePackSchedule)]
        public IResult RemovePackSchedule(IRemovePackScheduleParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var packScheduleKeyResult = KeyParserHelper.ParseResult<IPackScheduleKey>(parameters.PackScheduleKey);
            if (!packScheduleKeyResult.Success)
            {
                return packScheduleKeyResult.ConvertTo<DateTime?>();
            }

            DateTime? packSchId;
            var result = new RemovePackScheduleConductor(_productionUnitOfWork).Execute(new PackScheduleKey(packScheduleKeyResult.ResultingObject), parameters.UserToken, out packSchId);
            if(!result.Success)
            {
                return result.ConvertTo<DateTime?>();
            }

            _productionUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), packSchId);
        }

        public IResult<IQueryable<IPackScheduleSummaryReturn>> GetPackSchedules()
        {
            var packScheduleSummaryReturns = _productionUnitOfWork.PackScheduleRepository.All()
                .SplitSelect(PackScheduleProjectors.SplitSelectSummary());
            return new SuccessResult<IQueryable<IPackScheduleSummaryReturn>>(packScheduleSummaryReturns);
        }

        public IResult<IPackScheduleDetailReturn> GetPackSchedule(string packScheduleKey)
        {
            if(packScheduleKey == null) { throw new ArgumentNullException("packScheduleKey"); }

            var packScheduleKeyResult = KeyParserHelper.ParseResult<IPackScheduleKey>(packScheduleKey);
            if(!packScheduleKeyResult.Success)
            {
                return packScheduleKeyResult.ConvertTo<IPackScheduleDetailReturn>();
            }

            var key = new PackScheduleKey(packScheduleKeyResult.ResultingObject);
            var packSchedule = _productionUnitOfWork.PackScheduleRepository
                .FilterByKey(key)
                .SplitSelect(PackScheduleProjectors.SplitSelectDetail())
                .FirstOrDefault();
            if(packSchedule == null)
            {
                return new InvalidResult<IPackScheduleDetailReturn>(null, string.Format(UserMessages.PackScheduleNotFound, key));
            }

            return new SuccessResult<IPackScheduleDetailReturn>(packSchedule);
        }

        [SynchronizeOldContext(NewContextMethod.CreateProductionBatch)]
        public IResult<ICreateProductionBatchReturn> CreateProductionBatch(ICreateProductionBatchParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult.ConvertTo<ICreateProductionBatchReturn>();
            }

            var result = new CreateProductionBatchCommand(_productionUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<ICreateProductionBatchReturn>();
            }

            _productionUnitOfWork.Commit();

            var lotKey = result.ResultingObject.ToLotKey();
            return SyncParameters.Using(new SuccessResult<ICreateProductionBatchReturn>(new CreateProductionBatchReturn
                {
                    ProductionBatchKey = lotKey,
                    InstructionNotebookKey = result.ResultingObject.InstructionNotebook.ToNotebookKey()
                }), lotKey);
        }

        [SynchronizeOldContext(NewContextMethod.UpdateProductionBatch)]
        public IResult<string> UpdateProductionBatch(IUpdateProductionBatchParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult.ConvertTo<string>();
            }

            var result = new UpdateProductionBatchCommand(_productionUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _productionUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(parameters.ProductionBatchKey), parametersResult.ResultingObject.ProductionBatchKey);
        }

        [SynchronizeOldContext(NewContextMethod.DeleteProductionBatch)]
        public IResult RemoveProductionBatch(string productionBatchKey)
        {
            if(productionBatchKey == null) { throw new ArgumentNullException("productionBatchKey"); }

            var productionBatchKeyResult = KeyParserHelper.ParseResult<ILotKey>(productionBatchKey);
            if(!productionBatchKeyResult.Success)
            {
                return productionBatchKeyResult;
            }
            var parsedProductionBatchKey = productionBatchKeyResult.ResultingObject;

            var lotKey = parsedProductionBatchKey.ToLotKey();
            var removeResult = new RemoveProductionBatchConductor(_productionUnitOfWork).Execute(lotKey);
            if(!removeResult.Success)
            {
                return removeResult;
            }

            _productionUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), lotKey);
        }

        [SynchronizeOldContext(NewContextMethod.PickInventoryForProductionBatch)]
        public IResult PickInventoryForProductionBatch(string productionBatchKey, ISetPickedInventoryParameters pickedInventory)
        {
            if(productionBatchKey == null) { throw new ArgumentNullException("productionBatchKey"); }
            if(pickedInventory == null) { throw new ArgumentNullException("pickedInventory"); }

            var productionBatchKeyResult = KeyParserHelper.ParseResult<ILotKey>(productionBatchKey);
            if(!productionBatchKeyResult.Success)
            {
                return productionBatchKeyResult;
            }
            var lotKey = productionBatchKeyResult.ResultingObject.ToLotKey();

            var result = new ProductionBatchPickInventoryConductor(_productionUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, pickedInventory, lotKey);
            if(!result.Success)
            {
                return result;
            }

            _productionUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), lotKey);
        }

        public IResult<IProductionBatchDetailReturn> GetProductionBatch(string lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<IProductionBatchDetailReturn>();
            }

            return new GetProductionBatchDetailCommand(_productionUnitOfWork).Execute(new LotKey(lotKeyResult.ResultingObject), _timeStamper.CurrentTimeStamp.Date);
        }

        public IResult<IPickableInventoryReturn> GetInventoryItemsToPickBatch(FilterInventoryForBatchParameters parameters)
        {
            parameters = parameters ?? new FilterInventoryForBatchParameters();

            var filterResults = parameters.ParseToPredicateBuilderFilters();
            if(!filterResults.Success)
            {
                return filterResults.ConvertTo<IPickableInventoryReturn>();
            }
            var filters = filterResults.ResultingObject;

            var itemsResult = new GetPickableInventoryCommand(_productionUnitOfWork).Execute(filters, _timeStamper.CurrentTimeStamp, PickedInventoryValidator.ForProductionBatch, false);
            return itemsResult;
        }

        public IResult<IEnumerable<string>> GetProductionBatchInstructions()
        {
            var instructions = _productionUnitOfWork.InstructionRepository
                .Filter(i => i.InstructionType == InstructionType.ProductionBatchInstruction).Select(i => i.InstructionText).Distinct().ToList()
                .Where(s => !string.IsNullOrWhiteSpace(s)).OrderBy(s => s).ToList();
            return new SuccessResult<IEnumerable<string>>(instructions);
        }

        public IResult<IProductionPacketReturn> GetProductionPacketForBatch(string productionBatchKey)
        {
            var batchKeyResult = KeyParserHelper.ParseResult<ILotKey>(productionBatchKey);
            if(!batchKeyResult.Success)
            {
                return batchKeyResult.ConvertTo<IProductionPacketReturn>();
            }

            return new GetProductionPacketConductor(_productionUnitOfWork).Execute(batchKeyResult.ResultingObject, _timeStamper.CurrentTimeStamp);
        }

        public IResult<IProductionPacketReturn> GetProductionPacketForPackSchedule(string packScheduleKey)
        {
            var packScheduleKeyResult = KeyParserHelper.ParseResult<IPackScheduleKey>(packScheduleKey);
            if(!packScheduleKeyResult.Success)
            {
                return packScheduleKeyResult.ConvertTo<IProductionPacketReturn>();
            }

            return new GetProductionPacketConductor(_productionUnitOfWork).Execute(packScheduleKeyResult.ResultingObject, _timeStamper.CurrentTimeStamp);
        }

        public IResult<IPackSchedulePickSheetReturn> GetPackSchedulePickSheet(string packScheduleKey)
        {
            var packScheduleKeyResult = KeyParserHelper.ParseResult<IPackScheduleKey>(packScheduleKey);
            if(!packScheduleKeyResult.Success)
            {
                return packScheduleKeyResult.ConvertTo<IPackSchedulePickSheetReturn>();
            }

            var pickSheet = _productionUnitOfWork.PackScheduleRepository
                .Filter(new PackScheduleKey(packScheduleKeyResult.ResultingObject).FindByPredicate)
                .SplitSelect(PackScheduleProjectors.SplitSelectPickSheet(_productionUnitOfWork, _timeStamper.CurrentTimeStamp.Date))
                .FirstOrDefault();
            if(pickSheet == null)
            {
                return new InvalidResult<IPackSchedulePickSheetReturn>(null, string.Format(UserMessages.PackScheduleNotFound, packScheduleKey));
            }

            return new SuccessResult<IPackSchedulePickSheetReturn>(pickSheet);
        }

        public IResult<IQueryable<IWorkTypeReturn>> GetWorkTypes()
        {
            var selector = WorkTypeProjectors.Select();

            var query = _productionUnitOfWork.WorkTypeRepository.All().AsExpandable().Select(selector);
            return new SuccessResult<IQueryable<IWorkTypeReturn>>(query);
        }

        public IResult<IQueryable<ILocationReturn>> GetProductionLines()
        {
            var predicate = LocationPredicates.ProductionLinesFilter;
            var selector = LocationProjectors.SelectLocation();

            var query = _productionUnitOfWork.LocationRepository.Filter(predicate).AsExpandable().Select(selector);
            return new SuccessResult<IQueryable<ILocationReturn>>(query);
        }

        [SynchronizeOldContext(NewContextMethod.ProductionSchedule)]
        public IResult<string> CreateProductionSchedule(ICreateProductionScheduleParameters parameters)
        {
            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult.ConvertTo<string>();
            }

            var createResult = new CreateProductionScheduleConductor(_productionUnitOfWork).Create(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject);
            if(!createResult.Success)
            {
                return createResult.ConvertTo<string>();
            }

            _productionUnitOfWork.Commit();

            var productionScheduleKey = createResult.ResultingObject.ToProductionScheduleKey();
            return SyncParameters.Using(new SuccessResult<string>(productionScheduleKey), productionScheduleKey);
        }

        [SynchronizeOldContext(NewContextMethod.ProductionSchedule)]
        public IResult UpdateProductionSchedule(IUpdateProductionScheduleParameters parameters)
        {
            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult.ConvertTo<string>();
            }

            var updateResult = new UpdateProductionScheduleConductor(_productionUnitOfWork).Update(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject);
            if(!updateResult.Success)
            {
                return updateResult.ConvertTo<string>();
            }

            _productionUnitOfWork.Commit();

            var productionScheduleKey = updateResult.ResultingObject.ToProductionScheduleKey();
            return SyncParameters.Using(new SuccessResult<string>(productionScheduleKey), productionScheduleKey);
        }

        [SynchronizeOldContext(NewContextMethod.ProductionSchedule)]
        public IResult DeleteProductionSchedule(string productionScheduleKey)
        {
            var keyResult = KeyParserHelper.ParseResult<IProductionScheduleKey>(productionScheduleKey);
            if(!keyResult.Success)
            {
                return keyResult;
            }

            var scheduleKey = keyResult.ResultingObject.ToProductionScheduleKey();
            var productionSchedule = _productionUnitOfWork.ProductionScheduleRepository.FindByKey(scheduleKey,
                p => p.ProductionLineLocation,
                p => p.ScheduledItems);
            if(productionSchedule == null)
            {
                return new NoWorkRequiredResult();
            }

            foreach(var item in productionSchedule.ScheduledItems.ToList())
            {
                _productionUnitOfWork.ProductionScheduleItemRepository.Remove(item);
            }
            _productionUnitOfWork.ProductionScheduleRepository.Remove(productionSchedule);

            _productionUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), scheduleKey);
        }

        public IResult<IQueryable<IProductionScheduleSummaryReturn>> GetProductionSchedules(FilterProductionScheduleParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<IQueryable<IProductionScheduleSummaryReturn>>();
            }

            var predicateResult = ProductionSchedulePredicateBuilder.BuildPredicate(parsedParameters.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<IProductionScheduleSummaryReturn>>();
            }

            var results = _productionUnitOfWork.ProductionScheduleRepository
                .Filter(predicateResult.ResultingObject).Select(ProductionScheduleProjectors.SelectSummary());

            return new SuccessResult<IQueryable<IProductionScheduleSummaryReturn>>(results);
        }

        public IResult<IProductionScheduleDetailReturn> GetProductionSchedule(string productionScheduleKey)
        {
            var keyResult = KeyParserHelper.ParseResult<IProductionScheduleKey>(productionScheduleKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<IProductionScheduleDetailReturn>();
            }

            var result = _productionUnitOfWork.ProductionScheduleRepository
                .FilterByKey(keyResult.ResultingObject.ToProductionScheduleKey())
                .SplitSelect(ProductionScheduleProjectors.SelectDetail())
                .FirstOrDefault();
            if(result == null)
            {
                return new InvalidResult<IProductionScheduleDetailReturn>(null, string.Format(UserMessages.ProductionScheduleNotFound, productionScheduleKey));
            }

            return new SuccessResult<IProductionScheduleDetailReturn>(result);
        }

        public IResult<IEnumerable<IProductionScheduleReportReturn>> GetProductionScheduleReport(DateTime productionDate, string productionLocationKey)
        {
            ILocationKey locationKey = null;
            if(!string.IsNullOrWhiteSpace(productionLocationKey))
            {
                var keyResult = KeyParserHelper.ParseResult<ILocationKey>(productionLocationKey);
                if(!keyResult.Success)
                {
                    return keyResult.ConvertTo<IEnumerable<IProductionScheduleReportReturn>>();
                }
                locationKey = keyResult.ResultingObject;
            }

            var predicate = ProductionSchedulePredicates.ByProductionDate(productionDate);
            if(locationKey != null)
            {
                predicate = predicate.AndExpanded(ProductionSchedulePredicates.ByProductionLineLocationKey(locationKey));
            }
            var result = _productionUnitOfWork.ProductionScheduleRepository
                .Filter(predicate)
                .SplitSelect(ProductionScheduleProjectors.SelectReport())
                .ToList();

            return new SuccessResult<IEnumerable<IProductionScheduleReportReturn>>(result);
        }
    }
}