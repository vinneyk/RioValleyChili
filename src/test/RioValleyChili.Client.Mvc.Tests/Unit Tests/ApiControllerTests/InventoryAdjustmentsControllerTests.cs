using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;
using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;
using RioValleyChili.Tests.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ControllerTests
{
    [TestFixture]
    public class InventoryAdjustmentsControllerTests
    {
        protected Mock<IInventoryAdjustmentsService> MockInventoryAdjustmentsService;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected InventoryAdjustmentsController SystemUnderTest;
        protected IFixture Fixture = AutoFixtureHelper.BuildFixture();

        [SetUp]
        public void SetUp()
        {
            MockInventoryAdjustmentsService = new Mock<IInventoryAdjustmentsService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            SystemUnderTest = new InventoryAdjustmentsController(MockInventoryAdjustmentsService.Object, MockUserIdentityProvider.Object);
        }

        [TestFixture]
        public class Post : InventoryAdjustmentsControllerTests
        {
            [SetUp]
            public void SetUp()
            {
                base.SetUp();
                MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>())).Verifiable();
                MockInventoryAdjustmentsService
                    .Setup(m => m.CreateInventoryAdjustment(It.IsAny<ICreateInventoryAdjustmentParameters>()))
                    .Returns(new SuccessResult<string>("123456"));
            }

            [Test]
            public async void UtilizesUserIdentityProvider()
            {
                // Arrange
                var values = Fixture.Create<InventoryAdjustmentDto>();

                // Act
                await SystemUnderTest.Post(values);

                // Assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(values), Times.Once());
            }

            [Test]
            public async void Returns201ResponseOnSuccess()
            {
                // arrange
                var values = Fixture.Create<InventoryAdjustmentDto>();

                // act
                var result = await SystemUnderTest.Post(values);

                // assert
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
            }

            [Test]
            public async void ReturnsInventoryAdjustmentDetailsOnSuccess()
            {
                // arrange
                var values = Fixture.Create<InventoryAdjustmentDto>();
                var expectedReturn = Fixture.Create<IInventoryAdjustmentReturn>();
                MockInventoryAdjustmentsService
                    .Setup(m => m.GetInventoryAdjustment(It.IsAny<string>()))
                    .Returns(new SuccessResult<IInventoryAdjustmentReturn>(expectedReturn));

                // act
                var result = await SystemUnderTest.Post(values);

                // assert
                Assert.IsNotNull(result.Content, "The Result's Content was null.");
                var content = result.Content as ObjectContent<IInventoryAdjustmentReturn>;
                Assert.IsNotNull(content);
                Assert.AreEqual(expectedReturn, content.Value);
            }
        }
    }
}