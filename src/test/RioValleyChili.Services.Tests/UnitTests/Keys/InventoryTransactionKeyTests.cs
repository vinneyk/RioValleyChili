using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class InventoryTransactionKeyTests : DateSquenceKeyTestsBase<InventoryTransactionKey, IInventoryTransactionKey>
    {
        protected override void SetUpValidMock(Mock<IInventoryTransactionKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.InventoryTransactionKey_Date).Returns(ExpectedDate);
            mockKeyInterface.SetupGet(m => m.InventoryTransactionKey_Sequence).Returns(ExpectedSequence);
        }

        protected override InventoryTransactionKey BuildKey(IInventoryTransactionKey keyInterface)
        {
            return new InventoryTransactionKey(keyInterface);
        }

        protected override void AssertValidKey(IInventoryTransactionKey resultingKey)
        {
            Assert.AreEqual(ExpectedDate, resultingKey.InventoryTransactionKey_Date);
            Assert.AreEqual(ExpectedSequence, resultingKey.InventoryTransactionKey_Sequence);
        }
    }
}