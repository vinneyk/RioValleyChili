using System.Collections.Generic;
using Solutionhead.EntityKey;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public abstract class KeyReservationHelperBase<TKey, TKeyInterface>
        where TKeyInterface : class
        where TKey : EntityKeyBase.Of<TKeyInterface>, TKeyInterface, new()
    {
        private static readonly TKey Key = new TKey();
        private readonly HashSet<string> _registeredKeys = new HashSet<string>();

        public TKeyInterface ParseAndRegisterKey(string key)
        {
            TKeyInterface keyInterface;
            if(!string.IsNullOrWhiteSpace(key) && Key.TryParse(key, out keyInterface))
            {
                _registeredKeys.Add(key);
                return keyInterface;
            }

            return null;
        }

        public TKeyInterface GetNextAndRegisterKey(TKeyInterface keyInterface)
        {
            var keyString = Key.BuildKeyValue(keyInterface);
            while(_registeredKeys.Contains(keyString))
            {
                keyInterface = GetNextKey(keyInterface);
                keyString = Key.BuildKeyValue(keyInterface);
            }
            _registeredKeys.Add(keyString);

            return keyInterface;
        }

        protected abstract TKeyInterface GetNextKey(TKeyInterface keyInterface);
    }
}