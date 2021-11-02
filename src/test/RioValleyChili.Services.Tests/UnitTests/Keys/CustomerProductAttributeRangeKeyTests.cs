using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class CustomerProductAttributeRangeKeyTests : KeyTestsBase<CustomerProductAttributeRangeKey, ICustomerProductAttributeRangeKey>
    {
        private const int expectedCustomerId = 33;
        private const int expectedChileProductId = 2;
        private const string expectedAttributeName = "Asta";
        private const string expectedString = "33-2-Asta";
        private const string invalidParse = "Death by bad key.";

        #region Overrides of KeyTestsBase<CustomerProductAttributeSpecKey, ICustomerProductAttributeSpecKey>

        protected override string ExpectedStringValue
        {
            get { return expectedString; }
        }

        protected override string ValidParseInput
        {
            get { return expectedString; }
        }

        protected override string InvalidParseInput
        {
            get { return invalidParse; }
        }

        protected override void SetUpValidMock(Mock<ICustomerProductAttributeRangeKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.CustomerKey_Id).Returns(expectedCustomerId);
            mockKeyInterface.SetupGet(m => m.ChileProductKey_ProductId).Returns(expectedChileProductId);
            mockKeyInterface.SetupGet(m => m.AttributeNameKey_ShortName).Returns(expectedAttributeName);
        }

        protected override CustomerProductAttributeRangeKey BuildKey(ICustomerProductAttributeRangeKey keyInterface)
        {
            return new CustomerProductAttributeRangeKey(keyInterface);
        }

        protected override void AssertValidKey(ICustomerProductAttributeRangeKey resultingKey)
        {
            Assert.AreEqual(expectedCustomerId, resultingKey.CustomerKey_Id);
            Assert.AreEqual(expectedChileProductId, resultingKey.ChileProductKey_ProductId);
            Assert.AreEqual(expectedAttributeName, resultingKey.AttributeNameKey_ShortName);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = CustomerProductAttributeRangeKey.Null;
            var n2 = CustomerProductAttributeRangeKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}