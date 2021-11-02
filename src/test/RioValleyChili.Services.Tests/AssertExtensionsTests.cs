using System;
using NUnit.Framework;
using Solutionhead.Services;

namespace RioValleyChili.Services.Tests
{
    [TestFixture]
    public class AssertExtensionsTests
    {
        [Test]
        public void Matches_expected_message()
        {
            const string received = "What's the frequency, Kenneth?";
            const string expected = "What's {0} {1}, Kenneth?";

            var result = new FailureResult(received);
            result.AssertNotSuccess(expected);
        }

        [Test]
        public void Throws_exception_if_message_does_not_match()
        {
            const string received = "What you have";
            const string expected = "what I want.";

            try
            {
                var result = new FailureResult(received);
                result.AssertFailure(expected);
            }
            catch(Exception ex)
            {
                Assert.Pass(ex.Message);
            }
            Assert.Fail("\"{0}\" is not \"{1}\"", received, expected);
        }
    }
}