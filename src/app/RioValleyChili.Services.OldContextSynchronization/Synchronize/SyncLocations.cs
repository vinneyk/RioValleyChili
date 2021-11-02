using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using LinqKit;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncLocations)]
    public class SyncLocations : SyncCommandBase<IFacilityUnitOfWork, SyncLocationsParameters>
    {
        public SyncLocations(IFacilityUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SyncLocationsParameters> getInput)
        {
            var parameters = getInput();
            var keys = parameters.Locations.Select(k => new LocationKey(k)).ToList();
            if(keys.Any())
            {
                bool newContext;
                var oldLocations = SynchronizeLocations(parameters.EmployeeKey, keys, out newContext);
                if(newContext)
                {
                    UnitOfWork.Commit();
                }
                OldContext.SaveChanges();

                Console.Write(ConsoleOutput.SyncLocation, oldLocations[0].LocID);
            }
        }

        private List<tblLocation> SynchronizeLocations(IEmployeeKey employeeKey, List<LocationKey> locationKeys, out bool syncNewContext)
        {
            var predicate = PredicateBuilder.False<Location>().Expand();
            locationKeys.ForEach(k => predicate = predicate.Or(k.FindByPredicate).Expand());

            var locations = UnitOfWork.LocationRepository.Filter(predicate, l => l.Facility);
            var missing = locationKeys.FirstOrDefault(k => !locations.Any(k.FindByPredicate));
            if(missing != null)
            {
                throw new Exception(string.Format("Cannot find Location[{0}].", missing.KeyValue));
            }

            var oldPredicate = PredicateBuilder.False<tblLocation>().Expand();
            var expectingOldLocations = locations.Where(l => l.LocID != null).ToList();
            expectingOldLocations.ForEach(l => oldPredicate = oldPredicate.Or(p => p.LocID == l.LocID.Value).Expand());

            var oldLocations = OldContext.tblLocations.Where(oldPredicate).ToDictionary(l => l.LocID, l => l);
            var oldMissing = expectingOldLocations.FirstOrDefault(l => !oldLocations.ContainsKey(l.LocID.Value));
            if(oldMissing != null)
            {
                throw new Exception(string.Format("Could not find tblLocation[{0}].", oldMissing.LocID.Value));
            }

            syncNewContext = false;
            int? locId = null;
            foreach(var location in locations)
            {
                if(location.LocID == null)
                {
                    syncNewContext = true;
                    var oldLocation = Create(employeeKey, location, ref locId);
                    oldLocations.Add(oldLocation.LocID, oldLocation);
                }
                else
                {
                    Update(employeeKey, location, oldLocations[location.LocID.Value]);
                }
            }

            return oldLocations.Values.ToList();
        }

        private tblLocation Create(IEmployeeKey employeeKey, Location location, ref int? locID)
        {
            locID = (locID ?? (OldContext.tblLocations.Any() ? OldContext.tblLocations.Max(l => l.LocID) : 0)) + 1;

            var tblLocation = Update(employeeKey, location, new tblLocation
                {
                    LocID = locID.Value
                });

            OldContext.tblLocations.AddObject(tblLocation);

            return tblLocation;
        }

        private static tblLocation Update(IEmployeeKey employeeKey, Location location, tblLocation tblLocation)
        {
            string street;
            int row;
            LocationDescriptionHelper.GetStreetRow(location.Description, out street, out row);

            tblLocation.EmployeeID = employeeKey == null ? tblLocation.EmployeeID : employeeKey.EmployeeKey_Id;
            tblLocation.EntryDate = tblLocation.EntryDate ?? DateTime.UtcNow.ConvertUTCToLocal();
            tblLocation.Street = street;
            tblLocation.Row = row;
            tblLocation.WHID = location.Facility.WHID.Value;
            tblLocation.InActive = !location.Active;
            tblLocation.FreezeRow = location.Locked ? "Yes" : null;

            return tblLocation;
        }
    }
}