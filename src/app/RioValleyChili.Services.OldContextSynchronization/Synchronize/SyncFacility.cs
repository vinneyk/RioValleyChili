using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncFacility)]
    public class SyncFacility : SyncCommandBase<IFacilityUnitOfWork, SyncFacilityParameters>
    {
        public SyncFacility(IFacilityUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SyncFacilityParameters> getInput)
        {
            var parameters = getInput();

            int whid;
            if(parameters.DeleteWHID == null)
            {
                var facilityKey = new FacilityKey(parameters.FacilityKey);
                var facility = UnitOfWork.FacilityRepository.FindByKey(facilityKey);
                if(facility == null)
                {
                    throw new Exception(string.Format("Could not find Facility[{0}].", facilityKey));
                }
            
                if(facility.WHID == null)
                {
                    Create(facility);
                    UnitOfWork.Commit();
                }
                else 
                {
                    Update(GetTblWarehouse(facility.WHID.Value), facility);
                }
                whid = facility.WHID.Value;
            }
            else
            {
                OldContext.tblWarehouses.DeleteObject(GetTblWarehouse(parameters.DeleteWHID.Value));
                whid = parameters.DeleteWHID.Value;
            }

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.SyncFacility, whid);
        }

        private tblWarehouse GetTblWarehouse(int whid)
        {
            var tblWarehouse = OldContext.tblWarehouses.FirstOrDefault(w => w.WHID == whid);
            if(tblWarehouse == null)
            {
                throw new Exception(string.Format("Could not find tblWarehouse[{0}] record.", whid));
            }

            return tblWarehouse;
        }

        private void Create(Facility facility)
        {
            var tblWarehouse = new tblWarehouse { WHID = (OldContext.tblWarehouses.Any() ? OldContext.tblWarehouses.Max(w => w.WHID) : 0) + 1 };
            facility.WHID = tblWarehouse.WHID;
            OldContext.tblWarehouses.AddObject(tblWarehouse);
            Update(tblWarehouse, facility);
        }

        private static void Update(tblWarehouse tblWarehouse, Facility facility)
        {
            tblWarehouse.InActive = !facility.Active;
            tblWarehouse.WhouseAbbr = facility.Name;
            tblWarehouse.Phone = facility.PhoneNumber;
            tblWarehouse.Email = facility.EMailAddress;
            tblWarehouse.Address1 = facility.Address.AddressLine1;
            tblWarehouse.Address2 = facility.Address.AddressLine2;
            tblWarehouse.Address3 = facility.Address.AddressLine3;
            tblWarehouse.City = facility.Address.City;
            tblWarehouse.State = facility.Address.State;
            tblWarehouse.Zip = facility.Address.PostalCode;
            tblWarehouse.Country = facility.Address.Country;
        }
    }
}