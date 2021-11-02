using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class LotKeyTests : KeyTestsBase<LotKey, ILotKey>
    {
        private readonly DateTime _expectedDateCreated = new DateTime(2012, 1, 17);
        private const int EXPECTED_DATE_SEQUENCE = 11;
        private const int EXPECTED_LOT_TYPE_ID = 1;
        private const string EXPECTED_STRING_VALUE = "01 12 017 11";
        private const string VALID_PARSE_INPUT = "01 12 017 11";
        private const string INVALID_PARSE_INPUT = "no bueno";

        #region Overrides of KeyTestsBase<LotKey,ILotKey>

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

        protected override void SetUpValidMock(Mock<ILotKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LotKey_DateCreated).Returns(_expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.LotKey_DateSequence).Returns(EXPECTED_DATE_SEQUENCE);
            mockKeyInterface.SetupGet(m => m.LotKey_LotTypeId).Returns(EXPECTED_LOT_TYPE_ID);
        }

        protected override LotKey BuildKey(ILotKey keyInterface)
        {
            return new LotKey(keyInterface);
        }

        protected override void AssertValidKey(ILotKey resultingKey)
        {
            Assert.AreEqual(_expectedDateCreated, resultingKey.LotKey_DateCreated);
            Assert.AreEqual(EXPECTED_DATE_SEQUENCE, resultingKey.LotKey_DateSequence);
            Assert.AreEqual(EXPECTED_LOT_TYPE_ID, resultingKey.LotKey_LotTypeId);
            Assert.AreEqual(EXPECTED_LOT_TYPE_ID, resultingKey.LotKey_LotTypeId);
        }

        #endregion

        [Test]
        public void Will_build_expected_lot_number_string()
        {
            // Arrange
            var mock = new Mock<ILotKey>();
            SetUpValidMock(mock);
            var lotKey = new LotKey(mock.Object);

            // Act
            var lotNumber = lotKey.KeyValue;

            // Assert
            Assert.AreEqual(EXPECTED_STRING_VALUE, lotNumber);
        }

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = LotKey.Null;
            var n2 = LotKey.Null;

            Assert.IsTrue(n1 == n2);
        }

        [Test]
        public void GivenValueIsValidWithLeadingZeroButDoesNotContainSpaces_ParserSucceeds()
        {
            // Arrange
            var keyValue = "011201255"; // 01 12 012 55
            var keyParser = new LotKey();
            var expectedDate = new DateTime(2012, 1, 12);

            // Act
            var key = keyParser.Parse(keyValue);

            // Assert
            Assert.AreEqual(expectedDate, key.LotKey_DateCreated);
            Assert.AreEqual(55, key.LotKey_DateSequence);
            Assert.AreEqual(1, key.LotKey_LotTypeId);
        }

        [Test]
        public void GivenValueIsValidWithoutLeadingZeroButDoesNotContainSpaces_ParserSucceeds()
        {
            // Arrange
            var keyValue = "11201255"; // 01 12 012 55
            var keyParser = new LotKey();
            var expectedDate = new DateTime(2012, 1, 12);

            // Act
            var key = keyParser.Parse(keyValue);

            // Assert
            Assert.AreEqual(expectedDate, key.LotKey_DateCreated);
            Assert.AreEqual(55, key.LotKey_DateSequence);
            Assert.AreEqual(1, key.LotKey_LotTypeId);
        }

        [Test]
        public void GivenValueIsValidWithDoubleDigitBatchTypeButDoesNotContainSpaces_ParserSucceeds()
        {
            // Arrange
            var keyValue = "991201255"; // 99 12 012 55
            var keyParser = new LotKey();
            var expectedDate = new DateTime(2012, 1, 12);

            // Act
            var key = keyParser.Parse(keyValue);

            // Assert
            Assert.AreEqual(expectedDate, key.LotKey_DateCreated);
            Assert.AreEqual(55, key.LotKey_DateSequence);
            Assert.AreEqual(99, key.LotKey_LotTypeId);
        }
    }
}