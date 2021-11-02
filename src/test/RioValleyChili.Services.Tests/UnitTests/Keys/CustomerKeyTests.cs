using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class CustomerKeyTests : KeyTestsBase<CustomerKey, ICustomerKey>
    {
        private const int expectedId = 1;
        private const string expectedString = "1";
        private const string invalidParse = "no bueno";

        #region Overrides of KeyTestsBase<CustomerKey, ICustomerKey>

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

        protected override void SetUpValidMock(Mock<ICustomerKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.CustomerKey_Id).Returns(expectedId);
        }

        protected override CustomerKey BuildKey(ICustomerKey keyInterface)
        {
            return new CustomerKey(keyInterface);
        }

        protected override void AssertValidKey(ICustomerKey resultingKey)
        {
            Assert.AreEqual(expectedId, resultingKey.CustomerKey_Id);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = CustomerKey.Null;
            var n2 = CustomerKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}