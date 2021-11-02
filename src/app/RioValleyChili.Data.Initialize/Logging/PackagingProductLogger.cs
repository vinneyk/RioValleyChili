using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class PackagingProductLogger : EntityLoggerBase<PackagingProductEntityObjectMother.CallbackParameters, PackagingProductEntityObjectMother.CallbackReason>
    {
        public PackagingProductLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(PackagingProductEntityObjectMother.CallbackParameters parameters)
        {
            return null;
        }
    }
}