using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Moq;
using Ninject;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc;
using RioValleyChili.Client.Mvc.App_Start;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Tests.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Tests.ApiControllerTests
{
    [TestFixture]
    public class LotsControllerTests
    {
        protected Mock<ILotService> MockLotService;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected LotsController LotsControler;
        protected IFixture Fixture = AutoFixtureHelper.BuildFixture();
        
        [SetUp]
        public void SetUp()
        {
            MockLotService = new Mock<ILotService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();

            LotsControler = new LotsController(MockLotService.Object, MockUserIdentityProvider.Object);
        }


        [TestFixture]
        public class GetMany : LotsControllerTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
            }

            [Test]
            public void ReturnsServiceResultOnSuccess()
            {
                // Arrange
                const int recordCount = 10;
                var expectedReturn = Fixture.Build<TestableLotSummariesReturn>()
                    .With(m => m.LotSummaries, Fixture.CreateMany<ILotQualitySummaryReturn>(recordCount).AsQueryable())
                    .Create();
                
                MockLotService.Setup(m => m.GetLotSummaries(It.IsAny<FilterLotParameters>()))
                    .Returns(() => new SuccessResult<ILotQualitySummariesReturn>(expectedReturn));
                Assess.IsNotNull(expectedReturn);
                Assess.IsNotNull(expectedReturn.LotSummaries);

                // Act
                var result = LotsControler.Get(LotTypeEnum.Additive, pageSize: 10);

                // Assert
                MockLotService.Verify(m => m.GetLotSummaries(It.IsAny<FilterLotParameters>()), Times.Once());
                Assert.AreEqual(recordCount, result.LotSummaries.Count());
            }

            [Test]
            public void ReturnsPagedResultsSetOnSuccess()
            {
                // Arrange
                const int recordCount = 10;
                var expectedReturn = Fixture.Build<TestableLotSummariesReturn>()
                    .With(m => m.LotSummaries, Fixture.CreateMany<ILotQualitySummaryReturn>(recordCount*2).AsQueryable())
                    .Create();

                Assess.IsNotNull(expectedReturn);
                Assess.IsNotNull(expectedReturn.LotSummaries);
                Assess.IsTrue(expectedReturn.LotSummaries.Count() == recordCount*2);

                MockLotService.Setup(m => m.GetLotSummaries(It.IsAny<FilterLotParameters>()))
                    .Returns(new SuccessResult<ILotQualitySummariesReturn>(expectedReturn));

                // Act
                var result = LotsControler.Get(LotTypeEnum.Additive, pageSize: 10);

                // Assert
                MockLotService.Verify(m => m.GetLotSummaries(It.IsAny<FilterLotParameters>()), Times.Once());
                Assert.AreEqual(recordCount, result.LotSummaries.Count());
            }

            [Test]
            public void CreatesFilterLotParameterObjectCorrectly()
            {
                // Arrange
                const LotTypeEnum expectedLotType = LotTypeEnum.GRP;
                const LotProductionStatus expectedStatus = LotProductionStatus.Produced;
                
                FilterLotParameters actualParameters = null;

                MockLotService
                    .Setup(m => m.GetLotSummaries(It.IsAny<FilterLotParameters>()))
                    .Callback((FilterLotParameters param) => actualParameters = param)
                    .Returns(new SuccessResult<ILotQualitySummariesReturn>(Fixture.Create<ILotQualitySummariesReturn>()));

                // Act
                LotsControler.Get(expectedLotType, expectedStatus);

                // Assert
                Assert.IsNotNull(actualParameters);
                Assert.AreEqual(expectedLotType, actualParameters.LotType);
                Assert.AreEqual(expectedStatus, actualParameters.ProductionStatus);
            }

            [Test]
            public void ConvertsDateFilterParametersToUTC()
            {
                // Arrange
                var productionDateStart = DateTime.Now.AddDays(-5);
                var productionDateEnd = DateTime.Now;
                var expectedStart = productionDateStart.ToUniversalTime();
                var expectedEnd = productionDateEnd.ToUniversalTime();

                if (productionDateStart == expectedStart || productionDateEnd == expectedEnd)
                {
                    Assert.Inconclusive("Production dates are already in UTC");
                }

                FilterLotParameters actualParameters = null;

                MockLotService
                    .Setup(m => m.GetLotSummaries(It.IsAny<FilterLotParameters>()))
                    .Callback((FilterLotParameters param) => actualParameters = param)
                    .Returns(new SuccessResult<ILotQualitySummariesReturn>(Fixture.Create<ILotQualitySummariesReturn>()));

                // Act
                LotsControler.Get(productionStart: productionDateStart, productionEnd: productionDateEnd);

                // Assert
                Assert.IsNotNull(actualParameters);
                Assert.AreEqual(expectedStart, actualParameters.ProductionStartRangeStart);
                Assert.AreEqual(expectedEnd, actualParameters.ProductionStartRangeEnd);
            }
        }

        [TestFixture]
        public class GetByLotKey : LotsControllerTests
        {
            const string Key = "12345";
            private ILotQualitySingleSummaryReturn _expectedResult;
            private LotDetailsResponse _expectedReturn = null;


            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _expectedResult = Fixture.Create<ILotQualitySingleSummaryReturn>();

                MockLotService
                    .Setup(m => m.GetLotSummary(Key))
                    .Returns(new SuccessResult<ILotQualitySingleSummaryReturn>(_expectedResult));

                AutoMapperConfiguration.Configure();
            }

            [Test]
            public void CallsServiceMethod()
            {
                // Arrange
                // Act
                LotsControler.Get(Key);

                // Assert
                MockLotService.Verify(m => m.GetLotSummary(Key), Times.Once());
            }

            [Test]
            public void ReturnsProjectedResult()
            {
                // Arrange
                // Act
                var result = LotsControler.Get(Key);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(_expectedReturn, result);
            }
        }

        [TestFixture]
        public class PutTests : LotsControllerTests
        {
            private const string LotKey = "12345";
            private UpdateLotRequest _values;

            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                MockLotService.Setup(m => m.SetLotAttributes(It.IsAny<ISetLotAttributeParameters>()))
                    .Returns(new SuccessResult<ILotStatInfoReturn>());

                _values = Fixture.Create<UpdateLotRequest>();
            }

            [Test]
            public void CallsServiceMethodAsExpected()
            {
                // Arrange
                // Act
                LotsControler.Put(LotKey, _values);

                // Assert
                MockLotService.Verify(m => m.SetLotAttributes(It.IsAny<ISetLotAttributeParameters>()), Times.Once());
            }

            [Test]
            public void ParametersAreCorrectlyTranslatedToDto()
            {
                // Arrange 
                ISetLotAttributeParameters actualParameters = null;
                MockLotService.Setup(m => m.SetLotAttributes(It.IsAny<ISetLotAttributeParameters>()))
                    .Callback((ISetLotAttributeParameters p) => actualParameters = p)
                    .Returns(new SuccessResult<ILotStatInfoReturn>());

                // Act
                LotsControler.Put(LotKey, _values);

                // Assert
                Assert.IsNotNull(actualParameters);
                Assert.AreEqual(LotKey, actualParameters.LotKey);
                Assert.AreEqual(((ISetLotAttributeParameters)_values).Attributes, actualParameters.Attributes);
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // Arrange
                // Act
                LotsControler.Put(LotKey, _values);

                // Assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(It.IsAny<ISetLotAttributeParameters>()), Times.Once());
            }

            [Test]
            public async Task Returns400IfValuesAreInvalid()
            {
                // Arrange
                LotsControler.ModelState.AddModelError("", "This is invalid.");

                // Act
                var result = await LotsControler.Put(LotKey, _values);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.IsNotNullOrEmpty(result.ReasonPhrase);
            }

            [Test]
            public async Task Returns200OnSuccess()
            {
                // Arrange
                // Act
                var result = await LotsControler.Put(LotKey, _values);

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            }

            [Test]
            public async Task Returns400OnInvalid()
            {
                // Arrange
                const string message = "invalid stuff happened";
                MockLotService.Setup(m => m.SetLotAttributes(It.IsAny<ISetLotAttributeParameters>()))
                    .Returns(new InvalidResult<ILotStatInfoReturn>(null, message));

                // Act
                var result = await LotsControler.Put(LotKey, _values);

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.AreEqual(message, result.ReasonPhrase);
            }

            [Test]
            public async Task Returns500OnFailure()
            {
                // Arrange
                const string message = "an error occurred";
                MockLotService.Setup(m => m.SetLotAttributes(It.IsAny<ISetLotAttributeParameters>()))
                    .Returns(new FailureResult<ILotStatInfoReturn>(null, message));

                // Act
                var result = await LotsControler.Put(LotKey, _values);

                // Assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
                Assert.AreEqual(message, result.ReasonPhrase);
            }

            [Test]
            public void EnforcesAntiForgeryTokenValidation()
            {
                LotsControler.GetType().AssertAntiForgeryTokenValidationForMethod("Put");
            }

            [Test]
            public void EnforcesClaimsAuthorization()
            {
                LotsControler.GetType().AssertClaimsForMethod("Put", new[] { ClaimTypes.QualityControlClaimTypes.LotAttributes }, ClaimActions.Full);
            }
        }

        [TestFixture]
        public class IntegratedResultsProjector : LotsControllerTests
        {
            protected new LotsController SystemUnderTest;

            [SetUp]
            public new void SetUp()
            {
                base.SetUp();

                MockLotService = new Mock<ILotService>();
                SystemUnderTest = new LotsController(MockLotService.Object, MockUserIdentityProvider.Object);
            }

            [Test]
            public void GetManyWithIntegratedProjector()
            {
                // Arrange
                var testFixture = new GetMany();
                testFixture.SetUp();

                var expectedReturn = Fixture.Create<TestableLotSummariesReturn>();
                MockLotService.Setup(m => m.GetLotSummaries(It.IsAny<FilterLotParameters>()))
                    .Returns(new SuccessResult<ILotQualitySummariesReturn>(expectedReturn));

                // Act
                var results = SystemUnderTest.Get();

                // Assert 
                Assert.IsNotNull(results);
                Assert.IsNotEmpty(results.LotSummaries);
            }

            [Test]
            public async Task GetByKeyWithIntegratedProjector()
            {
                // Arrange
                const string lotKey = "12345";
                var expectedResult = Fixture.Create<ILotQualitySingleSummaryReturn>();
                MockLotService
                    .Setup(m => m.GetLotSummary(lotKey))
                    .Returns(new SuccessResult<ILotQualitySingleSummaryReturn>(expectedResult));

                // Act
                var result = await SystemUnderTest.Get(lotKey);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedResult.AttributeNamesByProductType, result.AttributeNamesByProductType);
            }
        }

        [TestFixture]
        public class SetStatusTests : LotsControllerTests
        {
            [Test]
            public void AcceptsPutVerb()
            {
                Assert.IsNotNull(LotsControler.GetType().GetMethod("SetStatus")
                    .GetCustomAttribute<HttpPutAttribute>());
            }

            [Test]
            public void EnforcesAntiForgeryTokenValidation()
            {
                LotsControler.GetType().AssertAntiForgeryTokenValidationForMethod("SetStatus");
            }

            [Test]
            public void EnforcesClaimsAuthorization()
            {
                LotsControler.GetType().AssertClaimsForMethod("SetStatus", new[] { ClaimTypes.QualityControlClaimTypes.LotStatus }, ClaimActions.Full);
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // arrange
                const string lotKey = "12345";
                const LotQualityStatus expectedStatus = LotQualityStatus.Released;
                var dto = new SetLotStatusDto
                    {
                        Status = expectedStatus
                    };
                SetLotStatusParameter actualParameters = null;
                MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<SetLotStatusParameter>()))
                    .Callback((SetLotStatusParameter p) => actualParameters = p)
                    .Returns(actualParameters);
                MockLotService.Setup(m => m.SetLotQualityStatus(It.IsAny<ISetLotStatusParameters>()))
                    .Returns(new SuccessResult<ILotStatInfoReturn>());

                // act
                LotsControler.SetStatus(lotKey, dto);

                // assert
                Assert.IsNotNull(actualParameters);
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(actualParameters), Times.Once());
            }

            [Test]
            public void ParametersAreCorrectlyTransposedToDto()
            {
                // arrange
                ISetLotStatusParameters actualParameters = null;
                MockLotService.Setup(m => m.SetLotQualityStatus(It.IsAny<ISetLotStatusParameters>()))
                    .Callback((ISetLotStatusParameters p) => actualParameters = p)
                    .Returns(new SuccessResult<ILotStatInfoReturn>());
                const string lotKey = "12345";
                const LotQualityStatus expectedStatus = LotQualityStatus.Released;
                var dto = new SetLotStatusDto
                {
                    Status = expectedStatus
                };

                // act
                LotsControler.SetStatus(lotKey, dto);

                // assert
                Assert.IsNotNull(actualParameters);
                Assert.AreEqual(lotKey, actualParameters.LotKey);
                Assert.AreEqual(expectedStatus, actualParameters.QualityStatus);
            }

            [Test]
            public void Returns200OnSuccess()
            {
                // arrange
                MockLotService.Setup(m => m.SetLotQualityStatus(It.IsAny<ISetLotStatusParameters>()))
                    .Returns(new SuccessResult<ILotStatInfoReturn>());
                const string lotKey = "12345";
                const LotQualityStatus expectedStatus = LotQualityStatus.Released;
                var dto = new SetLotStatusDto
                {
                    Status = expectedStatus
                };

                // act
                var response = LotsControler.SetStatus(lotKey, dto);

                // assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        #region Private Test Objects

        private static IQueryable<LotQualitySummaryResponse> ProjectLotSummaries(IEnumerable<ILotQualitySummaryReturn> input)
        {
            return input.Select(l => new LotQualitySummaryResponse
            {
                AstaCalc = l.AstaCalc,
                Attributes = l.Attributes.Select(a => new LotAttribute
                {
                    AttributeDate = a.AttributeDate.ToShortDateString(),
                    Key = a.Key,
                    Name = a.Name,
                    Value = a.Value,
                }),
                Defects = l.Defects.Select(d => new LotDefect
                {
                    LotDefectKey = d.LotDefectKey,
                    DefectType = d.DefectType,
                    Description = d.Description,
                    Resolution = new LotDefectResolutionReturn
                    {
                        Description = d.Resolution.Description,
                        ResolutionType = d.Resolution.ResolutionType,
                    }
                }),
                LoBac = l.LoBac,
                LotDate = l.LotDateCreated,
                LotKey = l.LotKey,
                HoldType = l.HoldType,
                HoldDescription = l.HoldDescription,
                Product = new InventoryProductResponse
                {
                    ProductKey = l.LotProduct.ProductKey,
                    ProductName = l.LotProduct.ProductName,
                    ProductSubType = l.LotProduct.ProductSubType,
                    ProductType = l.LotProduct.ProductType,
                },
                ProductionStatus = l.ProductionStatus,
            }).AsQueryable();
        }

        private static LotDetailsResponse ProjectLotDetailsResult(ILotQualitySingleSummaryReturn lotDetails)
        {
            return new LotDetailsResponse
            {
                LotSummary = Mapper.Map<LotQualitySummaryResponse>(lotDetails),
                AttributeNamesByProductType = lotDetails.AttributeNamesByProductType,
            };
        }

        private class TestableLotSummariesReturn : ILotQualitySummariesReturn
        {
            public IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>>
                AttributeNamesByProductType { get; set; }

            public IQueryable<ILotQualitySummaryReturn> LotSummaries { get; set; }
        }

        #endregion

    }
}