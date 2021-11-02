using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class CustomerOrderItemKeyTests : KeyTestsBase<SalesOrderItemKey, ISalesOrderItemKey>
    {
        private DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedOrderSequence = 42;
        private const int expectedItemSequence = 99;
        private const string expectedKeyString = "20120329-42-99";
        private const string invalidKeyString = "20120329-42-BIG-BUNS";

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

        protected override void SetUpValidMock(Mock<ISalesOrderItemKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.SalesOrderKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.SalesOrderKey_Sequence).Returns(expectedOrderSequence);
            mockKeyInterface.SetupGet(m => m.SalesOrderItemKey_ItemSequence).Returns(expectedItemSequence);
        }

        protected override SalesOrderItemKey BuildKey(ISalesOrderItemKey keyInterface)
        {
            return new SalesOrderItemKey(keyInterface);
        }

        protected override void AssertValidKey(ISalesOrderItemKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.SalesOrderKey_DateCreated);
            Assert.AreEqual(expectedOrderSequence, resultingKey.SalesOrderKey_Sequence);
            Assert.AreEqual(expectedItemSequence, resultingKey.SalesOrderItemKey_ItemSequence);
        }

        #endregion
    }
}