using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LocationExtensions
    {
        internal static Location ConstrainByKeys(this Location location, IFacilityKey facilityKey = null)
        {
            if(location == null) { throw new ArgumentNullException("location"); }

            if(facilityKey != null)
            {
                location.Facility = null;
                location.FacilityId = facilityKey.FacilityKey_Id;
            }

            return location;
        }

        internal static void AssertEqual(this Location location, ILocationReturn locationReturn)
        {
            if(location == null) { throw new ArgumentNullException("location"); }
            if(locationReturn == null) { throw new ArgumentNullException("locationReturn"); }

            Assert.AreEqual(new LocationKey(location).KeyValue, locationReturn.LocationKey);
            Assert.AreEqual(new FacilityKey(location.Facility).KeyValue, locationReturn.FacilityKey);
            Assert.AreEqual(location.Description, locationReturn.Description);
            Assert.AreEqual(location.Facility.Name, locationReturn.FacilityName);

            if(!location.Active)
            {
                Assert.AreEqual(LocationStatus.InActive, locationReturn.Status);
            }
            else if(location.Locked)
            {
                Assert.AreEqual(LocationStatus.Locked, locationReturn.Status);
            }
            else
            {
                Assert.AreEqual(LocationStatus.Available, locationReturn.Status);
            }
        }

        internal static void AssertEqual(this Location location, ICreateLocationParameters parameters)
        {
            if(location == null) { throw new ArgumentNullException("location"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            
            Assert.AreEqual(parameters.FacilityKey, new FacilityKey(location).KeyValue);
            Assert.AreEqual(parameters.LocationType, location.LocationType);
            Assert.AreEqual(parameters.Description, location.Description);
            Assert.AreEqual(parameters.Active, location.Active);
            Assert.AreEqual(parameters.Locked, location.Locked);
        }
        internal static void AssertEqual(this Location location, IUpdateLocationParameters parameters)
        {
            if(location == null) { throw new ArgumentNullException("location"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            
            Assert.AreEqual(parameters.Description, location.Description);
            Assert.AreEqual(parameters.Active, location.Active);
            Assert.AreEqual(parameters.Locked, location.Locked);
        }
    }
}