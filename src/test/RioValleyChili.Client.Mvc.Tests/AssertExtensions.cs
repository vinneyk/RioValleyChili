using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Moq.Language.Flow;
using NUnit.Framework;
using RioValleyChili.Client.Core;

namespace RioValleyChili.Tests
{
    //todo migrate into Solutionhead.MVC library

    internal static class ControllerAssert
    {
        internal static void AssertRedirectsToRoute(this ActionResult resultUnderTest, ActionResult expectedResult)
        {
            var redirectResult = resultUnderTest as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult, "Should have returned a RedirectToRouteResult.");
            Assert.AreEqual(expectedResult.GetRouteValueDictionary(), redirectResult.RouteValues);
        }

        [Obsolete("Use the ActionResult overload instead.")]
        internal static void AssertRedirectsToRoute(this ActionResult resultUnderTest, string controller, string action, Dictionary<string, string> parameters = null)
        {
            var redirectResult = resultUnderTest as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult, "Should have returned a RedirectToRouteResult.");
            Assert.AreEqual(controller, redirectResult.RouteValues["controller"], string.Format("Should have returned a RedirectToRouteResult with the controller of {0}.", controller));
            Assert.AreEqual(action, redirectResult.RouteValues["action"], string.Format("Should have returned a RedirectToRouteResult with the action method of {0}.", action));
        }

        internal static void AssertReturnsViewResult(this ActionResult resultUnderTest, string expectedViewName)
        {
            var viewResult = resultUnderTest as ViewResult;

            Assert.IsNotNull(viewResult, "Should have returned a ViewResult.");
            Assert.AreEqual(expectedViewName, viewResult.ViewName);
        }
    }

    public static class RouteDataAssert
    {
        public static void AssertRouteValuesEqual(this RouteData routeData, RouteValueDictionary expectedRouteValues)
        {
            var orderedRouteData = routeData.Values.OrderBy(v => v.Key);
            var orderedExpectedValues = expectedRouteValues.OrderBy(v => v.Key);

            Assert.IsNotNull(routeData);
            Assert.IsTrue(
                orderedRouteData.SequenceEqual(orderedExpectedValues,
                    new StrictKeyEqualityComparer<KeyValuePair<string, object>, string>(k => k.Key.ToLower())),
                    string.Format("Route keys do not match. \n  Expected Values: {{ {0} }} \n  Actual Values: {{ {1} }}",
                        string.Join(" , ", orderedExpectedValues.Select(v => v.Key)),
                        string.Join(" , ", orderedRouteData.Select(v => v.Key))));

            Assert.IsTrue(
                orderedRouteData.SequenceEqual(orderedExpectedValues,
                    new StrictKeyEqualityComparer<KeyValuePair<string, object>, string>(k => k.Value == null ? string.Empty : k.Value.ToString().ToLower())),
                    string.Format("Route values do not match. \n  Expected Values: {{ {0} }} \n  Actual Values:   {{ {1} }}",
                        string.Join(" , ", orderedExpectedValues.Select(v => v.Value)),
                        string.Join(" , ", orderedRouteData.Select(v => v.Key))));
        }
    }
}
