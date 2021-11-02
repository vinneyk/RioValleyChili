using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class LotAllowancesEntityObjectMother : EntityMotherLogBase<LotAllowancesEntityObjectMother.Results, LotAllowancesEntityObjectMother.CallbackParameters>
    {
        public LotAllowancesEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
        }

        protected override IEnumerable<Results> BirthRecords()
        {
            _loadCount.Reset();

            foreach(var lot in SelectLotsToLoad(OldContext))
            {
                var serialized = SerializableLot.Deserialize(lot.Serialized);
                var results = LoadAllowances(lot.Lot, serialized);

                _loadCount.AddLoaded(EntityTypes.LotCustomerAllowance, (uint)(results.CustomerAllowances.Count));
                _loadCount.AddLoaded(EntityTypes.LotContractAllowance, (uint)(results.ContractAllowances.Count));
                _loadCount.AddLoaded(EntityTypes.LotCustomerOrderAllowance, (uint)(results.CustomerOrderAllowances.Count));

                yield return results;
            }
        }

        public enum EntityTypes
        {
            LotCustomerAllowance,
            LotContractAllowance,
            LotCustomerOrderAllowance
        }

        private static IEnumerable<LotDTO> SelectLotsToLoad(ObjectContext objectContext)
        {
            return objectContext.CreateObjectSet<tblLot>().AsNoTracking()
                .Where(l => l.Serialized != null)
                .Select(l => new LotDTO
                    {
                        Lot = l.Lot,
                        Serialized = l.Serialized
                    })
                .ToList();
        }

        private class LotDTO
        {
            public int Lot { get; set; }
            public string Serialized { get; set; }
        }

        private readonly NewContextHelper _newContextHelper;
        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        private Results LoadAllowances(int lotNumber, SerializableLot deserializedLot)
        {
            var results = new Results();
            var lotKey = LotNumberParser.ParseLotNumber(lotNumber);

            foreach(var customer in deserializedLot.CustomerAllowances ?? new List<SerializableLot.CustomerAllowance>())
            {
                var company = _newContextHelper.GetCompany(customer.CompanyName, CompanyType.Customer);
                if(company == null)
                {
                    Log(new CallbackParameters(lotNumber, CallbackReason.CustomerNotLoaded)
                        {
                            CustomerAllowance = customer
                        });
                }
                else
                {
                    results.CustomerAllowances.Add(new LotCustomerAllowance
                        {
                            LotDateCreated = lotKey.LotKey_DateCreated,
                            LotDateSequence = lotKey.LotKey_DateSequence,
                            LotTypeId = lotKey.LotKey_LotTypeId,
                            CustomerId = company.Id
                        });
                }
            }

            foreach(var contract in deserializedLot.ContractAllowances ?? new List<SerializableLot.ContractAllowance>())
            {
                var contractKey = _newContextHelper.GetContractKey(contract.ContractId);
                if(contractKey == null)
                {
                    Log(new CallbackParameters(lotNumber, CallbackReason.ContractNotLoaded)
                        {
                            ContractAllowance = contract
                        });
                }
                else
                {
                    results.ContractAllowances.Add(new LotContractAllowance
                        {
                            LotDateCreated = lotKey.LotKey_DateCreated,
                            LotDateSequence = lotKey.LotKey_DateSequence,
                            LotTypeId = lotKey.LotKey_LotTypeId,
                            ContractYear = contractKey.ContractKey_Year,
                            ContractSequence = contractKey.ContractKey_Sequence
                        });
                }
            }

            foreach(var customerOrder in deserializedLot.CustomerOrderAllowances ?? new List<SerializableLot.CustomerOrderAllowance>())
            {
                var order = _newContextHelper.GetInventoryShipmentOrder(customerOrder.OrderNum, InventoryShipmentOrderTypeEnum.SalesOrder);
                if(order == null)
                {
                    Log(new CallbackParameters(lotNumber, CallbackReason.SalesOrderNotLoaded)
                        {
                            CustomerOrderAllowance = customerOrder
                        });
                }
                else
                {
                    results.CustomerOrderAllowances.Add(new LotSalesOrderAllowance
                        {
                            LotDateCreated = lotKey.LotKey_DateCreated,
                            LotDateSequence = lotKey.LotKey_DateSequence,
                            LotTypeId = lotKey.LotKey_LotTypeId,
                            SalesOrderDateCreated = order.DateCreated,
                            SalesOrderSequence = order.Sequence
                        });
                }
            }

            return results;
        }

        public class Results
        {
            public List<LotCustomerAllowance> CustomerAllowances = new List<LotCustomerAllowance>();
            public List<LotContractAllowance> ContractAllowances = new List<LotContractAllowance>();
            public List<LotSalesOrderAllowance> CustomerOrderAllowances = new List<LotSalesOrderAllowance>();
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,
            CustomerNotLoaded,
            ContractNotLoaded,
            SalesOrderNotLoaded
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public int Lot { get; set; }
            public SerializableLot.CustomerAllowance CustomerAllowance { get; set; }
            public SerializableLot.ContractAllowance ContractAllowance { get; set; }
            public SerializableLot.CustomerOrderAllowance CustomerOrderAllowance { get; set; }

            protected override CallbackReason ExceptionReason { get { return LotAllowancesEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return LotAllowancesEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return LotAllowancesEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() {}
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) {}
            public CallbackParameters(string summaryMessage) : base(summaryMessage) {}

            public CallbackParameters(int lot, CallbackReason callbackReason)
                : base(callbackReason)
            {
                Lot = lot;
            }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case LotAllowancesEntityObjectMother.CallbackReason.Exception:
                        return ReasonCategory.Error;

                    case LotAllowancesEntityObjectMother.CallbackReason.CustomerNotLoaded:
                    case LotAllowancesEntityObjectMother.CallbackReason.ContractNotLoaded:
                    case LotAllowancesEntityObjectMother.CallbackReason.SalesOrderNotLoaded:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}