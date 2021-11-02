using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class LotLogger : EntityLoggerBase<LotEntityObjectMother.CallbackParameters, LotEntityObjectMother.CallbackReason>
    {
        public LotLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(LotEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case LotEntityObjectMother.CallbackReason.NullProductReference:
                    return string.Format("{0} null product reference.", LotString(parameters));

                case LotEntityObjectMother.CallbackReason.InvalidLotNumber:
                    return string.Format("{0} invalid lot number.", LotString(parameters));

                case LotEntityObjectMother.CallbackReason.DuplicateLotNumber:
                    return string.Format("{0} lot number already parsed by LotNumber[{1}]", LotString(parameters), parameters.ExistingLotNumber);

                case LotEntityObjectMother.CallbackReason.InvalidLotType:
                    return string.Format("{0} invalid LotType[{1}]", LotString(parameters), parameters.LotKey.LotKey_LotTypeId);

                case LotEntityObjectMother.CallbackReason.ExpectedChileProductButFoundAdditive:
                    return string.Format("{0} - expected ChileProduct[{1}] but is loaded as Additive[{2}].", LotString(parameters), parameters.Lot.tblProduct.ProdID, parameters.Product.AdditiveProduct.Product.Name);

                case LotEntityObjectMother.CallbackReason.ExpectedChileProductButFoundPackaging:
                    return string.Format("{0} - expected ChileProduct[{1}] but is loaded as Packaging[{2}].", LotString(parameters), parameters.Lot.tblProduct.ProdID, parameters.Product.PackagingProduct.Product.Name);

                case LotEntityObjectMother.CallbackReason.ChileProductNotLoaded:
                    return string.Format("{0} - ChileProduct[{1}] not loaded.", LotString(parameters), parameters.Lot.tblProduct.ProdID);

                case LotEntityObjectMother.CallbackReason.ExpectedAdditiveProductButFoundChile:
                    return string.Format("{0} - expected AdditiveProduct[{1}] but is loaded as Chile[{2}].", LotString(parameters), parameters.Lot.tblProduct.ProdID, parameters.Product.ChileProduct.Product.Name);

                case LotEntityObjectMother.CallbackReason.ExpectedAdditiveProductButFoundPackaging:
                    return string.Format("{0} - expected AdditiveProduct[{1}] but is loaded as Packaging[{2}].", LotString(parameters), parameters.Lot.tblProduct.ProdID, parameters.Product.PackagingProduct.Product.Name);

                case LotEntityObjectMother.CallbackReason.AdditiveProductNotLoaded:
                    return string.Format("{0} - AdditiveProduct[{1}] not loaded.", LotString(parameters), parameters.Lot.tblProduct.ProdID);

                case LotEntityObjectMother.CallbackReason.PackagingProductNullPackagingId:
                    return string.Format("{0} PackagingProduct with ProdID[{1}] with null PkgID.", LotString(parameters), parameters.Lot.tblProduct.ProdID);

                case LotEntityObjectMother.CallbackReason.PackagingProductNotLoaded:
                    return string.Format("{0} - PackagingProduct[{1}] not loaded.", LotString(parameters), parameters.Lot.tblProduct.PkgID);

                case LotEntityObjectMother.CallbackReason.UnableToDetermineContamination:
                    return string.Format("{0} LotStat[{1}] but no {2} defects found.", LotString(parameters), LotStat.Contaminated, DefectTypeEnum.BacterialContamination);

                case LotEntityObjectMother.CallbackReason.TesterIDNullUsedDefault:
                    return string.Format("{0} TesterID is null, used default EmployeeID[{1}].", LotString(parameters), parameters.DefaultEmployeeID);

                case LotEntityObjectMother.CallbackReason.TestDateNullCurrentDateUsed:
                    return string.Format("{0} TestDate is null, used current date intstead.", LotString(parameters));

                case LotEntityObjectMother.CallbackReason.EntryDateNull:
                    return string.Format("{0} EntryDate is null.", LotString(parameters));

                case LotEntityObjectMother.CallbackReason.ChileLotCompletedButMissingAttributes:
                    return string.Format("Chile{0} marked Completed but is missing product defined attribute data.", LotString(parameters));

                case LotEntityObjectMother.CallbackReason.ChileLotCompletedButMissingBacterialAttributes:
                    return string.Format("Chile{0} marked Completed but is missing bacterial attributes: {1}", LotString(parameters), parameters.MissingAttributes.Aggregate("", (seed, element) => string.IsNullOrWhiteSpace(seed) ? element : seed + ", " + element));

                case LotEntityObjectMother.CallbackReason.ChileLotStatusConflict:
                    return string.Format("Chile{0} LotStatus conflict. Determined LotStatus[{1}]; OldContextLotStat[{2}]; Unresolved defects: {3}", LotString(parameters), parameters.ChileLot.Lot.QualityStatus, parameters.Lot.LotStat, GetUnresolvedDefectsString(parameters.ChileLot));
                    
                case LotEntityObjectMother.CallbackReason.SerializedReceivedPackagingNotLoaded:
                    return string.Format("{0} serialized data references tblPackaging[{1}] which is not loaded.", LotString(parameters), parameters.ReceivedPkgID);
                    
                case LotEntityObjectMother.CallbackReason.CompanyNotLoaded:
                    return string.Format("{0} Company_IA[{1}] not loaded.", LotString(parameters), parameters.Lot.Company_IA);
            }

            return null;
        }

        private static string LotString(LotEntityObjectMother.CallbackParameters parameters)
        {
            return string.Format("Lot[{0}]", parameters.Lot.Lot);
        }

        private static string GetUnresolvedDefectsString(ChileLot chileLot)
        {
            var defects = "";
            CheckAndAddDefectString(chileLot, DefectTypeEnum.ProductSpec, ref defects);
            CheckAndAddDefectString(chileLot, DefectTypeEnum.BacterialContamination, ref defects);
            CheckAndAddDefectString(chileLot, DefectTypeEnum.InHouseContamination, ref defects);
            CheckAndAddDefectString(chileLot, DefectTypeEnum.ActionableDefect, ref defects);
            return string.IsNullOrWhiteSpace(defects) ? "none" : defects;
        }

        private static void CheckAndAddDefectString(ChileLot chileLot, DefectTypeEnum defectType, ref string defectString)
        {
            if(LotStatusHelper.LotHasUnresolvedDefects(chileLot.Lot, defectType))
            {
                if(string.IsNullOrWhiteSpace(defectString))
                {
                    defectString += defectType.ToString();
                }
                else
                {
                    defectString += ", " + defectType.ToString();
                }
            }
        }
    }
}