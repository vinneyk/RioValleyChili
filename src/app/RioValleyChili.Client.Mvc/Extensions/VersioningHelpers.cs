using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Extensions
{
    public static class VersioningHelpers
    {
        public static string AppVersion(this HtmlHelper helper)
        {
            return System.Reflection.Assembly.GetExecutingAssembly()
                .GetName()
                .Version
                .ToString();
        }
    }
}