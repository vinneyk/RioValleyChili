using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncContract)]
    public class SyncContract : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, SyncCustomerContractParameters>
    {
        public SyncContract(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        private bool _commitNewContext;

        public override void Synchronize(Func<SyncCustomerContractParameters> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var parameters = getInput();
            var contractKey = parameters.ContractKey;
            var contract = UnitOfWork.ContractRepository.GetContractForSynch(contractKey);
            if(contract == null)
            {
                throw new Exception(string.Format("Could not find Contract[{0}]", contractKey));
            }

            Synchronize(contract, parameters.New);
        }

        public void Synchronize(Contract contract, bool createNew)
        {
            if(contract == null) { throw new ArgumentNullException("contract"); }
            _commitNewContext = false;

            var oldContract = GetOrCreateContract(contract, createNew);
            SynchronizeOldContract(contract, oldContract);
            if(_commitNewContext)
            {
                UnitOfWork.Commit();
            }

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.SynchronizedContract, oldContract.ContractID);
        }

        private tblContract GetOrCreateContract(Contract contract, bool createNew)
        {
            tblContract oldContract;
            if(contract.ContractId == null || createNew)
            {
                oldContract = new tblContract
                    {
                        ContractID = OldContext.tblContracts.Select(c => c.ContractID).DefaultIfEmpty(0).Max() + 1,
                        EntryDate = contract.TimeStamp.ConvertUTCToLocal(),
                        SerializedKey = new ContractKey(contract)
                    };
                OldContext.tblContracts.AddObject(oldContract);
                contract.ContractId = oldContract.ContractID;
                _commitNewContext = true;
                return oldContract;
            }

            oldContract = OldContext.tblContracts
                .Include("tblContractDetails")
                .FirstOrDefault(c => c.ContractID == contract.ContractId);
            if(oldContract == null)
            {
                throw new Exception(string.Format("Could not find tblContract record with ContractID[{0}]", contract.ContractId));
            }

            return oldContract;
        }

        private void SynchronizeOldContract(Contract contract, tblContract oldContract)
        {
            var warehouse = OldContextHelper.GetWarehouse(contract.DefaultPickFromFacility.Name);
            if(warehouse == null)
            {
                throw new Exception(string.Format("Could not find tblWarehouse with WhouseAbbr[{0}].", contract.DefaultPickFromFacility.Name));
            }

            oldContract.EmployeeID = contract.EmployeeId;
            oldContract.KDate = contract.ContractDate;
            oldContract.BegDate = contract.TermBegin;
            oldContract.EndDate = contract.TermEnd;

            oldContract.Contact_IA = contract.ContactName;
            oldContract.Company_IA = contract.Customer.Company.Name;
            oldContract.Address1_IA = contract.ContactAddress.AddressLine1;
            oldContract.Address2_IA = contract.ContactAddress.AddressLine2;
            oldContract.Address3_IA = contract.ContactAddress.AddressLine3;
            oldContract.City_IA = contract.ContactAddress.City;
            oldContract.State_IA = contract.ContactAddress.State;
            oldContract.Zip_IA = contract.ContactAddress.PostalCode;
            oldContract.Country_IA = contract.ContactAddress.Country;
            oldContract.Broker = contract.Broker.Name;
            oldContract.PmtTerms = contract.PaymentTerms;
            oldContract.PONum = contract.CustomerPurchaseOrder;
            oldContract.KType = contract.ContractType.ToString();
            oldContract.KStatus = contract.ContractStatus.ToString();
            oldContract.FOB = contract.FOB;
            oldContract.Comments = contract.Comments.Notes.AggregateNotes();
            oldContract.Notes2Print = contract.NotesToPrint;
            oldContract.WHID = warehouse.WHID;

            oldContract.Serialized = SerializableContract.Serialize(contract);

            var existingDetails = oldContract.tblContractDetails != null ? oldContract.tblContractDetails.ToList() : new List<tblContractDetail>();
            var kDetailId = OldContext.tblContractDetails.Any() ? OldContext.tblContractDetails.Max(d => d.KDetailID) : DateTime.UtcNow.ConvertUTCToLocal();
            foreach(var item in contract.ContractItems)
            {
                var detail = item.KDetailID == null ? null : existingDetails.FirstOrDefault(d => d.KDetailID == item.KDetailID.Value);
                if(detail == null)
                {
                    kDetailId = kDetailId.AddSeconds(1);
                    detail = new tblContractDetail
                        {
                            KDetailID = kDetailId,
                            ContractID = oldContract.ContractID
                        };
                    OldContext.tblContractDetails.AddObject(detail);
                    item.KDetailID = kDetailId;
                    _commitNewContext = true;
                }
                else
                {
                    existingDetails.Remove(detail);
                }

                detail.ProdID = OldContextHelper.GetProduct(item.ChileProduct).ProdID;
                detail.PkgID = OldContextHelper.GetPackaging(item.PackagingProduct).PkgID;
                detail.Quantity = item.Quantity;
                detail.TrtmtID = OldContextHelper.GetTreatment(item).TrtmtID;
                detail.Price = (decimal?) item.PriceBase;
                detail.FreightP = (decimal?) item.PriceFreight;
                detail.TrtnmntP = (decimal?) item.PriceTreatment;
                detail.WHCostP = (decimal?) item.PriceWarehouse;
                detail.Rebate = (decimal?) item.PriceRebate;
                detail.CustProductCode = item.CustomerProductCode;
                detail.Spec = item.UseCustomerSpec ? "Cust" : "RVC";
                detail.TtlWgt = (decimal?) (item.Quantity * item.PackagingProduct.Weight);
            }

            foreach(var detail in existingDetails)
            {
                OldContext.tblContractDetails.DeleteObject(detail);
            }
        }
    }
}