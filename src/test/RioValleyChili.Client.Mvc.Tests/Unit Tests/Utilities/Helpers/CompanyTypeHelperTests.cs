using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Tests.Helpers;

namespace RioValleyChili.Tests.Unit_Tests.Utilities.Helpers
{
    [TestFixture]
    public class CompanyTypeHelperTests
    {
        private readonly IFixture _fixture = AutoFixtureHelper.BuildFixture();
        
        #region IsVendorDelegate tests

        [Test]
        public void IsVendorDelegateFiltersCustomerTypesFromList()
        {
            // Arrange
            var input = new List<ICompanySummaryReturn>
                            {
                                BuildCompany(CompanyType.Customer),
                                BuildCompany(CompanyType.Freight),
                                BuildCompany(CompanyType.Supplier),
                            };

            // Act
            var actualResults = input.Where(CompanyTypeHelper.IsVendor).ToList();

            // Assert
            Assert.False(actualResults.Any(c => c.CompanyTypes.Contains(CompanyType.Customer)));
            Assert.IsTrue(actualResults.Count == 2);
        }

        [Test]
        public void IsVendorDelegateFiltersBrokerTypesFromList()
        {
            // Arrange
            var input = new List<ICompanySummaryReturn>
                            {
                                BuildCompany(CompanyType.Broker),
                                BuildCompany(CompanyType.Dehydrator),
                                BuildCompany(CompanyType.TreatmentFacility),
                            };

            // Act
            var actualResults = input.Where(CompanyTypeHelper.IsVendor).ToList();

            // Assert
            Assert.False(actualResults.Any(c => c.CompanyTypes.Contains(CompanyType.Broker)));
            Assert.IsTrue(actualResults.Count == 2);
        }

        [Test]
        public void IsVendorDelegateDoesNotFilterCompaniesWhichAreBothCustomersAndVendors()
        {
            // Arrange
            var input = new List<ICompanySummaryReturn>
                            {
                                BuildCompany(new[] {CompanyType.Customer, CompanyType.Supplier}),
                                BuildCompany(CompanyType.Dehydrator),
                                BuildCompany(CompanyType.TreatmentFacility),
                            };

            // Act
            var actualResults = input.Where(CompanyTypeHelper.IsVendor).ToList();

            // Assert
            Assert.AreEqual(input, actualResults);
        }

        [Test]
        public void IsVendorDelegateFiltersCompaniesWhichAreBothBrokersAndVendors()
        {
            // Arrange
            var input = new List<ICompanySummaryReturn>
                            {
                                BuildCompany(new[] {CompanyType.Customer, CompanyType.Broker}),
                                BuildCompany(CompanyType.Dehydrator),
                                BuildCompany(CompanyType.TreatmentFacility),
                            };

            // Act
            var actualResults = input.Where(CompanyTypeHelper.IsVendor).ToList();

            // Assert
            Assert.AreEqual(input.Skip(1), actualResults);
        }

        [Test]
        public void IsVendorDelegateDoesNotFilterCompaniesWhichAreBothBrokersAndVendors()
        {
            // Arrange
            var input = new List<ICompanySummaryReturn>
                            {
                                BuildCompany(new[] {CompanyType.Broker, CompanyType.Supplier}),
                                BuildCompany(CompanyType.Dehydrator),
                                BuildCompany(CompanyType.TreatmentFacility),
                            };

            // Act
            var actualResults = input.Where(CompanyTypeHelper.IsVendor).ToList();

            // Assert
            Assert.AreEqual(input, actualResults);
        }

        #endregion

        [Test]
        public void AllCompanyTypesAreHandled()
        {
            // Arrange
            var companyTypeValues = Enum.GetValues(typeof (CompanyType)).Cast<CompanyType>().ToList();
            
            // Act
            var vendorTypes = companyTypeValues.Where(CompanyTypeHelper.KnownVendorTypes.Contains);
            var customerTypes = companyTypeValues.Where(CompanyTypeHelper.KnownCustomerTypes.Contains);

            // Assert
            Assert.AreEqual(companyTypeValues.Count, vendorTypes.Count() + customerTypes.Count());
        }

        [Test]
        public void CompanyTypesDoNotExistAsKnownCustomersAndVendors()
        {
            // Arrange
            var vendorTypes = CompanyTypeHelper.KnownVendorTypes;
            var customerTypes = CompanyTypeHelper.KnownCustomerTypes;

            // Act
            var duplicated = vendorTypes.Intersect(customerTypes);

            // Assert
            Assert.IsEmpty(duplicated);
        }

        #region private members

        private ICompanySummaryReturn BuildCompany(CompanyType companyType)
        {
            return BuildCompany(new[] {companyType});
        }

        private ICompanySummaryReturn BuildCompany(IEnumerable<CompanyType> companyTypes)
        {
            return _fixture.Build<TestableCompanySummary>()
                           .With(m => m.CompanyTypes, companyTypes)
                           .Create();
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class TestableCompanySummary : ICompanySummaryReturn
            // ReSharper restore ClassNeverInstantiated.Local
        {
            public string CompanyKey { get; set; }
            public string ParentCompanyKey { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
            public int Depth { get; set; }
            public IEnumerable<CompanyType> CompanyTypes { get; set; }
        }

        #endregion

    }
}
