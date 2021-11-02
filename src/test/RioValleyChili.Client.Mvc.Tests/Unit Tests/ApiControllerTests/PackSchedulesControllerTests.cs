using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Models.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class PackSchedulesControllerTests : ApiControllerTestsBase<PackSchedulesController>
    {
        private PackSchedulesController _systemUnderTest;
        protected Mock<IProductionService> MockPackScheduleService;
        protected Mock<IUserIdentityProvider> MockIdentityProvider;
        protected readonly IFixture Fixture = AutoFixtureHelper.BuildFixture();

        protected override PackSchedulesController SystemUnderTest
        {
            get { return _systemUnderTest; }
        }

        protected override string[] ClaimResources
        {
            get { return new[] { ClaimTypes.ProductionClaimTypes.PackSchedules }; }
        }

        [SetUp]
        public void SetUp()
        {
            MockPackScheduleService = new Mock<IProductionService>();
            MockIdentityProvider = new Mock<IUserIdentityProvider>();
            _systemUnderTest = new PackSchedulesController(MockPackScheduleService.Object, MockIdentityProvider.Object);
            AutoMapperConfiguration.Configure();
        }
         
        [Test]
        public void DefinesPackScheduleViewClaim()
        {
            // arrange
            // act
            var claimAttribute = SystemUnderTest.GetType().GetCustomAttribute<ClaimsAuthorizeAttribute>();

            // assert
            Assert.IsNotNull(claimAttribute);
        }

        [TestFixture]
        public class GetTests : PackSchedulesControllerTests
        {
            [Test]
            public void CallsServiceAsExpected()
            {
                // arrange
                var exptectedServiceResponse = Fixture.CreateMany<IPackScheduleSummaryReturn>().AsQueryable();
                MockPackScheduleService.Setup(m => m.GetPackSchedules())
                    .Returns(new SuccessResult<IQueryable<IPackScheduleSummaryReturn>>(exptectedServiceResponse));

                // act
                SystemUnderTest.Get();

                // assert
                MockPackScheduleService.Verify(m => m.GetPackSchedules());
            }
            
            [Test]
            public void ReturnsPageResultsAsExpected()
            {
                // arrange
                const int expectedStartIndexParam = 10;
                const int expectedCountParam = 20;
                var expectedServiceResponse = Fixture.CreateMany<IPackScheduleSummaryReturn>(10).AsQueryable();
                MockPackScheduleService.Setup(m => m.GetPackSchedules())
                    .Returns(new SuccessResult<IQueryable<IPackScheduleSummaryReturn>>(expectedServiceResponse));

                IEnumerable<IPackScheduleSummaryReturn> expectedPagedResults = null;
                
                // act
                var response = SystemUnderTest.Get(expectedStartIndexParam, expectedCountParam);

                // assert
                Assert.AreEqual(expectedPagedResults, response);
            }
            
            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsHttpResponseExcpetionWhenServiceReturnsFailure()
            {
                // arrange
                MockPackScheduleService.Setup(m => m.GetPackSchedules())
                    .Returns(new FailureResult<IQueryable<IPackScheduleSummaryReturn>>());

                // act
                try
                {
                    SystemUnderTest.Get();
                }
                catch (HttpResponseException ex)
                {
                    Assert.AreEqual(HttpStatusCode.InternalServerError, ex.Response.StatusCode);
                    throw;
                }

                // assert
                Assert.Fail();
            }

        }

        [TestFixture]
        public class GetByIdTests : PackSchedulesControllerTests
        {
            [Test]
            public void CallsServiceAsExpected()
            {
                // arrange
                const string expectedPackScheduleKey = "123456-789";
                MockPackScheduleService
                    .Setup(m => m.GetPackSchedule(expectedPackScheduleKey))
                    .Returns(new SuccessResult<IPackScheduleDetailReturn>(Fixture.Create<IPackScheduleDetailReturn>()));

                // act
                SystemUnderTest.Get(expectedPackScheduleKey);

                // assert
                MockPackScheduleService.Verify(m => m.GetPackSchedule(expectedPackScheduleKey), Times.Once());
            }

            [Test]
            public void ReturnsServiceResultOnSuccess()
            {
                // arrange
                const string expectedPackScheduleKey = "123456-789";
                var expectedServiceResponse = Fixture.Create<IPackScheduleDetailReturn>();
                MockPackScheduleService
                    .Setup(m => m.GetPackSchedule(expectedPackScheduleKey))
                    .Returns(new SuccessResult<IPackScheduleDetailReturn>(expectedServiceResponse));
                
                // act
                var response = SystemUnderTest.Get(expectedPackScheduleKey);

                // assert
                Assert.AreEqual(expectedServiceResponse, response);
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsHttpResponseExceptionOnFailure()
            {
                // arrange
                const string expectedPackScheduleKey = "123456-789";
                MockPackScheduleService
                    .Setup(m => m.GetPackSchedule(expectedPackScheduleKey))
                    .Returns(new FailureResult<IPackScheduleDetailReturn>());
                
                // act
                try
                {
                    SystemUnderTest.Get(expectedPackScheduleKey);
                }
                catch (HttpResponseException ex)
                {
                    Assert.AreEqual(HttpStatusCode.InternalServerError, ex.Response.StatusCode);
                    throw;
                }

                // assert
                Assert.Fail();
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsHttpResponseExceptionOnInvalid()
            {
                // arrange
                const string expectedPackScheduleKey = "123456-789";
                MockPackScheduleService
                    .Setup(m => m.GetPackSchedule(expectedPackScheduleKey))
                    .Returns(new InvalidResult<IPackScheduleDetailReturn>());
                
                // act
                try
                {
                    SystemUnderTest.Get(expectedPackScheduleKey);
                }
                catch (HttpResponseException ex)
                {
                    Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
                    throw;
                }

                // assert
                Assert.Fail();
            }

            [Test]
            public void AttemptsToGetPackScheduleByPSNumOnInvalid()
            {
                // arrange
                const string PSNum = "123456";
                var serviceQuery = Fixture.CreateMany<PackScheduleSummaryReturn>().ToList();
                serviceQuery.First().PSNum = int.Parse(PSNum);
                var expectedPackScheduleKey = serviceQuery.First().PackScheduleKey;
                var expectedResult = Fixture.Create<IPackScheduleDetailReturn>();

                MockPackScheduleService
                    .Setup(m => m.GetPackSchedule(PSNum))
                    .Returns(new InvalidResult<IPackScheduleDetailReturn>());

                MockPackScheduleService
                    .Setup(m => m.GetPackSchedule(expectedPackScheduleKey))
                    .Returns(new SuccessResult<IPackScheduleDetailReturn>(expectedResult));

                MockPackScheduleService.Setup(m => m.GetPackSchedules())
                    .Returns(new SuccessResult<IQueryable<IPackScheduleSummaryReturn>>(serviceQuery.AsQueryable()));

                // act
                SystemUnderTest.Get(PSNum);

                // assert
                MockPackScheduleService.Verify(m => m.GetPackSchedule(expectedPackScheduleKey), Times.Once());
            }
        }

        public class PackScheduleSummaryReturn :IPackScheduleSummaryReturn
        {
            public string PackScheduleKey { get; set; }
            public int? PSNum { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime ScheduledProductionDate { get; set; }
            public DateTime? ProductionDeadline { get; set; }
            public string WorkTypeKey { get; set; }
            public string WorkType { get; set; }
            public string ChileProductKey { get; set; }
            public string ChileProductName { get; set; }
            public string ProductionLineKey { get; set; }
            public string ProductionLineDescription { get; set; }
            public string OrderNumber { get; set; }
            public ICompanyHeaderReturn Customer { get; set; }
            public IProductionBatchTargetParameters TargetParameters { get; set; }
        }


        [TestFixture]
        public class PostTests : PackSchedulesControllerTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                MockPackScheduleService.Setup(m => m.CreatePackSchedule(It.IsAny<ICreatePackScheduleParameters>()))
                    .Returns(new SuccessResult<string>("new-key"));
            }

            [Test]
            public void CallsServiceAsExpected()
            {
                // arrange
                var inputParams = Fixture.Create<CreatePackSchedule>();

                // act
                SystemUnderTest.Post(inputParams);

                // assert
                MockPackScheduleService.Verify(m => m.CreatePackSchedule(It.IsAny<ICreatePackScheduleParameters>()), Times.Once());
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // arrange
                var inputParams = Fixture.Create<CreatePackSchedule>();
                var userIdentityProviderCalled = false;

                MockIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<CreatePackScheduleParameters>()))
                    .Callback((IUserIdentifiable p) =>
                    {
                        userIdentityProviderCalled = true;
                    });

                // act
                SystemUnderTest.Post(inputParams);

                // assert
                MockIdentityProvider.Verify(m => m.SetUserIdentity(It.IsAny<CreatePackScheduleParameters>()), Times.Once());
                Assert.True(userIdentityProviderCalled);
            }

            [Test]
            public void Returns400IfModelStateContainsErrors()
            {
                // arrange
                var inputParams = Fixture.Create<CreatePackSchedule>();
                SystemUnderTest.ModelState.AddModelError("", "error");

                // act
                var response = SystemUnderTest.Post(inputParams);

                // assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.IsNotNull(response.ReasonPhrase);
            }

            [Test]
            public void Returns201OnSuccess()
            {
                // arrange
                var inputParams = Fixture.Create<CreatePackSchedule>();

                // act
                var response = SystemUnderTest.Post(inputParams);

                // assert
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }

            [Test]
            public void Returns400OnInvalid()
            {
                // arrange
                var inputParams = Fixture.Create<CreatePackSchedule>();
                MockPackScheduleService.Setup(m => m.CreatePackSchedule(It.IsAny<ICreatePackScheduleParameters>()))
                    .Returns(new InvalidResult<string>());

                // act
                var response = SystemUnderTest.Post(inputParams);

                // assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void Returns500OnFailure()
            {
                // arrange
                var inputParams = Fixture.Create<CreatePackSchedule>();
                MockPackScheduleService.Setup(m => m.CreatePackSchedule(It.IsAny<ICreatePackScheduleParameters>()))
                    .Returns(new FailureResult<string>());

                // act
                var response = SystemUnderTest.Post(inputParams);

                // assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }            
        }
        
        [TestFixture]
        public class PutTests : PackSchedulesControllerTests
        {
            private IUpdatePackScheduleParameters _actualValues = null;

            [SetUp]
            public new void SetUp()
            {
                MockPackScheduleService.Setup(m => m.UpdatePackSchedule(It.IsAny<IUpdatePackScheduleParameters>()))
                    .Callback((IUpdatePackScheduleParameters valuesParam) =>
                    {
                        _actualValues = valuesParam;
                    })
                    .Returns(new SuccessResult<string>("newkey"));
            }

            [Test]
            public void CallsServiceAsExpected()
            {
                // arrange
                const string key = "1234-5";
                var input = Fixture.Create<UpdatePackScheduleParameters>();

                // act
                SystemUnderTest.Put(key, input);

                // assert
                MockPackScheduleService.Verify(m => m.UpdatePackSchedule(It.IsAny<IUpdatePackScheduleParameters>()), Times.Once());
                Assert.AreEqual(key, input.PackScheduleKey);
            }

            [Test]
            public void Returns200OnSuccess()
            {
                // arrange
                const string key = "1234-5";
                var input = Fixture.Create<UpdatePackScheduleParameters>();

                // act
                var response = SystemUnderTest.Put(key, input);

                // assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var content = response.Content as ObjectContent<string>;
                Assert.IsNotNull(content);
                Assert.AreEqual("newkey", content.Value);
            }

            [Test]
            public void Returns400OnInvalid()
            {
                // arrange
                const string key = "1234-5";
                var input = Fixture.Create<UpdatePackScheduleParameters>();
                MockPackScheduleService.Setup(m => m.UpdatePackSchedule(It.IsAny<IUpdatePackScheduleParameters>()))
                    .Returns(new InvalidResult<string>());

                // act
                var response = SystemUnderTest.Put(key, input);

                // assert
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Test]
            public void Returns500OnFailure()
            {
                // arrange
                const string key = "1234-5";
                var input = Fixture.Create<UpdatePackScheduleParameters>();
                MockPackScheduleService.Setup(m => m.UpdatePackSchedule(It.IsAny<IUpdatePackScheduleParameters>()))
                    .Returns(new FailureResult<string>());

                // act
                var response = SystemUnderTest.Put(key, input);

                // assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // arrange
                const string key = "1234-5";
                var input = Fixture.Create<UpdatePackScheduleParameters>();

                // act
                SystemUnderTest.Put(key, input);

                // assert
                MockIdentityProvider.Verify(m => m.SetUserIdentity(It.IsAny<IUpdatePackScheduleParameters>()), Times.Once());
            }

            [Test]
            public void Returns400WithoutCallingServiceIfModelStateIsInvalid()
            {
                // arrange
                const string key = "1234-5";
                var input = Fixture.Create<UpdatePackScheduleParameters>();
                SystemUnderTest.ModelState.AddModelError("", "error");

                // act
                var response = SystemUnderTest.Put(key, input);

                // assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                MockPackScheduleService.Verify(m => m.UpdatePackSchedule(It.IsAny<IUpdatePackScheduleParameters>()),
                    Times.Never());
            }            
        }

        [TestFixture]
        public class DeleteTests : PackSchedulesControllerTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                MockPackScheduleService
                    .Setup(m => m.RemovePackSchedule(It.IsAny<PackSchedulesController.DeletePackScheduleParameters>()))
                    .Returns(new SuccessResult<string>());
            }

            [Test]
            public void CallsServiceAsExpected()
            {
                // arrange
                const string key = "1234";
                IRemovePackScheduleParameters actualParameters = null;
                MockPackScheduleService.Setup(m => m.RemovePackSchedule(It.IsAny<IRemovePackScheduleParameters>()))
                    .Callback((IRemovePackScheduleParameters p) => actualParameters = p)
                    .Returns(new SuccessResult<string>());
                
                // act
                SystemUnderTest.Delete(key);

                // assert
                Assert.IsNotNull(actualParameters);
                MockPackScheduleService.Verify(m => m.RemovePackSchedule(It.IsAny<IRemovePackScheduleParameters>()), Times.Once());
                Assert.AreEqual(actualParameters.PackScheduleKey, key);
            }

            [Test]
            public void UtilizesUserTokenProvider()
            {
                // arrange
                const string key = "1234";
                IRemovePackScheduleParameters actualParameters = null;
                MockPackScheduleService.Setup(m => m.RemovePackSchedule(It.IsAny<IRemovePackScheduleParameters>()))
                    .Callback((IRemovePackScheduleParameters p) => actualParameters = p)
                    .Returns(new SuccessResult<string>());

                // act
                SystemUnderTest.Delete(key);

                // assert
                Assert.IsNotNull(actualParameters);
                MockIdentityProvider.Verify(m => m.SetUserIdentity(actualParameters), Times.Once());
            }

            [Test]
            public void Returns200OnSuccess()
            {
                // arrange
                const string key = "key";

                // act
                var result = SystemUnderTest.Delete(key);

                // assert
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            }

            [Test]
            public void Returns401OnInvalid()
            {
                // arrange
                const string key = "key";
                MockPackScheduleService
                    .Setup(m => m.RemovePackSchedule(It.IsAny<PackSchedulesController.DeletePackScheduleParameters>()))
                    .Returns(new InvalidResult<string>());

                // act
                var result = SystemUnderTest.Delete(key);

                // assert
                Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            }

            [Test]
            public void Returns500OnFailure()
            {
                // arrange
                const string key = "key";
                MockPackScheduleService
                    .Setup(m => m.RemovePackSchedule(It.IsAny<PackSchedulesController.DeletePackScheduleParameters>()))
                    .Returns(new FailureResult<string>());

                // act
                var result = SystemUnderTest.Delete(key);

                // assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            }
        }
    }
}
