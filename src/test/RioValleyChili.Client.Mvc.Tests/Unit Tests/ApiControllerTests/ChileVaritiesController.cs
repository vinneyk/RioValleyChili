using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Tests.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class ChileVaritiesControllerTests
    {
        private readonly IFixture _fixture = AutoFixtureHelper.BuildFixture();
        private Mock<IMaterialsReceivedService> mockDehydratedMaterialsService;
        private ChileVaritiesController systemUnderTest;

        [SetUp]
        public void SetUp()
        {
            mockDehydratedMaterialsService = new Mock<IMaterialsReceivedService>();
            systemUnderTest = new ChileVaritiesController(mockDehydratedMaterialsService.Object);
        }

        [Test]
        public void Get_ReturnsResultsFromServiceMethod()
        {
            // Arrange
            var expectedResults = _fixture.CreateMany<string>();
            mockDehydratedMaterialsService.Setup(m => m.GetChileVarieties())
                                          .Returns(new SuccessResult<IEnumerable<string>>(expectedResults));

            // Assess
            Assess.IsNotEmpty(expectedResults);

            // Act
            var results = systemUnderTest.Get();

            // Assert
            Assert.AreEqual(expectedResults, results);
        }

        [Test]
        public void WhenServiceResultsAreEmpty_ReturnsEmpty()
        {
            // Arrange
            var expectedResults = new string[0];
            mockDehydratedMaterialsService.Setup(m => m.GetChileVarieties())
                                          .Returns(new SuccessResult<IEnumerable<string>>(expectedResults));

            // Assess
            Assess.IsEmpty(expectedResults);

            // Act
            var results = systemUnderTest.Get();

            // Assert
            Assert.AreEqual(expectedResults, results);
        }
    }
}