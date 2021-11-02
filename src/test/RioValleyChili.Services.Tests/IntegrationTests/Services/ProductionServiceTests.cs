using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class ProductionServiceTests : ServiceIntegrationTestBase<ProductionService>
    {
        [TestFixture]
        public class GetWorkTypes : ProductionServiceTests
        {
            [Test]
            public void Returns_an_empty_collection_if_there_are_no_WorkTypes_in_database()
            {
                //Act
                var result = Service.GetWorkTypes();

                //Assert
                result.AssertSuccess();
                var workTypes = result.ResultingObject.ToList();
                Assert.IsEmpty(workTypes);
            }

            [Test]
            public void Returns_all_WorkTypes_in_database()
            {
                //Arrange
                var expectedWorkTypeKey0 = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>());
                var expectedWorkTypeKey1 = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>());
                var expectedWorkTypeKey2 = new WorkTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<WorkType>());

                //Act
                var result = Service.GetWorkTypes();

                //Assert
                result.AssertSuccess();
                var workTypes = result.ResultingObject.ToList();
                Assert.IsNotNull(workTypes.SingleOrDefault(w => w.WorkTypeKey == expectedWorkTypeKey0.KeyValue));
                Assert.IsNotNull(workTypes.SingleOrDefault(w => w.WorkTypeKey == expectedWorkTypeKey1.KeyValue));
                Assert.IsNotNull(workTypes.SingleOrDefault(w => w.WorkTypeKey == expectedWorkTypeKey2.KeyValue));
            }
        }

        [TestFixture]
        public class GetProductionLines : ProductionServiceTests
        {
            [Test]
            public void Returns_an_empty_collection_if_there_are_no_ProductionLocations_of_type_Line_in_database()
            {
                //Arrange
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionStaging);

                //Act
                var result = Service.GetProductionLines();

                //Assert
                result.AssertSuccess();
                var lines = result.ResultingObject.ToList();
                Assert.IsEmpty(lines);
            }

            [Test]
            public void Returns_all_ProductionLocations_of_type_Line_only()
            {
                //Arrange
                var unexpectedLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionStaging));
                var expectedLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));
                var unexpectedLocationKey1 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionStaging));
                var expectedLocationKey1 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>(l => l.LocationType = LocationType.ProductionLine));

                //Act
                var result = Service.GetProductionLines();

                //Assert
                result.AssertSuccess();
                var lines = result.ResultingObject.ToList();
                Assert.IsNull(lines.SingleOrDefault(l => l.LocationKey == unexpectedLocationKey0.KeyValue));
                Assert.IsNull(lines.SingleOrDefault(l => l.LocationKey == unexpectedLocationKey1.KeyValue));
                Assert.IsNotNull(lines.SingleOrDefault(l => l.LocationKey == expectedLocationKey0.KeyValue));
                Assert.IsNotNull(lines.SingleOrDefault(l => l.LocationKey == expectedLocationKey1.KeyValue));
            }
        }
    }
}