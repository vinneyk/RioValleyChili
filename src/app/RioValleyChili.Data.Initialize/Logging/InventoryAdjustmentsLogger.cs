using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class InventoryAdjustmentsLogger : EntityLoggerBase<InventoryAdjustmentsMother.CallbackParameters, InventoryAdjustmentsMother.CallbackReason>
    {
        public InventoryAdjustmentsLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(InventoryAdjustmentsMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case InventoryAdjustmentsMother.CallbackReason.NoOutgoingItems:
                    return string.Format("Adjustment[AdjustID[{0}]] not loaded due to it not having associated Outgoing records.", parameters.Adjustment.AdjustID);

                case InventoryAdjustmentsMother.CallbackReason.LotNotLoaded:
                    return string.Format("{0} Lot[{1}] not loaded.", Outgoing(parameters), parameters.Outgoing.Lot);

                case InventoryAdjustmentsMother.CallbackReason.WarehouseLocationNotLoaded:
                    return string.Format("{0} LocID[{1}] not loaded.", Outgoing(parameters), parameters.Outgoing.LocID);

                case InventoryAdjustmentsMother.CallbackReason.PackagingNotLoaded:
                    return string.Format("{0} Packaging[{1}] not loaded.", Outgoing(parameters), parameters.Outgoing.PkgID);

                case InventoryAdjustmentsMother.CallbackReason.TreatmentNotLoaded:
                    return string.Format("{0} Treatment[{1}] not loaded.", Outgoing(parameters), parameters.Outgoing.TrtmtID);

                case InventoryAdjustmentsMother.CallbackReason.InvalidLotNumber:
                    return string.Format("{0} LotNumber[{1}] invalid.", Outgoing(parameters), parameters.Outgoing.Lot);

                case InventoryAdjustmentsMother.CallbackReason.DefaultEmployee:
                    return string.Format("Adjustment[AdjustID[{0}]] missing employee, used default EmployeeID[{1}].", parameters.Adjustment.AdjustID, parameters.EmployeeID);
            }

            return null;
        }

        private static string Outgoing(InventoryAdjustmentsMother.CallbackParameters parameters)
        {
            return string.Format("Outgoing[ID[{0}] AdjustID[{1}]]", parameters.Outgoing.ID, parameters.Outgoing.AdjustID);
        }
    }
}