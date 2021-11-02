using System.Net.Http;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Tests.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class InventoryControllerTests
    {
        protected InventoryController SystemUnderTest;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected Mock<IInventoryService> MockInventoryService;
        protected IFixture Fixture = AutoFixtureHelper.BuildFixture();

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfiguration.Configure();

            MockInventoryService = new Mock<IInventoryService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            MockUserIdentityProvider
                .Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()))
                .Verifiable();

            SystemUnderTest = new InventoryController(MockInventoryService.Object, MockUserIdentityProvider.Object);
        }

        public class GetMany : InventoryControllerTests
        {
            [Test]
            public void TypicalSuccessTest()
            {
                // Arrange
                const ProductTypeEnum productType = ProductTypeEnum.Chile;
                MockInventoryService.Setup(m => m.GetInventory(It.IsAny<FilterInventoryParameters>()))
                    .Returns(new SuccessResult<IInventoryReturn>(Fixture.Create<IInventoryReturn>()));

                // Act
                var result = SystemUnderTest.Get(productType);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsNotEmpty(result);
            }
        }
        public class GetDetails : InventoryControllerTests
        {
            [Test]
            public void TypicalSuccessTest()
            {
                // Arrange
                const string lotKey = "03 14 201 01";
                MockInventoryService.Setup(m => m.GetInventory(It.IsAny<FilterInventoryParameters>()))
                    .Returns(new SuccessResult<IInventoryReturn>(Fixture.Create<IInventoryReturn>()));

                // Act
                var result = SystemUnderTest.Get(lotKey);

                // Assert
                Assert.IsNotNull(result);
            }
        }
        public class PutMany : InventoryControllerTests
        {
            [Test]
            public void AntiForgerTokenValidationIsPresent()
            {
                ApiControllerTestHelpers.AssertAntiForgeryTokenValidationForMethod<ReceiveInventoryDto, HttpResponseMessage>(SystemUnderTest.Post);
            }

            [Test]
            public void ClaimsAuthorizationIsPresent()
            {
                ApiControllerTestHelpers.AssertClaimsForMethod<ReceiveInventoryDto, HttpResponseMessage>(SystemUnderTest.Post, new [] { ClaimTypes.InventoryClaimTypes.Inventory }, ClaimActions.Create);
            }
        }
    }
}