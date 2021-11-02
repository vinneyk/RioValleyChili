using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IInventoryService 
    {
        IResult<IInventoryReturn> GetInventory(FilterInventoryParameters parameters = null);

        IResult<IEnumerable<KeyValuePair<string, double>>> CalculateAttributeWeightedAverages(IEnumerable<KeyValuePair<string, int>> inventoryKeysAndQuantites);

        IResult<string> ReceiveInventory(IReceiveInventoryParameters parameters);

        IResult<IQueryable<IInventoryTransactionReturn>> GetInventoryReceived(GetInventoryReceivedParameters parameters);

        IResult<IQueryable<IInventoryTransactionReturn>> GetInventoryTransactions(string lotKey);

        IResult<IInventoryTransactionsByLotReturn> GetInventoryTransactionsByDestinationLot(string lotKey);

        IResult<IInventoryCycleCountReportReturn> GetInventoryCycleCountReport(string facilityKey, string groupName);

        IResult<Dictionary<string, string>> GetFacilityKeys();

        IResult<IEnumerable<ILocationGroupNameReturn>> GetFacilityGroupNames(string facilityKey);
    }
}