using System;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Core.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class BypassKillSwitchFilter : FilterAttribute { }
}