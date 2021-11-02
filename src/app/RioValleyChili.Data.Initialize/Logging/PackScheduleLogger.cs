using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class PackScheduleLogger : EntityLoggerBase<PackScheduleEntityObjectMother.CallbackParameters, PackScheduleEntityObjectMother.CallbackReason>
    {
        public PackScheduleLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(PackScheduleEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case PackScheduleEntityObjectMother.CallbackReason.NullProduct:
                    return string.Format("{0} null product.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.NullProductType:
                    return string.Format("{0} Product[{1}] null PTypeID.", KeyString(parameters), parameters.PackSchedule.ProdID);

                case PackScheduleEntityObjectMother.CallbackReason.NullPackSchDate:
                    return string.Format("{0} null PackSchDate.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.NullBatchTypeID:
                    return string.Format("{0} null BatchTypeID.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.NoBatchItems:
                    return string.Format("{0} no batch items..", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.PackagingNotLoaded:
                    return string.Format("{0} Packaging[{1}] not loaded.", KeyString(parameters), parameters.PkgID);

                case PackScheduleEntityObjectMother.CallbackReason.ChileProductNotLoaded:
                    return string.Format("{0} ChileProduct[{1}] not loaded.", KeyString(parameters), parameters.PackSchedule.ProdID);

                case PackScheduleEntityObjectMother.CallbackReason.ProductionLineNotLoaded:
                    return string.Format("{0} ProductionLine[{1}] not loaded.", KeyString(parameters), parameters.Line);

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_CouldNotDetermine:
                    return string.Format("{0} Packaging could not be determined, default Packaging[{1}] used.", KeyString(parameters), parameters.Packaging.Packaging);

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_ResolvedFromMultiplePickedPackaging:
                    return string.Format("{0} Packaging resolved from multiple picked packaging.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromResultingLotIncoming:
                    return string.Format("{0} Packaging determined from resulting lot incoming.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromRelabelInputs:
                    return string.Format("{0} Packaging determined from relabel inputs.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromRelabelInputsFromDescription:
                    return string.Format("{0} Packaging determined from relabel inputs from description.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_ToteInDescription:
                    return string.Format("{0} Packaging determined from \"TOTE\" in description.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_DrumInDescriptionMesh20:
                    return string.Format("{0} Packaging determined from \"DRUM\" in description with mesh 20.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_DrumInDescriptionNotMesh20:
                    return string.Format("{0} Packaging determined from \"DRUM\" in description.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_BagInDescription:
                    return string.Format("{0} Packaging determined from \"BAG\" in description.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_BoxInDescription:
                    return string.Format("{0} Packaging determined from \"BOX\" in description.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromResultingLotInventory:
                    return string.Format("{0} Packaging determined from resulting lot inventory.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromReworkInputs:
                    return string.Format("{0} Packaging determined from rework inputs.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Line_CouldNotDetermine:
                    return string.Format("{0} ProductionLine could not be determined.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Line_DeterminedFromResultingLots:
                    return string.Format("{0} ProductionLine determined from resulting lots.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Line_DeterminedFromDescription:
                    return string.Format("{0} ProductionLine determined from description.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.Line_DeterminedFromBatchType:
                    return string.Format("{0} ProductionLine determined from batch type.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.DefaultEmployee:
                    return string.Format("{0} DefaultEmployeeId[{1}] used.", KeyString(parameters), parameters.DefaultEmployeeId);

                case PackScheduleEntityObjectMother.CallbackReason.BatchLotNotLoaded:
                    return string.Format("{0} Lot[{1}] not loaded.", KeyString(parameters), parameters.LotKey);

                case PackScheduleEntityObjectMother.CallbackReason.NoSingleBatchType:
                    return string.Format("{0} batch tblLot records do not have single BatchTypeID.", KeyString(parameters));

                case PackScheduleEntityObjectMother.CallbackReason.CustomerNotLoaded:
                    return string.Format("{0} Customer[{1}] not loaded.", KeyString(parameters), parameters.PackSchedule.Company_IA);

                case PackScheduleEntityObjectMother.CallbackReason.MismatchedBatchItemPackSchID:
                    return string.Format("{0} tblBatchItem for Lot[{1}] referencing different PackSchID.", KeyString(parameters), parameters.BatchNumber);
            }

            return null;
        }

        private static string KeyString(PackScheduleEntityObjectMother.CallbackParameters parameters)
        {
            return string.Format("PackSchedule[{0}]", parameters.PackSchedule.PackSchID);
        }
    }
}