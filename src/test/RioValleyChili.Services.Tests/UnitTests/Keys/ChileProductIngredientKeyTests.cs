using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ChileProductIngredientKeyTests : KeyTestsBase<ChileProductIngredientKey, IChileProductIngredientKey>
    {
        private const int EXPECTED_CHILE_PRODUCT_ID = 10;
        private const int EXPECTED_ADDITIVE_TYPE_ID = 12;
        private const string EXPECTED_STRING_VALUE = "10-12";
        private const string VALID_PARSE_INPUT = "10-12";
        private const string INVALID_PARSE_INPUT = "Holy Toledo, Batman!";

        #region Overrides of KeyTestsBase<ChileProductIngredientKey,IChileProductIngredientKey>

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

        protected override void SetUpValidMock(Mock<IChileProductIngredientKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ChileProductIngredientKey_AdditiveTypeId).Returns(EXPECTED_ADDITIVE_TYPE_ID);
            mockKeyInterface.SetupGet(m => m.ChileProductIngredientKey_ChileProductId).Returns(EXPECTED_CHILE_PRODUCT_ID);
        }

        protected override ChileProductIngredientKey BuildKey(IChileProductIngredientKey keyInterface)
        {
            return new ChileProductIngredientKey(keyInterface);
        }

        protected override void AssertValidKey(IChileProductIngredientKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_ADDITIVE_TYPE_ID, resultingKey.ChileProductIngredientKey_AdditiveTypeId);
            Assert.AreEqual(EXPECTED_CHILE_PRODUCT_ID, resultingKey.ChileProductIngredientKey_ChileProductId);
        }

        #endregion
        
        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = ChileProductIngredientKey.Null;
            var n2 = ChileProductIngredientKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}