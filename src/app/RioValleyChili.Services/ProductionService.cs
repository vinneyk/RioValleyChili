using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Interfaces.ServiceCompositions;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class ProductionService : IProductionService
    {
        #region Fields and Constructors.

        private readonly ProductionServiceProvider _productionServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public ProductionService(ProductionServiceProvider productionServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(productionServiceProvider == null) { throw new ArgumentNullException("productionServiceProvider");}
            _productionServiceProvider = productionServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger");}
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        #region Implementation of IProductionService.

        public IResult<string> CreatePackSchedule(ICreatePackScheduleParameters parameters)
        {
            try
            {
                var result = _productionServiceProvider.CreatePackSchedule(parameters);
                return result;
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<string> UpdatePackSchedule(IUpdatePackScheduleParameters parameters)
        {
            try
            {
                var result = _productionServiceProvider.UpdatePackSchedule(parameters);
                return result;
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<string> RemovePackSchedule(IRemovePackScheduleParameters parameters)
        {
            try
            {
                return _productionServiceProvider.RemovePackSchedule(parameters).ConvertTo(parameters.PackScheduleKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(parameters.PackScheduleKey, ex.GetInnermostException().Message);
            }
        }

        public IResult<IQueryable<IPackScheduleSummaryReturn>> GetPackSchedules()
        {
            try
            {
                return _productionServiceProvider.GetPackSchedules();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IPackScheduleSummaryReturn>>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IPackScheduleDetailReturn> GetPackSchedule(string packScheduleKey)
        {
            try
            {
                return _productionServiceProvider.GetPackSchedule(packScheduleKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IPackScheduleDetailReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<ICreateProductionBatchReturn> CreateProductionBatch(ICreateProductionBatchParameters parameters)
        {
            try
            {
                var result = _productionServiceProvider.CreateProductionBatch(parameters);
                if(!result.Success)
                {
                    return result.ConvertTo<ICreateProductionBatchReturn>();
                }
                return result.ConvertTo(result.ResultingObject);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ICreateProductionBatchReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<string> UpdateProductionBatch(IUpdateProductionBatchParameters parameters)
        {
            try
            {
                var result =  _productionServiceProvider.UpdateProductionBatch(parameters);
                if(!result.Success)
                {
                    return result.ConvertTo<string>();
                }
                return result.ConvertTo(result.ResultingObject);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<string> RemoveProductionBatch(string productionBatchKey)
        {
            try
            {
                return _productionServiceProvider.RemoveProductionBatch(productionBatchKey).ConvertTo(productionBatchKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(productionBatchKey, ex.GetInnermostException().Message);
            }
        }

        IResult IPickInventoryServiceComponent.SetPickedInventory(string contextKey, ISetPickedInventoryParameters parameters)
        {
            try
            {
                return _productionServiceProvider.PickInventoryForProductionBatch(contextKey, parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.GetInnermostException().Message);
            }
        }

        IResult<IPickableInventoryReturn> IPickInventoryServiceComponent.GetPickableInventoryForContext(FilterInventoryForPickingContextParameters parameters)
        {
            return GetInventoryItemsToPickBatch((FilterInventoryForBatchParameters)parameters);
        }

        public IResult<IEnumerable<IPickedInventoryItemReturn>> GetPickedInventoryForContext(string contextKey)
        {
            var getBatchResult = GetProductionBatch(contextKey);
            return !getBatchResult.Success 
                ? getBatchResult.ConvertTo((IEnumerable<IPickedInventoryItemReturn>)null) 
                : new SuccessResult<IEnumerable<IPickedInventoryItemReturn>>(getBatchResult.ResultingObject.PickedInventoryItems);
        }

        public IResult SetPickedInventoryForProductionBatch(string productionBatchKey, ISetPickedInventoryParameters parameters)
        {
            return ((IPickInventoryServiceComponent)this).SetPickedInventory(productionBatchKey, parameters);
        }

        public IResult<IProductionBatchDetailReturn> GetProductionBatch(string productionBatchKey)
        {
            try
            {
                return _productionServiceProvider.GetProductionBatch(productionBatchKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IProductionBatchDetailReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IPickableInventoryReturn> GetInventoryItemsToPickBatch(FilterInventoryForBatchParameters parameters = null)
        {
            try
            {
                return _productionServiceProvider.GetInventoryItemsToPickBatch(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IPickableInventoryReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IEnumerable<string>> GetProductionBatchInstructions()
        {
            try
            {
                return _productionServiceProvider.GetProductionBatchInstructions();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<string>>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IProductionPacketReturn> GetProductionPacketForBatch(string productionBatchKey)
        {
            try
            {
                return _productionServiceProvider.GetProductionPacketForBatch(productionBatchKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IProductionPacketReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IProductionPacketReturn> GetProductionPacketForPackSchedule(string packScheduleKey)
        {
            try
            {
                return _productionServiceProvider.GetProductionPacketForPackSchedule(packScheduleKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IProductionPacketReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IPackSchedulePickSheetReturn> GetPackSchedulePickSheet(string packScheduleKey)
        {
            try
            {
                return _productionServiceProvider.GetPackSchedulePickSheet(packScheduleKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IPackSchedulePickSheetReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<string> CreateProductionSchedule(ICreateProductionScheduleParameters parameters)
        {
            try
            {
                return _productionServiceProvider.CreateProductionSchedule(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateProductionSchedule(IUpdateProductionScheduleParameters parameters)
        {
            try
            {
                return _productionServiceProvider.UpdateProductionSchedule(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult DeleteProductionSchedule(string productionScheduleKey)
        {
            try
            {
                return _productionServiceProvider.DeleteProductionSchedule(productionScheduleKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IQueryable<IProductionScheduleSummaryReturn>> GetProductionSchedules(FilterProductionScheduleParameters parameters = null)
        {
            try
            {
                return _productionServiceProvider.GetProductionSchedules(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IProductionScheduleSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult<IProductionScheduleDetailReturn> GetProductionSchedule(string productionScheduleKey)
        {
            try
            {
                return _productionServiceProvider.GetProductionSchedule(productionScheduleKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IProductionScheduleDetailReturn>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<IProductionScheduleReportReturn>> GetProductionScheduleReport(DateTime productionDate, string productionLocationKey)
        {
            try
            {
                return _productionServiceProvider.GetProductionScheduleReport(productionDate, productionLocationKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<IProductionScheduleReportReturn>>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IWorkTypeReturn>> GetWorkTypes()
        {
            try
            {
                return _productionServiceProvider.GetWorkTypes();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IWorkTypeReturn>>(null, ex.Message);
            }
        }

        public IResult<IQueryable<ILocationReturn>> GetProductionLines()
        {
            try
            {
                return _productionServiceProvider.GetProductionLines();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ILocationReturn>>(null, ex.Message);
            }
        }

        #endregion
    }
}