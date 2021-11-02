using System;

namespace RioValleyChili.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception GetInnermostException(this Exception ex)
        {
            if(ex == null)
            {
                return null;
            }

            while(ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex;
        }
    }
}