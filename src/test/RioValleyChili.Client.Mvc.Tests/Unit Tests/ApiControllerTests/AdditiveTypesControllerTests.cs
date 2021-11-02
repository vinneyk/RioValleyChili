using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Tests.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ControllerTests
{
    [TestFixture]
    public class AdditiveTypesControllerTests
    {
        private Mock<IProductService> _productServiceMock;
        private readonly IFixture _fixture = AutoFixtureHelper.BuildFixture();
        private AdditiveTypesController _systemUnderTest;
        
        [SetUp]
        public void Setup()
        {
            _productServiceMock = new Mock<IProductService>();
            _systemUnderTest = new AdditiveTypesController(_productServiceMock.Object);
        }

        [Test]
        public void CallsServiceMethod()
        {
            // arrange
            SetupServiceSuccess();
            
            // act
            _systemUnderTest.Get();

            // assert
            _productServiceMock.Verify(m => m.GetAdditiveTypes(), Times.Once());
        }

        [Test]
        public void ReturnsServiceResuls_OnSuccess()
        {
            // arrange
            var expectedServiceResult = SetupServiceSuccess().Select(r => new AdditiveTypeDto
            {
                Key = r.AdditiveTypeKey,
                Description = r.AdditiveTypeDescription,
            });

            // act
            var actualResults = _systemUnderTest.Get();

            // assert
            Assert.AreEqual(expectedServiceResult, actualResults);
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void Throws404_OnInvalid()
        {
            // arrange
            SetupServiceInvalid();

            // act
            try
            {
                _systemUnderTest.Get();
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
                throw;
            }
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void Throws500_OnFailure()
        {
            // arrange
            SetupServiceFailure();

            // act
            try
            {
                _systemUnderTest.Get();
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, ex.Response.StatusCode);
                throw;
            }
        }

        #region private functions

        private IEnumerable<IAdditiveTypeReturn> SetupServiceSuccess()
        {
            var expectedServiceResult = _fixture.CreateMany<IAdditiveTypeReturn>().AsQueryable();
            _productServiceMock.Setup(m => m.GetAdditiveTypes())
                .Returns(new SuccessResult<IEnumerable<IAdditiveTypeReturn>>(expectedServiceResult));
            return expectedServiceResult;
        }

        private void SetupServiceFailure()
        {
            _productServiceMock.Setup(m => m.GetAdditiveTypes())
                .Returns(new FailureResult<IQueryable<IAdditiveTypeReturn>>());
        }

        private void SetupServiceInvalid()
        {
            _productServiceMock.Setup(m => m.GetAdditiveTypes())
                .Returns(new InvalidResult<IQueryable<IAdditiveTypeReturn>>());
        }

        #endregion
    }
}
