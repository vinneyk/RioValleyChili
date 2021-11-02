namespace RioValleyChili.Core
{
    public static class OldContextSynchronizationSwitch
    {
        public static IOldContextSynchronizationSwitch Instance
        {
            private get { return _instance ?? (_instance = new DefaultSynchronizationSwitch()); }
            set { _instance = value; }
        }
        private static IOldContextSynchronizationSwitch _instance;

        public static bool Enabled
        {
            get { return Instance.Enabled; }
            set { Instance.Enabled = value; }
        }

        private class DefaultSynchronizationSwitch : IOldContextSynchronizationSwitch
        {
            bool IOldContextSynchronizationSwitch.Enabled
            {
                get { return _enabled; }
                set { _enabled = value; }
            }
            private bool _enabled = true;
        }
    }

    public interface IOldContextSynchronizationSwitch
    {
        bool Enabled { get; set; }
    }
}