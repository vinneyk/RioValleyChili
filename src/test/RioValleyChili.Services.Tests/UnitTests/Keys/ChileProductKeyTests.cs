using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ChileProductKeyTests : KeyTestsBase<ChileProductKey, IChileProductKey>
    {
        private const int EXPECTED_CHILE_PRODUCT_ID = 113;
        private const string EXPECTED_STRING_VALUE = "113";
        private const string VALID_PARSE_INPUT = "113";
        private const string INVALID_PARSE_INPUT = "I am chile :)";

        #region Overrides of KeyTestsBase<ChileProductKey,IChileProductKey>

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

        protected override void SetUpValidMock(Mock<IChileProductKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ChileProductKey_ProductId).Returns(EXPECTED_CHILE_PRODUCT_ID);
        }

        protected override ChileProductKey BuildKey(IChileProductKey keyInterface)
        {
            return new ChileProductKey(keyInterface);
        }

        protected override void AssertValidKey(IChileProductKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_CHILE_PRODUCT_ID, resultingKey.ChileProductKey_ProductId);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = ChileProductKey.Null;
            var n2 = ChileProductKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}