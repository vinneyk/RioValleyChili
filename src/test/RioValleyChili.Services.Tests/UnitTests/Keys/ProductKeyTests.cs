using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ProductKeyTests : KeyTestsBase<ProductKey, IProductKey>
    {
        private const int EXPECTED_PRODUCT_ID = 123;
        private const string VALID_PRODUCT_KEY_STRING = "123";
        private const string VALID_PARSE_INPUT = "123";
        private const string INVALID_PARSE_INPUT = "Hello friend!";

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

        protected override void SetUpValidMock(Mock<IProductKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ProductKey_ProductId).Returns(EXPECTED_PRODUCT_ID);
        }

        protected override ProductKey BuildKey(IProductKey keyInterface)
        {
            return new ProductKey(keyInterface);
        }

        protected override void AssertValidKey(IProductKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_PRODUCT_ID, resultingKey.ProductKey_ProductId);
        }

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = PackagingProductKey.Null;
            var n2 = PackagingProductKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}