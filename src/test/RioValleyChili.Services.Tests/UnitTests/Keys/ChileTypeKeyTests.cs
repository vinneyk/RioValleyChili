using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ChileTypeKeyTests : KeyTestsBase<ChileTypeKey, IChileTypeKey>
    {
        private const int EXPECTED_CHILE_PRODUCT_ID = 1;
        private const string EXPECTED_STRING_VALUE = "1";
        private const string VALID_PARSE_INPUT = "1";
        private const string INVALID_PARSE_INPUT = "I AM BATMAN!";

        #region Overrides of KeyTestsBase<ChileTypeKey,ChileTypeKey>

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

        protected override void SetUpValidMock(Mock<IChileTypeKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ChileTypeKey_ChileTypeId).Returns(EXPECTED_CHILE_PRODUCT_ID);
        }

        protected override ChileTypeKey BuildKey(IChileTypeKey keyInterface)
        {
            return new ChileTypeKey(keyInterface);
        }

        protected override void AssertValidKey(IChileTypeKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_CHILE_PRODUCT_ID, resultingKey.ChileTypeKey_ChileTypeId);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = ChileTypeKey.Null;
            var n2 = ChileTypeKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}