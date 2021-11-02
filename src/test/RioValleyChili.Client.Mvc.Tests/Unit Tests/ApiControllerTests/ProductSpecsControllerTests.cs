using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
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
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class ProductSpecsControllerTests
    {
        protected Mock<IProductService> MockProductService;
        protected Mock<IUserIdentityProvider> UserIdentityProviderMock;
        protected ProductSpecsController SystemUnderTest;
        protected readonly IFixture Fixture = AutoFixtureHelper.BuildFixture();

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfiguration.Configure();
            MockProductService = new Mock<IProductService>();
            MockProductService.Setup(m => m.SetChileProductAttributeRanges(It.IsAny<ISetChileProductAttributeRangesParameters>()))
                              .Returns(new SuccessResult<string>());
            UserIdentityProviderMock = new Mock<IUserIdentityProvider>();
            UserIdentityProviderMock.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>())).Verifiable();
            SystemUnderTest = new ProductSpecsController(MockProductService.Object, UserIdentityProviderMock.Object);
        }

        #region GET Tests

        [Test]
        public void Get_CallsServiceMethodWithAppropriateChileProductKey()
        {
            // Arrange
            var expectedProduct = Fixture.Create<IChileProductDetailReturn>();
            MockProductService.Setup(m => m.GetChileProductDetail(expectedProduct.ProductKey))
                              .Returns(new SuccessResult<IChileProductDetailReturn>(expectedProduct));

            // Act
            SystemUnderTest.Get(expectedProduct.ProductKey);


            // Assert
            MockProductService.Verify(m => m.GetChileProductDetail(expectedProduct.ProductKey), Times.Once());
        }

        [Test]
        public void Get_ReturnsProductAttributeRangesForSpecifiedChileProduct()
        {
            // Arrange
            var serviceData = Fixture.CreateMany<IChileProductDetailReturn>(5);
            MockProductService.Setup(m => m.GetChileProductDetail(It.IsAny<string>()))
                              .Returns(
                                  (string productKey) =>
                                  new SuccessResult<IChileProductDetailReturn>(
                                      serviceData.Single(p => p.ProductKey == productKey)));
            var expectedProduct = serviceData.Skip(2).Take(1).Single();

            // Act
            var result = SystemUnderTest.Get(expectedProduct.ProductKey);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(expectedProduct.AttributeRanges, result);
        }

        [Test, ExpectedException(typeof (HttpResponseException))]
        public void Get_ThrowsIfSpecifiedChileProductIsNotFound()
        {
            // Arrange
            MockProductService.Setup(m => m.GetChileProductDetail(It.IsAny<string>()))
                              .Returns(new InvalidResult<IChileProductDetailReturn>());

            // Act
            SystemUnderTest.Get("blah");

            // Assert
        }

        #endregion

        #region GET(id) Tests

        [Test]
        public void GetById_CallsServiceWithAppropriateParameters()
        {
            // Arrange
            var serviceData = Fixture.CreateMany<IChileProductDetailReturn>(5).ToList();
            var expectedProduct = serviceData.Skip(2).Take(1).Single();
            MockProductService.Setup(m => m.GetChileProductDetail(It.IsAny<string>()))
                              .Returns(
                                  (string productKey) =>
                                  new SuccessResult<IChileProductDetailReturn>(
                                      serviceData.Single(p => p.ProductKey == productKey)));

            // Act
            SystemUnderTest.Get(expectedProduct.ProductKey);

            // Assert
            MockProductService.Verify(m => m.GetChileProductDetail(expectedProduct.ProductKey), Times.Once());
        }

        [Test]
        public void GetById_ReturnsProductAttributeRangeForSpecifiedChileProduct()
        {
            // Arrange
            var serviceData = Fixture.CreateMany<IChileProductDetailReturn>(5).ToList();
            var expectedProduct = serviceData.Skip(2).Take(1).Single();
            var expectedAttribute = expectedProduct.AttributeRanges;
            MockProductService.Setup(m => m.GetChileProductDetail(It.IsAny<string>()))
                              .Returns(
                                  (string productKey) =>
                                  new SuccessResult<IChileProductDetailReturn>(
                                      serviceData.Single(p => p.ProductKey == productKey)));

            // Act
            var result = SystemUnderTest.Get(expectedProduct.ProductKey);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(expectedAttribute.Count(), result.Count());
        }

        #endregion

        public class PostTests : ProductSpecsControllerTests
        {
            [SetUp]
            public new void SetUp()
            {
                MockProductService
                    .Setup(m => m.SetChileProductAttributeRanges(It.IsAny<ISetChileProductAttributeRangesParameters>()))
                    .Returns(new SuccessResult<string>(null));
            }

            [Test]
            public void SetsDtoValuesFromUri()
            {
                // Arrange
                const string expectedProductKey = "p12345";
                const string expectedAttributeKey = "asta";
                var postData = new SetChileProductAttributeRangesRequest
                    {
                        AttributeRanges = new List<AttributeRangeRequest>
                            {
                                new AttributeRangeRequest
                                    {
                                        AttributeNameKey = expectedAttributeKey
                                    }
                            }
                    };
                ISetChileProductAttributeRangesParameters actualParams = null;
                MockProductService.Setup(
                    m => m.SetChileProductAttributeRanges(It.IsAny<ISetChileProductAttributeRangesParameters>()))
                    .Callback((ISetChileProductAttributeRangesParameters p) => actualParams = p)
                    .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Post(expectedProductKey, postData);

                // Assert
                Assert.AreEqual(expectedProductKey, actualParams.ChileProductKey);
                Assert.AreEqual(expectedAttributeKey, actualParams.AttributeRanges.Single().AttributeNameKey);
            }

            [Test]
            public void CallsServiceMethodAsExpected()
            {
                // Arrange
                const string expectedProductKey = "p12345";
                var postData = Fixture.Create<SetChileProductAttributeRangesRequest>();
                
                // Act
                SystemUnderTest.Post(expectedProductKey, postData);

                // Assert
                MockProductService.Verify(m => m.SetChileProductAttributeRanges(It.IsAny<ISetChileProductAttributeRangesParameters>()), Times.Once());
            }

            [Test]
            public void Returns200OnSuccess()
            {
                // Arrange
                const string expectedProductKey = "p12345";
                var postData = Fixture.Create<SetChileProductAttributeRangesRequest>();

                // Act
                var response = SystemUnderTest.Post(expectedProductKey, postData);

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            [Test]
            public void Returns400OnInvalid()
            {
                // Arrange
                const string expectedProductKey = "p12345";
                var postData = Fixture.Create<SetChileProductAttributeRangesRequest>();
                MockProductService.Setup(m => m.SetChileProductAttributeRanges(It.IsAny<ISetChileProductAttributeRangesParameters>()))
                                  .Returns(new InvalidResult());

                // Act
                var response = SystemUnderTest.Post(expectedProductKey, postData);

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void Returns500OnFailure()
            {
                // Arrange
                const string expectedProductKey = "p12345";
                var postData = Fixture.Create<SetChileProductAttributeRangesRequest>();
                MockProductService.Setup(m => m.SetChileProductAttributeRanges(It.IsAny<ISetChileProductAttributeRangesParameters>()))
                                  .Returns(new FailureResult());

                // Act
                var response = SystemUnderTest.Post(expectedProductKey, postData);

                // Assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // Arrange
                const string expectedProductKey = "p12345";
                var postData = Fixture.Create<SetChileProductAttributeRangesRequest>();
                ISetChileProductAttributeRangesParameters actual = null;
                MockProductService.Setup(m => m.SetChileProductAttributeRanges(It.IsAny<ISetChileProductAttributeRangesParameters>()))
                    .Callback((ISetChileProductAttributeRangesParameters param) => actual = param)
                    .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Post(expectedProductKey, postData);

                // Assert
                Assert.IsNotNull(actual);
                UserIdentityProviderMock.Verify(m => m.SetUserIdentity(actual), Times.Once());
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsIfModelStateIsInvalid()
            {
                // Arrange
                var postData = Fixture.Create<SetChileProductAttributeRangesRequest>();
                SystemUnderTest.ModelState.AddModelError("", "ERROR!");
                
                // Act
                try
                {
                    SystemUnderTest.Post("key", postData);
                }
                catch (HttpResponseException ex)
                {
                    Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
                    throw;
                }
            }
        }
    }
}
