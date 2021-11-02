using System.Collections.Generic;
using System.Net;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;
using RioValleyChili.Tests.ControllerTests;
using RioValleyChili.Tests.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Tests.ApiControllerTests
{
    [TestFixture]
    public class EmployeeClaimsControllerTests
    {
        protected EmployeeClaimsController SystemUnderTest;
        protected Mock<IEmployeesService> MockEmployeesService;
        protected IFixture Fixture = AutoFixtureHelper.BuildFixture();

        [SetUp]
        public void Setup()
        {
            MockEmployeesService = new Mock<IEmployeesService>();
            SystemUnderTest = new EmployeeClaimsController(MockEmployeesService.Object);
        }

        public class PostTests : EmployeeClaimsControllerTests
        {
            private TestableEmployeeDetailsReturn _employee;

            [SetUp]
            public new void Setup()
            {
                _employee = Fixture.Create<TestableEmployeeDetailsReturn>();

                MockEmployeesService.Setup(m => m.GetEmployeeDetailsByKey(It.IsAny<string>()))
                    .Returns((string key) =>
                    {
                        _employee.EmployeeKey = key;
                        return new SuccessResult<IEmployeeDetailsReturn>(_employee);
                    });

                MockEmployeesService.Setup(m => m.UpdateEmployee(It.IsAny<IUpdateEmployeeParameters>()))
                    .Returns(new SuccessResult());
            }

            [Test]
            public void CallsEmployeeServiceUpdateMethod()
            {
                // Arrange
                var claims = Fixture.Create<Dictionary<string, string>>();
                
                // Act
                SystemUnderTest.Post("eKey1", claims);

                // Assert
                MockEmployeesService.Verify(m => m.UpdateEmployee(It.IsAny<IUpdateEmployeeParameters>()), Times.Once());
            }

            [Test]
            public void CallsEmployeeServiceUpdateMethodWithExpectedParameters()
            {
                // Arrange
                const string employeeKey = "ekey1";
                var claims = Fixture.Create<Dictionary<string, string>>();
                IUpdateEmployeeParameters actualValues = null;
                MockEmployeesService.Setup(m => m.UpdateEmployee(It.IsAny<IUpdateEmployeeParameters>()))
                    .Callback((IUpdateEmployeeParameters input) => actualValues = input)
                    .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Post(employeeKey, claims);

                // Assert
                Assert.AreEqual(employeeKey, actualValues.EmployeeKey);
                Assert.AreEqual(claims, actualValues.Claims);
            }

            [Test]
            public void OnlyClaimsEmployeePropertyIsChanged()
            {
                // Arrange
                const string employeeKey = "ekey1";
                var claims = Fixture.Create<Dictionary<string, string>>();
                IUpdateEmployeeParameters actualValues = null;
                MockEmployeesService.Setup(m => m.UpdateEmployee(It.IsAny<IUpdateEmployeeParameters>()))
                    .Callback((IUpdateEmployeeParameters input) => actualValues = input)
                    .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Post(employeeKey, claims);

                // Assert
                Assert.AreEqual(_employee.EmployeeKey, actualValues.EmployeeKey);
                Assert.AreEqual(_employee.DisplayName, actualValues.DisplayName);
                Assert.AreEqual(_employee.EmailAddress, actualValues.EmailAddress);
                Assert.AreEqual(_employee.UserName, actualValues.UserName);
            }

            [Test]
            public void CallsEmployeeService_GetEmployeeDetails_WithIdParameter()
            {
                // Arrange
                const string employeeKey = "ekey1";
                var claims = Fixture.Create<Dictionary<string, string>>();

                // Act
                SystemUnderTest.Post(employeeKey, claims);

                // Assert
                MockEmployeesService.Verify(m => m.GetEmployeeDetailsByKey(employeeKey), Times.Once());
            }

            [Test]
            public void Returns404IfGetEmployeeDetailsByKeyReturnsInvalid()
            {
                // Arrange
                const string employeeKey = "ekey1";
                MockEmployeesService.Setup(m => m.GetEmployeeDetailsByKey(employeeKey))
                    .Returns(new InvalidResult<IEmployeeDetailsReturn>());
                var claims = Fixture.Create<Dictionary<string, string>>();

                // Act
                var result = SystemUnderTest.Post(employeeKey, claims);

                // Assert
                Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            }

            [Test]
            public void Returns500IfGetEmployeeDetailsByKeyReturnsFailure()
            {
                // Arrange
                const string employeeKey = "ekey1";
                MockEmployeesService.Setup(m => m.GetEmployeeDetailsByKey(employeeKey))
                    .Returns(new FailureResult<IEmployeeDetailsReturn>());
                var claims = Fixture.Create<Dictionary<string, string>>();

                // Act
                var result = SystemUnderTest.Post(employeeKey, claims);

                // Assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            }

            [Test]
            public void Returns200OnSuccess()
            {
                // Arrange
                const string id = "key1";
                var claims = Fixture.Create<Dictionary<string, string>>();

                // Act
                var response = SystemUnderTest.Post(id, claims);

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            [Test]
            public void Returns400OnInvalidResult()
            {
                // Arrange
                const string id = "key1";
                var claims = Fixture.Create<Dictionary<string, string>>();
                MockEmployeesService.Setup(m => m.UpdateEmployee(It.IsAny<IUpdateEmployeeParameters>()))
                    .Returns(new InvalidResult());

                // Act
                var response = SystemUnderTest.Post(id, claims);

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void Returns500OnFailureResult()
            {
                // Arrange
                const string id = "key1";
                var claims = Fixture.Create<Dictionary<string, string>>();
                MockEmployeesService.Setup(m => m.UpdateEmployee(It.IsAny<IUpdateEmployeeParameters>()))
                    .Returns(new FailureResult());

                // Act
                var response = SystemUnderTest.Post(id, claims);

                // Assert
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }
    }
}