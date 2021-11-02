using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class CustomerProductCodesLogger : EntityLoggerBase<CustomerProductCodeMother.CallbackParameters, CustomerProductCodeMother.CallbackReason>
    {
        public CustomerProductCodesLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(CustomerProductCodeMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case CustomerProductCodeMother.CallbackReason.MultipleCodes:
                    return string.Format("Multiple codes for Customer[{0}] ChileProduct[{1}].", parameters.Order.Company_IA, parameters.Product.ProdID);

                case CustomerProductCodeMother.CallbackReason.CustomerNotFound:
                    return string.Format("Could not load Customer[{0}].", parameters.Order.Company_IA);

                case CustomerProductCodeMother.CallbackReason.ChileProductNotFound:
                    return string.Format("Could not load ChileProduct[{0}].", parameters.Product.ProdID);
            }
            return null;
        }
    }
}