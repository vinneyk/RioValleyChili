using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class IntraWarehouseMovementOrderKeyTests : KeyTestsBase<IntraWarehouseOrderKey, IIntraWarehouseOrderKey>
    {
        private DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedOrderSequence = 33;
        private const string expectedKeyString = "20120329-33";
        private const string invalidKeyString = "Drawing a blank here.";

        #region Overrides of KeyTestsBase<IntraWarehouseOrderKey, ITreatmentOrderKey>

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

        protected override void SetUpValidMock(Mock<IIntraWarehouseOrderKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.IntraWarehouseOrderKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.IntraWarehouseOrderKey_Sequence).Returns(expectedOrderSequence);
        }

        protected override IntraWarehouseOrderKey BuildKey(IIntraWarehouseOrderKey keyInterface)
        {
            return new IntraWarehouseOrderKey(keyInterface);
        }

        protected override void AssertValidKey(IIntraWarehouseOrderKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.IntraWarehouseOrderKey_DateCreated);
            Assert.AreEqual(expectedOrderSequence, resultingKey.IntraWarehouseOrderKey_Sequence);
        }

        #endregion
    }
}