using RioValleyChili.Data.DataSeeders.Mothers.Base;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class MovementOrdersLogger<T> : EntityLoggerBase<MovementOrdersMotherBase<T>.CallbackParameters, MovementOrdersMotherBase<T>.CallbackReason>
        where T : class
    {
        public MovementOrdersLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(MovementOrdersMotherBase<T>.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case MovementOrdersMotherBase<T>.CallbackReason.TreatmentNotDetermined:
                    return string.Format("{0} treatment could not be determined.", KeyString(parameters));

                case MovementOrdersMotherBase<T>.CallbackReason.TreatmentFaciltyNotLoaded:
                    return string.Format("{0} TreatmentFacility[{1}] not loaded.", KeyString(parameters), parameters.MovementOrder.SCompany);

                case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailUnknownWarehouseLocationNotLoaded:
                    return string.Format("{0} Unknown WarehouseLocation not loaded.", KeyString(parameters));

                case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailDefaultUnknownWarehouseLocation:
                    return string.Format("{0} default WarehouseLocation[{1}] used.", KeyString(parameters), parameters.Location);

                case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailInvalidLotNumber:
                    return string.Format("{0} invalid LotNumber[{1}]", KeyString(parameters), parameters.MoveDetail.Lot);

                case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailPackagingNotLoaded:
                    return string.Format("{0} Packaging[{1}]", KeyString(parameters), parameters.MoveDetail.PkgID);

                case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailTreatmentNotLoaded:
                    return string.Format("{0} Treatment[{1}]", KeyString(parameters), parameters.MoveDetail.TrtmtID);

                case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailLotNotLoaded:
                    return string.Format("{0} Lot[{1}]", KeyString(parameters), parameters.MoveDetail.Lot);

                case MovementOrdersMotherBase<T>.CallbackReason.NullEntryDate:
                    return string.Format("{0} null EntryDate.", KeyString(parameters));

                case MovementOrdersMotherBase<T>.CallbackReason.DestinationWarehouseNotLoaded:
                    return string.Format("{0} DestinationWarehouse[{1}] not loaded.", KeyString(parameters), parameters.MovementOrder.ToWHID);

                case MovementOrdersMotherBase<T>.CallbackReason.SourceWarehouseNotLoaded:
                    return string.Format("{0} SourceWarehouse[{1}] not loaded.", KeyString(parameters), parameters.MovementOrder.FromWHID);

                case MovementOrdersMotherBase<T>.CallbackReason.PickedInventoryNoTimestamp:
                    return string.Format("{0} could not determine Timestamp for PickedInventory.", KeyString(parameters));

                case MovementOrdersMotherBase<T>.CallbackReason.PickedInventoryDefaultEmployee:
                    return string.Format("{0} used DefaultEmployeeID[{1}] for PickedInventory.", KeyString(parameters), parameters.DefaultEmployeeId);

                case MovementOrdersMotherBase<T>.CallbackReason.UndeterminedShipmentDate:
                    return string.Format("{0} could not determine ShipmentDate.", KeyString(parameters));

                case MovementOrdersMotherBase<T>.CallbackReason.OrderItemCustomerNotFound:
                    return string.Format("{0} CustomerID[{1}] not loaded.", KeyString(parameters), parameters.MoveOrderDetail.CustomerID);

                case MovementOrdersMotherBase<T>.CallbackReason.OrderItemProductNotFound:
                    return string.Format("{0} ProdID[{1}] not loaded.", KeyString(parameters), parameters.MoveOrderDetail.ProdID);
            }

            return null;
        }

        private static string KeyString(MovementOrdersMotherBase<T>.CallbackParameters parameters)
        {
            if(parameters.MovementOrder != null)
            {
                return string.Format("tblMove[MoveNum[{0}]]", parameters.MovementOrder.MoveNum);
            }

            if(parameters.MoveDetail != null)
            {
                return string.Format("tblMoveDetail[MDetail[{0}] MoveNum[{1}]]", parameters.MoveDetail.MDetail, parameters.MoveDetail.MoveNum);
            }

            if(parameters.MoveOrderDetail != null)
            {
                return string.Format("tblMovOrderDetail[MOrderDetail[{0}] MoveNum[{1}]]", parameters.MoveOrderDetail.MOrderDetail, parameters.MoveOrderDetail.MoveNum);

            }

            return null;
        }
    }
}