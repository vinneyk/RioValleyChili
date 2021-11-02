using System;
using System.Web.Http;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class SiteStatusController : ApiController
    {
        public SiteStatusModel Get()
        {
            return new SiteStatusModel
            {
                IsRunning = !KillSwitch.IsEngaged,
                TimeOfDeathValue = KillSwitch.EngagedTimeStamp
            };
        }
    }

    public class SiteStatusModel
    {
        public Boolean IsRunning { get; set; }

        internal DateTime? TimeOfDeathValue { private get; set; }

        public string TimeOfDeath
        {
            get
            {
                return TimeOfDeathValue.HasValue ? TimeOfDeathValue.Value.ToString("G") : "";
            }
        }
    }
}
