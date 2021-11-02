namespace RioValleyChili.Client.Mvc.Core.Messaging
{
    public interface IMessengerService
    {
        void SetMessage(MessageType messageType, string message, params object[] args);
        UserMessage GetMessage();
    }
}