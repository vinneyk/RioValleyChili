using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class CustomerProductCodeKeyTest : KeyTestsBase<CustomerProductCodeKey, ICustomerProductCodeKey>
    {
        private const int expectedCustomerId = 1;
        private const int expectedChileProductId = 43;
        private const string expectedString = "1-43";
        private const string invalidParse = "1-forty-three";

        #region Overrides of KeyTestsBase<CustomerProductCodeKey, ICustomerProductCodeKey>

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

        protected override void SetUpValidMock(Mock<ICustomerProductCodeKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.CustomerKey_Id).Returns(expectedCustomerId);
            mockKeyInterface.SetupGet(m => m.ChileProductKey_ProductId).Returns(expectedChileProductId);
        }

        protected override CustomerProductCodeKey BuildKey(ICustomerProductCodeKey keyInterface)
        {
            return new CustomerProductCodeKey(keyInterface);
        }

        protected override void AssertValidKey(ICustomerProductCodeKey resultingKey)
        {
            Assert.AreEqual(expectedCustomerId, resultingKey.CustomerKey_Id);
            Assert.AreEqual(expectedChileProductId, resultingKey.ChileProductKey_ProductId);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = CustomerProductCodeKey.Null;
            var n2 = CustomerProductCodeKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}