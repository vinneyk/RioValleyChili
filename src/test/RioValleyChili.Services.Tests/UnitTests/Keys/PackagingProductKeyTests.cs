using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class PackagingProductKeyTests : KeyTestsBase<PackagingProductKey, IPackagingProductKey>
    {
        private const int EXPECTED_PACKAGING_PRODUCT_ID = 99;
        private const string EXPECTED_STRING_VALUE = "99";
        private const string VALID_PARSE_INPUT = "99";
        private const string INVALID_PARSE_INPUT = "no bueno";

        #region Overrides of KeyTestsBase<PackagingProductKey,IPackagingProductKey>

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

        protected override void SetUpValidMock(Mock<IPackagingProductKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.PackagingProductKey_ProductId).Returns(EXPECTED_PACKAGING_PRODUCT_ID);
        }

        protected override PackagingProductKey BuildKey(IPackagingProductKey keyInterface)
        {
            return new PackagingProductKey(keyInterface);
        }

        protected override void AssertValidKey(IPackagingProductKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_PACKAGING_PRODUCT_ID, resultingKey.PackagingProductKey_ProductId);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = PackagingProductKey.Null;
            var n2 = PackagingProductKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}