using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    internal class EmployeeDto : IEmployeeDetailsReturn
    {
        public string EmployeeKey { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public IDictionary<string, string> Claims { get; set; }

        public static EmployeeDto CreateFrom(IEmployeeDetailsReturn values)
        {
            if(values == null) throw new ArgumentNullException("values");

            return new EmployeeDto
            {
                EmployeeKey = values.EmployeeKey,
                Claims = values.Claims,
                EmailAddress = values.EmailAddress,
                DisplayName = values.DisplayName,
                IsActive = values.IsActive,
                UserName = values.UserName,
            };
        }
    }
}