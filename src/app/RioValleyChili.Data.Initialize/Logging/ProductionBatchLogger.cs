using System.Linq;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class ProductionBatchLogger : EntityLoggerBase<ProductionBatchEntityObjectMother.CallbackParameters, ProductionBatchEntityObjectMother.CallbackReason>
    {
        public ProductionBatchLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(ProductionBatchEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case ProductionBatchEntityObjectMother.CallbackReason.NullPackSchID:
                    return string.Format("{0} null PackSchID.", BatchItem(parameters));

                case ProductionBatchEntityObjectMother.CallbackReason.NullNewLot:
                    return string.Format("{0} null NewLot reference.", BatchItem(parameters));

                case ProductionBatchEntityObjectMother.CallbackReason.NullResultLot:
                    return string.Format("{0} null ResultLot reference.", BatchItem(parameters));

                case ProductionBatchEntityObjectMother.CallbackReason.InvalidLotNumber:
                    return string.Format("LotNumber[{0}] invalid.", parameters.Batch.ResultLot.Lot);

                case ProductionBatchEntityObjectMother.CallbackReason.PackScheduleNotLoaded:
                    return string.Format("PackSchedule[{0}] not loaded.", parameters.PackSchedule);

                case ProductionBatchEntityObjectMother.CallbackReason.OutputChileLotNotLoaded:
                    return string.Format("{0} ChileLot[{1}] not loaded.", BatchItem(parameters), parameters.Batch.ResultLot.Lot);

                case ProductionBatchEntityObjectMother.CallbackReason.BatchItemInvalidLotNumber:
                    return string.Format("{0} LotNumber[{1}] invalid.", BatchItem(parameters), parameters.BatchItem.Lot);

                case ProductionBatchEntityObjectMother.CallbackReason.BatchItemPackagingNotLoaded:
                    return string.Format("{0} Packaging[{1}] not loaded.", BatchItem(parameters), parameters.BatchItem.PkgID);

                case ProductionBatchEntityObjectMother.CallbackReason.BatchItemTreatmentNotLoaded:
                    return string.Format("{0} Treatment[{1}] not loaded.", BatchItem(parameters), parameters.BatchItem.TrtmtID);

                case ProductionBatchEntityObjectMother.CallbackReason.BatchItemCurrentLocationCouldnotBeDetermined:
                    return string.Format("{0} CurrentLocation could not be determined.", BatchItem(parameters));

                case ProductionBatchEntityObjectMother.CallbackReason.BatchItemDefaultPickedLocation:
                    return string.Format("{0} default picked from location used.", BatchItem(parameters));

                case ProductionBatchEntityObjectMother.CallbackReason.BatchItemPickedLocationNotDetermined:
                    return string.Format("{0} picked from location not determined.", BatchItem(parameters));

                case ProductionBatchEntityObjectMother.CallbackReason.BatchItemLotNotLoaded:
                    return string.Format("{0} Lot[{1}] not loaded.", BatchItem(parameters), parameters.BatchItem.Lot);

                case ProductionBatchEntityObjectMother.CallbackReason.LotAlreadyProcessed:
                    return string.Format("The Lot[{0}] has already been processed!", parameters.Batch.ResultLot.Lot);
            }

            return null;
        }

        private static string BatchItem(ProductionBatchEntityObjectMother.CallbackParameters parameters)
        {
            if(parameters.BatchItem != null)
            {
                return string.Format("BatchItem[{0}]", parameters.BatchItem.RowId);
            }

            if(parameters.Batch != null)
            {
                var firstBatchItem = parameters.Batch.BatchItems.FirstOrDefault();
                if(firstBatchItem != null)
                {
                    return string.Format("BatchItem[{0}]", firstBatchItem.RowId);
                }
            }

            return "UNKNOWN";
        }
    }
}