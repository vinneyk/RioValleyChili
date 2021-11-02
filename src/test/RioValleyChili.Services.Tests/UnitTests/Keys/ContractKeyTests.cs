using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ContractKeyTests : KeyTestsBase<ContractKey, IContractKey>
    {
        private const int expectedYear = 1920;
        private const int expectedSequence = 42;
        private const string expectedString = "1920-42";
        private const string invalidString = "1920-42-ContractKeyHurray!";

        #region Overrides of KeyTestsBase<ContractKey, IContractKey>

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

        protected override void SetUpValidMock(Mock<IContractKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ContractKey_Year).Returns(expectedYear);
            mockKeyInterface.SetupGet(m => m.ContractKey_Sequence).Returns(expectedSequence);
        }

        protected override ContractKey BuildKey(IContractKey keyInterface)
        {
            return new ContractKey(keyInterface);
        }

        protected override void AssertValidKey(IContractKey resultingKey)
        {
            Assert.AreEqual(expectedYear, resultingKey.ContractKey_Year);
            Assert.AreEqual(expectedSequence, resultingKey.ContractKey_Sequence);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = ContractKey.Null;
            var n2 = ContractKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}