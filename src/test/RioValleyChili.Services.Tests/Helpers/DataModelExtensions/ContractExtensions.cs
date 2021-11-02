using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ContractExtensions
    {
        internal static Contract ConstrainByKeys(this Contract contract, ICustomerKey customerKey, ICompanyKey brokerKey = null)
        {
            if(contract == null) { throw new ArgumentNullException("contract"); }

            if(customerKey != null)
            {
                contract.Customer = null;
                contract.CustomerId = customerKey.CustomerKey_Id;
            }

            if(brokerKey != null)
            {
                contract.Broker = null;
                contract.BrokerId = brokerKey.CompanyKey_Id;
            }

            return contract;
        }

        internal static Contract SetTermDates(this Contract contract, DateTime termBegin, DateTime termEnd)
        {
            if(contract == null) { throw new ArgumentNullException("contract"); }

            contract.TermBegin = termBegin;
            contract.TermEnd = termEnd;

            return contract;
        }

        internal static void AssertEqual(this Contract expected, ICustomerContractSummaryReturn result)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }
            if(result == null) { throw new ArgumentNullException("result"); }

            Assert.AreEqual(new ContractKey(expected).KeyValue, result.CustomerContractKey);
            Assert.AreEqual(expected.ContractDate, result.ContractDate);
            Assert.AreEqual(expected.TermBegin, result.TermBegin);
            Assert.AreEqual(expected.TermEnd, result.TermEnd);
            Assert.AreEqual(expected.PaymentTerms, result.PaymentTerms);
            Assert.AreEqual(expected.ContractType, result.ContractType);
            Assert.AreEqual(expected.ContractStatus, result.ContractStatus);
            Assert.AreEqual(expected.ContactName, result.ContactName);
            Assert.AreEqual(expected.FOB, result.FOB);

            expected.Customer.Company.AssertEqual(result.Customer);
            expected.Broker.AssertEqual(result.Broker);
        }
    }
}