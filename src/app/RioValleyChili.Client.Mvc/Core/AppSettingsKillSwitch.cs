using System;
using System.Configuration;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core
{
    public class AppSettingsKillSwitch : IKillSwitch
    {
        public void Engage()
        {
            IsEngaged = true;
            EngagedTimeStamp = DateTime.Now;
        }

        public void Disengage()
        {
            IsEngaged = false;
            EngagedTimeStamp = null;
        }

        public bool IsEngaged
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["KillSwitchEngaged"]); }
            set { ConfigurationManager.AppSettings["KillSwitchEngaged"] = value.ToString(); }
        }

        public DateTime? EngagedTimeStamp { get; private set; }
    }
}