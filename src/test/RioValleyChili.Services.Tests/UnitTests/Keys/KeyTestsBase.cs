using System;
using Moq;
using NUnit.Framework;
using Solutionhead.EntityKey;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public abstract class KeyTestsBase<TKey, TKeyInterface>
        where TKeyInterface : class
        where TKey : EntityKeyBase.Of<TKeyInterface>, new()
    {
        protected readonly Mock<TKeyInterface> MockKey = new Mock<TKeyInterface>();

        protected abstract string ExpectedStringValue { get; }
        protected abstract string ValidParseInput { get; }
        protected abstract string InvalidParseInput { get; }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void Parse_throws_if_null_value_is_passed()
        {
            // Arrange
            var keyParser = new TKey();

            // Act & Assert
            keyParser.Parse(null);
        }

        [Test]
        public void Key_interface_constructor_sets_properties_correctly()
        {
            // Arrange
            SetUpValidMock(MockKey);

            // Act
            var key = BuildKey(MockKey.Object);

            // Assert
            AssertValidKey(key as TKeyInterface);
        }

        [Test]
        public void ToString_produces_correct_string_value()
        {
            // Arrange
            SetUpValidMock(MockKey);
            var key = BuildKey(MockKey.Object);

            // Act
            var keyValue = key.ToString();

            // Assert
            Assert.AreEqual(ExpectedStringValue, keyValue);
        }

        [Test]
        public void Parse_method_generates_proper_key_with_valid_input()
        {
            // Arrange
            SetUpValidMock(MockKey);

            // Act
            var k = new TKey();
            var key = k.Parse(ValidParseInput);

            // Assert
            AssertValidKey(key);
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void Parse_method_throws_with_invalid_input()
        {
            var key = new TKey();
            key.Parse(InvalidParseInput);
        }

        [Test]
        public void TryParse_method_returns_false_with_invalid_input()
        {
            // Act
            TKeyInterface key;
            var parser = new TKey();
            var result = parser.TryParse(InvalidParseInput, out key);

            // Assert
            result.AssertFalse();
        }

        [Test]
        public void TryParse_returns_true_with_valid_input()
        {
            // Act
            TKeyInterface key;
            var parser = new TKey();
            var result = parser.TryParse(ValidParseInput, out key);

            // Assert
            result.AssertTrue();
        }

        [Test]
        public void TryParse_generates_proper_key_with_valid_input()
        {
            // Act
            TKeyInterface key;
            var parser = new TKey();
            parser.TryParse(ValidParseInput, out key);

            // Assert
            AssertValidKey(key);
        }

        [Test]
        public void Default_returns_non_null_object()
        {
            var defaultKey = new TKey().Default;
            Assert.IsNotNull(defaultKey);
        }

        [Test]
        public void Equates_Two_Keys_With_Same_Values()
        {
            // Arrange
            SetUpValidMock(MockKey);
            var key1 = BuildKey(MockKey.Object);
            var key2 = BuildKey(MockKey.Object);

            // Act
            var areEqual = key1.Equals(key2);

            // Assert
            Assert.IsFalse(ReferenceEquals(key1, key2));
            Assert.IsTrue(areEqual);
        }

        protected abstract void SetUpValidMock(Mock<TKeyInterface> mockKeyInterface);

        protected abstract TKey BuildKey(TKeyInterface keyInterface);
        
        protected abstract void AssertValidKey(TKeyInterface resultingKey);
    }
}
