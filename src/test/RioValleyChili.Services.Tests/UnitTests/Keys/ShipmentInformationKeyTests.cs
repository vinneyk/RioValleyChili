using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ShipmentInformationKeyTests : KeyTestsBase<ShipmentInformationKey, IShipmentInformationKey>
    {
        private DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedOrderSequence = 33;
        private const string expectedKeyString = "20120329-33";
        private const string invalidKeyString = "I used to be a contender...";

        #region Overrides of KeyTestsBase<ShipmentInformationKey, IShipmentInformationKey>

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

        protected override void SetUpValidMock(Mock<IShipmentInformationKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ShipmentInfoKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.ShipmentInfoKey_Sequence).Returns(expectedOrderSequence);
        }

        protected override ShipmentInformationKey BuildKey(IShipmentInformationKey keyInterface)
        {
            return new ShipmentInformationKey(keyInterface);
        }

        protected override void AssertValidKey(IShipmentInformationKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.ShipmentInfoKey_DateCreated);
            Assert.AreEqual(expectedOrderSequence, resultingKey.ShipmentInfoKey_Sequence);
        }

        #endregion
    }
}