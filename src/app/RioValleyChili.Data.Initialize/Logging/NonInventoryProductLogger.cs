using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class NonInventoryProductLogger : EntityLoggerBase<NonInventoryProductEntityObjectMother.CallbackParameters, NonInventoryProductEntityObjectMother.CallbackReason>
    {
        public NonInventoryProductLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(NonInventoryProductEntityObjectMother.CallbackParameters parameters)
        {
            return null;
        }
    }
}