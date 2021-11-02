using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ProductionResultItemKeyTests : KeyTestsBase<LotProductionResultItemKey, ILotProductionResultItemKey>
    {
        private readonly DateTime _expectedLotDateCreated = new DateTime(2012, 3, 29);
        private const int _expectedLotDateSequence = 1;
        private const int _expectedLotTypeId = 2;
        private const int _expectedSequence = 42;
        private const string EXPECTED_STRING_VALUE = "20120329-1-2-42";
        private const string INVALID_PARSE_INPUT = "20120329-1-2-gotcha!";

        #region Overrides of NewKeyTestsBase<ProductionResultItemKeyTests>

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

        protected override void SetUpValidMock(Mock<ILotProductionResultItemKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LotKey_DateCreated).Returns(_expectedLotDateCreated);
            mockKeyInterface.SetupGet(m => m.LotKey_DateSequence).Returns(_expectedLotDateSequence);
            mockKeyInterface.SetupGet(m => m.LotKey_LotTypeId).Returns(_expectedLotTypeId);
            mockKeyInterface.SetupGet(m => m.ProductionResultItemKey_Sequence).Returns(_expectedSequence);
        }

        protected override LotProductionResultItemKey BuildKey(ILotProductionResultItemKey keyInterface)
        {
            return new LotProductionResultItemKey(keyInterface);
        }

        protected override void AssertValidKey(ILotProductionResultItemKey resultingKey)
        {
            Assert.AreEqual(_expectedLotDateCreated, resultingKey.LotKey_DateCreated);
            Assert.AreEqual(_expectedLotDateSequence, resultingKey.LotKey_DateSequence);
            Assert.AreEqual(_expectedLotTypeId, resultingKey.LotKey_LotTypeId);
            Assert.AreEqual(_expectedSequence, resultingKey.ProductionResultItemKey_Sequence);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = LotProductionResultItemKey.Null;
            var n2 = LotProductionResultItemKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}