using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class CompanyKeyTests : KeyTestsBase<CompanyKey, ICompanyKey>
    {
        private const int EXPECTED_CHILE_PRODUCT_ID = 2;
        private const string EXPECTED_STRING_VALUE = "2";
        private const string VALID_PARSE_INPUT = "2";
        private const string INVALID_PARSE_INPUT = "no-no-no, don't waste my water";

        #region Overrides of KeyTestsBase<CompanyKey, ICompanyKey>

        protected override string ExpectedStringValue
        {
            get { return EXPECTED_STRING_VALUE; }
        }

        protected override string ValidParseInput
        {
            get { return VALID_PARSE_INPUT; }
        }

        protected override string InvalidParseInput
        {
            get { return INVALID_PARSE_INPUT; }
        }

        protected override void SetUpValidMock(Mock<ICompanyKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.CompanyKey_Id).Returns(EXPECTED_CHILE_PRODUCT_ID);
        }

        protected override CompanyKey BuildKey(ICompanyKey keyInterface)
        {
            return new CompanyKey(keyInterface);
        }

        protected override void AssertValidKey(ICompanyKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_CHILE_PRODUCT_ID, resultingKey.CompanyKey_Id);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = CompanyKey.Null;
            var n2 = CompanyKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}