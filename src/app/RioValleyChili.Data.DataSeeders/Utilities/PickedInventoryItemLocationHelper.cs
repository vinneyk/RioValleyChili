using System;
using System.Diagnostics;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class PickedInventoryItemLocationHelper
    {
        private readonly NewContextHelper _newContextHelper;

        public PickedInventoryItemLocationHelper(NewContextHelper newContextHelper)
        {
            if(newContextHelper == null) { throw new ArgumentNullException("newContextHelper"); }
            _newContextHelper = newContextHelper;
        }

        public enum Result
        {
            UnableToDetermine,
            DeterminedFromOutgoingRecords,
            DeterminedFromBatchItemLocation
        }

        public Result DeterminePickedFromLocation(ProductionBatchEntityObjectMother.tblBatchItemDTO batchItem, out int? warehouseLocationId)
        {
            warehouseLocationId = null;
            var outgoing = batchItem.tblLot.tblOutgoings.Where(o => o.BatchLot == batchItem.NewLot).OrderByDescending(o => o.EntryDate).ToList();

            Location facilityLocation = null;
            if(outgoing.Any(o => (facilityLocation = _newContextHelper.GetLocation(o.LocID)) != null))
            {
                warehouseLocationId = facilityLocation.Id;
                return Result.DeterminedFromOutgoingRecords;
            }
            
            if((facilityLocation = _newContextHelper.GetLocation(batchItem.LocID)) != null)
            {
                warehouseLocationId = facilityLocation.Id;
                return Result.DeterminedFromBatchItemLocation;
            }

            return Result.UnableToDetermine;
        }
    }
}