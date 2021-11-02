using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class SalesQuoteKeyTests : DateSquenceKeyTestsBase<SalesQuoteKey, ISalesQuoteKey>
    {
        protected override void SetUpValidMock(Mock<ISalesQuoteKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.SalesQuoteKey_DateCreated).Returns(ExpectedDate);
            mockKeyInterface.SetupGet(m => m.SalesQuoteKey_Sequence).Returns(ExpectedSequence);
        }

        protected override SalesQuoteKey BuildKey(ISalesQuoteKey keyInterface)
        {
            return new SalesQuoteKey(keyInterface);
        }

        protected override void AssertValidKey(ISalesQuoteKey resultingKey)
        {
            Assert.AreEqual(ExpectedDate, resultingKey.SalesQuoteKey_DateCreated);
            Assert.AreEqual(ExpectedSequence, resultingKey.SalesQuoteKey_Sequence);
        }
    }
}