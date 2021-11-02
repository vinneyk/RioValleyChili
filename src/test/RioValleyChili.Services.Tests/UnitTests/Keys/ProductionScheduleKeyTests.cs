using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ProductionScheduleKeyTests : KeyTestsBase<ProductionScheduleKey, IProductionScheduleKey>
    {
        private readonly DateTime _expectedProductionDate = new DateTime(2012, 01, 20);
        private const int EXPECTED_PRODUCTION_LINE_ID = 2;
        private const string EXPECTED_STRING_VALUE = "20120120-2";
        private const string VALID_PARSE_INPUT = "20120120-2";
        private const string INVALID_PARSE_INPUT = "no bueno";

        #region Overrides of KeyTestsBase<ProductionScheduleKey,IProductionScheduleKey>

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

        protected override void SetUpValidMock(Mock<IProductionScheduleKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.ProductionScheduleKey_ProductionDate).Returns(_expectedProductionDate);
            mockKeyInterface.SetupGet(m => m.LocationKey_Id).Returns(EXPECTED_PRODUCTION_LINE_ID);
        }

        protected override ProductionScheduleKey BuildKey(IProductionScheduleKey keyInterface)
        {
            return new ProductionScheduleKey(keyInterface);
        }

        protected override void AssertValidKey(IProductionScheduleKey resultingKey)
        {
            Assert.AreEqual(_expectedProductionDate, resultingKey.ProductionScheduleKey_ProductionDate);
            Assert.AreEqual(EXPECTED_PRODUCTION_LINE_ID, resultingKey.LocationKey_Id);
        }

        #endregion
    }
}