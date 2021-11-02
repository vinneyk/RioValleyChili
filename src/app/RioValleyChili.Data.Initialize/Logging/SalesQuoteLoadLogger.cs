using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class SalesQuoteLoadLogger : EntityLoggerBase<SalesQuoteEntityObjectMother.CallbackParameters, SalesQuoteEntityObjectMother.CallbackReason>
    {
        public SalesQuoteLoadLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(SalesQuoteEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case SalesQuoteEntityObjectMother.CallbackReason.DefaultEmployee:
                    return string.Format("{0} assigned DefaultEmployee.", GetKey(parameters));

                case SalesQuoteEntityObjectMother.CallbackReason.FacilityNotFound:
                    return string.Format("{0} WHID[{1}] not loaded.", GetKey(parameters), parameters.tblQuote.WHID);

                case SalesQuoteEntityObjectMother.CallbackReason.CompanyNotFound:
                    return string.Format("{0} Company_IA[{1}] not loaded.", GetKey(parameters), parameters.tblQuote.Company_IA);

                case SalesQuoteEntityObjectMother.CallbackReason.BrokerNotFound:
                    return string.Format("{0} Broker[{1}] not loaded.", GetKey(parameters), parameters.tblQuote.Broker);

                case SalesQuoteEntityObjectMother.CallbackReason.ProductNotFound:
                    return string.Format("{0} ProdID[{1}] not loaded.", GetKey(parameters), parameters.tblQuoteDetail.ProdID);

                case SalesQuoteEntityObjectMother.CallbackReason.PackagingNotFound:
                    return string.Format("{0} PkgID[{1}] not loaded.", GetKey(parameters), parameters.tblQuoteDetail.PkgID);

                case SalesQuoteEntityObjectMother.CallbackReason.TreatmentNotFound:
                    return string.Format("{0} TrtmtID[{1}] not loaded.", GetKey(parameters), parameters.tblQuoteDetail.TrtmtID);
            }

            return null;
        }

        private static string GetKey(SalesQuoteEntityObjectMother.CallbackParameters parameters)
        {
            if(parameters.tblQuote != null)
            {
                return string.Format("tblQuote[{0}]", parameters.tblQuote.QuoteNum);
            }

            if(parameters.tblQuoteDetail != null)
            {
                return string.Format("tblQuoteDetail[{0}]", parameters.tblQuoteDetail.QDetail);
            }

            return null;
        }
    }
}