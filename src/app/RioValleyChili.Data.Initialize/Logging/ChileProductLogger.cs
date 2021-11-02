using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class ChileProductLogger : EntityLoggerBase<ChileProductEntityObjectMother.CallbackParameters, ChileProductEntityObjectMother.CallbackReason>
    {
        public ChileProductLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(ChileProductEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case ChileProductEntityObjectMother.CallbackReason.NoProductName:
                    return string.Format("{0} has no name.", ProductString(parameters));

                case ChileProductEntityObjectMother.CallbackReason.InvalidAttributeData:
                    return string.Format("{0} invalid data for Attribute[{1}].", ProductString(parameters), parameters.AttributeName.Name);

                case ChileProductEntityObjectMother.CallbackReason.MissingAttributeData:
                    return string.Format("{0} missing data for Attribute[{1}].", ProductString(parameters), parameters.AttributeName.Name);

                case ChileProductEntityObjectMother.CallbackReason.UnmappedChileType:
                    return string.Format("The ProdGroup[{1}] is unmapped, used default ChileVariety[{2}] instead for {0}.", ProductString(parameters), ProductGroupString(parameters), parameters.ChileType.Description);
            }

            return null;
        }

        private static string ProductString(ChileProductEntityObjectMother.CallbackParameters parameters)
        {
            return string.Format("ProdID[{0}] Name[{1}]", parameters.Product.ProdID, parameters.Product.Product ?? "NULL");
        }

        private static string ProductGroupString(ChileProductEntityObjectMother.CallbackParameters parameters)
        {
            if(parameters.Product.tblProductGrp == null)
            {
                return "null";
            }

            return string.Format("{0} - {1}", parameters.Product.tblProductGrp.ProdGrpID, parameters.Product.tblProductGrp.ProdGroup);
        }
    }
}