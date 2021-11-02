using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class ChileMaterialsReceivedLogger : EntityLoggerBase<ChileMaterialsReceivedEntityMother.CallbackParameters, ChileMaterialsReceivedEntityMother.CallbackReason>
    {
        public ChileMaterialsReceivedLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(ChileMaterialsReceivedEntityMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {

                case ChileMaterialsReceivedEntityMother.CallbackReason.NullEntryDate:
                    return string.Format("{0} null EntryDate.", KeyString(parameters));

                case ChileMaterialsReceivedEntityMother.CallbackReason.NoSingleSupplierName:
                    return string.Format("{0} no single dehydrator name.", KeyString(parameters));

                case ChileMaterialsReceivedEntityMother.CallbackReason.SupplierNotLoaded:
                    return string.Format("{0} Company[{1}] not loaded.", KeyString(parameters), parameters.DehydratorName);

                case ChileMaterialsReceivedEntityMother.CallbackReason.InvalidLotNumber:
                    return string.Format("{0} invalid LotNumber.", KeyString(parameters));

                case ChileMaterialsReceivedEntityMother.CallbackReason.WarehouseLocationNotLoaded:
                    return string.Format("{0} WarehouseLocation[{1}] not loaded.", KeyString(parameters), parameters.Items.Key.LocID);

                case ChileMaterialsReceivedEntityMother.CallbackReason.PackagingNotLoaded:
                    return string.Format("{0} PackagingProduct[{1}] not loaded.", KeyString(parameters), parameters.Items.Key.PkgID);

                case ChileMaterialsReceivedEntityMother.CallbackReason.DefaultEmployee:
                    return string.Format("{0} missing employee, used default EmployeeId[{1}]", KeyString(parameters), parameters.EmployeeId);

                case ChileMaterialsReceivedEntityMother.CallbackReason.ChileLotNotLoaded:
                    return string.Format("ChileLot[{0}] not loaded.", parameters.Lot.Lot);

                case ChileMaterialsReceivedEntityMother.CallbackReason.UsedDehyLocaleAsDehydrator:
                    return string.Format("ChileLot[{0}] used single DehyLocale[{1}] as Dehydrator name.", KeyString(parameters), parameters.DehyLocale);

                case ChileMaterialsReceivedEntityMother.CallbackReason.NoSingleTrmtID:
                    return string.Format("{0} no single TrmtID.", KeyString(parameters));

                case ChileMaterialsReceivedEntityMother.CallbackReason.TreatmentNotLoaded:
                    return string.Format("{0} Treatment[{1}] not loaded.", KeyString(parameters), parameters.TrmtID);
            }

            return null;
        }

        private static string KeyString(ChileMaterialsReceivedEntityMother.CallbackParameters parameters)
        {
            if(parameters.Lot != null)
            {
                return string.Format("tblLot[{0}]", parameters.Lot.Lot);
            }

            if(parameters.Items != null)
            {
                return string.Format("tblIncoming[{0}]", parameters.Items.Key.Lot);
            }

            return null;
        }
    }
}