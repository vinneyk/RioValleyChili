using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class FacilityExtensions
    {
        internal static void AssertEqual(this Facility facility, IFacilitySummaryReturn facilitySummary)
        {
            if(facilitySummary == null) { throw new ArgumentNullException("facilitySummary"); }

            Assert.AreEqual(new FacilityKey(facility).KeyValue, facilitySummary.FacilityKey);
            Assert.AreEqual(facility.Name, facilitySummary.FacilityName);
        }

        internal static void AssertEqual(this Facility facility, ICreateFacilityParameters parameters)
        {
            Assert.AreEqual(parameters.FacilityType, facility.FacilityType);
            Assert.AreEqual(parameters.Name, facility.Name);
            Assert.AreEqual(parameters.Active, facility.Active);
            Assert.AreEqual(parameters.PhoneNumber, facility.PhoneNumber);
            Assert.AreEqual(parameters.EMailAddress, facility.EMailAddress);
            Assert.AreEqual(parameters.ShippingLabelName, facility.ShippingLabelName);
            facility.Address.AssertEqual(parameters.Address);
        }
    }
}