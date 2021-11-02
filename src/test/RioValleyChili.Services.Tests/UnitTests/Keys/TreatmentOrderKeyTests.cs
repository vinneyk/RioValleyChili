using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class TreatmentOrderKeyTests : KeyTestsBase<TreatmentOrderKey, ITreatmentOrderKey>
    {
        private DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedOrderSequence = 33;
        private const string expectedKeyString = "20120329-33";
        private const string invalidKeyString = "Oh nevermind.";

        #region Overrides of KeyTestsBase<TreatmentOrderKey, ITreatmentOrderKey>

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

        protected override void SetUpValidMock(Mock<ITreatmentOrderKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.InventoryShipmentOrderKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.InventoryShipmentOrderKey_Sequence).Returns(expectedOrderSequence);
        }

        protected override TreatmentOrderKey BuildKey(ITreatmentOrderKey keyInterface)
        {
            return new TreatmentOrderKey(keyInterface);
        }

        protected override void AssertValidKey(ITreatmentOrderKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.InventoryShipmentOrderKey_DateCreated);
            Assert.AreEqual(expectedOrderSequence, resultingKey.InventoryShipmentOrderKey_Sequence);
        }

        #endregion
    }
}