using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class CustomerSpecsLogger : EntityLoggerBase<CustomerProductSpecMother.CallbackParameters, CustomerProductSpecMother.CallbackReason>
    {
        public CustomerSpecsLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(CustomerProductSpecMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case CustomerProductSpecMother.CallbackReason.ChileProductNotLoaded:
                    return string.Format("{0} - ChileProduct not loaded.", KeyString(parameters));

                case CustomerProductSpecMother.CallbackReason.CustomerNotLoaded:
                    return string.Format("{0} - customer Company not loaded.", KeyString(parameters));
            }
            return null;
        }

        private static string KeyString(CustomerProductSpecMother.CallbackParameters parameters)
        {
            return string.Format("ProdId[{0}] Company_IA[{1}]", parameters.Spec.ProdID, parameters.Spec.Company_IA);
        }
    }
}