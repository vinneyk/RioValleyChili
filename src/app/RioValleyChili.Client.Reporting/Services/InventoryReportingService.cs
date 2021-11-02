using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public class InventoryReportingService : ReportingService
    {
        public InventoryReportingService() : this(ResolveService<IInventoryService>()) { }

        public InventoryReportingService(IInventoryService inventoryService)
        {
            if(inventoryService == null) { throw new ArgumentNullException("inventoryService"); }
            _inventoryService = inventoryService;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<InventoryCycleCountLocation> GetCycleCount(string facilityKey, string groupName)
        {
            var result = _inventoryService.GetInventoryCycleCountReport(facilityKey, groupName);
            if(result.Success)
            {
                var mapped = result.ResultingObject.Map().To<InventoryCycleCountReportModel>();
                return mapped.Locations.ToList();
            }

            return null;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<KeyValuePair<string, string>> GetFacilities()
        {
            return _inventoryService.GetFacilityKeys().ResultingObject;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<string> GetLocationGroupsNames(string facilityKey)
        {
            var result = _inventoryService.GetFacilityGroupNames(facilityKey);
            if(result.Success)
            {
                return result.ResultingObject.Select(r => r.LocationGroupName).ToList();
            }

            return null;
        }

        private readonly IInventoryService _inventoryService;
    }
}