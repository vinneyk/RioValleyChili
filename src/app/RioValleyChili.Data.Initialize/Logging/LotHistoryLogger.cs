using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class LotHistoryLogger : EntityLoggerBase<LotHistoryEntityObjectMother.CallbackParameters, LotHistoryEntityObjectMother.CallbackReason>
    {
        public LotHistoryLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(LotHistoryEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case LotHistoryEntityObjectMother.CallbackReason.InvalidLotNumber:
                    return string.Format("{0} - cannot parse lot number.", KeyString(parameters));

                case LotHistoryEntityObjectMother.CallbackReason.LotNotLoaded:
                    return string.Format("{0} - Lot not loaded.", KeyString(parameters));

                case LotHistoryEntityObjectMother.CallbackReason.EmployeeIDIsNull:
                    return string.Format("{0} - EmployeeID is null, default used.", KeyString(parameters));
            }

            return null;
        }

        private static string KeyString(LotHistoryEntityObjectMother.CallbackParameters parameters)
        {
            if(parameters.Lot != null)
            {
                return string.Format("Lot[{0}]", parameters.Lot.Lot);
            }

            if(parameters.History != null)
            {
                return string.Format("Lot[{0}] ArchiveDate[{1}]", parameters.History.Lot, parameters.History.ArchiveDate);
            }

            return null;
        }
    }
}