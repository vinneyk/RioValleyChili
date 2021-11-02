using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class CompanyAndContactsLogger : EntityLoggerBase<CompanyAndContactsMother.CallbackParameters, CompanyAndContactsMother.CallbackReason>
    {
        public CompanyAndContactsLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(CompanyAndContactsMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case CompanyAndContactsMother.CallbackReason.CustomerDefaultBrokerAssigned:
                    return string.Format("Customer Company[{0}] has had default broker assigned.", parameters.Company.Name);

                case CompanyAndContactsMother.CallbackReason.CustomerUnresolvedBroker:
                    return string.Format("Customer[{0}] did not resolve its broker association.", parameters.Customer.Company.Name);

                case CompanyAndContactsMother.CallbackReason.NullEntryDate:
                    return string.Format("Company[{0}] nas null EntryDate.", parameters.Company.Name);

                case CompanyAndContactsMother.CallbackReason.CompanyTypeUndetermined:
                    return string.Format("Company[{0}] could not determine company type.", parameters.Company.Name);
            }

            return null;
        }
    }
}