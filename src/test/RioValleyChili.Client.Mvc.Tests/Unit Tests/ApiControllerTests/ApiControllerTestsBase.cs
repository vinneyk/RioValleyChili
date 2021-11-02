using System.Web.Http;
using NUnit.Framework;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Tests.Helpers;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    public abstract class ApiControllerTestsBase<TController>
        where TController : ApiController
    {
        protected abstract TController SystemUnderTest { get; }
        protected abstract string[] ClaimResources { get; }


        [Test]
        public void ControllerDefinesViewClaim()
        {
            SystemUnderTest.GetType().AssertClaimsForType(ClaimResources, ClaimActions.View);
        }

        [Test(Description = "Asserts that the Post method is decorated with an ValidateAntiForgeryTokenFromCookie attribute")]
        public void PostMethodValidatesAntiForgeryToken()
        {
            typeof (TController).AssertAntiForgeryTokenValidationForMethod("Post");
        }

        [Test]
        public void PostMethodValidatesClaim()
        {
            typeof(TController).AssertClaimsForMethod(
                "Post",
                ClaimResources,
                ClaimActions.Create);
        }

        [Test(Description = "Asserts that the Put method is decorated with an ValidateAntiForgeryTokenFromCookie attribute")]
        public void PutMethodValidatesAntiForgeryToken()
        {
            typeof(TController).AssertAntiForgeryTokenValidationForMethod("Put");
        }

        [Test]
        public void PutMethodValidatesClaim()
        {
            typeof(TController).AssertClaimsForMethod("Put", ClaimResources, ClaimActions.Modify);            
        }

        [Test(Description = "Asserts that the Delete method is decorated with an ValidateAntiForgeryTokenFromCookie attribute")]
        public void DeleteMethodValidatesAntiForgeryToken()
        {
            typeof(TController).AssertAntiForgeryTokenValidationForMethod("Delete");
        }

        [Test]
        public void DeleteMethodValidatesClaim()
        {
            typeof(TController).AssertClaimsForMethod("Delete", ClaimResources, ClaimActions.Delete);            
        }
    }
}