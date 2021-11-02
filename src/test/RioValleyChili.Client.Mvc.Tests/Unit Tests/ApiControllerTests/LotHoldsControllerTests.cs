using System.Linq;
using System.Net;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Tests.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class LotHoldsControllerTests
    {
        protected Mock<ILotService> MockLotService;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected LotHoldsController SystemUnderTest;
        protected IFixture Fixture;

        [SetUp]
        public void SetUp()
        {
            MockLotService = new Mock<ILotService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>())).Verifiable();
            SystemUnderTest = new LotHoldsController(MockLotService.Object, MockUserIdentityProvider.Object);
            Fixture = AutoFixtureHelper.BuildFixture();
        }

        [TestFixture]
        public class PutTests : LotHoldsControllerTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                MockLotService.Setup(m => m.SetLotHoldStatus(It.IsAny<ISetLotHoldStatusParameters>()))
                    .Returns(new SuccessResult<ILotStatInfoReturn>());
            }

            [Test]
            public void CallsSetLotHoldStatusServiceMethodAsExpected()
            {
                // arrange
                const string expectedLotKey = "04 14 001 01";
                var holdParameters = Fixture.Create<LotHoldDto>();
                ISetLotHoldStatusParameters actualParameters = null;
                MockLotService.Setup(m => m.SetLotHoldStatus(It.IsAny<ISetLotHoldStatusParameters>()))
                    .Callback((ISetLotHoldStatusParameters p) => { actualParameters = p; })
                    .Returns(new SuccessResult<ILotStatInfoReturn>());

                // act
                SystemUnderTest.Put(expectedLotKey, holdParameters);

                // assert
                MockLotService.Verify(m => m.SetLotHoldStatus(It.IsAny<ISetLotHoldStatusParameters>()), Times.Once());
                Assert.AreEqual(expectedLotKey, actualParameters.LotKey);
                Assert.AreEqual(holdParameters, actualParameters.Hold);
            }

            [Test]
            public void UtilizedUserIdentityProvider()
            {
                // arrange
                var parameters = Fixture.Create<LotHoldDto>();

                // act
                SystemUnderTest.Put("lotkey", parameters);

                // assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()), Times.Once());
            }

            [Test]
            public void RequiresAntiForgeryToken()
            {
                // arrange
                var putMethod = SystemUnderTest.GetType().GetMethod("Put");

                // act
                var antiForgeryTokenValidationAttribute = putMethod.GetCustomAttribute<ValidateAntiForgeryTokenFromCookieAttribute>();

                // assert
                Assert.IsNotNull(antiForgeryTokenValidationAttribute);
            }

            [Test]
            public void RequiresQAHoldsClaim()
            {
                // arrange
                var method = SystemUnderTest.GetType().GetMethod("Put");
                var claimsAuthorize = method.GetCustomAttribute<ClaimsAuthorizeAttribute>();

                // act
                var claims = claimsAuthorize.GetClaims().ToList();

                // assert
                Assert.IsNotNull(claims);
                Assert.IsNotEmpty(claims);
                Assert.IsTrue(claims.Any(c =>
                    c.Type == ClaimActions.Modify
                    && c.Value == ClaimTypes.QualityControlClaimTypes.QAHolds));
            }

            [Test]
            public void Returns200OnSuccess()
            {
                // arrange
                var parameters = Fixture.Create<LotHoldDto>();

                // act
                var result = SystemUnderTest.Put("02 14 01 001", parameters);

                // assert
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            }

            [Test]
            public void Returns400AndBypassesServiceCallWhenModelIsInvalid()
            {
                // arrange
                var parameters = Fixture.Create<LotHoldDto>();
                SystemUnderTest.ModelState.AddModelError("", "error message");

                // act
                var result = SystemUnderTest.Put("02 14 01 001", parameters);

                // assert
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.IsNotNull(result.ReasonPhrase);
                MockLotService.Verify(m => m.SetLotAttributes(It.IsAny<ISetLotAttributeParameters>()), Times.Never());
            }
        }
    }
}
