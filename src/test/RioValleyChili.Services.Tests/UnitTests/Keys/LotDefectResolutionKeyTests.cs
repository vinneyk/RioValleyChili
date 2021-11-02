using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class LotDefectResolutionKeyTests : KeyTestsBase<LotDefectResolutionKey, ILotDefectResolutionKey>
    {
        private readonly DateTime expectedLotDate = new DateTime(2013, 3, 29);
        private const int expectedLotSequence = 2;
        private const int expectedLotType = 4;
        private const int expectedDefectId = 22;
        private const string expectedStringValue = "20130329-2-4-22";
        private const string invalidKey = "...";

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

        protected override void SetUpValidMock(Mock<ILotDefectResolutionKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LotKey_DateCreated).Returns(expectedLotDate);
            mockKeyInterface.SetupGet(m => m.LotKey_DateSequence).Returns(expectedLotSequence);
            mockKeyInterface.SetupGet(m => m.LotKey_LotTypeId).Returns(expectedLotType);
            mockKeyInterface.SetupGet(m => m.LotDefectKey_Id).Returns(expectedDefectId);
        }

        protected override LotDefectResolutionKey BuildKey(ILotDefectResolutionKey keyInterface)
        {
            return new LotDefectResolutionKey(keyInterface);
        }

        protected override void AssertValidKey(ILotDefectResolutionKey resultingKey)
        {
            Assert.AreEqual(expectedLotDate, resultingKey.LotKey_DateCreated);
            Assert.AreEqual(expectedLotSequence, resultingKey.LotKey_DateSequence);
            Assert.AreEqual(expectedLotType, resultingKey.LotKey_LotTypeId);
            Assert.AreEqual(expectedDefectId, resultingKey.LotDefectKey_Id);
        }
    }
}