using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class ContractEntityObjectMother : EntityMotherLogBase<ContractEntityObjectMother.ContractLoad, ContractEntityObjectMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;
        private readonly NotebookFactory _notebookFactory;

        public ContractEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            _newContextHelper = new NewContextHelper(newContext);
            _notebookFactory = NotebookFactory.Create(newContext);
        }

        private enum EntityType
        {
            Contract,
            ContractItem
        }

        private readonly MotherLoadCount<EntityType> _loadCount = new MotherLoadCount<EntityType>();
        private ContractKeyReservationHelper _contractKeyReservationHelper;

        protected override IEnumerable<ContractLoad> BirthRecords()
        {
            _loadCount.Reset();
            _contractKeyReservationHelper = new ContractKeyReservationHelper();

            foreach(var oldContract in SelectContractsToLoad(OldContext))
            {
                var newContract = GetContract(oldContract);

                if(newContract != null)
                {
                    _loadCount.AddLoaded(EntityType.Contract);
                    _loadCount.AddLoaded(EntityType.ContractItem, (uint) newContract.ContractItems.Count);
                    yield return newContract;
                }
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private ContractLoad GetContract(tblContractDTO oldContract)
        {
            var key = _contractKeyReservationHelper.ParseAndRegisterKey(oldContract.SerializedKey);
            var requiresKeySync = key == null;
            _loadCount.AddRead(EntityType.Contract);

            var timestamp = oldContract.EntryDate.ConvertLocalToUTC() ?? oldContract.KDate ?? oldContract.BegDate;
            if(timestamp == null)
            {
                Log(new CallbackParameters(CallbackReason.UnderterminedTimestamp) { OldContract = oldContract });
                return null;
            }

            if(oldContract.KDate == null)
            {
                Log(new CallbackParameters(CallbackReason.NullContractDate) { OldContract = oldContract });
                return null;
            }

            var employeeId = _newContextHelper.DefaultEmployee.EmployeeId;
            if(oldContract.EmployeeId == null)
            {
                Log(new CallbackParameters(CallbackReason.DefaultEmployee) { OldContract = oldContract });
            }
            else
            {
                employeeId = oldContract.EmployeeId.Value;
            }

            var customer = _newContextHelper.GetCompany(oldContract.Company_IA, CompanyType.Customer);
            if(customer == null)
            {
                Log(new CallbackParameters(CallbackReason.CustomerNotLoaded) { OldContract = oldContract });
                return null;
            }

            var broker = _newContextHelper.GetCompany(oldContract.Broker, CompanyType.Broker);
            if(broker == null)
            {
                Log(new CallbackParameters(CallbackReason.BrokerNotLoaded) { OldContract = oldContract });
                return null;
            }

            Facility facility = null;
            if(oldContract.WHID == null)
            {
                facility = _newContextHelper.Facilities.Values.FirstOrDefault(f => f.Name == "Rincon");
                Log(new CallbackParameters(CallbackReason.DefaultRinconWarehouse) { OldContract = oldContract });
            }
            else
            {
                facility = _newContextHelper.GetFacility(oldContract.WHID);
            }

            if(facility == null)
            {
                Log(new CallbackParameters(CallbackReason.WarehouseNotLoaded) { OldContract = oldContract });
                return null;
            }

            var contractType = GetContractType(oldContract.KType);
            if(contractType == null)
            {
                Log(new CallbackParameters(CallbackReason.UndeterminedContractType) { OldContract = oldContract });
                return null;
            }

            var contractStatus = GetContractStatus(oldContract.KStatus);
            if(contractStatus == null)
            {
                Log((new CallbackParameters(CallbackReason.UndeterminedContractStatus) { OldContract = oldContract }));
                return null;
            }

            var commentsNotebook = _notebookFactory.BirthNext(timestamp.Value, employeeId, oldContract.Comments);

            key = key ?? _contractKeyReservationHelper.GetNextAndRegisterKey(new ContractKeyReservationHelper.Key
                {
                    ContractKey_Year = oldContract.KDate.Value.Year,
                    ContractKey_Sequence = 0
                });

            var newContract = new ContractLoad
                {
                    RequiresKeySync = requiresKeySync,
                    EmployeeId = oldContract.EmployeeId ?? _newContextHelper.DefaultEmployee.EmployeeId,
                    TimeStamp = timestamp.Value,

                    ContractYear = key.ContractKey_Year,
                    ContractSequence = key.ContractKey_Sequence,

                    ContractId = oldContract.ContractID,
                    CustomerId = customer.Id,
                    ContactName = oldContract.Contact_IA,
                    FOB = oldContract.FOB,
                    BrokerId = broker.Id,
                    DefaultPickFromWarehouseId = facility.Id,

                    ContractType = contractType.Value,
                    ContractStatus = contractStatus.Value,
                    ContractDate = oldContract.KDate.Value,
                    TimeCreated = timestamp.Value.Date,
                    TermBegin = oldContract.BegDate.GetDate(),
                    TermEnd = oldContract.EndDate.GetDate(),

                    PaymentTerms = oldContract.PmtTerms,
                    NotesToPrint = oldContract.Notes2Print,
                    CustomerPurchaseOrder = oldContract.PONum,
                    ContactAddress = new Address
                        {
                            AddressLine1 = oldContract.Address1_IA,
                            AddressLine2 = oldContract.Address2_IA,
                            AddressLine3 = oldContract.Address3_IA,
                            City = oldContract.City_IA,
                            State = oldContract.State_IA,
                            PostalCode = oldContract.Zip_IA,
                            Country = oldContract.Country_IA
                        },

                    CommentsDate =  commentsNotebook.Date,
                    CommentsSequence = commentsNotebook.Sequence,
                    Comments = commentsNotebook
                };

            SerializableContract.DeserializeIntoContract(newContract, oldContract.Serialized);

            GetContractItems(newContract, oldContract);

            return newContract;
        }

        private void GetContractItems(Contract newContract, tblContractDTO oldContract)
        {
            var sequence = 0;
            var items = new List<ContractItem>();
            foreach(var oldDetail in oldContract.Details)
            {
                _loadCount.AddRead(EntityType.ContractItem);

                var chileProduct = _newContextHelper.GetChileProduct(oldDetail.ProdID);
                if(chileProduct == null)
                {
                    Log(new CallbackParameters(CallbackReason.ChileProductNotLoaded) { Detail = oldDetail });
                    continue;
                }

                var packaging = _newContextHelper.GetPackagingProduct(oldDetail.PkgID);
                if(packaging == null)
                {
                    Log(new CallbackParameters(CallbackReason.PackagingProductNotLoaded) { Detail = oldDetail });
                    continue;
                }

                var treatment = _newContextHelper.NoTreatment;
                if(oldDetail.TrtmtID != null)
                {
                    treatment = _newContextHelper.GetInventoryTreatment(oldDetail.TrtmtID.Value);
                }

                if(treatment == null)
                {
                    Log(new CallbackParameters(CallbackReason.TreatmentNotLoaded) { Detail = oldDetail });
                    continue;
                }

                items.Add(new ContractItem
                    {
                        KDetailID = oldDetail.KDetailID,
                        ContractYear = newContract.ContractYear,
                        ContractSequence = newContract.ContractSequence,
                        ContractItemSequence = sequence++,

                        ChileProductId = chileProduct.Id,
                        PackagingProductId = packaging.Id,
                        TreatmentId = treatment.Id,
                        UseCustomerSpec = (oldDetail.Spec ?? "").Replace(" ", "").ToUpper() == "CUST",
                        
                        CustomerProductCode = oldDetail.CustProductCode,
                        Quantity = (int) (oldDetail.Quantity ?? 0),
                        PriceBase = (double) (oldDetail.Price ?? 0),
                        PriceFreight = (double) (oldDetail.FreightP ?? 0),
                        PriceTreatment = (double) (oldDetail.TrtnmntP ?? 0),
                        PriceWarehouse = (double) (oldDetail.WHCostP ?? 0),
                        PriceRebate = (double) (oldDetail.Rebate ?? 0)
                    });
            }

            newContract.ContractItems = items;
        }

        private static ContractType? GetContractType(string contractType)
        {
            if(!string.IsNullOrWhiteSpace(contractType))
            {
                switch(contractType.Replace(" ", "").ToUpper())
                {
                    case "CONTRACT": return ContractType.Contract;
                    case "SPOT": return ContractType.Spot;
                    case "INTERMIN": return ContractType.Interim;

                    case "CONTRACTQUOTE":
                    case "QUOTE":
                        return ContractType.Quote;
                }
            }
            return null;
        }

        private static ContractStatus? GetContractStatus(string contractStatus)
        {
            if(!string.IsNullOrWhiteSpace(contractStatus))
            {
                switch(contractStatus.Replace(" ", "").ToUpper())
                {
                    case "PENDING": return ContractStatus.Pending;
                    case "CONFIRMED": return ContractStatus.Confirmed;
                    case "REJECTED": return ContractStatus.Rejected;
                    case "COMPLETED": return ContractStatus.Completed;
                }
            }
            return null;
        }

        public static IEnumerable<tblContractDTO> SelectContractsToLoad(ObjectContext objectContext)
        {
            return objectContext.CreateObjectSet<tblContract>()
                .AsNoTracking()
                .OrderByDescending(c => c.SerializedKey != null)
                .Select(c => new tblContractDTO
                    {
                        Serialized = c.Serialized,
                        SerializedKey = c.SerializedKey,
                        ContractID = c.ContractID,
                        EmployeeId = c.EmployeeID,
                        EntryDate = c.EntryDate,
                        KDate = c.KDate,
                        Contact_IA = c.Contact_IA,
                        Company_IA = c.Company_IA,
                        Address1_IA = c.Address1_IA,
                        Address2_IA = c.Address2_IA,
                        Address3_IA = c.Address3_IA,
                        City_IA = c.City_IA,
                        State_IA = c.State_IA,
                        Zip_IA = c.Zip_IA,
                        Country_IA = c.Country_IA,
                        Broker = c.Broker,
                        PmtTerms = c.PmtTerms,
                        PONum = c.PONum,
                        BegDate = c.BegDate,
                        EndDate = c.EndDate,
                        KType = c.KType,
                        KStatus = c.KStatus,
                        FOB = c.FOB,
                        Comments = c.Comments,
                        Notes2Print = c.Notes2Print,
                        WHID = c.WHID,

                        Details = c.tblContractDetails.Select(d => new tblContractDetailDTO
                            {
                                ContractID = d.ContractID,
                                KDetailID = d.KDetailID,
                                ProdID = d.ProdID,
                                PkgID = d.PkgID,
                                Quantity = d.Quantity,
                                TrtmtID = d.TrtmtID,
                                Price = d.Price,
                                FreightP = d.FreightP,
                                TrtnmntP = d.TrtnmntP,
                                WHCostP = d.WHCostP,
                                Rebate = d.Rebate,
                                CustProductCode = d.CustProductCode,
                                Spec = d.Spec,
                                TtlWgt = d.TtlWgt
                            })
                    });
        }

        public class tblContractDTO
        {
            public string Serialized { get; set; }
            public string SerializedKey { get; set; }
            public int ContractID { get; set; }
            public int? EmployeeId { get; set; }
            public DateTime? EntryDate { get; set; }
            public DateTime? KDate { get; set; }
            public DateTime? BegDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Contact_IA { get; set; }
            public string Company_IA { get; set; }
            public string Address1_IA { get; set; }
            public string Address2_IA { get; set; }
            public string Address3_IA { get; set; }
            public string City_IA { get; set; }
            public string State_IA { get; set; }
            public string Zip_IA { get; set; }
            public string Country_IA { get; set; }
            public string Broker { get; set; }
            public string PmtTerms { get; set; }
            public string PONum { get; set; }
            public string KType { get; set; }
            public string KStatus { get; set; }
            public string FOB { get; set; }
            public string Comments { get; set; }
            public string Notes2Print { get; set; }
            public int? WHID { get; set; }

            public IEnumerable<tblContractDetailDTO> Details { get; set; }
        }

        public class tblContractDetailDTO
        {
            public int? ContractID { get; set; }
            public DateTime KDetailID { get; set; }
            public int? ProdID { get; set; }
            public int? PkgID { get; set; }
            public decimal? Quantity { get; set; }
            public int? TrtmtID { get; set; }
            public decimal? Price { get; set; }
            public decimal? FreightP { get; set; }
            public decimal? TrtnmntP { get; set; }
            public decimal? WHCostP { get; set; }
            public decimal? Rebate { get; set; }
            public string CustProductCode { get; set; }
            public string Spec { get; set; }
            public decimal? TtlWgt { get; set; }
        }

        public class ContractLoad : Contract
        {
            public bool RequiresKeySync = false;
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,

            UnderterminedTimestamp,
            NullContractDate,
            CustomerNotLoaded,
            ContactNotLoaded,
            BrokerNotLoaded,
            WarehouseNotLoaded,
            DefaultEmployee,
            DefaultRinconWarehouse,
            UndeterminedContractType,
            UndeterminedContractStatus,
            ChileProductNotLoaded,
            PackagingProductNotLoaded,
            TreatmentNotLoaded
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            protected override CallbackReason ExceptionReason { get { return ContractEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return ContractEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return ContractEntityObjectMother.CallbackReason.StringTruncated; } }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case ContractEntityObjectMother.CallbackReason.Exception:
                        return ReasonCategory.Error;
                    
                    case ContractEntityObjectMother.CallbackReason.UnderterminedTimestamp:
                    case ContractEntityObjectMother.CallbackReason.NullContractDate:
                    case ContractEntityObjectMother.CallbackReason.CustomerNotLoaded:
                    case ContractEntityObjectMother.CallbackReason.ContactNotLoaded:
                    case ContractEntityObjectMother.CallbackReason.BrokerNotLoaded:
                    case ContractEntityObjectMother.CallbackReason.WarehouseNotLoaded:
                    case ContractEntityObjectMother.CallbackReason.UndeterminedContractType:
                    case ContractEntityObjectMother.CallbackReason.UndeterminedContractStatus:
                    case ContractEntityObjectMother.CallbackReason.ChileProductNotLoaded:
                    case ContractEntityObjectMother.CallbackReason.PackagingProductNotLoaded:
                    case ContractEntityObjectMother.CallbackReason.TreatmentNotLoaded:
                        return ReasonCategory.RecordSkipped;

                    case ContractEntityObjectMother.CallbackReason.DefaultEmployee:
                    case ContractEntityObjectMother.CallbackReason.DefaultRinconWarehouse:
                        return ReasonCategory.Informational;
                }

                return base.DerivedGetReasonCategory(reason);
            }

            public tblContractDTO OldContract { get; set; }

            public tblContractDetailDTO Detail { get; set; }
        }
    }
}