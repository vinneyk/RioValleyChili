using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class EmployeeClaimsController : ApiController
    {
        private readonly IEmployeesService _employeesService;

        public EmployeeClaimsController(IEmployeesService employeesService)
        {
            if(employeesService == null) throw new ArgumentNullException("employeesService");
            _employeesService = employeesService;
        }

        // POST: /api/employees/1/claims
        public HttpResponseMessage Post(string employeeKey, [FromBody] IEnumerable<KeyValuePair<string, string>> claims)
        {
            var employeeResult = _employeesService.GetEmployeeDetailsByKey(employeeKey);
            if(!employeeResult.Success) return employeeResult.ToHttpResponseMessage(HttpVerbs.Get);

            var parameters = new UpdateEmployeeClaimsParameters(employeeResult.ResultingObject);
            parameters.Claims = claims.ToDictionary();

            var updateResult = _employeesService.UpdateEmployee(parameters);
            return updateResult.ToHttpResponseMessage(HttpVerbs.Put);
        }

        #region private parameter classes

        private class UpdateEmployeeClaimsParameters : IUpdateEmployeeParameters
        {
            private readonly EmployeeDto _backing;

            public UpdateEmployeeClaimsParameters(IEmployeeDetailsReturn input)
            {
                _backing = EmployeeDto.CreateFrom(input);
                Claims = input.Claims;
            }

            public IDictionary<string, string> Claims { private get; set; }

            #region explicit implementations

            string IUpdateEmployeeParameters.EmployeeKey
            {
                get { return _backing.EmployeeKey; }
            }

            string IUpdateEmployeeParameters.DisplayName
            {
                get { return _backing.DisplayName; }
            }

            string IUpdateEmployeeParameters.UserName
            {
                get { return _backing.UserName; }
            }

            string IUpdateEmployeeParameters.EmailAddress
            {
                get { return _backing.EmailAddress; }
            }

            bool IUpdateEmployeeParameters.IsActive
            {
                get { return _backing.IsActive; }
            }
            
            IDictionary<string, string> IUpdateEmployeeParameters.Claims
            {
                get { return Claims; }
            }

            #endregion

        }

        #endregion
    }
}