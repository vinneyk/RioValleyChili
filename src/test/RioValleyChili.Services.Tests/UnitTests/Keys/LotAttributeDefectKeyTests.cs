using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class LotAttributeDefectKeyTests : KeyTestsBase<LotAttributeDefectKey, ILotAttributeDefectKey>
    {
        private readonly DateTime expectedLotDate = new DateTime(2013, 3, 29);
        private const int expectedLotSequence = 2;
        private const int expectedLotType = 4;
        private const int expectedDefectId = 10;
        private const string expectedAttributeShortName = "AstaLaVista";
        private const string expectedStringValue = "20130329-2-4-10-AstaLaVista";
        private const string invalidKey = "20130323-2-4-10-oh no-thereit is";

        protected override string ExpectedStringValue
        {
            get { return expectedStringValue; }
        }

        protected override string ValidParseInput
        {
            get { return expectedStringValue; }
        }

        protected override string InvalidParseInput
        {
            get { return invalidKey; }
        }

        protected override void SetUpValidMock(Mock<ILotAttributeDefectKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LotKey_DateCreated).Returns(expectedLotDate);
            mockKeyInterface.SetupGet(m => m.LotKey_DateSequence).Returns(expectedLotSequence);
            mockKeyInterface.SetupGet(m => m.LotKey_LotTypeId).Returns(expectedLotType);
            mockKeyInterface.SetupGet(m => m.LotDefectKey_Id).Returns(expectedDefectId);
            mockKeyInterface.SetupGet(m => m.AttributeNameKey_ShortName).Returns(expectedAttributeShortName);
        }

        protected override LotAttributeDefectKey BuildKey(ILotAttributeDefectKey keyInterface)
        {
            return new LotAttributeDefectKey(keyInterface);
        }

        protected override void AssertValidKey(ILotAttributeDefectKey resultingKey)
        {
            Assert.AreEqual(expectedLotDate, resultingKey.LotKey_DateCreated);
            Assert.AreEqual(expectedLotSequence, resultingKey.LotKey_DateSequence);
            Assert.AreEqual(expectedLotType, resultingKey.LotKey_LotTypeId);
            Assert.AreEqual(expectedDefectId, resultingKey.LotDefectKey_Id);
            Assert.AreEqual(expectedAttributeShortName, resultingKey.AttributeNameKey_ShortName);
        }
    }
}