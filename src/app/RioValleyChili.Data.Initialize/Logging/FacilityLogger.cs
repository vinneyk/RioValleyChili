using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class FacilityLogger : EntityLoggerBase<FacilityEntityObjectMother.CallbackParameters, FacilityEntityObjectMother.CallbackReason>
    {
        public FacilityLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(FacilityEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case FacilityEntityObjectMother.CallbackReason.FacilityExcluded:
                    return string.Format("Facility[{0}] explicitly excluded.", parameters.WarehouseAbbreviation);
            }

            return null;
        }
    }
}