using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class AdditiveProductKeyTests : KeyTestsBase<AdditiveProductKey, IAdditiveProductKey>
    {
        private const int EXPECTED_PRODUCT_KEY = 123;
        private const string VALID_PRODUCT_KEY_STRING = "123";
        private const string VALID_PARSE_INPUT = "123";
        private const string INVALID_PARSE_INPUT = "Hello friend!";

        #region Overrides of KeyTestsBase<AdditiveProductKey, IAdditiveProductKey>

        protected override string ExpectedStringValue
        {
            get { return VALID_PRODUCT_KEY_STRING; }
        }

        protected override string ValidParseInput
        {
            get { return VALID_PARSE_INPUT; }
        }

        protected override string InvalidParseInput
        {
            get { return INVALID_PARSE_INPUT; }
        }

        protected override void SetUpValidMock(Mock<IAdditiveProductKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.AdditiveProductKey_Id).Returns(EXPECTED_PRODUCT_KEY);
        }

        protected override AdditiveProductKey BuildKey(IAdditiveProductKey keyInterface)
        {
            return new AdditiveProductKey(keyInterface);
        }

        protected override void AssertValidKey(IAdditiveProductKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_PRODUCT_KEY, resultingKey.AdditiveProductKey_Id);
        }
        
        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = AdditiveProductKey.Null;
            var n2 = AdditiveProductKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}