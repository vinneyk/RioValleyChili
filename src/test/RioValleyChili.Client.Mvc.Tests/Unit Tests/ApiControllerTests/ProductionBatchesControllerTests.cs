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
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Models.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class ProductionBatchesControllerTests : ApiControllerTestsBase<ProductionBatchesController>
    {
        protected ProductionBatchesController ControllerUnderTest;
        protected Mock<IProductionService> MockPackScheduleService;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected IFixture Fixture = AutoFixtureHelper.BuildFixture();

        public ProductionBatchesControllerTests()
        {
            AutoMapperConfiguration.Configure();
        }

        [SetUp]
        public void SetUp()
        {
            MockPackScheduleService = new Mock<IProductionService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>())).Verifiable();

            ControllerUnderTest = new ProductionBatchesController(MockPackScheduleService.Object, MockUserIdentityProvider.Object);
        }

        protected override ProductionBatchesController SystemUnderTest
        {
            get { return ControllerUnderTest; }
        }

        protected override string[] ClaimResources
        {
            get { return new[] { ClaimTypes.ProductionClaimTypes.ProductionBatch }; }
        }

        [TestFixture]
        public class GetByIdTests : ProductionBatchesControllerTests
        {
            [Test]
            public void ReturnsServiceResultObjectOnSuccess()
            {
                // arrange
                var expectedPackSchedule = Fixture.Create<IProductionBatchDetailReturn>();
                const string key = "03 14 010 01";
                MockPackScheduleService.Setup(m => m.GetProductionBatch(key))
                    .Returns(new SuccessResult<IProductionBatchDetailReturn>(expectedPackSchedule));

                // act
                var result = ControllerUnderTest.Get(key);

                // assert
                MockPackScheduleService.Verify(m => m.GetProductionBatch(key));
                Assert.AreEqual(expectedPackSchedule, result);
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void Throws404HttpResponseOnInvalid()
            {
                // arrange
                const string key = "03 14 010 01";
                MockPackScheduleService.Setup(m => m.GetProductionBatch(key))
                    .Returns(new InvalidResult<IProductionBatchDetailReturn>());

                // act
                try
                {
                    ControllerUnderTest.Get(key);
                }
                catch (HttpResponseException ex)
                {
                    // assert
                    Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
                    throw;
                }
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void Throws500HttpResponseOnFailure()
            {
                // arrange
                const string key = "03 14 010 01";
                MockPackScheduleService.Setup(m => m.GetProductionBatch(key))
                    .Returns(new FailureResult<IProductionBatchDetailReturn>());

                // act
                try
                {
                    ControllerUnderTest.Get(key);
                }
                catch (HttpResponseException ex)
                {
                    // assert
                    Assert.AreEqual(HttpStatusCode.InternalServerError, ex.Response.StatusCode);
                    throw;
                }
            }
        }

        [TestFixture]
        public class PostTests : ProductionBatchesControllerTests
        {
            protected class CreateProductonBatchReturn : ICreateProductionBatchReturn
            {
                public string ProductionBatchKey { get; set; }
                public string InstructionNotebookKey { get; set; }
            }
            

            [Test]
            public void CallsServiceAsExpected()
            {
                // arrange
                var input = Fixture.Create<CreateProductionBatchDto>();
                var @return = Fixture.Create<CreateProductonBatchReturn>();

                MockPackScheduleService.Setup(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()))
                    .Returns(new SuccessResult<ICreateProductionBatchReturn>(@return));

                // act
                ControllerUnderTest.Post(input);

                // assert
                MockPackScheduleService.Verify(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()), Times.Once());
            }

            [Test]
            public void Returns201OnSuccess()
            {
                // arrange
                var input = Fixture.Create<CreateProductionBatchDto>();
                var @return = Fixture.Create<CreateProductonBatchReturn>();
                MockPackScheduleService.Setup(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()))
                    .Returns(new SuccessResult<ICreateProductionBatchReturn>(@return));

                // act
                var result = ControllerUnderTest.Post(input);

                // assert
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // arrange
                var input = Fixture.Create<CreateProductionBatchDto>();
                var @return = Fixture.Create<CreateProductonBatchReturn>();
                const string expectedUserToken = "hello";
                var actualUserToken = string.Empty;

                MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<CreateProductionBatchParameters>()))
                    .Callback((IUserIdentifiable i) => i.UserToken = expectedUserToken);
                MockPackScheduleService.Setup(m => m.CreateProductionBatch(It.IsAny<ICreateProductionBatchParameters>()))
                    .Callback((ICreateProductionBatchParameters p) => actualUserToken = p.UserToken)
                    .Returns(new SuccessResult<ICreateProductionBatchReturn>(@return));

                // act
                ControllerUnderTest.Post(input);

                // assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(It.IsAny<CreateProductionBatchParameters>()), Times.Once());
                Assert.AreEqual(expectedUserToken, actualUserToken);
            }

            [Test, ExpectedException(typeof (HttpResponseException))]
            public void ThrowsBadRequestExceptionOnModelError()
            {
                // arrange
                var input = Fixture.Create<CreateProductionBatchDto>();
                MockPackScheduleService.Setup(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()))
                    .Verifiable();
                ControllerUnderTest.ModelState.AddModelError("", "error");

                try
                {
                    // act
                    ControllerUnderTest.Post(input);
                }
                catch (HttpResponseException ex)
                {
                    // assert
                    Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
                    MockPackScheduleService.Verify(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()), Times.Never());
                    throw;
                }
            }

            [Test]
            public void ThrowsBadRequestOnInvalid()
            {
                // arrange
                var input = Fixture.Create<CreateProductionBatchDto>();
                MockPackScheduleService
                    .Setup(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()))
                    .Returns(new InvalidResult<ICreateProductionBatchReturn>());
                
                // act
                var response = ControllerUnderTest.Post(input);

                // assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                MockPackScheduleService.Verify(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()), Times.Once());
                
            }

            [Test]
            public void ThrowsInternalServerErrorOnFailure()
            {
                // arrange
                var input = Fixture.Create<CreateProductionBatchDto>();
                MockPackScheduleService
                    .Setup(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()))
                    .Returns(new FailureResult<ICreateProductionBatchReturn>());
                
                // act
                var response = ControllerUnderTest.Post(input);

                // assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
                MockPackScheduleService.Verify(m => m.CreateProductionBatch(It.IsAny<CreateProductionBatchParameters>()), Times.Once());
            }
        }

        [TestFixture]
        public class PutTests : ProductionBatchesControllerTests
        {
            [Test]
            public void CallsServiceAsExpected()
            {
                // arrange
                const string id = "12345";
                var input = Fixture.Create<UpdateProductionBatchParameters>();
                IUpdateProductionBatchParameters actualParams = null;
                MockPackScheduleService.Setup(m => m.UpdateProductionBatch(input))
                    .Callback((IUpdateProductionBatchParameters i) => actualParams = i)
                    .Returns(new SuccessResult<string>("key"));

                // act
                ControllerUnderTest.Put(id, input);

                // assert
                MockPackScheduleService.Verify(m => m.UpdateProductionBatch(input), Times.Once());
                Assert.AreEqual(id, actualParams.ProductionBatchKey);
            }

            [Test]
            public void UtilizedUserIdentityProvider()
            {
                // arrange
                const string id = "12345";
                var input = Fixture.Create<UpdateProductionBatchParameters>();
                IUpdateProductionBatchParameters actualParams = null;
                MockUserIdentityProvider.Setup(m => m.SetUserIdentity(input))
                    .Callback((IUserIdentifiable i) => i.UserToken = "user token");
                MockPackScheduleService.Setup(m => m.UpdateProductionBatch(input))
                    .Callback((IUpdateProductionBatchParameters i) => actualParams = i)
                    .Returns(new SuccessResult<string>("key", null));

                // act
                ControllerUnderTest.Put(id, input);

                // assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(input), Times.Once());
                Assert.AreEqual("user token", actualParams.UserToken);
            }

            [Test]
            public void ThrowBadRequestOnModelError()
            {
                // arrange
                const string id = "12345";
                var input = Fixture.Create<UpdateProductionBatchParameters>();
                ControllerUnderTest.ModelState.AddModelError("", "error");

                // act
                var response = ControllerUnderTest.Put(id, input);

                // assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void Returns200OnSuccess()
            {
                // arrange
                const string id = "12345";
                var input = Fixture.Create<UpdateProductionBatchParameters>();
                MockPackScheduleService.Setup(m => m.UpdateProductionBatch(input))
                    .Returns(new SuccessResult<string>());
                
                // act
                var response = ControllerUnderTest.Put(id, input);

                // assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            [Test]
            public void ReturnsBadRequeOnInvalid()
            {
                // arrange
                const string id = "12345";
                var input = Fixture.Create<UpdateProductionBatchParameters>();
                MockPackScheduleService.Setup(m => m.UpdateProductionBatch(input))
                    .Returns(new InvalidResult<string>());
                
                // act
                var response = ControllerUnderTest.Put(id, input);

                // assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void ReturnsInternalServerErrorOnFailure()
            {
                // arrange
                const string id = "12345";
                var input = Fixture.Create<UpdateProductionBatchParameters>();
                MockPackScheduleService.Setup(m => m.UpdateProductionBatch(input))
                    .Returns(new FailureResult<string>());
                
                // act
                var response = ControllerUnderTest.Put(id, input);

                // assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [TestFixture]
        public class DeleteTests : ProductionBatchesControllerTests
        {
            [Test]
            public void CallsServiceAsExpected()
            {
                // arrange
                const string key = "key";
                MockPackScheduleService.Setup(m => m.RemoveProductionBatch(key))
                    .Returns(new SuccessResult<string>());

                // act
                ControllerUnderTest.Delete(key);

                // assert
                MockPackScheduleService.Verify(m => m.RemoveProductionBatch(key));
            }

            [Test]
            public void Returns200OnSuccess()
            {
                // arrange
                const string key = "key";
                MockPackScheduleService.Setup(m => m.RemoveProductionBatch(key))
                    .Returns(new SuccessResult<string>());

                // act
                var response = ControllerUnderTest.Delete(key);

                // assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            [Test]
            public void ReturnsNotFoundOnInvalid()
            {
                // arrange
                const string key = "key";
                MockPackScheduleService.Setup(m => m.RemoveProductionBatch(key))
                    .Returns(new InvalidResult<string>());

                // act
                var response = ControllerUnderTest.Delete(key);

                // assert
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Test]
            public void ReturnsInternalServerErrorOnFailure()
            {
                // arrange
                const string key = "key";
                MockPackScheduleService.Setup(m => m.RemoveProductionBatch(key))
                    .Returns(new FailureResult<string>());

                // act
                var response = ControllerUnderTest.Delete(key);

                // assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }
    }
}