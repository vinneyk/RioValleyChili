using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class CompanyTypeRecordKeyTests : KeyTestsBase<CompanyTypeRecordKey, ICompanyTypeRecordKey>
    {
        private const int expectedCompanyId = 2;
        private const int expectedCompanyType = 3;
        private const string validString = "2-3";
        private const string invalidString = "one-2-5";

        #region Overrides of KeyTestsBase<CompanyTypeRecordKey, ICompanyTypeRecordKey>

        protected override string ExpectedStringValue
        {
            get { return validString; }
        }

        protected override string ValidParseInput
        {
            get { return validString; }
        }

        protected override string InvalidParseInput
        {
            get { return invalidString; }
        }

        protected override void SetUpValidMock(Mock<ICompanyTypeRecordKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.CompanyKey_Id).Returns(expectedCompanyId);
            mockKeyInterface.SetupGet(m => m.CompanyType).Returns(expectedCompanyType);
        }

        protected override CompanyTypeRecordKey BuildKey(ICompanyTypeRecordKey keyInterface)
        {
            return new CompanyTypeRecordKey(keyInterface);
        }

        protected override void AssertValidKey(ICompanyTypeRecordKey resultingKey)
        {
            Assert.AreEqual(expectedCompanyId, resultingKey.CompanyKey_Id);
            Assert.AreEqual(expectedCompanyType, resultingKey.CompanyType);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = CompanyTypeRecordKey.Null;
            var n2 = CompanyTypeRecordKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}