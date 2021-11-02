using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class LotDefectKeyTest : KeyTestsBase<LotDefectKey, ILotDefectKey>
    {
        private readonly DateTime expectedLotDate = new DateTime(2013, 3, 29);
        private const int expectedLotSequence = 2;
        private const int expectedLotType = 4;
        private const int expectedDefectId = 10;
        private const string expectedStringValue = "20130329-2-4-10";
        private const string invalidKey = "20130323-2-4-10-death";

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

        protected override void SetUpValidMock(Mock<ILotDefectKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LotKey_DateCreated).Returns(expectedLotDate);
            mockKeyInterface.SetupGet(m => m.LotKey_DateSequence).Returns(expectedLotSequence);
            mockKeyInterface.SetupGet(m => m.LotKey_LotTypeId).Returns(expectedLotType);
            mockKeyInterface.SetupGet(m => m.LotDefectKey_Id).Returns(expectedDefectId);
        }

        protected override LotDefectKey BuildKey(ILotDefectKey keyInterface)
        {
            return new LotDefectKey(keyInterface);
        }

        protected override void AssertValidKey(ILotDefectKey resultingKey)
        {
            Assert.AreEqual(expectedLotDate, resultingKey.LotKey_DateCreated);
            Assert.AreEqual(expectedLotSequence, resultingKey.LotKey_DateSequence);
            Assert.AreEqual(expectedLotType, resultingKey.LotKey_LotTypeId);
            Assert.AreEqual(expectedDefectId, resultingKey.LotDefectKey_Id);
        }
    }
}