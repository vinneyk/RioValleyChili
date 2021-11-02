using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class DehydratedMaterialsReceivedItemKeyTests : KeyTestsBase<ChileMaterialsReceivedItemKey, IChileMaterialsReceivedItemKey>
    {
        private readonly DateTime _expectedLotDate = new DateTime(2012, 3, 29);
        private const int _expectedLotSequence = 1;
        private const int _expectedLotTypeId = 2;
        private const int _expectedItemSequence = 3;
        private const string EXPECTED_STRING_VALUE = "20120329-1-2-3";
        private const string INVALID_PARSE_INPUT = "20120329-1-FAIL-3";

        #region Overrides of NewKeyTestsBase<DehydratedMaterialsReceivedKey>

        protected override string ExpectedStringValue
        {
            get { return EXPECTED_STRING_VALUE; }
        }

        protected override string ValidParseInput
        {
            get { return EXPECTED_STRING_VALUE; }
        }

        protected override string InvalidParseInput
        {
            get { return INVALID_PARSE_INPUT; }
        }

        protected override void SetUpValidMock(Mock<IChileMaterialsReceivedItemKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LotKey_DateCreated).Returns(_expectedLotDate);
            mockKeyInterface.SetupGet(m => m.LotKey_DateSequence).Returns(_expectedLotSequence);
            mockKeyInterface.SetupGet(m => m.LotKey_LotTypeId).Returns(_expectedLotTypeId);
            mockKeyInterface.SetupGet(m => m.ChileMaterialsReceivedKey_ItemSequence).Returns(_expectedItemSequence);
        }

        protected override ChileMaterialsReceivedItemKey BuildKey(IChileMaterialsReceivedItemKey keyInterface)
        {
            return new ChileMaterialsReceivedItemKey(keyInterface);
        }

        protected override void AssertValidKey(IChileMaterialsReceivedItemKey resultingKey)
        {
            Assert.AreEqual(_expectedLotDate, resultingKey.LotKey_DateCreated);
            Assert.AreEqual(_expectedLotSequence, resultingKey.LotKey_DateSequence);
            Assert.AreEqual(_expectedLotTypeId, resultingKey.LotKey_LotTypeId);
            Assert.AreEqual(_expectedItemSequence, resultingKey.ChileMaterialsReceivedKey_ItemSequence);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = ChileMaterialsReceivedItemKey.Null;
            var n2 = ChileMaterialsReceivedItemKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}