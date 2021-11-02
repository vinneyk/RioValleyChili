using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Helpers;
using UpdateFacilityParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.UpdateFacilityParameters;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class FacilityServiceTests : ServiceIntegrationTestBase<FacilityService>
    {
        [TestFixture]
        public class CreateFacility : FacilityServiceTests
        {
            [Test]
            public void Creates_new_Facility_as_expected()
            {
                //Arrange
                var parameters = new UpdateFacilityParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilityType = FacilityType.Internal,
                        Name = "Bob's House of Magical Wonders",
                        ShippingLabelName = "Bob Incorporated",
                        Active = false,
                        PhoneNumber = "123-456-7890",
                        EMailAddress = "bob@wonderhouse.com",
                        Address = new Address(),
                    };

                //Act
                var result = Service.CreateFacility(parameters);

                //Assert
                result.AssertSuccess();
                var key = KeyParserHelper.ParseResult<IFacilityKey>(result.ResultingObject).ResultingObject.ToFacilityKey();
                RVCUnitOfWork.FacilityRepository.FindByKey(key).AssertEqual(parameters);
            }
        }

        [TestFixture]
        public class UpdateFacility : FacilityServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Facility_does_not_exist()
            {
                //Act
                var result = Service.UpdateFacility(new UpdateFacilityParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilityKey = new FacilityKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.FacilityNotFound);
            }

            [Test]
            public void Updates_Facility_as_expected()
            {
                //Arrange
                var facilityKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();

                var parameters = new UpdateFacilityParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilityKey = facilityKey,
                        FacilityType = FacilityType.Internal,
                        Name = "Bob's House of Magical Wonders",
                        Active = false,
                        PhoneNumber = "123-456-7890",
                        EMailAddress = "bob@wonderhouse.com",
                        Address = new Address(),
                    };

                //Act
                var result = Service.UpdateFacility(parameters);

                //Assert
                result.AssertSuccess();
                RVCUnitOfWork.FacilityRepository.FindByKey(facilityKey).AssertEqual(parameters);
            }
        }

        [TestFixture]
        public class DeleteFacility : FacilityServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Facility_does_not_exist()
            {
                //Act
                var result = Service.DeleteFacility(new FacilityKey());

                //Assert
                result.AssertNotSuccess(UserMessages.FacilityNotFound);
            }

            [Test]
            public void Updates_Facility_as_expected()
            {
                //Arrange
                var facilityKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>().ToFacilityKey();

                //Act
                var result = Service.DeleteFacility(facilityKey);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.FacilityRepository.FindByKey(facilityKey));
            }
        }

        [TestFixture]
        public class CreateLocation : FacilityServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Facility_does_not_exist()
            {
                //Act
                var result = Service.CreateLocation(new CreateLocationParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilityKey = new FacilityKey(),
                        Description = "MyLocation"
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.FacilityNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_Location_is_empty()
            {
                //Arrange
                var parameters = new CreateLocationParameters
                {
                    UserToken = TestUser.UserName,
                    FacilityKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>())
                };

                //Act
                var result = Service.CreateLocation(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.LocationDescriptionRequired);
            }

            [Test]
            public void Returns_non_successul_result_if_Location_exists_in_Facility_with_specified_Description()
            {
                //Arrange
                var location = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();

                //Act
                var result = Service.CreateLocation(new CreateLocationParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilityKey = new FacilityKey(location),
                        Description = location.Description
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.FacilityContainsLocationDescription);
            }

            [Test]
            public void Creates_new_Location_with_non_unique_Description_in_different_Facility()
            {
                //Arrange
                var location = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var facility = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>();

                var parameters = new CreateLocationParameters
                    {
                        UserToken = TestUser.UserName,
                        FacilityKey = new FacilityKey(facility),
                        Description = location.Description
                    };

                //Act
                var result = Service.CreateLocation(parameters);

                //Assert
                result.AssertSuccess();
                var locationKey = new LocationKey(KeyParserHelper.ParseResult<ILocationKey>(result.ResultingObject).ResultingObject);
                RVCUnitOfWork.LocationRepository.FindByKey(locationKey).AssertEqual(parameters);
            }
        }

        [TestFixture]
        public class UpdateLocation : FacilityServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Location_does_not_exist()
            {
                //Act
                var result = Service.UpdateLocation(new UpdateLocationParameters
                    {
                        UserToken = TestUser.UserName,
                        LocationKey = new LocationKey()
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.LocationNotFound);
            }

            [Test]
            public void Updates_Location_as_expected_on_success()
            {
                //Arrange
                var locationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                
                //Act
                var parameters = new UpdateLocationParameters
                    {
                        UserToken = TestUser.UserName,
                        LocationKey = locationKey
                    };
                var result = Service.UpdateLocation(parameters);

                //Assert
                result.AssertSuccess();
                RVCUnitOfWork.LocationRepository.FindByKey(locationKey).AssertEqual(parameters);
            }
        }

        [TestFixture]
        public class LockLocations : FacilityServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_any_Location_cannot_be_found()
            {
                //Act
                var result = Service.LockLocations(new List<string>
                    {
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()),
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()),
                        new LocationKey(),
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>())
                    });

                //Asset
                result.AssertNotSuccess(UserMessages.LocationNotFound);
            }

            [Test]
            public void Locks_Locations_as_expected_on_success()
            {
                //Arrange
                var locationKeys = new List<LocationKey>
                    {
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()),
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()),
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>())
                    };

                //Act
                var result = Service.LockLocations(locationKeys.Select(k => k.KeyValue));

                //Asset
                result.AssertSuccess();
                foreach(var key in locationKeys)
                {
                    Assert.IsTrue(RVCUnitOfWork.LocationRepository.FindByKey(key).Locked);
                }
            }

            [Test]
            public void Will_not_Lock_other_Locations_on_success()
            {
                //Arrnge
                var leaveAlone = new List<Location>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.Locked = false),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.Locked = true),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.Locked = false),
                    };

                //Act
                var result = Service.LockLocations(new List<string> { new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()) });

                //Assert
                result.AssertSuccess();
                foreach(var location in leaveAlone)
                {
                    Assert.AreEqual(location.Locked, RVCUnitOfWork.LocationRepository.FindByKey(new LocationKey(location)).Locked);
                }
            }
        }

        [TestFixture]
        public class UnlockLocations : FacilityServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_any_Location_cannot_be_found()
            {
                //Act
                var result = Service.UnlockLocations(new List<string>
                    {
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()),
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()),
                        new LocationKey(),
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>())
                    });

                //Asset
                result.AssertNotSuccess(UserMessages.LocationNotFound);
            }

            [Test]
            public void Unlocks_Locations_as_expected_on_success()
            {
                //Arrange
                var locationKeys = new List<LocationKey>
                    {
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()),
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()),
                        new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>())
                    };

                //Act
                var result = Service.UnlockLocations(locationKeys.Select(k => k.KeyValue));

                //Asset
                result.AssertSuccess();
                foreach(var key in locationKeys)
                {
                    Assert.IsFalse(RVCUnitOfWork.LocationRepository.FindByKey(key).Locked);
                }
            }

            [Test]
            public void Will_not_Lock_other_Locations_on_success()
            {
                //Arrnge
                var leaveAlone = new List<Location>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.Locked = false),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.Locked = true),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.Locked = false),
                    };

                //Act
                var result = Service.UnlockLocations(new List<string> { new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>()) });

                //Assert
                result.AssertSuccess();
                foreach(var location in leaveAlone)
                {
                    Assert.AreEqual(location.Locked, RVCUnitOfWork.LocationRepository.FindByKey(new LocationKey(location)).Locked);
                }
            }
        }
    }
}