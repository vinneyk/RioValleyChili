using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class EmployeeLogger : EntityLoggerBase<EmployeeObjectMother.CallbackParameters, EmployeeObjectMother.CallbackReason>
    {
        public EmployeeLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(EmployeeObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case EmployeeObjectMother.CallbackReason.EmployeeDataPreserved:
                    return string.Format("Employee[{0}] data preserved.", parameters.Employee.UserName);
            }

            return null;
        }
    }
}