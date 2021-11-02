using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using Solutionhead.TestFoundations.Utilities;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class EmployeesServiceTests : ServiceIntegrationTestBase<EmployeesService>
    {
        #region private interface implementations

// ReSharper disable ClassNeverInstantiated.Local
        private class UpdateEmployeeParameters : IUpdateEmployeeParameters
// ReSharper restore ClassNeverInstantiated.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
        {
            public string EmployeeKey { get; set; }
            public string DisplayName { get; set; }
            public string UserName { get; set; }
            public string EmailAddress { get; set; }
            public Dictionary<string, string> Claims { get; set; }
            public bool IsActive { get; set; }

            IDictionary<string, string> IUpdateEmployeeParameters.Claims
            {
                get { return Claims; }
            }
        }
// ReSharper restore UnusedAutoPropertyAccessor.Local

        #endregion

        [Test]
        public void UpdateEmployeeTest()
        {
            // Arrange
            var employee = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Employee>();
            var employeeKey = new EmployeeKey(employee);

            var fixture = new Fixture();
            var input = fixture
                .Build<UpdateEmployeeParameters>()
                    .With(m => m.EmployeeKey, employeeKey.KeyValue)
                    .With(m => m.Claims, new Dictionary<string, string>()
                    {
                        {"1", "one" },
                        {"2", "two" },
                        {"3", "three" },
                    })
                .CreateAnonymous();
            input.DisplayName = input.DisplayName.Substring(0, 15);
            input.UserName = input.UserName.Substring(0, 15);

            // Act
            var result = Service.UpdateEmployee(input);

            // Assert
            result.AssertSuccess();
        }
    }
}
