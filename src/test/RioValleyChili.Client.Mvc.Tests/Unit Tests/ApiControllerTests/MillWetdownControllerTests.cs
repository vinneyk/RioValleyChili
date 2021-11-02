using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class MillWetdownControllerTests
    {
        protected MillWetdownController SystemUnderTest;
        protected Mock<IMillAndWetDownService> MockMillAndWetDownService;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected readonly IFixture Fixture = AutoFixtureHelper.BuildFixture();

        [SetUp]
        public void SetUp()
        {
            MockMillAndWetDownService = new Mock<IMillAndWetDownService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            SystemUnderTest = new MillWetdownController(MockMillAndWetDownService.Object, MockUserIdentityProvider.Object);
            AutoMapperConfiguration.Configure();
        }

        [TestFixture]
        public class GetMethodTests : MillWetdownControllerTests
        {
            [Test]
            public void Get_ReturnsPagedResultsFromService()
            {
                // Arrange
                var expectedResults = Fixture.CreateMany<IMillAndWetdownSummaryReturn>().AsQueryable();
                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownSummaries())
                                         .Returns(new SuccessResult<IQueryable<IMillAndWetdownSummaryReturn>>(expectedResults));
                
                // Act
                var actualResults = SystemUnderTest.Get();

                // Assert
                Assert.AreEqual(expectedResults, actualResults);
            }

            [Test]
            public void GivenPagedDataParameters_Get_ExecutesPagingAccordingly()
            {
                // Arrange
                const int pageSize = 5;
                const int skipCount = 5;

                var dataResults = Fixture.CreateMany<IMillAndWetdownSummaryReturn>(10).AsQueryable();
                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownSummaries())
                                         .Returns(new SuccessResult<IQueryable<IMillAndWetdownSummaryReturn>>(dataResults));

                var expectedResults = dataResults.Skip(skipCount).Take(pageSize);
                
                // Act
                var actualResults = SystemUnderTest.Get(skipCount: skipCount, pageSize: pageSize);

                // Assert
                Assert.AreEqual(expectedResults, actualResults);
            }

            [Test]
            public void WhenServiceReturnsFailureResult_Get_ThrowsHttpResponceExceptionWith500InternalServerErrorStatusCode()
            {
                // Arrange
                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownSummaries())
                                         .Returns(new FailureResult<IQueryable<IMillAndWetdownSummaryReturn>>());
                HttpStatusCode? statusCode = null;

                // Act
                try
                {
                    SystemUnderTest.Get();
                }
                catch (HttpResponseException ex)
                {
                    statusCode = ex.Response.StatusCode;
                }

                // Assert
                Assert.IsNotNull(statusCode);
                Assert.AreEqual(HttpStatusCode.InternalServerError, statusCode);
            }

            [Test]
            public void WhenServiceReturnsInvalidResult_Get_ThrowsHttpResponceExceptionWith404NotFoundStatusCode()
            {
                // Arrange
                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownSummaries())
                                         .Returns(new InvalidResult<IQueryable<IMillAndWetdownSummaryReturn>>());

                HttpStatusCode? statusCode = null;

                // Act
                try
                {
                    SystemUnderTest.Get();
                }
                catch (HttpResponseException ex)
                {
                    statusCode = ex.Response.StatusCode;
                }

                // Assert
                Assert.IsNotNull(statusCode);
                Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
            }

            [Test]
            public void IntegratedDataResultsAdapterTest()
            {
                // Arrange
                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownSummaries())
                                         .Returns(new SuccessResult<IQueryable<IMillAndWetdownSummaryReturn>>(Fixture.CreateMany<IMillAndWetdownSummaryReturn>().AsQueryable()));
                var systemUnderTest = new MillWetdownController(MockMillAndWetDownService.Object, MockUserIdentityProvider.Object);

                // Act
                systemUnderTest.Get();

                // Assert
                Assert.Pass();
            }

            [Test]
            public void GivenSingleDateFilter_Get_ReturnsResultsWithProductionStartDateOfDateSpecified()
            {
                // Arrange
                var dateFilter = DateTime.Now;
                Fixture.Customizations.Add(new MillAndWetdownSummaryDateRangeSpecimenBuilder(dateFilter));
                var dataResults = Fixture.CreateMany<IMillAndWetdownSummaryReturn>(10).AsQueryable();
                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownSummaries())
                                         .Returns(new SuccessResult<IQueryable<IMillAndWetdownSummaryReturn>>(dataResults));

                IList<IMillAndWetdownSummaryReturn> dataPagerInput = null;
                var expectedResults = dataResults.Where(m => m.ProductionBegin >= dateFilter.Date && m.ProductionBegin < dateFilter.Date.AddDays(1)).ToList();
                
                //Assess
                Assess.IsNotEmpty(expectedResults);
                Assess.AreNotEqual(expectedResults.Count, dataResults.Count());

                // Act
                var results = SystemUnderTest.Get(dateFilter);

                // Assert
                Assert.AreEqual(expectedResults, results);
                Assert.AreEqual(expectedResults.Count, dataPagerInput.Count());
            }

            [Test]
            public void GivenDateRangeFilter_Get_ReturnsResultsWithProductionStartDateOfDateSpecified()
            {
                // Arrange
                var startDate = DateTime.Now.Date;
                var endDate = DateTime.Now.AddDays(1).Date;

                Fixture.Customizations.Add(new MillAndWetdownSummaryDateRangeSpecimenBuilder(startDate, endDate));
                var dataResults = Fixture.CreateMany<IMillAndWetdownSummaryReturn>(10).AsQueryable();
                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownSummaries())
                                         .Returns(new SuccessResult<IQueryable<IMillAndWetdownSummaryReturn>>(dataResults));

                IList<IMillAndWetdownSummaryReturn> dataPagerInput = null;
                var expectedResults = dataResults.Where(m => m.ProductionBegin >= startDate.Date && m.ProductionBegin < endDate.AddDays(1).Date).ToList();
                
                //Assess
                Assess.IsNotEmpty(expectedResults);
                Assess.IsTrue(expectedResults.Any(r => r.ProductionBegin.Date == startDate.Date), "No records contain a beginning production date equal to the start date range filter.");
                Assess.IsTrue(expectedResults.Any(r => r.ProductionBegin.Date == endDate.Date), "No records contain a beginning production date equal to the end date range filter.");
                Assess.IsTrue(expectedResults.Count < dataResults.Count());

                // Act
                var results = SystemUnderTest.Get(startDate, endDate);

                // Assert
                Assert.AreEqual(expectedResults, results);
                Assert.AreEqual(expectedResults.Count, dataPagerInput.Count());
            }

            [Test]
            public void GivenLineKeyFilter_Get_ReturnsResultsFilteredByLineKey()
            {
                // Arrange
                const string expectedLineKey = "1";
                Fixture.Customizations.Add(new MillAndWetdownSummaryLineKeySpecimenBuilder(expectedLineKey));
                var dataResults = Fixture.CreateMany<IMillAndWetdownSummaryReturn>(10).AsQueryable();
                Assess.IsTrue(dataResults.Any(r => r.ProductionLineKey == expectedLineKey), "The data results do not include the expected line key value.");
                Assess.IsTrue(dataResults.Any(r => r.ProductionLineKey != expectedLineKey), "The data results do not include any line key values other than the expected value.");

                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownSummaries())
                                         .Returns(() => new SuccessResult<IQueryable<IMillAndWetdownSummaryReturn>>(dataResults));

                var expectedResults = dataResults.Where(d => d.ProductionLineKey == expectedLineKey).ToList();
                Assess.IsNotEmpty(expectedResults);
                Assess.IsTrue(expectedResults.Any(r => r.ProductionLineKey == expectedLineKey));
                Assert.IsTrue(expectedResults.Count < dataResults.Count());

                IList<IMillAndWetdownSummaryReturn> dataPagerInput = null;
                
                // Act
                var results = SystemUnderTest.Get(lineKey: expectedLineKey);

                // Assert
                Assert.AreEqual(expectedResults, results);
                Assert.AreEqual(expectedResults, dataPagerInput);
            }

            [Test]
            public void ReturnsExpectedResultForLotKey()
            {
                // Arrange
                const string expectedLot = "01 13 123 01";
                var expectedResult = Fixture.Create<IMillAndWetdownDetailReturn>();
                MockMillAndWetDownService.Setup(m => m.GetMillAndWetdownDetail(expectedLot))
                    .Returns(new SuccessResult<IMillAndWetdownDetailReturn>(expectedResult));

                // Act
                var actualResult = SystemUnderTest.Get(expectedLot);

                // Assert
                Assert.IsNotNull(actualResult);
                Assert.AreEqual(expectedResult, actualResult);
            }
        }
        
        [TestFixture]
        public class PostMethodTests : MillWetdownControllerTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();

                MockMillAndWetDownService.Setup(m => m.CreateMillAndWetdown(It.IsAny<ICreateMillAndWetdownParameters>()))
                                         .Returns(new SuccessResult<string>());
            }

            [Test]
            public void AssignsUserIdentity()
            {
                // Arrange
                const string expectedUser = "test_user";
                MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()))
                                        .Callback((IUserIdentifiable u) => u.UserToken = expectedUser);
                var param = Fixture.Create<CreateMillAndWetdownRequest>();

                // Act
                SystemUnderTest.Post(param);

                // Assert
                MockUserIdentityProvider.Verify(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()), Times.Once());
                Assert.AreEqual(expectedUser, param.UserToken);
            }
        }

        private class MillAndWetdownSummaryDateRangeSpecimenBuilder : ISpecimenBuilder
        {
            private readonly DateTime? _startDateRange;
            private readonly DateTime? _endDateRange;
            int counter, dateCounter;
            private DateTime dateValue;

            public MillAndWetdownSummaryDateRangeSpecimenBuilder(DateTime? startDateRange, DateTime? endDateRange = null)
            {
                _startDateRange = startDateRange;
                _endDateRange = endDateRange ?? startDateRange;
            }

            public object Create(object request, ISpecimenContext context)
            {
                if(_startDateRange == null) { return new NoSpecimen(request); }

                var propInfo = request as PropertyInfo;
                if (propInfo == null) { return new NoSpecimen(request); }

                if (propInfo.DeclaringType == typeof (IMillAndWetdownSummaryReturn))
                {
                    if (propInfo.Name == "ProductionBegin")
                    {
                        counter++;
                        if (counter%3 != 0)
                        {
                            dateCounter++;
                            dateValue = dateCounter%2 == 0 
                                ? _startDateRange.Value
                                : _endDateRange.Value;

                            return dateValue;
                        }
                        return DateTime.Now.AddDays(-5);
                    }

                    if (propInfo.Name == "ProductionEnd")
                    {
                        return dateValue.AddHours(1.25);
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class MillAndWetdownSummaryLineKeySpecimenBuilder : ISpecimenBuilder
        {
            private int counter;

            private readonly string _lineKeyValue;
            public MillAndWetdownSummaryLineKeySpecimenBuilder(string lineKeyValue)
            {
                if (string.IsNullOrWhiteSpace(lineKeyValue)) { throw new ArgumentNullException("lineKeyValue"); }
                _lineKeyValue = lineKeyValue;
            }

            public object Create(object request, ISpecimenContext context)
            {
                var propInfo = request as PropertyInfo;
                if(propInfo == null) { return new NoSpecimen(request); }

                if (propInfo.DeclaringType == typeof (IMillAndWetdownSummaryReturn))
                {
                    if (propInfo.Name == "ProductionLineKey")
                    {
                        counter++;
                        if (counter%3 == 0)
                        {
                            return _lineKeyValue;
                        }
                    }
                }

                return new NoSpecimen(request);
            }
        }
    }
}
