using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly InventoryServiceProvider _inventoryServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public InventoryService(InventoryServiceProvider inventoryServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(inventoryServiceProvider == null) { throw new ArgumentNullException("inventoryServiceProvider"); }
            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }

            _inventoryServiceProvider = inventoryServiceProvider;
            _exceptionLogger = exceptionLogger;
        }

        public IResult<IInventoryReturn> GetInventory(FilterInventoryParameters parameters = null)
        {
            try
            {
                return _inventoryServiceProvider.GetInventory(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryReturn>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<KeyValuePair<string, double>>> CalculateAttributeWeightedAverages(IEnumerable<KeyValuePair<string, int>> inventoryKeysAndQuantites)
        {
            try
            {
                return _inventoryServiceProvider.CalculateAttributeWeightedAverages(inventoryKeysAndQuantites);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<KeyValuePair<string, double>>>(null, ex.Message);
            }
        }

        public IResult<string> ReceiveInventory(IReceiveInventoryParameters parameters)
        {
            try
            {
                return _inventoryServiceProvider.ReceiveInventory(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IInventoryTransactionReturn>> GetInventoryReceived(GetInventoryReceivedParameters parameters)
        {
            try
            {
                return _inventoryServiceProvider.GetInventoryReceived(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IInventoryTransactionReturn>>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IInventoryTransactionReturn>> GetInventoryTransactions(string lotKey)
        {
            try
            {
                return _inventoryServiceProvider.GetInventoryTransactions(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IInventoryTransactionReturn>>(null, ex.Message);
            }
        }

        public IResult<IInventoryTransactionsByLotReturn> GetInventoryTransactionsByDestinationLot(string lotKey)
        {
            try
            {
                return _inventoryServiceProvider.GetLotInputTransactions(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryTransactionsByLotReturn>(null, ex.Message);
            }
        }

        public IResult<IInventoryCycleCountReportReturn> GetInventoryCycleCountReport(string facilityKey, string groupName)
        {
            try
            {
                return _inventoryServiceProvider.GetInventoryCycleCountReport(facilityKey, groupName);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryCycleCountReportReturn>(null, ex.Message);
            }
        }

        public IResult<Dictionary<string, string>> GetFacilityKeys()
        {
            try
            {
                return _inventoryServiceProvider.GetFacilityKeys();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<Dictionary<string, string>>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<ILocationGroupNameReturn>> GetFacilityGroupNames(string facilityKey)
        {
            try
            {
                return _inventoryServiceProvider.GetFacilityLocationGroups(facilityKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<ILocationGroupNameReturn>>(null, ex.Message);
            }
        }
    }
}