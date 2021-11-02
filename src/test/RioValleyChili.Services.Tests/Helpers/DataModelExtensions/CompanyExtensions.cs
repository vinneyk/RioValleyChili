using System;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class CompanyExtensions
    {
        internal static Company EmptyItems(this Company company)
        {
            if(company == null) { throw new ArgumentNullException("company"); }

            company.Contacts = null;

            return company;
        }

        internal static Company SetCompanyTypes(this Company company, params CompanyType[] companyTypes)
        {
            if(company == null) { throw new ArgumentNullException("company"); }
            company.CompanyTypes = companyTypes.Select(c => new CompanyTypeRecord
                {
                    CompanyId = company.Id,
                    CompanyType = (int) c
                }).ToList();

            return company;
        }

        internal static void AssertEqual(this Company company, ICompanySummaryReturn companySummaryReturn)
        {
            if(company == null) { throw new ArgumentNullException("company"); }
            if(companySummaryReturn == null) { throw new ArgumentNullException("companySummaryReturn"); }

            Assert.AreEqual(new CompanyKey(company).KeyValue, companySummaryReturn.CompanyKey);
            Assert.AreEqual(company.Name, companySummaryReturn.Name);
            if(company.CompanyTypes == null)
            {
                if(companySummaryReturn.CompanyTypes == null)
                {
                    Assert.IsEmpty(companySummaryReturn.CompanyTypes);
                }
            }
            else
            {
                Assert.IsTrue(company.CompanyTypes.All(t => companySummaryReturn.CompanyTypes.Count(s => s == t.CompanyTypeEnum) == 1));
            }

            Assert.AreEqual(company.Active, companySummaryReturn.Active);
        }

        internal static void AssertEqual(this Company company, ICompanyDetailReturn companyDetailReturn)
        {
            if(company == null) { throw new ArgumentNullException("company"); }
            if(companyDetailReturn == null) { throw new ArgumentNullException("companyDetailReturn"); }

            company.AssertEqual((ICompanySummaryReturn)companyDetailReturn);
            company.Customer.AssertEqual(companyDetailReturn.Customer);
        }

        internal static void AssertEqual(this Company expected, ICompanyHeaderReturn result)
        {
            if(expected == null)
            {
                Assert.IsNull(result);
                return;
            }

            Assert.AreEqual(expected.ToCompanyKey().KeyValue, result.CompanyKey);
            Assert.AreEqual(expected.Name, result.Name);
        }
    }
}