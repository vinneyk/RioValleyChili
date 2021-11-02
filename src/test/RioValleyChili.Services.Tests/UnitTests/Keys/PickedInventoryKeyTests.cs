using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class PickedInventoryKeyTests : DateSquenceKeyTestsBase<PickedInventoryKey, IPickedInventoryKey>
    {
        protected override void SetUpValidMock(Mock<IPickedInventoryKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.PickedInventoryKey_DateCreated).Returns(ExpectedDate);
            mockKeyInterface.SetupGet(m => m.PickedInventoryKey_Sequence).Returns(ExpectedSequence);
        }

        protected override PickedInventoryKey BuildKey(IPickedInventoryKey keyInterface)
        {
            return new PickedInventoryKey(keyInterface);
        }

        protected override void AssertValidKey(IPickedInventoryKey resultingKey)
        {
            Assert.AreEqual(ExpectedDate, resultingKey.PickedInventoryKey_DateCreated);
            Assert.AreEqual(ExpectedSequence, resultingKey.PickedInventoryKey_Sequence);
        }
    }
}