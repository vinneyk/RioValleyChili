using System;

namespace RioValleyChili.Core
{
    public static class DBConnectionStatus
    {
        private static IDBConnectionStatus _oldContextConnectionStatus;
        private static IDBConnectionStatus _newContextConnectionStatus;

        public static void RegisterOldContextStatus(IDBConnectionStatus connectionStatus)
        {
            _oldContextConnectionStatus = connectionStatus;
        }

        public static void RegisterNewContextStatus(IDBConnectionStatus connectionStatus)
        {
            _newContextConnectionStatus = connectionStatus;
        }

        public static bool OldContextIsValid
        {
            get { return _oldContextConnectionStatus != null && _oldContextConnectionStatus.IsValid; }
        }

        public static Exception OldContextException
        {
            get { return _oldContextConnectionStatus == null ? null : _oldContextConnectionStatus.Exception; }
        }

        public static bool NewContextIsValid
        {
            get { return _newContextConnectionStatus != null && _newContextConnectionStatus.IsValid; }
        }

        public static Exception NewContextException
        {
            get { return _newContextConnectionStatus == null ? null : _newContextConnectionStatus.Exception; }
        }
    }

    public interface IDBConnectionStatus
    {
        bool IsValid { get; }

        Exception Exception { get; }
    }
}