using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class ProductionScheduleLogger : EntityLoggerBase<ProductionScheduleEntityObjectMother.CallbackParameters, ProductionScheduleEntityObjectMother.CallbackReason>
    {
        public ProductionScheduleLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(ProductionScheduleEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case ProductionScheduleEntityObjectMother.CallbackReason.NullPackSchedule:
                    return string.Format("{0} referencing missing PackSchedule[PSNum[{1}]].", ProductionScheduleString(parameters.ProductionSchedule), parameters.ProductionScheduleItem.PSNum);

                case ProductionScheduleEntityObjectMother.CallbackReason.DuplicateIndex:
                    return string.Format("{0} referencing PackSchedule[PSNum[{1}]] duplicate Index[{2}].", ProductionScheduleString(parameters.ProductionSchedule), parameters.ProductionScheduleItem.PSNum, parameters.ProductionScheduleItem.Index);

                case ProductionScheduleEntityObjectMother.CallbackReason.PackScheduleNotLoaded:
                    return string.Format("{0} referencing PackSchedule[PackSchID[{1}]] not loaded.", ProductionScheduleString(parameters.ProductionSchedule), parameters.ProductionScheduleItem.PackSchedule.PackSchID);

                case ProductionScheduleEntityObjectMother.CallbackReason.ProductionLineNotLoaded:
                    return string.Format("{0} ProductionLine[{1}] not loaded.", ProductionScheduleString(parameters.ProductionSchedule), parameters.Line);
            }

            return null;
        }

        private static string ProductionScheduleString(ProductionScheduleEntityObjectMother.ProductionScheduleDTO productionSchedule)
        {
            return string.Format("ProductionSchedule[ProductionDate[{0:yyyy-MM-dd}], LineNumber[{1}]]", productionSchedule.ProductionDate, productionSchedule.LineNumber);
        }
    }
}