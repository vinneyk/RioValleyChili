using System.Net;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.Models.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Tests.ApiControllerTests
{
    [TestFixture]
    public class ProductionResultsControllerTests
    {
        protected ProductionResultsController SystemUnderTest;
        protected Mock<IProductionResultsService> MockProductionResultsService;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected IFixture Fixture = AutoFixtureHelper.BuildFixture();

        public ProductionResultsControllerTests()
        {
            AutoMapperConfiguration.Configure();
        }

        [SetUp]
        public void SetUp()
        {
            MockProductionResultsService = new Mock<IProductionResultsService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            SystemUnderTest = new ProductionResultsController(MockProductionResultsService.Object, MockUserIdentityProvider.Object);

            MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()))
                .Verifiable();
        }

        [TestFixture]
        public class GetTests : ProductionResultsControllerTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void Returns404IfNotFound()
            {
                // Arrange
                const string id = "12345";
                MockProductionResultsService.Setup(m => m.GetProductionResultDetailByProductionBatchKey(id))
                    .Returns(new InvalidResult<IProductionResultDetailReturn>());

                // Act
                try
                {
                    SystemUnderTest.Get(id);
                }
                catch (HttpResponseException ex)
                {
                    // Assert
                    Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
                    throw;
                }
            }
        }

        [TestFixture]
        public class PutTests : ProductionResultsControllerTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();

                MockProductionResultsService
                    .Setup(m => m.UpdateProductionBatchResults(It.IsAny<IUpdateProductionBatchResultsParameters>()))
                    .Returns(new SuccessResult());
                MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<UpdateProductionBatchResultsParameters>()))
                    .Returns((UpdateProductionBatchResultsParameters p) =>
                        {
                            p.UserToken = "UserToken";
                            return p;
                        });
            }

            [Test]
            public void SetsProductionResultsKeyFromParameter()
            {
                // Arrange
                const string key = "123";
                var data = Fixture.Create<UpdateProductionBatchResultsDto>();

                string actualKeyParam = null;
                MockProductionResultsService.Setup(m => m.UpdateProductionBatchResults(It.IsAny<IUpdateProductionBatchResultsParameters>()))
                    .Callback((IUpdateProductionBatchResultsParameters input) => actualKeyParam = input.ProductionResultKey)
                    .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Put(key, data);

                // Assert
                Assert.AreEqual(key, actualKeyParam);
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // Arrange
                var data = Fixture.Create<UpdateProductionBatchResultsDto>();
                UpdateProductionBatchResultsParameters parameters = null;
                MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<UpdateProductionBatchResultsParameters>()))
                    .Callback((UpdateProductionBatchResultsParameters p) => parameters = p)
                    .Returns((UpdateProductionBatchResultsParameters p) =>
                        {
                            p.UserToken = "UserToken";
                            return p;
                        });

                // Act
                SystemUnderTest.Put("123", data);

                // Assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(parameters), Times.Once());
            }

            [Test]
            public void Returns400IfModelIsInvalid()
            {
                // Arrange
                SystemUnderTest.ModelState.AddModelError("", "Error message");
                var data = Fixture.Create<UpdateProductionBatchResultsDto>();

                // Act
                var result = SystemUnderTest.Put("1", data);

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }

        [TestFixture]
        public class PostTests : ProductionResultsControllerTests
        {
            private const string SuccessResultValue = "New Key 123";
            private const string LinkValue = "http://localhost/link/1";

            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                MockProductionResultsService.Setup(
                    m => m.CreateProductionBatchResults(It.IsAny<CreateProductionBatchResultsParameters>()))
                    .Returns(new SuccessResult<string>(SuccessResultValue));
            }

            [Test]
            public void Returns400_WhenModelStateIsInvalid()
            {
                // Arrange
                SystemUnderTest.ModelState.AddModelError("", "Bad Santa.");
                var input = Fixture.Create<CreateProductionBatchResultsDto>();

                // Act
                var result = SystemUnderTest.Post(input);

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }

            [Test]
            public void Returns201_OnSuccess()
            {
                // Arrange
                var input = Fixture.Create<CreateProductionBatchResultsDto>();

                // Act
                var result = SystemUnderTest.Post(input);

                // Assert
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
            }

            [Test]
            public void Returns500_OnFailure()
            {
                // Arrange
                var input = Fixture.Create<CreateProductionBatchResultsDto>();
                MockProductionResultsService.Setup(m => m.CreateProductionBatchResults(It.IsAny<CreateProductionBatchResultsParameters>()))
                    .Returns(new FailureResult<string>());

                // Act
                var result = SystemUnderTest.Post(input);

                // Assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // Arrange
                var input = Fixture.Create<CreateProductionBatchResultsDto>();
                
                // Act
                SystemUnderTest.Post(input);

                // Assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(It.IsAny<CreateProductionBatchResultsParameters>()), Times.Once());
            }
            
            [Test]
            public void CallsCreateProductionBatchResultsMethod()
            {
                // Arrange
                var input = Fixture.Create<CreateProductionBatchResultsDto>();
                
                // Act
                SystemUnderTest.Post(input);

                // Assert
                MockProductionResultsService.Verify(m => m.CreateProductionBatchResults(It.IsAny<CreateProductionBatchResultsParameters>()), Times.Once());
            }
        }
    }
}
