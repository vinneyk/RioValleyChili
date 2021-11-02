using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Controllers;
using RioValleyChili.Client.Mvc.Models.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;
using RioValleyChili.Tests.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Tests.ControllerTests
{
    public class TestableEmployeeDetailsReturn : IEmployeeDetailsReturn
    {
        public string EmployeeKey { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public IDictionary<string, string> Claims { get; set; }
    }

    [TestFixture]
    public class SecurityControllerTests
    {
        protected Mock<IEmployeesService> MockEmployeesService;
        protected SecurityController SystemUnderTest;
        protected readonly IFixture Fixture = AutoFixtureHelper.BuildFixture();

        [SetUp]
        public void Setup()
        {
            MockEmployeesService = new Mock<IEmployeesService>();
            SystemUnderTest = new SecurityController(MockEmployeesService.Object);
        }

        public class Register_Post : SecurityControllerTests
        {
            protected RegisterModel RegisterParameters;
            protected IEmployeeDetailsReturn EmployeeDetailsReturned; 

            [SetUp]
            public new void Setup()
            {
                base.Setup();
                EmployeeDetailsReturned = null;
                RegisterParameters = Fixture.Create<RegisterModel>();

                MockEmployeesService.Setup(m => m.ActivateEmployee(It.IsAny<IActivateEmployeeParameters>()))
                    .Returns(new SuccessResult());

                MockEmployeesService.Setup(m => m.GetEmployeeDetailsByUserName(RegisterParameters.UserName))
                    .Returns(new SuccessResult<TestableEmployeeDetailsReturn>(
                        Fixture.Build<TestableEmployeeDetailsReturn>()
                            .With(m => m.EmployeeKey, RegisterParameters.EmployeeKey)
                            .Create()
                        ));
            }

            [Test]
            public void SetsEmployeeEmailAddress_IfEmployeeKeyIsProvided()
            {
                // Arrange
                IActivateEmployeeParameters actualUpdateValues = null;
                
                MockEmployeesService.Setup(m => m.ActivateEmployee(It.IsAny<IActivateEmployeeParameters>()))
                    .Callback((IActivateEmployeeParameters input) => actualUpdateValues = input)
                    .Returns(new SuccessResult());

                // Act
                SystemUnderTest.Register(RegisterParameters);

                // Assert
                Assert.IsNotNull(actualUpdateValues);
                Assert.AreEqual(RegisterParameters.Email, actualUpdateValues.EmailAddress);
                MockEmployeesService.Verify(m => m.ActivateEmployee(It.IsAny<IActivateEmployeeParameters>()), Times.Once());
            }

            [Test]
            public void ReturnsViewResultInInvalidState_IfEmployeeKeyProvidedIsInvalid()
            {
                // Arrange
                MockEmployeesService.Setup(m => m.GetEmployeeDetailsByUserName(RegisterParameters.UserName))
                    .Returns(new InvalidResult<IEmployeeDetailsReturn>());

                // Act
                var result = SystemUnderTest.Register(RegisterParameters) as ViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(MVC.Security.Views.Register, result.ViewName);
                Assert.IsFalse(SystemUnderTest.ModelState.IsValid);
                MockEmployeesService.Verify(
                    m => m.ActivateEmployee(It.IsAny<IActivateEmployeeParameters>()), 
                    Times.Never());
            }

            [Test]
            public void ReturnsViewResultInInvalidState_IfEmployeeKeyIsValid()
            {
                // Arrange
                MockEmployeesService.Setup(m => m.ActivateEmployee(It.IsAny<IActivateEmployeeParameters>()))
                    .Returns(new InvalidResult());

                // Act
                var result = SystemUnderTest.Register(RegisterParameters) as ViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(MVC.Security.Views.Register, result.ViewName);
                Assert.IsFalse(SystemUnderTest.ModelState.IsValid);
                MockEmployeesService.Verify(
                    m => m.ActivateEmployee(It.IsAny<IActivateEmployeeParameters>()), 
                    Times.Once());
            }

            [Test]
            public void CallsCreateEmployee_IfEmployeeKeyIsEmpty()
            {
                // Arrange
                MockEmployeesService.Setup(m => m.CreateEmployee(It.IsAny<ICreateEmployeeParameters>()))
                    .Returns(new SuccessResult<string>());

                RegisterParameters.EmployeeKey = string.Empty;

                // Act
                var result = SystemUnderTest.Register(RegisterParameters);

                // Assert
                MockEmployeesService.Verify(m => m.CreateEmployee(It.IsAny<ICreateEmployeeParameters>()),
                    Times.Once());
            }

            [Test]
            public void ReturnsViewResultInInvalidState_IfEmployeeKeyIsEmptyAndServiceResultIsUnsuccessful()
            {
                // Arrange
                MockEmployeesService.Setup(m => m.CreateEmployee(It.IsAny<ICreateEmployeeParameters>()))
                    .Returns(new FailureResult<string>());

                RegisterParameters.EmployeeKey = string.Empty;

                // Act
                var result = SystemUnderTest.Register(RegisterParameters) as ViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(MVC.Security.Views.Register, result.ViewName);
                Assert.IsFalse(SystemUnderTest.ModelState.IsValid);
                MockEmployeesService.Verify(
                    m => m.CreateEmployee(It.IsAny<ICreateEmployeeParameters>()),
                    Times.Once());
            }

            [Test]
            public void ReturnsViewResultInInvalidState_IfUserNameIsInvalidForEmployeeKey()
            {
                // Arrange
                RegisterParameters.EmployeeKey = "InvalidEmployeeKey";

                // Act
                var result = SystemUnderTest.Register(RegisterParameters) as ViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(MVC.Security.Views.Register, result.ViewName);
                Assert.IsFalse(SystemUnderTest.ModelState.IsValid);
                MockEmployeesService.Verify(
                    m => m.GetEmployeeDetailsByUserName(RegisterParameters.UserName),
                    Times.Once());
                MockEmployeesService.Verify(
                    m => m.CreateEmployee(It.IsAny<ICreateEmployeeParameters>()),
                    Times.Never());
            }
        }
    }
}
