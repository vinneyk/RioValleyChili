using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class ContractLogger : EntityLoggerBase<ContractEntityObjectMother.CallbackParameters, ContractEntityObjectMother.CallbackReason>
    {
        public ContractLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(ContractEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case ContractEntityObjectMother.CallbackReason.UnderterminedTimestamp:
                    return string.Format("{0} could not determine timestamp.", KeyString(parameters));

                case ContractEntityObjectMother.CallbackReason.NullContractDate:
                    return string.Format("{0} KDate is null.", KeyString(parameters));

                case ContractEntityObjectMother.CallbackReason.CustomerNotLoaded:
                    return string.Format("{0} could not load Customer[{1}].", KeyString(parameters), parameters.OldContract.Company_IA);

                case ContractEntityObjectMother.CallbackReason.ContactNotLoaded:
                    return string.Format("{0} could not load Contact[{1}].", KeyString(parameters), parameters.OldContract.Contact_IA);

                case ContractEntityObjectMother.CallbackReason.BrokerNotLoaded:
                    return string.Format("{0} could not load Broker[{1}]", KeyString(parameters), parameters.OldContract.Broker);

                case ContractEntityObjectMother.CallbackReason.WarehouseNotLoaded:
                    return string.Format("{0} could not load Warehouse[{1}].", KeyString(parameters), parameters.OldContract.WHID);

                case ContractEntityObjectMother.CallbackReason.DefaultEmployee:
                    return string.Format("{0} used default employee.", KeyString(parameters));

                case ContractEntityObjectMother.CallbackReason.DefaultRinconWarehouse:
                    return string.Format("{0} used default warehouse.", KeyString(parameters));

                case ContractEntityObjectMother.CallbackReason.UndeterminedContractType:
                    return string.Format("{0} could not determine ContractType[{1}].", KeyString(parameters), parameters.OldContract.KType);

                case ContractEntityObjectMother.CallbackReason.UndeterminedContractStatus:
                    return string.Format("{0} could not determine ContractStatus[{1}].", KeyString(parameters), parameters.OldContract.KStatus);

                case ContractEntityObjectMother.CallbackReason.ChileProductNotLoaded:
                    return string.Format("{0} could not load ChileProduct[{1}].", KeyString(parameters), parameters.Detail.ProdID);

                case ContractEntityObjectMother.CallbackReason.PackagingProductNotLoaded:
                    return string.Format("{0} could not load PackagingProduct[{1}].", KeyString(parameters), parameters.Detail.PkgID);

                case ContractEntityObjectMother.CallbackReason.TreatmentNotLoaded:
                    return string.Format("{0} could not load Treatment[{1}].", KeyString(parameters), parameters.Detail.TrtmtID);
            }

            return null;
        }

        private static string KeyString(ContractEntityObjectMother.CallbackParameters parameters)
        {
            if(parameters.OldContract != null)
            {
                return string.Format("ContractID[{0}]", parameters.OldContract.ContractID);
            }

            if(parameters.Detail != null)
            {
                return string.Format("ContractID[{0}] KDetailID[{1}]", parameters.Detail.ContractID, parameters.Detail.KDetailID);
            }

            return null;
        }
    }
}