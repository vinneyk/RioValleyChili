using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class AdditiveProductLogger : EntityLoggerBase<AdditiveProductEntityObjectMother.CallbackParameters, AdditiveProductEntityObjectMother.CallbackReason>
    {
        public AdditiveProductLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(AdditiveProductEntityObjectMother.CallbackParameters parameters)
        {
            return null;
        }
    }
}