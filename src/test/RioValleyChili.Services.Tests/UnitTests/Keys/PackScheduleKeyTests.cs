using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class PackScheduleKeyTests : KeyTestsBase<PackScheduleKey, IPackScheduleKey>
    {
        private readonly DateTime _expectedDateCreated = new DateTime(2012, 01, 17);
        private const int EXPECTED_DATE_SEQUENCE = 99;
        private const string EXPECTED_STRING_VALUE = "2012017-99";
        private const string VALID_PARSE_INPUT = "2012017-99";
        private const string INVALID_PARSE_INPUT = "no bueno";

        #region Overrides of KeyTestsBase<PackScheduleKey,IPackScheduleKey>

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

        protected override void SetUpValidMock(Mock<IPackScheduleKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.PackScheduleKey_DateCreated).Returns(_expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.PackScheduleKey_DateSequence).Returns(EXPECTED_DATE_SEQUENCE);
        }

        protected override PackScheduleKey BuildKey(IPackScheduleKey keyInterface)
        {
            return new PackScheduleKey(keyInterface);
        }

        protected override void AssertValidKey(IPackScheduleKey resultingKey)
        {
            Assert.AreEqual(_expectedDateCreated, resultingKey.PackScheduleKey_DateCreated);
            Assert.AreEqual(EXPECTED_DATE_SEQUENCE, resultingKey.PackScheduleKey_DateSequence);
        }

        #endregion

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void Will_throw_exception_if_string_cannot_be_split_with_expected_character_()
        {
            var packSchedule = new PackScheduleKey();
            packSchedule.Parse("123435");
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void Will_throw_expcetion_if_string_is_split_into_more_than_2_substrings_with_expected_character()
        {
            var packSchedule = new PackScheduleKey();
            packSchedule.Parse("1-2-3-4-5");
        }

        [Test]
        [ExpectedException]
        public void Will_throw_exception_if_1st_substring_is_less_than_7_characters_long()
        {
            var packSchedule = new PackScheduleKey();
            packSchedule.Parse("12346-123");
        }

        [Test]
        [ExpectedException]
        public void Will_throw_exception_if_1st_substring_is_greater_than_7_characters_long()
        {
            var packSchedule = new PackScheduleKey();
            packSchedule.Parse("12345678-123");
        }

        [Test]
        [ExpectedException]
        public void Will_throw_exception_if_the_first_4_characters_of_the_1st_substring_cannot_be_parsed_into_an_integer()
        {
            var packSchedule = new PackScheduleKey();
            packSchedule.Parse("123X567-123");
        }

        [Test]
        [ExpectedException]
        public void Will_throw_exception_if_the_last_3_characters_of_the_1st_substring_cannot_be_parsed_into_an_integer()
        {
            var packSchedule = new PackScheduleKey();
            packSchedule.Parse("1234X67-123");
        }

        [Test]
        [ExpectedException]
        public void Will_throw_exception_if_the_2nd_substring_cannot_be_parsed_into_an_integer()
        {
            var packSchedule = new PackScheduleKey();
            packSchedule.Parse("1234567-1X3");
        }

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = PackScheduleKey.Null;
            var n2 = PackScheduleKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}