using System;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class PackScheduleHelperExtensions
    {
        public static PackScheduleEntityObjectMother.CallbackReason ToCallbackReason(this PackSchedulePackagingHelper.PackagingResult packagingResult)
        {
            switch(packagingResult)
            {
                case PackSchedulePackagingHelper.PackagingResult.CouldNotDetermine:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_CouldNotDetermine;

                case PackSchedulePackagingHelper.PackagingResult.ResolvedFromMultiplePickedPackaging:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_ResolvedFromMultiplePickedPackaging;
                    
                case PackSchedulePackagingHelper.PackagingResult.DeterminedFromResultingLotIncoming:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromResultingLotIncoming;
                    
                case PackSchedulePackagingHelper.PackagingResult.DeterminedFromRelabelInputs:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromRelabelInputs;

                case PackSchedulePackagingHelper.PackagingResult.DeterminedFromRelabelInputsFromDescription:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromRelabelInputsFromDescription;

                case PackSchedulePackagingHelper.PackagingResult.ToteInDescription:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_ToteInDescription;

                case PackSchedulePackagingHelper.PackagingResult.DrumInDescriptionMesh20:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_DrumInDescriptionMesh20;

                case PackSchedulePackagingHelper.PackagingResult.DrumInDescriptionNotMesh20:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_DrumInDescriptionNotMesh20;

                case PackSchedulePackagingHelper.PackagingResult.BagInDescription:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_BagInDescription;

                case PackSchedulePackagingHelper.PackagingResult.BoxInDescription:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_BoxInDescription;

                case PackSchedulePackagingHelper.PackagingResult.DeterminedFromResultingLotInventory:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromResultingLotInventory;

                case PackSchedulePackagingHelper.PackagingResult.DeterminedFromReworkInputs:
                    return PackScheduleEntityObjectMother.CallbackReason.Packaging_DeterminedFromReworkInputs;

                default: throw new ArgumentOutOfRangeException("packagingResult");
            }
        }

        public static PackScheduleEntityObjectMother.CallbackReason ToCallbackReason(this PackScheduleProductionLineHelper.ProductionLineResult lineResult)
        {
            switch(lineResult)
            {
                case PackScheduleProductionLineHelper.ProductionLineResult.CouldNotDetermine:
                    return PackScheduleEntityObjectMother.CallbackReason.Line_CouldNotDetermine;

                case PackScheduleProductionLineHelper.ProductionLineResult.DeterminedFromResultingLots:
                    return PackScheduleEntityObjectMother.CallbackReason.Line_DeterminedFromResultingLots;

                case PackScheduleProductionLineHelper.ProductionLineResult.DeterminedFromDescription:
                    return PackScheduleEntityObjectMother.CallbackReason.Line_DeterminedFromDescription;

                case PackScheduleProductionLineHelper.ProductionLineResult.DeterminedFromBatchType:
                    return PackScheduleEntityObjectMother.CallbackReason.Line_DeterminedFromBatchType;

                default: throw new ArgumentOutOfRangeException("lineResult");
            }
        }
    }
}