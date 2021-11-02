using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class ModelStateHelper
    {
        public static string GetErrorSummary(this ModelStateDictionary modelState)
        {
            return modelState.IsValid ? null : "The supplied model contains errors. Please correct the errors and retry.";
        }

        public static void EnsureValidModelStateWithHttpResponseException(this ModelStateDictionary modelState)
        {
            if(!modelState.IsValid)
            {
                var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = modelState.GetErrorSummary()
                    };
                throw new HttpResponseException(message);
            }
        }
    }
}