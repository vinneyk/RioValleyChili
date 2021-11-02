using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class IntraWarehouseOrdersLogger : EntityLoggerBase<IntraWarehouseOrdersMother.CallbackParameters, IntraWarehouseOrdersMother.CallbackReason>
    {
        public IntraWarehouseOrdersLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(IntraWarehouseOrdersMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case IntraWarehouseOrdersMother.CallbackReason.InvalidLotKey:
                    return string.Format("{0} invalid LotKey[{1}].", KeyString(parameters), parameters.Detail.Lot);

                case IntraWarehouseOrdersMother.CallbackReason.FromWarehouseLocationNotLoaded:
                    return string.Format("{0} FromWarehouseLocation[{1}] not loaded.", KeyString(parameters), parameters.Detail.CurrLocID);

                case IntraWarehouseOrdersMother.CallbackReason.MissingQuantity:
                    return string.Format("{0} missing Quantity.", KeyString(parameters));

                case IntraWarehouseOrdersMother.CallbackReason.PackagingNotLoaded:
                    return string.Format("{0} PackagingProduct[{1}] not loaded.", KeyString(parameters), parameters.Detail.PkgID);

                case IntraWarehouseOrdersMother.CallbackReason.TreatmentNotLoaded:
                    return string.Format("{0} Treatment[{1}] not loaded.", KeyString(parameters), parameters.Detail.TrtmtID);

                case IntraWarehouseOrdersMother.CallbackReason.DestinationLocationNotLoaded:
                    return string.Format("{0} DestinationWarehouseLocation[{1}] not loaded.", KeyString(parameters), parameters.Detail.DestLocID);

                case IntraWarehouseOrdersMother.CallbackReason.LotNotLoaded:
                    return string.Format("{0} Lot[{1}] not loaded.", KeyString(parameters), parameters.Detail.Lot);

                case IntraWarehouseOrdersMother.CallbackReason.PickedInventoryUndeterminedTimestamp:
                    return string.Format("{0} could not determine PickedInventory Timestamp.", KeyString(parameters));

                case IntraWarehouseOrdersMother.CallbackReason.PickedInventoryUsedDefaultEmployee:
                    return string.Format("{0} PickedInventory used DefaultEmployeeID[{1}].", KeyString(parameters), parameters.DefaultEmployeeID);

                case IntraWarehouseOrdersMother.CallbackReason.NullEmployeeID:
                    return string.Format("{0} used default EmployeeId.", KeyString(parameters));
            }

            return null;
        }

        private static string KeyString(IntraWarehouseOrdersMother.CallbackParameters parameters)
        {
            if(parameters.Rincon != null)
            {
                return string.Format("tblRincon[{0}]", parameters.Rincon.RinconID);
            }
            return string.Format("tblRinconDetail[RDetailID[{0}] RinconID[{1}]]", parameters.Detail.RDetailID, parameters.Detail.RinconID);
        }
    }
}