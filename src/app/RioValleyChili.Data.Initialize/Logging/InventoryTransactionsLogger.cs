using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class InventoryTransactionsLogger : EntityLoggerBase<InventoryTransactionsMother.CallbackParameters, InventoryTransactionsMother.CallbackReason>
    {
        public InventoryTransactionsLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(InventoryTransactionsMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case InventoryTransactionsMother.CallbackReason.UnmappedTTypeID:
                    return string.Format("{0} unmapped TTypeID[{1}].", Key(parameters), parameters.Transaction.TTypeID);

                case InventoryTransactionsMother.CallbackReason.NullEmployeeID:
                    return string.Format("{0} null EmployeeID, used default instead.", Key(parameters));

                case InventoryTransactionsMother.CallbackReason.CannotParseLotNumber:
                    return string.Format("{0} cannot parse LotNumber[{1}].", Key(parameters), parameters.Transaction.Lot);

                case InventoryTransactionsMother.CallbackReason.CannotParseDestLotNumber:
                    return string.Format("{0} cannot parse LotNumber[{1}].", Key(parameters), parameters.Transaction.NewLot);
                    
                case InventoryTransactionsMother.CallbackReason.LotNotLoaded:
                    return string.Format("{0} Lot[{1}] not loaded.", Key(parameters), parameters.Transaction.Lot);
    
                case InventoryTransactionsMother.CallbackReason.DestLotNotLoaded:
                    return string.Format("{0} Lot[{1}] not loaded.", Key(parameters), parameters.Transaction.NewLot);

                case InventoryTransactionsMother.CallbackReason.PackagingNotLoaded:
                    return string.Format("{0} Packaging[{1}] not loaded.", Key(parameters), parameters.Transaction.PkgID);

                case InventoryTransactionsMother.CallbackReason.LocationNotLoaded:
                    return string.Format("{0} Location[{1}] not loaded.", Key(parameters), parameters.Transaction.LocID);

                case InventoryTransactionsMother.CallbackReason.TreatmentNotLoaded:
                    return string.Format("{0} Treatment[{1}] not loaded.", Key(parameters), parameters.Transaction.TrtmtID);

                case InventoryTransactionsMother.CallbackReason.IntraWarehouseOrderNotLoaded:
                    return string.Format("{0} IntraWarehouseOrder RinconID[{1}] not loaded.", Key(parameters), parameters.Transaction.RinconID);

                case InventoryTransactionsMother.CallbackReason.InventoryShipmentOrderNotLoaded:
                    return string.Format("{0} InventoryShipmentOrder of Type[{1}] MoveNum[{2}] not loaded.", Key(parameters), parameters.OrderType, parameters.Transaction.MoveNum);

                case InventoryTransactionsMother.CallbackReason.InventoryAdjustmentNotLoaded:
                    return string.Format("{0} InventoryAdjustment TimeStamp[{1}] not loaded.", Key(parameters), parameters.Transaction.AdjustID.ConvertLocalToUTC());

                case InventoryTransactionsMother.CallbackReason.PackScheduleNotLoaded:
                    return string.Format("{0} PackSchedule PackSchId[{1}] not loaded.", Key(parameters), parameters.Transaction.PackSchId);
            }

            return null;
        }

        private static string Key(InventoryTransactionsMother.CallbackParameters parameters)
        {
            if(parameters.Transaction.IncomingID != null)
            {
                return string.Format("tblIncoming[{0}]", parameters.Transaction.IncomingID.Value);
            }

            if(parameters.Transaction.OutgoingID != null)
            {
                return string.Format("tblOutgoing[{0}]", parameters.Transaction.OutgoingID.Value);
            }

            return null;
        }
    }
}