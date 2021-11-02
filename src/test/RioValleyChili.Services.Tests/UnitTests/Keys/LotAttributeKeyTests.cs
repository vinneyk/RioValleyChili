using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class LotAttributeKeyTests : KeyTestsBase<LotAttributeKey, ILotAttributeKey>
    {
        private readonly DateTime _expectedDateCreated = new DateTime(2012, 1, 07);
        private const int EXPECTED_DATE_SEQUENCE = 11;
        private const int EXPECTED_LOT_TYPE_ID = 1;
        private const string EXPECTED_ATTRIBUTESHORTNAME = "Asta";
        private const string EXPECTED_STRING_VALUE = "20120107-11-1-Asta";
        private const string VALID_PARSE_INPUT = "20120107-11-1-Asta";
        private const string INVALID_PARSE_INPUT = "no no no don't waste my water";

        #region Overrides of NewKeyTestsBase<LotItemKey,ILotItemKey>

        protected override string ExpectedStringValue
        {
            get { return EXPECTED_STRING_VALUE; }
        }

        protected override string ValidParseInput
        {
            get { return VALID_PARSE_INPUT; }
        }

        protected override string InvalidParseInput
        {
            get { return INVALID_PARSE_INPUT; }
        }

        protected override void SetUpValidMock(Mock<ILotAttributeKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LotKey_DateCreated).Returns(_expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.LotKey_DateSequence).Returns(EXPECTED_DATE_SEQUENCE);
            mockKeyInterface.SetupGet(m => m.LotKey_LotTypeId).Returns(EXPECTED_LOT_TYPE_ID);
            mockKeyInterface.SetupGet(m => m.AttributeNameKey_ShortName).Returns(EXPECTED_ATTRIBUTESHORTNAME);
        }

        protected override LotAttributeKey BuildKey(ILotAttributeKey keyInterface)
        {
            return new LotAttributeKey(keyInterface);
        }

        protected override void AssertValidKey(ILotAttributeKey resultingKey)
        {
            Assert.AreEqual(_expectedDateCreated, resultingKey.LotKey_DateCreated);
            Assert.AreEqual(EXPECTED_DATE_SEQUENCE, resultingKey.LotKey_DateSequence);
            Assert.AreEqual(EXPECTED_LOT_TYPE_ID, resultingKey.LotKey_LotTypeId);
            Assert.AreEqual(EXPECTED_ATTRIBUTESHORTNAME, resultingKey.AttributeNameKey_ShortName);
        }

        #endregion
    }
}