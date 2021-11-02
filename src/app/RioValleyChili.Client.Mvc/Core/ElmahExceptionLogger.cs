using System;
using System.Web;
using Elmah;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core
{
    public class ElmahExceptionLogger : IExceptionLogger
    {
        public void LogException(Exception exception)
        {
            if (ContextOkForLogging()) LogExceptionFromContext(exception);
            else DefaultLog(exception);
        }

        public static void DefaultLog(Exception exception)
        {
            ErrorLog.GetDefault(null).Log(new Error(exception));
        }

        public static void LogExceptionFromContext(Exception exception)
        {
            try
            {
                var signal = ErrorSignal.FromCurrentContext();
                signal.Raise(exception);
            }
            catch (Exception ex)
            {
                DefaultLog(exception);
                DefaultLog(ex);
            }
        }

        private static bool ContextOkForLogging()
        {
            return HttpContext.Current != null && HttpContext.Current.ApplicationInstance != null;
        }
    }
}