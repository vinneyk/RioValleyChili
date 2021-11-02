using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class InventoryTreatmentLogger : EntityLoggerBase<InventoryTreatmentMother.CallbackParameters, InventoryTreatmentMother.CallbackReason>
    {
        public InventoryTreatmentLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(InventoryTreatmentMother.CallbackParameters parameters)
        {
            return null;
        }
    }
}