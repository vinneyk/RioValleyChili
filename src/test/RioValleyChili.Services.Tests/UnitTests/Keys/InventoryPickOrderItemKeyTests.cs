using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class InventoryPickOrderItemKeyTests : KeyTestsBase<InventoryPickOrderItemKey, IInventoryPickOrderItemKey>
    {
        private DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedOrderSequence = 42;
        private const int expectedItemSequence = 99;
        private const string expectedKeyString = "20120329-42-99";
        private const string invalidKeyString = "Of all the tests in all the test fixtures...";

        #region Overrides of KeyTestsBase<InventoryPickOrderItemKey, IInventoryPickOrderItemKey>

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

        protected override void SetUpValidMock(Mock<IInventoryPickOrderItemKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.InventoryPickOrderKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.InventoryPickOrderKey_Sequence).Returns(expectedOrderSequence);
            mockKeyInterface.SetupGet(m => m.InventoryPickOrderItemKey_Sequence).Returns(expectedItemSequence);
        }

        protected override InventoryPickOrderItemKey BuildKey(IInventoryPickOrderItemKey keyInterface)
        {
            return new InventoryPickOrderItemKey(keyInterface);
        }

        protected override void AssertValidKey(IInventoryPickOrderItemKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.InventoryPickOrderKey_DateCreated);
            Assert.AreEqual(expectedOrderSequence, resultingKey.InventoryPickOrderKey_Sequence);
            Assert.AreEqual(expectedItemSequence, resultingKey.InventoryPickOrderItemKey_Sequence);
        }

        #endregion
    }
}