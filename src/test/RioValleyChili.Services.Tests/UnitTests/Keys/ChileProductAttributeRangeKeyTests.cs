using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ChileProductAttributeRangeKeyTests : KeyTestsBase<ChileProductAttributeRangeKey, IChileProductAttributeRangeKey>
    {
        private const int EXPECTED_CHILEPRODUCTID = 123;
        private const string EXPECTED_ATTRIBUTESHORTNAME = "Asta";
        private const string VALID_KEY_STRING = "123-Asta";
        private const string INVALID_PARSE_INPUT = "123-Oh-Me-Oh-My";

        #region Overrides of KeyTestsBase<ChileProductAttributeRangeKey, IChileProductAttributeRangeKey>

        protected override string ExpectedStringValue
        {
            get { return VALID_KEY_STRING; }
        }

        protected override string ValidParseInput
        {
            get { return VALID_KEY_STRING; }
        }

        protected override string InvalidParseInput
        {
            get { return INVALID_PARSE_INPUT; }
        }

        protected override void SetUpValidMock(Mock<IChileProductAttributeRangeKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ChileProductKey_ProductId).Returns(EXPECTED_CHILEPRODUCTID);
            mockKeyInterface.SetupGet(m => m.AttributeNameKey_ShortName).Returns(EXPECTED_ATTRIBUTESHORTNAME);
        }

        protected override ChileProductAttributeRangeKey BuildKey(IChileProductAttributeRangeKey keyInterface)
        {
            return new ChileProductAttributeRangeKey(keyInterface);
        }

        protected override void AssertValidKey(IChileProductAttributeRangeKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_CHILEPRODUCTID, resultingKey.ChileProductKey_ProductId);
            Assert.AreEqual(EXPECTED_ATTRIBUTESHORTNAME, resultingKey.AttributeNameKey_ShortName);
        }

        #endregion
    }
}