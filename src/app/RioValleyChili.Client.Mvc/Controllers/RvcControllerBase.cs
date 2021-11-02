using System;
using System.Linq;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Messaging;

namespace RioValleyChili.Client.Mvc.Controllers
{
    public class RvcControllerBase : Controller
    {
        protected IMessengerService MessengerService;

        public RvcControllerBase()
        {
            MessengerService = new TempDataMessengerService(TempData);
            GetMessagesFromQueryString();
        }

        public void QueueUserMessage(MessageType messageType, string message, params object[] args)
        {
            MessengerService.SetMessage(messageType, message, args);
        }
        
        public void QueueUserMessage(string message, params object[] args)
        {
            MessengerService.SetMessage(MessageType.Informational, message, args);
        }

        private void GetMessagesFromQueryString()
        {
            if (Request == null || Request.QueryString == null) return;

            const string querystringMessageKey = "usermsg";
            var queryString = Request.QueryString;
            if (queryString.AllKeys.Any(k => string.Equals(k, querystringMessageKey, StringComparison.OrdinalIgnoreCase)))
            {
                var qsMessage = queryString.GetValues(querystringMessageKey);
                if (qsMessage != null && qsMessage.Any())
                {
                    var message = string.Join("<br/>", qsMessage);
                    MessengerService.SetMessage(MessageType.Informational, message);
                }
            }
        }
    }

}