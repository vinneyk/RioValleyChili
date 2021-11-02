using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class EmployeeObjectMother : EntityMotherLogBase<Employee, EmployeeObjectMother.CallbackParameters>
    {
        public EmployeeObjectMother(ObjectContext oldContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback) { }

        private enum EntityTypes
        {
            Employee
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<Employee> BirthRecords()
        {
            _loadCount.Reset();

            foreach(var employee in OldContext.CreateObjectSet<tblEmployee>().ToList())
            {
                _loadCount.AddRead(EntityTypes.Employee);

                var newEmployee = new Employee
                    {
                        EmployeeId = employee.EmployeeID,
                        UserName = employee.Employee,
                        DisplayName = employee.EName,
                        EmailAddress = "",
                        IsActive = false // users are activated when their web account is created - vk 12/16/2013
                    };

                var existingEmployee = PreservedData.Employees.FirstOrDefault(e => e.UserName == newEmployee.UserName);
                if(existingEmployee != null)
                {
                    newEmployee.EmailAddress = existingEmployee.EmailAddress;
                    newEmployee.IsActive = existingEmployee.IsActive;
                    newEmployee.Claims = existingEmployee.Claims;

                    Log(new CallbackParameters(CallbackReason.EmployeeDataPreserved)
                        {
                            Employee = newEmployee
                        });
                }

                _loadCount.AddLoaded(EntityTypes.Employee);
                yield return newEmployee;
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            EmployeeDataPreserved,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public Employee Employee { get; set; }

            protected override CallbackReason ExceptionReason { get { return EmployeeObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return EmployeeObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return EmployeeObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
        }
    }
}