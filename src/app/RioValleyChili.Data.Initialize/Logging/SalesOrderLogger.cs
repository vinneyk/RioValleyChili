using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class SalesOrderLogger : EntityLoggerBase<SalesOrderEntityObjectMother.CallbackParameters, SalesOrderEntityObjectMother.CallbackReason>
    {
        public SalesOrderLogger(string logFilePath) : base(logFilePath) {}

        protected override string GetLogMessage(SalesOrderEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case SalesOrderEntityObjectMother.CallbackReason.EntryDateNull:
                    return string.Format("{0} EntryDate is null.", KeyString(parameters));

                case SalesOrderEntityObjectMother.CallbackReason.EmployeeIDNull:
                    return string.Format("{0} EmployeeID is null, used DefaultEmployee.", KeyString(parameters));

                case SalesOrderEntityObjectMother.CallbackReason.CustomerNotFound:
                    return string.Format("{0} Customer[{1}] not found.", KeyString(parameters), parameters.Order.Company_IA);

                case SalesOrderEntityObjectMother.CallbackReason.BrokerNotFound:
                    return string.Format("{0} Broker[{1}] not found.", KeyString(parameters), parameters.Order.Broker);

                case SalesOrderEntityObjectMother.CallbackReason.WarehouseNotFound:
                    return string.Format("{0} Warehouse[{1}] not found.", KeyString(parameters), parameters.Order.WHID);

                case SalesOrderEntityObjectMother.CallbackReason.DateRecdNull:
                    return string.Format("{0} DateRecd is null, used EntryDate instead.", KeyString(parameters));

                case SalesOrderEntityObjectMother.CallbackReason.InvalidStatus:
                    return string.Format("{0} Status[{1}] is invalid.", KeyString(parameters), parameters.Order.Status);

                case SalesOrderEntityObjectMother.CallbackReason.DetailProductNotFound:
                    return string.Format("{0} ProdID[{1}] not found.", KeyString(parameters), parameters.Detail.ProdID);

                case SalesOrderEntityObjectMother.CallbackReason.DetailPackagingNotFound:
                    return string.Format("{0} PkgID[{1}] not found.", KeyString(parameters), parameters.Detail.PkgID);

                case SalesOrderEntityObjectMother.CallbackReason.DetailTreatmentNotFound:
                    return string.Format("{0} TrtmtID[{1}] not found.", KeyString(parameters), parameters.Detail.TrtmtID);

                case SalesOrderEntityObjectMother.CallbackReason.ContractItemNotFound:
                    return string.Format("{0} KDetailID[{1}] not found.", KeyString(parameters), parameters.Detail.KDetailID);

                case SalesOrderEntityObjectMother.CallbackReason.StagedInvalidLotNumber:
                    return string.Format("{0} Lot[{1}] was invalid.", KeyString(parameters), parameters.Staged.Lot);

                case SalesOrderEntityObjectMother.CallbackReason.StagedPackagingNotFound:
                    return string.Format("{0} PkgID[{1}] not found.", KeyString(parameters), parameters.Staged.PkgID);

                case SalesOrderEntityObjectMother.CallbackReason.StagedLocationNotFound:
                    return string.Format("{0} LocID[{1}] not found.", KeyString(parameters), parameters.Staged.LocID);

                case SalesOrderEntityObjectMother.CallbackReason.StagedTreatmentNotFound:
                    return string.Format("{0} TrtmtID[{1}] not found.", KeyString(parameters), parameters.Staged.TrtmtID);

                case SalesOrderEntityObjectMother.CallbackReason.UndeterminedShipmentDate:
                    return string.Format("{0} Cannot dermine ShipmentDate.", KeyString(parameters));

                case SalesOrderEntityObjectMother.CallbackReason.UndeterminedPickedFromLocation:
                    return string.Format("{0} could not determine location picked from.", KeyString(parameters));

                case SalesOrderEntityObjectMother.CallbackReason.OldAndEmpty:
                    return string.Format("{0} skipped because it is older than 6 months and has no order or picked items.", KeyString(parameters));
            }

            return null;
        }

        private static string KeyString(SalesOrderEntityObjectMother.CallbackParameters parameters)
        {
            if(parameters.Order != null)
            {
                if(parameters.Detail != null)
                {
                    return string.Format("tblOrder[{0}] tblOrderDetail[{1}]", parameters.Order.OrderNum, parameters.Detail.ODetail);
                }
                if(parameters.Staged != null)
                {
                    return string.Format("tblOrder[{0}] tblStagedFG[{1}]", parameters.Order.OrderNum, parameters.Staged.EntryDate);
                }
                return string.Format("tblOrder[{0}]", parameters.Order.OrderNum);
            }
            return null;
        }
    }
}