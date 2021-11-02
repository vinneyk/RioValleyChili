using System;

namespace RioValleyChili.Core
{
    public static class KillSwitch
    {
        public static IKillSwitch Instance { private get; set; }

        public static void Engage()
        {
            Instance.Engage();
        }

        public static void Disengage()
        {
            Instance.Disengage();
        }

        public static bool IsEngaged { get { return Instance.IsEngaged; } }
        
        public static DateTime? EngagedTimeStamp { get { return Instance.EngagedTimeStamp; } }
    }

    public interface IKillSwitch
    {
        void Engage();
        void Disengage();
        bool IsEngaged { get; }
        DateTime? EngagedTimeStamp { get; }
    }
}