using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class InventoryShipmentOrderKeyTests : KeyTestsBase<InventoryShipmentOrderKey, IInventoryShipmentOrderKey>
    {
        private readonly DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedSequence = 42;
        private const string expectedKeyString = "20120329-42";
        private const string invalidKeyString = "Oh no.";

        #region Overrides of KeyTestsBase<PickedInventoryKey, IPickedInventoryKey>

        protected override string ExpectedStringValue { get { return expectedKeyString; } }

        protected override string ValidParseInput { get { return expectedKeyString; } }

        protected override string InvalidParseInput { get { return invalidKeyString; } }

        protected override void SetUpValidMock(Mock<IInventoryShipmentOrderKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.InventoryShipmentOrderKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.InventoryShipmentOrderKey_Sequence).Returns(expectedSequence);
        }

        protected override InventoryShipmentOrderKey BuildKey(IInventoryShipmentOrderKey keyInterface)
        {
            return new InventoryShipmentOrderKey(keyInterface);
        }

        protected override void AssertValidKey(IInventoryShipmentOrderKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.InventoryShipmentOrderKey_DateCreated);
            Assert.AreEqual(expectedSequence, resultingKey.InventoryShipmentOrderKey_Sequence);
        }

        #endregion
    }
}