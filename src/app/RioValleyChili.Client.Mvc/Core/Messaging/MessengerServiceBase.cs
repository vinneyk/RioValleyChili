using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Core.Messaging
{
    public abstract class MessengerServiceBase : IMessengerService
    {
        public abstract void SetMessage(MessageType messageType, string message, params object[] args);

        public abstract UserMessage GetMessage();
    }

    public class UserMessage
    {
        public string Message { get; set; }

        public MessageType MessageType { get; set; }
    }
}