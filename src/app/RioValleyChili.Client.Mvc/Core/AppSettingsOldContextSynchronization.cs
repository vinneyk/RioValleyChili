// ReSharper disable RedundantDefaultFieldInitializer

using System.Configuration;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core
{
    public class AppSettingsOldContextSynchronization : IOldContextSynchronizationSwitch
    {
        public bool Enabled
        {
            get
            {
                bool enabled;
                if(bool.TryParse(ConfigurationManager.AppSettings[AppSettingName], out enabled))
                {
                    _enabled = enabled;
                }
                return enabled;
            }
            set
            {
                _enabled = value;
                if(ConfigurationManager.AppSettings[AppSettingName] != null)
                {
                    ConfigurationManager.AppSettings[AppSettingName] = _enabled.ToString();
                }
            }
        }

        private bool _enabled = false;
        private const string AppSettingName = "OldContextSynchronizationEnabled";
    }
}

// ReSharper restore RedundantDefaultFieldInitializer