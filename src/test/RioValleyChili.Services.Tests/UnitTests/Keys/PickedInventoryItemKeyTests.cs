using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class PickedInventoryItemKeyTests : KeyTestsBase<PickedInventoryItemKey, IPickedInventoryItemKey>
    {
        private readonly DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedOrderSequence = 42;
        private const int expectedItemSequence = 99;
        private const string expectedKeyString = "20120329-42-99";
        private const string invalidKeyString = "Somewhere a little girl is crying.";

        #region Overrides of KeyTestsBase<PickedInventoryItemKey, IPickedInventoryItemKey>

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

        protected override void SetUpValidMock(Mock<IPickedInventoryItemKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.PickedInventoryKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.PickedInventoryKey_Sequence).Returns(expectedOrderSequence);
            mockKeyInterface.SetupGet(m => m.PickedInventoryItemKey_Sequence).Returns(expectedItemSequence);
        }

        protected override PickedInventoryItemKey BuildKey(IPickedInventoryItemKey keyInterface)
        {
            return new PickedInventoryItemKey(keyInterface);
        }

        protected override void AssertValidKey(IPickedInventoryItemKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.PickedInventoryKey_DateCreated);
            Assert.AreEqual(expectedOrderSequence, resultingKey.PickedInventoryKey_Sequence);
            Assert.AreEqual(expectedItemSequence, resultingKey.PickedInventoryItemKey_Sequence);
        }

        #endregion
    }
}