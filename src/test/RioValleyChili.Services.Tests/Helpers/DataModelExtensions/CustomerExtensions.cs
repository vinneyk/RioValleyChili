using System;
using System.Collections.Generic;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class CustomerExtensions
    {
        internal static Customer EmptyItems(this Customer customer)
        {
            if(customer == null) { throw new ArgumentNullException("customer"); }
            
            customer.Company.Contacts = null;
            customer.ProductCodes = null;
            customer.ProductSpecs = null;
            customer.Contracts = null;
            customer.Orders = null;

            return customer;
        }

        internal static Customer SetBroker(this Customer customer, ICompanyKey brokerKey)
        {
            if(customer == null) { throw new ArgumentNullException("customer"); }
            if(brokerKey == null) { throw new ArgumentNullException("brokerKey"); }

            customer.Broker = null;
            customer.BrokerId = brokerKey.CompanyKey_Id;

            return customer;
        }

        internal static void AssertEqual(this Customer expected, ICustomerCompanyReturn result)
        {
            if(expected == null)
            {
                Assert.IsNull(result);
                return;
            }

            expected.Broker.AssertEqual(result.Broker);
            (expected.Notes ?? new List<CustomerNote>()).AssertEquivalent(result.CustomerNotes,
                e => e.ToCustomerNoteKey().KeyValue, r => r.NoteKey,
                (e, r) => e.AssertEqual(r));
        }
    }
}