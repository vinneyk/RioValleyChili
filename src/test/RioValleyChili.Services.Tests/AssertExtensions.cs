using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Solutionhead.Services;

namespace RioValleyChili.Services.Tests
{
    public static class AssertExtensions
    {
        public static void AssertSuccess(this IResult result, string errorMessagePattern)
        {
            if(!result.Success)
            {
                result.AssertSuccess();
            }
            else
            {
                AssertErrorMessageMatch(errorMessagePattern, result.Message);
            }
        }

        public static void AssertNotSuccess(this IResult result, string errorMessagePattern)
        {
            if(result.State == ResultState.Success)
            {
                result.AssertNotSuccess();
            }
            else
            {
                AssertErrorMessageMatch(errorMessagePattern, result.Message);
            }
        }

        public static void AssertNotSuccess(this IResult result, IEnumerable<string> errorMessagePatterns, string split)
        {
            if(result.State == ResultState.Success)
            {
                result.AssertNotSuccess();
            }
            else
            {
                var results = result.Message.Split(new[] { split }, StringSplitOptions.None).ToList();
                string unmatchedString = null;
                if(errorMessagePatterns.Any(e =>
                    {
                        if(results.All(r => !ErrorMessageMatch(e, r)))
                        {
                            unmatchedString = e;
                            return true;
                        }
                        return false;
                    }))
                {
                    Assert.Fail("Could not match: \"{0}\".", unmatchedString);
                }
            }
        }

        public static void AssertInvalid(this IResult result, string errorMessagePattern)
        {
            if(result.State != ResultState.Invalid)
            {
                result.AssertInvalid();
            }
            else
            {
                AssertErrorMessageMatch(errorMessagePattern, result.Message);
            }
        }

        public static void AssertFailure(this IResult result, string errorMessagePattern)
        {
            if(result.State != ResultState.Failure)
            {
                result.AssertFailure();
            }
            else
            {
                AssertErrorMessageMatch(errorMessagePattern, result.Message);
            }
        }

        public static void AssertNoWorkRequired(this IResult result, string errorMessagePattern)
        {
            if(result.State != ResultState.NoWorkRequired)
            {
                result.AssertNoWorkRequired();
            }
            else
            {
                AssertErrorMessageMatch(errorMessagePattern, result.Message);
            }
        }

        public static void AssertNotSuccess(this IResult result, bool assertMessage = true)
        {
            if(result.Success)
            {
                Assert.Fail("Expected non-successful result, but was successful. \n\nResult Message:\n" + (result.Message ?? "(Empty)"));
            }
            else if(assertMessage)
            {
                AssertMessage(result);
            }
        }

        public static void AssertSuccess(this IResult result)
        {
            if(!result.Success)
            {
                Assert.Fail("Expected successful result. Actual result state was '" + result.State + "'. \n\nResult Message:\n" + (result.Message ?? "(Empty)"));
            }
        }

        public static void AssertInvalid(this IResult result, bool assertMessage = true)
        {
            if(result.State != ResultState.Invalid)
            {
                Assert.Fail("Expected Invalid result. Actual result state was '" + result.State + "'. \n\nResult Message:\n" + (result.Message ?? "(Empty)"));
            }
            else if(assertMessage)
            {
                AssertMessage(result);
            }
        }

        public static void AssertFailure(this IResult result, bool assertMessage = true)
        {
            if (result.State != ResultState.Failure)
            {
                Assert.Fail("Expected Failure result. Actual result state was '" + result.State + "'. \n\nResult Message:\n" + (result.Message ?? "(Empty)"));
            }
            else if(assertMessage)
            {
                AssertMessage(result);
            }
        }

        public static void AssertNoWorkRequired(this IResult result, bool assertMessage = true)
        {
            if(result.State != ResultState.NoWorkRequired)
            {
                Assert.Fail("Expected NoWorkRequired result. Actual result state was '" + result.State + "'. \n\nResult Message:\n" + (result.Message ?? "(Empty)"));
            }
            else if(assertMessage)
            {
                AssertMessage(result);
            }
        }

        public static void AssertMessage(this IResult result)
        {
            Assert.Pass(result.Message);
        }

        public static void AssertTrue(this bool b)
        {
             Assert.IsTrue(b);
        }

        public static void AssertFalse(this bool b)
        {
             Assert.IsFalse(b);
        }

        public static void AssertSequenceEqual<TObject>(IEnumerable<TObject> expected, IEnumerable<TObject> actual, string message = null, IEqualityComparer<TObject> equalityComparer = null)
        {
            Assert.IsTrue(expected.SequenceEqual(actual, equalityComparer), message);
        }

        private static void AssertErrorMessageMatch(string errorMessagePattern, string errorMessage)
        {
            if(string.IsNullOrEmpty(errorMessagePattern) && string.IsNullOrEmpty(errorMessage))
            {
                return;
            }
            
            if(string.IsNullOrEmpty(errorMessagePattern))
            {
                Assert.Fail("Error message pattern not supplied.");
            }

            if(string.IsNullOrEmpty(errorMessage))
            {
                Assert.Fail("No error message in result.");
            }

            var patternToMatch = errorMessagePattern.Replace("(", "\\(");
            patternToMatch = patternToMatch.Replace(")", "\\)");
            patternToMatch = Regex.Replace(patternToMatch, @"{\d.*?}", @"(.*?)");

            var match = Regex.Match(errorMessage, patternToMatch);
            if(!match.Success)
            {
                Assert.Fail("Expected error message in the form of: \"{0}\" but received \"{1}\" instead.", errorMessagePattern, errorMessage);
            }
        }

        private static bool ErrorMessageMatch(string errorMessagePattern, string errorMessage)
        {
            try
            {
                AssertErrorMessageMatch(errorMessagePattern, errorMessage);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
