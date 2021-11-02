using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class LotAllowancesLogger : EntityLoggerBase<LotAllowancesEntityObjectMother.CallbackParameters, LotAllowancesEntityObjectMother.CallbackReason>
    {
        public LotAllowancesLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(LotAllowancesEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case LotAllowancesEntityObjectMother.CallbackReason.CustomerNotLoaded:
                    return string.Format("LotNumber[{0}] Customer[{1}] not loaded.", parameters.Lot, parameters.CustomerAllowance.CompanyName);

                case LotAllowancesEntityObjectMother.CallbackReason.ContractNotLoaded:
                    return string.Format("LotNumber[{0}] Contract[{1}] not loaded.", parameters.Lot, parameters.ContractAllowance.ContractId);

                case LotAllowancesEntityObjectMother.CallbackReason.SalesOrderNotLoaded:
                    return string.Format("LotNumber[{0}] SalesOrder[{1}] not loaded.", parameters.Lot, parameters.CustomerOrderAllowance.OrderNum);
            }

            return null;
        }
    }
}