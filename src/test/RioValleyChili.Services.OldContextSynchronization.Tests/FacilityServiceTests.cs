using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using Solutionhead.Services;
using UpdateFacilityParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.UpdateFacilityParameters;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class FacilityServiceTests
    {
        [TestFixture]
        public class SyncFacilityUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, SyncFacilityParameters>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncFacility; } }
        }

        [TestFixture]
        public class SyncLocationUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, SyncLocations>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncLocations; } }
        }

        [TestFixture]
        public class CreateFacility : SynchronizeOldContextIntegrationTestsBase<FacilityService>
        {
            [Test]
            public void Creates_new_tblWarehouse_record_as_expected()
            {
                //Arrange
                var parameters = new UpdateFacilityParameters
                    {
                        UserToken = TestUser.UserName,
                        Name = "Test Facility Inc.",
                        Address = new Address()
                    };
                var result = Service.CreateFacility(parameters);
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SyncFacility);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var whid = int.Parse(resultString);
                var tblMove = new RioAccessSQLEntities().tblWarehouses.FirstOrDefault(w => w.WHID == whid);
                Assert.AreEqual(parameters.Name, tblMove.WhouseAbbr);
            }
        }

        [TestFixture]
        public class UpdateFacility : SynchronizeOldContextIntegrationTestsBase<FacilityService>
        {
            [Test]
            public void Creates_new_tblWarehouse_record_as_expected()
            {
                //Arrange
                var facility = RVCUnitOfWork.FacilityRepository.All().FirstOrDefault();
                if(facility == null)
                {
                    Assert.Inconclusive("No Facility records to test with.");
                }
                
                var parameters = new UpdateFacilityParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilityKey = new FacilityKey(facility),
                        Name = facility.Name == "Test Facility Inc." ? "Happy Fun-Times Inc." : "Test Facility Inc.",
                        Address = new Address()
                    };
                var result = Service.UpdateFacility(parameters);
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SyncFacility);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var whid = int.Parse(resultString);
                var tblMove = new RioAccessSQLEntities().tblWarehouses.FirstOrDefault(w => w.WHID == whid);
                Assert.AreEqual(parameters.Name, tblMove.WhouseAbbr);
            }
        }

        [TestFixture]
        public class DeleteFacility : SynchronizeOldContextIntegrationTestsBase<FacilityService>
        {
            [Test]
            public void Deletes_tblWarehouse_record_as_expected()
            {
                //Arrange
                var facility = RVCUnitOfWork.FacilityRepository.Filter(f => !f.Locations.Any()).FirstOrDefault();
                if(facility == null)
                {
                    Assert.Inconclusive("No Facility records to test with.");
                }

                var result = Service.DeleteFacility(new FacilityKey(facility));
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SyncFacility);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var whid = int.Parse(resultString);
                Assert.IsNull(new RioAccessSQLEntities().tblWarehouses.FirstOrDefault(w => w.WHID == whid));
            }
        }

        [TestFixture]
        public class CreateLocaion : SynchronizeOldContextIntegrationTestsBase<FacilityService>
        {
            [Test]
            public void Creates_new_tblLocation_record_as_expected()
            {
                //Arrange
                var facility = RVCUnitOfWork.FacilityRepository.Filter(f => f.WHID != null).FirstOrDefault();
                if(facility == null)
                {
                    Assert.Inconclusive("No suitable Facility found.");
                }

                var locations = RVCUnitOfWork.LocationRepository.Filter(l => l.Description.StartsWith("A")).ToList();
                var row = 1;
                if(locations.Any())
                {
                    locations.ForEach(l =>
                        {
                            string street;
                            int otherRow;
                            LocationDescriptionHelper.GetStreetRow(l.Description, out street, out otherRow);
                            row = Math.Max(row, otherRow);
                        });
                }
                row += 1;

                //Act
                var result = Service.CreateLocation(new CreateLocationParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilityKey = new FacilityKey(facility),
                        LocationType = LocationType.ProductionLine,
                        Description = string.Format("A~{0}", row),
                        Active = true,
                        Locked = true,
                    });
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SyncLocation);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var locId = int.Parse(resultString);
                var location = new RioAccessSQLEntities().tblLocations.FirstOrDefault(l => l.LocID == locId);
                Assert.AreEqual("A", location.Street);
                Assert.AreEqual(row, location.Row);
                Assert.AreEqual(facility.WHID.Value, location.WHID);
            }
        }

        [TestFixture]
        public class UpdateLocation : SynchronizeOldContextIntegrationTestsBase<FacilityService>
        {
            [Test]
            public void Updates_tblLocation_record_as_expected()
            {
                //Arrange
                var location = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null).FirstOrDefault();
                if(location == null)
                {
                    Assert.Inconclusive("Could not find suitable Location to test.");
                }

                //Act
                var result = Service.UpdateLocation(new UpdateLocationParameters
                    {
                        UserToken = TestUser.UserName,
                        LocationKey = new LocationKey(location),
                        Description = location.Description,
                        Active = !location.Active,
                        Locked = true,
                    });
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SyncLocation);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var locId = int.Parse(resultString);
                var tblLocation = new RioAccessSQLEntities().tblLocations.FirstOrDefault(l => l.LocID == locId);
                Assert.AreNotEqual(!location.Active, tblLocation.InActive);
            }
        }

        [TestFixture]
        public class LockLocations : SynchronizeOldContextIntegrationTestsBase<FacilityService>
        {
            [Test]
            public void Locks_Locations_as_expected()
            {
                //Arrange
                var locations = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null && !l.Locked).OrderBy(l => l.Description).Take(2).ToList();
                if(locations.Count != 2)
                {
                    Assert.Inconclusive("Not enough unlocked locations found.");
                }

                //Act
                var result = Service.LockLocations(locations.Select(l => new LocationKey(l).KeyValue));

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var context = new RioAccessSQLEntities())
                {
                    foreach(var location in locations)
                    {
                        Assert.IsTrue(context.tblLocations.FirstOrDefault(l => l.LocID == location.LocID).FreezeRow == "Yes");
                    }
                }
            }
        }

        [TestFixture]
        public class UnlockLocations : SynchronizeOldContextIntegrationTestsBase<FacilityService>
        {
            [Test]
            public void Unlocks_Locations_as_expected()
            {
                //Arrange
                var locations = RVCUnitOfWork.LocationRepository.Filter(l => l.LocID != null && l.Locked).OrderBy(l => l.Description).Take(2).ToList();
                if(locations.Count != 2)
                {
                    Assert.Inconclusive("Not enough locked locations found.");
                }

                //Act
                var result = Service.UnlockLocations(locations.Select(l => new LocationKey(l).KeyValue));

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var context = new RioAccessSQLEntities())
                {
                    foreach(var location in locations)
                    {
                        Assert.IsTrue(context.tblLocations.FirstOrDefault(l => l.LocID == location.LocID).FreezeRow == null);
                    }
                }
            }
        }
    }
}