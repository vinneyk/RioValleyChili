using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    public class ContractItemKeyTests : KeyTestsBase<ContractItemKey, IContractItemKey>
    {
        private const int expectedYear = 1920;
        private const int expectedSequence = 42;
        private const int expectedItemSequence = 2;
        private const string expectedString = "1920-42-2";
        private const string invalidString = "1920-42-0-2";

        #region Overrides of KeyTestsBase<ContractItemKey, IContractItemKey>

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
            get { return invalidString; }
        }

        protected override void SetUpValidMock(Mock<IContractItemKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ContractKey_Year).Returns(expectedYear);
            mockKeyInterface.SetupGet(m => m.ContractKey_Sequence).Returns(expectedSequence);
            mockKeyInterface.SetupGet(m => m.ContractItemKey_Sequence).Returns(expectedItemSequence);
        }

        protected override ContractItemKey BuildKey(IContractItemKey keyInterface)
        {
            return new ContractItemKey(keyInterface);
        }

        protected override void AssertValidKey(IContractItemKey resultingKey)
        {
            Assert.AreEqual(expectedYear, resultingKey.ContractKey_Year);
            Assert.AreEqual(expectedSequence, resultingKey.ContractKey_Sequence);
            Assert.AreEqual(expectedItemSequence, resultingKey.ContractItemKey_Sequence);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = ContractItemKey.Null;
            var n2 = ContractItemKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}