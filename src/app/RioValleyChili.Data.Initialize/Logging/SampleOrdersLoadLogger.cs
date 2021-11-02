using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class SampleOrdersLoadLogger : EntityLoggerBase<SampleOrderEntityObjectMother.CallbackParameters, SampleOrderEntityObjectMother.CallbackReason>
    {
        public SampleOrdersLoadLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(SampleOrderEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case SampleOrderEntityObjectMother.CallbackReason.tblSample_EmployeeID_Null:
                case SampleOrderEntityObjectMother.CallbackReason.tblSampleNote_EmployeeID_Null:
                    return string.Format("{0} EmployeeID is null, used default.", GetKey(parameters));

                case SampleOrderEntityObjectMother.CallbackReason.tblSampleDetail_MissingProduct:
                    return string.Format("{0} Product[{1}] not loaded.", GetKey(parameters), parameters.tblSampleDetail.ProdID);

                case SampleOrderEntityObjectMother.CallbackReason.tblSampleDetail_InvalidLotNumber:
                    return string.Format("{0} Lot[{1}] could not be parsed.", GetKey(parameters), parameters.tblSampleDetail.Lot);

                case SampleOrderEntityObjectMother.CallbackReason.tblSampleDetail_MissingLot:
                    return string.Format("{0} Lot[{1}] not loaded.", GetKey(parameters), parameters.tblSampleDetail.Lot);

                case SampleOrderEntityObjectMother.CallbackReason.tblSample_MissingCompanyIA:
                    return string.Format("{0} CompanyIA[{1}] Customer not loaded.", GetKey(parameters), parameters.tblSample.Company_IA);

                case SampleOrderEntityObjectMother.CallbackReason.tblSample_MissingBroker:
                    return string.Format("{0} Broker[{1}] not loaded.", GetKey(parameters), parameters.tblSample.Broker);

                case SampleOrderEntityObjectMother.CallbackReason.tblSample_InvalidStatus:
                    return string.Format("{0} Status[{1}] cannot be parsed.", GetKey(parameters), parameters.tblSample.Status);
            }

            return null;
        }

        private static string GetKey(SampleOrderEntityObjectMother.CallbackParameters parameters)
        {
            if(parameters.tblSampleNote != null)
            {
                return string.Format("tblSampleNote[{0}]", parameters.tblSampleNote.SamNoteID);
            }

            if(parameters.tblSampleDetail != null)
            {
                return string.Format("tblSampleDetail[{0}]", parameters.tblSampleDetail.SampleDetailID);
            }

            if(parameters.tblSample != null)
            {
                return string.Format("tblSample[{0}]", parameters.tblSample.SampleID);
            }

            return null;
        }
    }
}