using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class AttributeNameKeyTests : KeyTestsBase<AttributeNameKey, IAttributeNameKey>
    {
        private const string EXPECTED_ATTRIBUTENAME_KEY = "Asta";
        private const string VALID_ATTRIBUTENAME_STRING = "Asta";
        private const string VALID_PARSE_INPUT = "Asta";
        private const string INVALID_PARSE_INPUT = null;

        #region Overrides of KeyTestsBase<AttributeNameKey, IAttributeNameKey>

        protected override string ExpectedStringValue
        {
            get { return VALID_ATTRIBUTENAME_STRING; }
        }

        protected override string ValidParseInput
        {
            get { return VALID_PARSE_INPUT; }
        }

        protected override string InvalidParseInput
        {
            get { return INVALID_PARSE_INPUT; }
        }

        protected override void SetUpValidMock(Mock<IAttributeNameKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.AttributeNameKey_ShortName).Returns(EXPECTED_ATTRIBUTENAME_KEY);
        }

        protected override AttributeNameKey BuildKey(IAttributeNameKey keyInterface)
        {
            return new AttributeNameKey(keyInterface);
        }

        protected override void AssertValidKey(IAttributeNameKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_ATTRIBUTENAME_KEY, resultingKey.AttributeNameKey_ShortName);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = AttributeNameKey.Null;
            var n2 = AttributeNameKey.Null;

            Assert.IsTrue(n1 == n2);
        }
        
    }
}