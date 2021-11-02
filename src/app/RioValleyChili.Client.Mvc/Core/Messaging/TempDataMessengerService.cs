using System;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Core.Messaging
{
    public class TempDataMessengerService : IMessengerService
    {
        private readonly TempDataDictionary _tempDataDictionary;

        public TempDataMessengerService(TempDataDictionary tempDataDictionary)
        {
            if (tempDataDictionary == null) { throw new ArgumentNullException("tempDataDictionary"); }
            _tempDataDictionary = tempDataDictionary;
        }

        public void SetMessage(MessageType messageType, string message, params object[] args)
        {
            _tempDataDictionary["__UserMessage"] = string.Format(message, args);
            _tempDataDictionary["__UserMessageType"] = messageType;
        }

        public virtual UserMessage GetMessage()
        {
            return _tempDataDictionary["__UserMessage"] == null
                       ? null
                       : new UserMessage
                             {
                                 Message = _tempDataDictionary["__UserMessage"] as string,
                                 MessageType = (MessageType)(_tempDataDictionary["__UserMessageType"] ?? MessageType.Informational),
                             };
        }
    }
}