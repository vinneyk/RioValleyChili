using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class MovementOrderKeyTests : KeyTestsBase<InventoryPickOrderKey, IInventoryPickOrderKey>
    {
        private DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedOrderSequence = 33;
        private const string expectedKeyString = "20120329-33";
        private const string invalidKeyString = "Because it is bitter, and because it is my heart.";

        #region Overrides of KeyTestsBase<InventoryPickOrderKey, IInventoryPickOrderKey>

        protected override string ExpectedStringValue
        {
            get { return expectedKeyString; }
        }

        protected override string ValidParseInput
        {
            get { return expectedKeyString; }
        }

        protected override string InvalidParseInput
        {
            get { return invalidKeyString; }
        }

        protected override void SetUpValidMock(Mock<IInventoryPickOrderKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.InventoryPickOrderKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.InventoryPickOrderKey_Sequence).Returns(expectedOrderSequence);
        }

        protected override InventoryPickOrderKey BuildKey(IInventoryPickOrderKey keyInterface)
        {
            return new InventoryPickOrderKey(keyInterface);
        }

        protected override void AssertValidKey(IInventoryPickOrderKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.InventoryPickOrderKey_DateCreated);
            Assert.AreEqual(expectedOrderSequence, resultingKey.InventoryPickOrderKey_Sequence);
        }

        #endregion
    }
}