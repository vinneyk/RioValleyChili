using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class MillAndWetdownLogger : EntityLoggerBase<MillAndWetdownMother.CallbackParameters, MillAndWetdownMother.CallbackReason>
    {
        public MillAndWetdownLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(MillAndWetdownMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case MillAndWetdownMother.CallbackReason.ChileLotNotLoaded:
                    return string.Format("{0} ChileLot[{1}] not loaded.", KeyString(parameters), parameters.LotKey);

                case MillAndWetdownMother.CallbackReason.DefaultProductionLine:
                    return string.Format("{0} default ProductionLine[{1}] used.", KeyString(parameters), parameters.ProductionLine.Description);

                case MillAndWetdownMother.CallbackReason.InvalidLotNumber:
                    return string.Format("{0} invalid LotNumber.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.NullEntryDate:
                    return string.Format("{0} null EntryDate.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.ProductionLineNotLoaded:
                    return string.Format("{0} ProductionLine[{1}] not loaded.", KeyString(parameters), parameters.ProductionLineNumber);

                case MillAndWetdownMother.CallbackReason.NullProdID:
                    return string.Format("{0} null ProdID.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.ChileProductNotLoaded:
                    return string.Format("{0} ChileProduct[{1}] not loaded.", KeyString(parameters), parameters.Lot.ProdID);

                case MillAndWetdownMother.CallbackReason.ProductionBeginCouldNotBeDetermined:
                    return string.Format("{0} null BatchBegTime and ProductionDate.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.ProductionBeginFromProductionDate:
                    return string.Format("{0} null BatchBegTime, used ProductionDate instead.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.ProductionEndTimeCouldNotBeDetermined:
                    return string.Format("{0} null BatchEndTime and ProductionDate.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.ProductionEndTimeFromProductionDate:
                    return string.Format("{0} null BatchEndTime, used ProductionDate instead.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.OutgoingInvalidLotNumber:
                    return string.Format("{0} invalid LotNumber.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.OutgoingPackagingNotLoaded:
                    return string.Format("{0} Packaging[{1}] not loaded.", KeyString(parameters), parameters.Outgoing.PkgID);

                case MillAndWetdownMother.CallbackReason.OutgoingWarehouseLocationNotLoaded:
                    return string.Format("{0} WarehouseLocation[{1}] not loaded.", KeyString(parameters), parameters.Outgoing.LocID);

                case MillAndWetdownMother.CallbackReason.OutgoingTreatmentNotLoaded:
                    return string.Format("{0} Treatment[{1}] not loaded.", KeyString(parameters), parameters.Outgoing.TrtmtID);

                case MillAndWetdownMother.CallbackReason.IncomingPackagingNotLoaded:
                    return string.Format("{0} Packaging[{1}] not loaded.", KeyString(parameters), parameters.Incoming.PkgID);

                case MillAndWetdownMother.CallbackReason.IncomingWarehouseLocationNotLoaded:
                    return string.Format("{0} WarehouseLocation[{1}] not loaded.", KeyString(parameters), parameters.Incoming.LocID);

                case MillAndWetdownMother.CallbackReason.OutgoingLotNotLoaded:
                    return string.Format("{0} lot not loaded.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.DefaultEmployee:
                    return string.Format("{0} missing employee, used default EmployeeId[{1}].", KeyString(parameters), parameters.EmployeeId);

                case MillAndWetdownMother.CallbackReason.PickedInventoryUndeterminedTimestamp:
                    return string.Format("{0} PickedInventory could not determine TimeStamp.", KeyString(parameters));

                case MillAndWetdownMother.CallbackReason.PickedInventoryUsedDefaultEmployee:
                    return string.Format("{0} PickedInventory used DefaultEmployeeID[{1}].", KeyString(parameters), parameters.EmployeeId);
            }

            return null;
        }

        private static string KeyString(MillAndWetdownMother.CallbackParameters parameters)
        {
            if(parameters.Lot != null)
            {
                return string.Format("Lot[{0}]", parameters.Lot.Lot);
            }

            if(parameters.Incoming != null)
            {
                return string.Format("Lot[{0}] IncomingID[{1}]", parameters.Incoming.Lot, parameters.Incoming.ID);
            }

            if(parameters.Outgoing != null)
            {
                return string.Format("Lot[{0}] OutgoingID[{1}]", parameters.Outgoing.Lot, parameters.Outgoing.ID);
            }

            return null;
        }
    }
}