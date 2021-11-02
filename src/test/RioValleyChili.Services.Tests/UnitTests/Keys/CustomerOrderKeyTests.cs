using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class CustomerOrderKeyTests : KeyTestsBase<SalesOrderKey, ISalesOrderKey>
    {
        private readonly DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedSequence = 42;
        private const string expectedKeyString = "20120329-42";
        private const string invalidKeyString = "20120329-snap";

        #region Overrides of KeyTestsBase<PickedInventoryKey, IPickedInventoryKey>

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

        protected override void SetUpValidMock(Mock<ISalesOrderKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.SalesOrderKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.SalesOrderKey_Sequence).Returns(expectedSequence);
        }

        protected override SalesOrderKey BuildKey(ISalesOrderKey keyInterface)
        {
            return new SalesOrderKey(keyInterface);
        }

        protected override void AssertValidKey(ISalesOrderKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.SalesOrderKey_DateCreated);
            Assert.AreEqual(expectedSequence, resultingKey.SalesOrderKey_Sequence);
        }

        #endregion
    }
}