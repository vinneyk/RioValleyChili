using System;
using System.Net;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class ProductIngredientsControllerTests
    {
        protected Mock<IProductService> MockProductService;
        protected Mock<IUserIdentityProvider> MockUserTokenProvider;
        protected ProductIngredientsController SystemUnderTest;
        protected static readonly IFixture Fixture = AutoFixtureHelper.BuildFixture();

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfiguration.Configure();
            MockProductService = new Mock<IProductService>();
            MockUserTokenProvider = new Mock<IUserIdentityProvider>();
            MockUserTokenProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>())).Verifiable();

            SystemUnderTest = new ProductIngredientsController(MockProductService.Object, MockUserTokenProvider.Object);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTest()
        {
            new ProductIngredientsController(null, MockUserTokenProvider.Object);
        }

        [TestFixture]
        public class PostTests : ProductIngredientsControllerTests
        {
            [Test]
            public void TranslatesDtoIntoServiceParameter()
            {
                // Arrange
                ISetChileProductIngredientsParameters actualParams = null;
                MockProductService.Setup(m => m.SetChileProductIngredients(It.IsAny<ISetChileProductIngredientsParameters>()))
                                  .Callback((ISetChileProductIngredientsParameters p) => actualParams = p)
                                  .Returns(new SuccessResult<string>());

                const string expectedUserToken = "user13";
                MockUserTokenProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()))
                                     .Callback((IUserIdentifiable o) => o.UserToken = expectedUserToken);

                const string expectedKey = "123";
                var param = Fixture.Create<SetChileProductIngredientsRequest>();

                // Act
                SystemUnderTest.Post(expectedKey, param);

                // Assert
                Assert.AreEqual(expectedKey, actualParams.ChileProductKey);
                Assert.AreEqual(expectedUserToken, actualParams.UserToken);
            }

            [Test]
            public void SetsUserTokenFromUserIdentityProvider()
            {
                // Arrange
                ISetChileProductIngredientsParameters actualParams = null;
                MockProductService.Setup(m => m.SetChileProductIngredients(It.IsAny<ISetChileProductIngredientsParameters>()))
                                  .Callback((ISetChileProductIngredientsParameters p) => actualParams = p)
                                  .Returns(new SuccessResult<string>());

                const string expectedUserToken = "user13";
                MockUserTokenProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()))
                                     .Callback((IUserIdentifiable o) => o.UserToken = expectedUserToken);

                // Act
                SystemUnderTest.Post("2134", Fixture.Create<SetChileProductIngredientsRequest>());

                // Assert
                Assert.AreEqual(expectedUserToken, actualParams.UserToken);
                MockUserTokenProvider.Verify(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()), Times.Once());
            }

            [Test]
            public void CallsSetChileProductIngredientMethod()
            {
                // Arrange
                MockProductService.Setup(m => m.SetChileProductIngredients(It.IsAny<ISetChileProductIngredientsParameters>()))
                                  .Returns(new SuccessResult<string>());

                const string key = "12345";
                var input = Fixture.Create<SetChileProductIngredientsRequest>();

                // Act
                SystemUnderTest.Post(key, input);

                // Assert
                MockProductService.Verify(m => m.SetChileProductIngredients(It.IsAny<ISetChileProductIngredientsParameters>()), Times.Once());
            }

            [Test]
            public void Returns201_OnSuccess()
            {
                // Arrange
                MockProductService.Setup(m => m.SetChileProductIngredients(It.IsAny<ISetChileProductIngredientsParameters>()))
                                  .Returns(new SuccessResult<string>());

                var input = Fixture.Create<SetChileProductIngredientsRequest>();

                // Act
                var result = SystemUnderTest.Post("key", input);

                // Assert
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
            }

            [Test]
            public void Returns400_OnInvalid()
            {
                // Arrange
                const string message = "Error Message";
                MockProductService.Setup(m => m.SetChileProductIngredients(It.IsAny<ISetChileProductIngredientsParameters>()))
                                  .Returns(new InvalidResult<string>(null, message));

                var input = Fixture.Create<SetChileProductIngredientsRequest>();

                // Act
                var result = SystemUnderTest.Post("key", input);

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.AreEqual(message, result.ReasonPhrase);
            }

            [Test]
            public void Returns500_OnFailure()
            {
                // Arrange
                const string message = "Error Message";
                MockProductService.Setup(m => m.SetChileProductIngredients(It.IsAny<ISetChileProductIngredientsParameters>()))
                                  .Returns(new FailureResult<string>(null, message));

                var input = Fixture.Create<SetChileProductIngredientsRequest>();

                // Act
                var result = SystemUnderTest.Post("key", input);

                // Assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
                Assert.AreEqual(message, result.ReasonPhrase);
            }
        }
    }
}
