using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class AdditiveTypeKeyTests : KeyTestsBase<AdditiveTypeKey, IAdditiveTypeKey>
    {
        private const int EXPECTED_ADDITIVE_TYPE_ID = 11;
        private const string EXPECTED_STRING_VALUE = "11";
        private const string VALID_PARSE_INPUT = "11";
        private const string INVALID_PARSE_INPUT = "Happy Days!";

        #region Overrides of KeyTestsBase<AdditiveTypeKeyReturn,IAdditiveTypeKey>

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

        protected override void SetUpValidMock(Mock<IAdditiveTypeKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.AdditiveTypeKey_AdditiveTypeId).Returns(EXPECTED_ADDITIVE_TYPE_ID);
        }

        protected override AdditiveTypeKey BuildKey(IAdditiveTypeKey keyInterface)
        {
            return new AdditiveTypeKey(keyInterface);
        }

        protected override void AssertValidKey(IAdditiveTypeKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_ADDITIVE_TYPE_ID, resultingKey.AdditiveTypeKey_AdditiveTypeId);
        }

        #endregion
        
        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = AdditiveTypeKey.Null;
            var n2 = AdditiveTypeKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}