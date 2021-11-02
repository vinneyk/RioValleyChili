using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using Ninject;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc;
using RioValleyChili.Client.Mvc.App_Start;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class IntraWarehouseInventoryMovementsControllerTests
    {
        protected readonly IFixture Fixture = AutoFixtureHelper.BuildFixture();
        protected Mock<IIntraWarehouseOrderService> MockIntraWarehouseOrderService;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected IntraWarehouseInventoryMovementsController SystemUnderTest;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfiguration.Configure();
            MockIntraWarehouseOrderService = new Mock<IIntraWarehouseOrderService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            MockUserIdentityProvider
                .Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()))
                .Verifiable();
            SystemUnderTest = new IntraWarehouseInventoryMovementsController(MockIntraWarehouseOrderService.Object, MockUserIdentityProvider.Object);
        }
        
        public class Get : IntraWarehouseInventoryMovementsControllerTests
        {
            [Test]
            public void ReturnsResultsFromServiceWithPaging_IntegratedTest()
            {
                // Arrange
                var integratedSystemUnderTest = new IntraWarehouseInventoryMovementsController(MockIntraWarehouseOrderService.Object, MockUserIdentityProvider.Object);

                const int pageSize = 5;
                const int skipCount = 1;

                var expectedResults = Fixture.CreateMany<IIntraWarehouseOrderSummaryReturn>(10).AsQueryable();
                MockIntraWarehouseOrderService.Setup(m => m.GetIntraWarehouseOrderSummaries())
                                              .Returns(new SuccessResult<IQueryable<IIntraWarehouseOrderSummaryReturn>>(expectedResults));

                // Act
                var results = integratedSystemUnderTest.Get(pageSize, skipCount);

                // Assert
                Assert.AreEqual(results.Count(), pageSize);
            }
        }

        public class GetById : IntraWarehouseInventoryMovementsControllerTests
        {
            [Test]
            public void ReturnsWarehouseOrderResultsFromService()
            {
                throw new NotImplementedException();
                //// Arrange
                //var availableData = Fixture.CreateMany<IIntraWarehouseOrderDetailReturn>();
                //var expectedResult = availableData.Last();
                //var key = expectedResult.TrackingSheetNumber;

                //MockIntraWarehouseOrderService.Setup(m => m.GetIntraWarehouseOrders())
                //    .Returns(new SuccessResult<IQueryable<IIntraWarehouseOrderDetailReturn>>(expectedResult));

                //// Act
                //var result = SystemUnderTest.Get(key);

                //// Assert
                //Assert.AreEqual(expectedResult.OrderKey, result.OrderKey);
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsWhenServiceResultsIsFailure()
            {
                // Arrange
                MockIntraWarehouseOrderService.Setup(m => m.GetIntraWarehouseOrders())
                                              .Returns(new FailureResult<IQueryable<IIntraWarehouseOrderDetailReturn>>());

                // Act
                try
                {
                    SystemUnderTest.Get(0);
                }
                catch (HttpResponseException ex)
                {
                    // Assert
                    Assert.AreEqual(HttpStatusCode.InternalServerError, ex.Response.StatusCode);
                    throw;
                }
            }
        }

        public class Post : IntraWarehouseInventoryMovementsControllerTests
        {
            [Test]
            public void CallsServiceMethodAsExpected()
            {
                // Arrange
                var postData = Fixture.Create<CreateIntraWarehouseOrder>();
                
                MockIntraWarehouseOrderService
                    .Setup(m => m.CreateIntraWarehouseOrder(It.IsAny<ICreateIntraWarehouseOrderParameters>()))
                    .Returns(new SuccessResult<string>());

                // Act
                SystemUnderTest.Post(postData);

                // Assert
                MockIntraWarehouseOrderService.Verify(m => m.CreateIntraWarehouseOrder(It.IsAny<ICreateIntraWarehouseOrderParameters>()), Times.Once());
            }

            [Test]
            public void ReturnsKeyOfNewObject_OnSuccess()
            {
                // Arrange
                const string newKeyValue = "newKey";
                var postData = Fixture.Create<CreateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.CreateIntraWarehouseOrder(It.IsAny<ICreateIntraWarehouseOrderParameters>()))
                                              .Returns(new SuccessResult<string>(newKeyValue));

                // Act
                var results = SystemUnderTest.Post(postData);

                // Assert
                var objectContent = results.Content as ObjectContent<string>;
                Assert.IsNotNull(objectContent);
                Assert.AreEqual(newKeyValue, objectContent.Value);
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsWhenServiceResultIsInvalid()
            {
                // Arrange
                var postData = Fixture.Create<CreateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.CreateIntraWarehouseOrder(It.IsAny<ICreateIntraWarehouseOrderParameters>()))
                                              .Returns(new InvalidResult<string>());

                // Act
                SystemUnderTest.Post(postData);

                // Assert
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsWhenServiceResultIsFailure()
            {
                // Arrange
                var postData = Fixture.Create<CreateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.CreateIntraWarehouseOrder(It.IsAny<ICreateIntraWarehouseOrderParameters>()))
                                              .Returns(new FailureResult<string>());

                // Act
                try
                {
                    SystemUnderTest.Post(postData);
                }
                catch (HttpResponseException ex)
                {
                    // Assert
                    Assert.AreEqual(HttpStatusCode.InternalServerError, ex.Response.StatusCode);
                    throw;
                }

            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void Throws400IfModelStateIsInvalid()
            {
                // Arrange
                SystemUnderTest.ModelState.AddModelError("", "This is an error!");
                var postData = Fixture.Create<CreateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.CreateIntraWarehouseOrder(It.IsAny<ICreateIntraWarehouseOrderParameters>()))
                                              .Returns(new SuccessResult<string>());

                // Act
                try
                {
                    SystemUnderTest.Post(postData);
                }
                catch (HttpResponseException ex)
                {
                    Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
                    throw;
                }
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // Arrange
                const string newKeyValue = "newKey";
                var postData = Fixture.Create<CreateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.CreateIntraWarehouseOrder(It.IsAny<ICreateIntraWarehouseOrderParameters>()))
                                              .Returns(new SuccessResult<string>(newKeyValue));

                // Act
                SystemUnderTest.Post(postData);

                // Assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()), Times.Once());
            }
        }

        public class Put : IntraWarehouseInventoryMovementsControllerTests
        {
            [Test]
            public void CallsServiceMethodAsExpected()
            {
                // Arrange
                const string key = "12345";
                var putData = Fixture.Create<UpdateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.UpdateIntraWarehouseOrder(It.IsAny<IUpdateIntraWarehouseOrderParameters>()))
                    .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Put(key, putData);

                // Assert
                MockIntraWarehouseOrderService.Verify(m => m.UpdateIntraWarehouseOrder(It.IsAny<IUpdateIntraWarehouseOrderParameters>()), Times.Once());
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsWhenServiceResultIsInvalid()
            {
                // Arrange
                const string key = "12345";
                var putData = Fixture.Create<UpdateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.UpdateIntraWarehouseOrder(It.IsAny<IUpdateIntraWarehouseOrderParameters>()))
                                              .Returns(new InvalidResult());

                // Act
                try
                {
                    SystemUnderTest.Put(key, putData);
                }
                catch (HttpResponseException exception)
                {
                    // Assert
                    Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
                    throw;
                }
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsWhenServiceResultIsFailure()
            {
                // Arrange
                const string key = "12345";
                var putData = Fixture.Create<UpdateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.UpdateIntraWarehouseOrder(It.IsAny<IUpdateIntraWarehouseOrderParameters>()))
                                              .Returns(new FailureResult());

                // Act
                try
                {
                    SystemUnderTest.Put(key, putData);
                }
                catch (HttpResponseException ex)
                {
                    Assert.AreEqual(HttpStatusCode.InternalServerError, ex.Response.StatusCode);
                    throw;
                }
            }

            [Test]
            public void SetsIntraWarehouseOrderKeyFromIdParameter()
            {
                // Arrange
                const string key = "12345";
                var putData = Fixture.Create<UpdateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.UpdateIntraWarehouseOrder(It.IsAny<IUpdateIntraWarehouseOrderParameters>()))
                                              .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Put(key, putData);

                // Assert
                Assert.AreEqual(key, putData.IntraWarehouseOrderKey);
            }

            [Test, ExpectedException(typeof(HttpResponseException))]
            public void ThrowsBadRequestIfModelStateIsInvalid()
            {
                // Arrange
                const string key = "12345";
                var putData = Fixture.Create<UpdateIntraWarehouseOrder>();
                MockIntraWarehouseOrderService.Setup(m => m.UpdateIntraWarehouseOrder(It.IsAny<IUpdateIntraWarehouseOrderParameters>()))
                                              .Returns(new FailureResult());
                SystemUnderTest.ModelState.AddModelError("", "Test Error");

                // Act
                try
                {
                    SystemUnderTest.Put(key, putData);
                }
                catch (HttpResponseException ex)
                {
                    Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
                    throw;
                }
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // Arrange
                const string key = "12345";
                var putData = Fixture.Create<UpdateIntraWarehouseOrder>();
                IUpdateIntraWarehouseOrderParameters actual = null;
                MockIntraWarehouseOrderService.Setup(m => m.UpdateIntraWarehouseOrder(It.IsAny<IUpdateIntraWarehouseOrderParameters>()))
                                              .Callback((IUpdateIntraWarehouseOrderParameters vals) => actual = vals)
                                              .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Put(key, putData);

                // Assert
                Assert.IsNotNull(actual);
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(actual), Times.Once());
            }
        }
    }
}
