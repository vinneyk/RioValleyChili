using System;
using System.Linq;
using Twilio;

namespace DataLoadCompletion
{
    class Program
    {
        private const int MaxErrorLength = 100;

        static void Main()
        {
            try
            {
                using(var context = new DataLoadContext())
                {
                    var nowDate = DateTime.Now.Date;
                    var result = context.DataLoadResult.ToList().OrderByDescending(d => d.TimeStamp).FirstOrDefault(d => d.TimeStamp.Date == nowDate);
                    if(result == null)
                    {
                        SendMessage(string.Format("No DataLoadResult record for '{0}' found!", nowDate.ToString("yyyy/MM/dd")));
                    }
                    else
                    {
                        SendMessage(string.Format("Result for '{0}': RanToCompletion[{1}] Success[{2}].", nowDate.ToString("yyyy/MM/dd"), result.RanToCompletion, result.Success));
                    }
                }
            }
            catch(Exception ex)
            {
                while(ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                {
                    ex = ex.InnerException;
                }

                var message = ex.Message ?? "";
                if(message.Length > MaxErrorLength)
                {
                    message = string.Format("{0}...", ex.Message.Substring(0, MaxErrorLength));
                }

                SendMessage(string.Format("Error! {0}", message));
                throw;
            }
        }

        private static void SendMessage(string message)
        {
            var twilio = new TwilioRestClient(TwilioSection.AccountSID, TwilioSection.AuthToken);
            twilio.SendMessage(TwilioSection.FromNumber, TwilioSection.ToNumber, message);
        }
    }
}
